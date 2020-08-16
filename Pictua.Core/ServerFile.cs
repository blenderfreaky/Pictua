namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;

    public class ServerFile
    {
        public FileDescriptor FileDescriptor { get; }
        public FileMetadata? Metadata { get; set; }

        public string? LocalPath { get; set; }

        public bool RemovalRequested { get; set; }

        public ICollection<IClientIdentity> Owners { get; set; }

        public ServerFile(FileDescriptor fileDescriptor, FileMetadata? metadata = null, string? localPath = null, bool removalRequested = false, ICollection<IClientIdentity>? owners = null)
        {
            FileDescriptor = fileDescriptor;
            Metadata = metadata;
            LocalPath = localPath;
            RemovalRequested = removalRequested;
            Owners = owners ?? new List<IClientIdentity>();
        }
    }
}
