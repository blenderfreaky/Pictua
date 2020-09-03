using System;
using System.IO;

namespace Pictua
{
    public sealed class FilePathConfig
    {
        public const string AppName = "Pictua";
        public const string LockFileName = ".lock";
        public const string StateFileName = "state.xml";
        public const string FilesFolderName = "Files";
        public const string TrashFolderName = "Trash";

        public string RootPath { get; }
        public string LockFilePath { get; }
        public string StateFilePath { get; }
        public string FilesFolderPath { get; }
        public string TrashFolderPath { get; }

        public static FilePathConfig Client => new FilePathConfig(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName));
        public static FilePathConfig Server => new FilePathConfig("");

        public FilePathConfig(string rootPath)
        {
            RootPath = rootPath;
            LockFilePath = Path.Combine(RootPath, LockFileName);
            StateFilePath = Path.Combine(RootPath, StateFileName);
            FilesFolderPath = Path.Combine(RootPath, FilesFolderName);
            TrashFolderPath = Path.Combine(RootPath, TrashFolderName);
        }

        public string GetFilePath(FileDescriptor fileDescriptor) => Path.Combine(FilesFolderPath, fileDescriptor.UniqueName);

        public string GetTrashFilePath(FileDescriptor fileDescriptor) => Path.Combine(TrashFolderPath, fileDescriptor.UniqueName);
    }
}