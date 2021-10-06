using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BaseElements
{
    public static class FileExtensions
    {
        public static String NormalizePath(String baseFileOrDirectoryName, String path)
        {
            Uri result;
            bool abs = Uri.TryCreate(path, UriKind.Absolute, out result);
            if (abs) return path;
            bool rel = Uri.TryCreate(path, UriKind.Relative, out result);
            if (rel)
            {
                if (Directory.Exists(baseFileOrDirectoryName))
                {
                    return Path.Combine(baseFileOrDirectoryName, path.TrimStart(Path.DirectorySeparatorChar));
                }
                if (File.Exists(baseFileOrDirectoryName))
                {
                    return Path.Combine(Path.GetDirectoryName(baseFileOrDirectoryName), path.TrimStart(Path.DirectorySeparatorChar));
                }
                    
            }
            throw new FileNotFoundException(path + " не может быть приведен к абсолютному или относительному пути");
        }
    }
}
