using System;
using System.IO;

namespace BuildCSystem.Projects
{
    public class PathTools
    {
        public static string NativeToRelative(string filespec, string folder)
        {
            if (!Path.IsPathRooted(filespec))
            {
                throw new ArgumentException(
                    string.Format("require absolute file path to '{0}'",filespec));
            }
            if (!Path.IsPathRooted(folder))
            {
                throw new ArgumentException(
                    string.Format("require absolute folder path to '{0}'",folder));
            }

            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string NativeToRelativeWindows(string filespec, string folder)
        {
            var p = NativeToRelative(filespec, folder);
            return p.Replace(Path.DirectorySeparatorChar, '\\');
        }
    }
}

