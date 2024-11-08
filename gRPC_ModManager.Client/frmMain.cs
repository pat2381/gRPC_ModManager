using Grpc.Net.Client;
using gRPC_ModManager.Shared;
using gRPC_ModManager.Shared.Models;
using Octodiff.Core;
using ProtoBuf.Grpc.Client;
using Octodiff.Diagnostics;


namespace gRPC_ModManager.Client;

public partial class frmMain : Form
{
    private GrpcChannel grpcChannel;
    private IDirectorySyncService GrpcClient;

    public frmMain()
    {
        InitializeComponent();
        grpcChannel = GrpcChannel.ForAddress("http://localhost:5283");
        GrpcClient = grpcChannel.CreateGrpcService<IDirectorySyncService>();
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            txtPath.Text = folderBrowserDialog.SelectedPath;
            Settings.Default.SyncPath = folderBrowserDialog.SelectedPath;
            Settings.Default.Save();
        }
    }


    private async Task SynchronizeFiles(IProgress<int> progress)
    {
        //using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        //var client = channel.CreateGrpcService<IDirectorySyncService>();

        string clientDirectoryPath = txtPath.Text;

        // Erstelle das Verzeichnis, falls es noch nicht existiert
        if (!Directory.Exists(clientDirectoryPath))
        {
            Directory.CreateDirectory(clientDirectoryPath);
        }

        // Prüfen, ob Dateien im Client-Verzeichnis existieren
        if (Directory.GetFiles(clientDirectoryPath, searchPattern: "*.*" , searchOption:SearchOption.AllDirectories).Length == 0)
        {
            // Initiale Daten vom Server abrufen, falls keine lokalen Dateien vorhanden sind
            var initialDataResponse = await GrpcClient.GetInitialDirectoryDataAsync(new EmptyRequest());

            foreach (var fileSignature in initialDataResponse.Files)
            {
                var filePath = Path.Combine(clientDirectoryPath, fileSignature.FileName!);
                await File.WriteAllBytesAsync(filePath, fileSignature.Signature!);
            }

            MessageBox.Show("Initiale Daten vom Server abgerufen und gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Signaturen für bestehende Dateien erstellen
        var clientSignatures = new List<FileSignature>();
        foreach (var filePath in Directory.GetFiles(clientDirectoryPath))
        {
            var signatureStream = new MemoryStream();
            var signatureBuilder = new SignatureBuilder();

            using (var fileStream = File.OpenRead(filePath))
            {
                await Task.Run(() => signatureBuilder.Build(fileStream, new SignatureWriter(signatureStream)));
            }

            clientSignatures.Add(new FileSignature
            {
                FileName = Path.GetFileName(filePath),
                Signature = signatureStream.ToArray()
            });
        }

        // Fordere Deltas für geänderte Dateien vom Server an
        var deltaResponse = await GrpcClient.GetDirectoryDeltaAsync(new DirectoryDeltaRequest { ClientSignatures = clientSignatures });

        int totalFiles = deltaResponse.Deltas.Count;
        int filesProcessed = 0;

        foreach (var fileDelta in deltaResponse.Deltas)
        {
            var clientFilePath = Path.Combine(clientDirectoryPath, fileDelta.FileName!);
            if (File.Exists(clientFilePath))
            {
                var deltaApplier = new DeltaApplier();
                using var basisStream = File.OpenRead(clientFilePath);
                using var deltaStream = new MemoryStream(fileDelta.Delta!);
                using var newVersionStream = File.Create(clientFilePath + "_updated");

                await Task.Run(() => deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()), newVersionStream));

                File.Delete(clientFilePath);
                File.Move(clientFilePath + "_updated", clientFilePath);
            }
            else
            {
                await File.WriteAllBytesAsync(clientFilePath, fileDelta.Delta!);
            }

            filesProcessed++;
            int progressPercentage = (filesProcessed * 100) / totalFiles;
            progress.Report(progressPercentage);
        }
    }


    private async void btnStart_Click(object sender, EventArgs e)
    {
        btnStart.Enabled = false;
        progressBar1.Value = 0;

        var progress = new Progress<int>(value => progressBar1.Value = value);

        try
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                throw new ArgumentNullException("path");
            }

            await SynchronizeFiles(progress);
            MessageBox.Show("Synchronisation abgeschlossen!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler bei der Synchronisation: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnStart.Enabled = true;
        }
    }
}
