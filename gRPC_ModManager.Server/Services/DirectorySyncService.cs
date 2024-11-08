using Grpc.Core;
using gRPC_ModManager.Shared;
using gRPC_ModManager.Shared.Models;
using Octodiff.Core;
using Octodiff.Diagnostics;
using System.Security.Cryptography;
namespace gRPC_ModManager.Server.Services
{
    public class DirectorySyncService : IDirectorySyncService
    {
        private readonly string serverDirectoryPath = "server_directory";

        public async Task<InitialDirectoryDataResponse> GetInitialDirectoryDataAsync(EmptyRequest request, ServerCallContext context)
        {
            var response = new InitialDirectoryDataResponse();

            foreach (var filePath in Directory.GetFiles(serverDirectoryPath))
            {
                var signatureStream = new MemoryStream();
                var signatureBuilder = new SignatureBuilder();
                using (var fileStream = File.OpenRead(filePath))
                {
                    signatureBuilder.Build(fileStream, new SignatureWriter(signatureStream));
                }

                response.Files.Add(new FileSignature
                {
                    FileName = Path.GetFileName(filePath),
                    Signature = signatureStream.ToArray()
                });
            }

            return response;
        }

        public async Task<DirectoryDeltaResponse> GetDirectoryDeltaAsync(DirectoryDeltaRequest request, ServerCallContext context)
        {
            var response = new DirectoryDeltaResponse();

            foreach (var clientFileSignature in request.ClientSignatures)
            {
                var serverFilePath = Path.Combine(serverDirectoryPath, clientFileSignature.FileName!);

                if (File.Exists(serverFilePath))
                {
                    var clientSignatureStream = new MemoryStream(clientFileSignature.Signature!);
                    var deltaStream = new MemoryStream();
                    var deltaBuilder = new DeltaBuilder();

                    using (var basisStream = File.OpenRead(serverFilePath))
                    {
                        deltaBuilder.BuildDelta(basisStream, new SignatureReader(clientSignatureStream, new ConsoleProgressReporter()), new BinaryDeltaWriter(deltaStream));
                    }

                    response.Deltas.Add(new FileDelta
                    {
                        FileName = clientFileSignature.FileName,
                        Delta = deltaStream.ToArray()
                    });
                }
                else
                {
                    response.Deltas.Add(new FileDelta
                    {
                        FileName = clientFileSignature.FileName,
                        Delta = await File.ReadAllBytesAsync(serverFilePath)
                    });
                }
            }

            return response;
        }

    }
}
