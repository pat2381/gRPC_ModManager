using ProtoBuf;


namespace gRPC_ModManager.Shared.Models
{
    [ProtoContract]
    public class DirectoryDeltaResponse
    {
        [ProtoMember(1)]
        public List<FileDelta> Deltas { get; set; } = new List<FileDelta>();
    }
}
