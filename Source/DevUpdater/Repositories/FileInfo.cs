using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevUpdater.Repositories
{
    /// <summary>
    /// Information about one file.
    /// </summary>
    public class FileInfo
    {
        public long Size { get; set; }

        public string FileName { get; set;  }

        public Hash Hash { get; set; }

        public void EnsureParentDirectoryExists(string rootDirectory)
        {
            var dirName = Path.GetDirectoryName(ResolveFullPath(rootDirectory));
            Directory.CreateDirectory(dirName);
        }

        public string ResolveFullPath(string rootDirectory)
        {
            return Path.Combine(rootDirectory, FileName);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode() ^ FileName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileInfo))
                return false;

            var second = (FileInfo)obj;

            if (Size != second.Size)
                return false;

            if (!Hash.Equals(second.Hash))
                return false;

            if (!string.Equals(FileName, second.FileName, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
