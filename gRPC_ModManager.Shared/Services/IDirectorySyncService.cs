using Grpc.Core;
using gRPC_ModManager.Shared.Models;
using System.ServiceModel;

namespace gRPC_ModManager.Shared
{
    [ServiceContract]
    public interface IDirectorySyncService
    {
        [OperationContract]
        Task<InitialDirectoryDataResponse> GetInitialDirectoryDataAsync(EmptyRequest request);

        [OperationContract]
        Task<DirectoryDeltaResponse> GetDirectoryDeltaAsync(DirectoryDeltaRequest request);
    }
}
