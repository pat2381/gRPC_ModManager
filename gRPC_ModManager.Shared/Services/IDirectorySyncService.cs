using Grpc.Core;
using gRPC_ModManager.Shared.Models;

namespace gRPC_ModManager.Shared
{
    public interface IDirectorySyncService
    {
        Task<InitialDirectoryDataResponse> GetInitialDirectoryDataAsync(EmptyRequest request, ServerCallContext? context = null);
        Task<DirectoryDeltaResponse> GetDirectoryDeltaAsync(DirectoryDeltaRequest request, ServerCallContext? context = null);
    }
}
