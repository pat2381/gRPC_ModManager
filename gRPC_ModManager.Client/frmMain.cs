using Grpc.Net.Client;
using gRPC_ModManager.Shared;
using gRPC_ModManager.Shared.Models;
using Octodiff.Core;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc;


namespace gRPC_ModManager.Client;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            txtPath.Text = folderBrowserDialog.SelectedPath;
            Settings.Default.SyncPath = folderBrowserDialog.SelectedPath;
            Settings.Default.Save();
        }
    }

    private async Task SynchronizeFiles(IProgress<int> progress)
    {
        // Erstelle den gRPC-Kanal und den Client
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = channel.CreateGrpcService<IDirectorySyncService>();

        string clientDirectoryPath = "client_directory";

        var clientSignatures = new List<FileSignature>();
        foreach (var filePath in Directory.GetFiles(clientDirectoryPath))
        {
            var signatureStream = new MemoryStream();
            var signatureBuilder = new SignatureBuilder();

            using (var fileStream = File.OpenRead(filePath))
            {
                signatureBuilder.Build(fileStream, new SignatureWriter(signatureStream));
            }

            clientSignatures.Add(new FileSignature
            {
                FileName = Path.GetFileName(filePath),
                Signature = signatureStream.ToArray()
            });
        }

        var deltaResponse = await client.GetDirectoryDeltaAsync(new DirectoryDeltaRequest { ClientSignatures = clientSignatures });

        int totalFiles = deltaResponse.Deltas.Count;
        int filesProcessed = 0;

        foreach (var fileDelta in deltaResponse.Deltas)
        {
            var clientFilePath = Path.Combine(clientDirectoryPath, fileDelta.FileName!);
            if (File.Exists(clientFilePath))
            {
                var deltaApplier = new DeltaApplier();
                using var basisStream = File.OpenRead(clientFilePath);
                using var deltaStream = new MemoryStream(fileDelta.Delta);
                using var newVersionStream = File.Create(clientFilePath + "_updated");

                deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, progress), newVersionStream);

                File.Delete(clientFilePath);
                File.Move(clientFilePath + "_updated", clientFilePath);
            }
            else
            {
                await File.WriteAllBytesAsync(clientFilePath, fileDelta.Delta);
            }

            filesProcessed++;
            int progressPercentage = (filesProcessed * 100) / totalFiles;
            progress.Report(progressPercentage);
        }
    }

}
}