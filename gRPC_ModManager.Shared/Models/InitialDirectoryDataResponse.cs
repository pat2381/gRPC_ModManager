using ProtoBuf;


namespace gRPC_ModManager.Shared.Models
{
    [ProtoContract]
    public class InitialDirectoryDataResponse
    {
        [ProtoMember(1)]
        public List<FileSignature> Files { get; set; } = new List<FileSignature>();
    }
}
