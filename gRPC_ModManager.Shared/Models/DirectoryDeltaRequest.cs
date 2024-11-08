using ProtoBuf;


namespace gRPC_ModManager.Shared.Models
{
    [ProtoContract]
    public class DirectoryDeltaRequest
    {
        [ProtoMember(1)]
        public List<FileSignature> ClientSignatures { get; set; } = new List<FileSignature>();
    }
}
