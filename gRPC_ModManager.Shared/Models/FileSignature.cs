using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gRPC_ModManager.Shared.Models
{
    [ProtoContract]
    public class FileSignature
    {
        [ProtoMember(1)]
        public string? FileName { get; set; }
        [ProtoMember(2)]
        public byte[]? Signature { get; set; }  
    }
}
