using System.Collections.Generic;

namespace Pictua
{
    public class ServerFileInfo
    {
        public FileMetadata? Metadata { get; set; }

        public ISet<ClientIdentity> Owners { get; set; }

        public bool IsContentOnline { get; set; }

        public ServerFileInfo(FileMetadata? metadata, ISet<ClientIdentity> owners, bool isContentOnline)
        {
            Metadata = metadata;
            Owners = owners;
            IsContentOnline = isContentOnline;
        }

        public ServerFileInfo() : this(null, new HashSet<ClientIdentity>(), false) { }
    }
}
