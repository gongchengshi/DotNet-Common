using System.IO;
using System.Linq;

namespace Gongchengshi
{
   public static class FileSystemHelpers
   {
      public static string GetBackupPath(string path)
      {
         var parent = Directory.GetParent(path);
         var matches = parent.GetFiles(Path.GetFileName(path) + "*");

         var proposedNameBase = Path.GetFileName(path) + ".bak";
         var proposedName = proposedNameBase;

         int backupNumber = 1;
         while (matches.Count(x => x.Name == proposedName) > 0)
         {
            proposedName = proposedNameBase + backupNumber++;
         }

         return Path.Combine(parent.FullName, proposedName);
      }

      public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
      {
         var dir = new DirectoryInfo(sourceDirName);
         var dirs = dir.GetDirectories();

         if (!dir.Exists)
         {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
         }

         if (!Directory.Exists(destDirName))
         {
            Directory.CreateDirectory(destDirName);
         }

         var files = dir.GetFiles();
         foreach (var file in files)
         {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
         }

         if (copySubDirs)
         {
            foreach (var subdir in dirs)
            {
               string temppath = Path.Combine(destDirName, subdir.Name);
               CopyDirectory(subdir.FullName, temppath, copySubDirs);
            }
         }
      }
   }
}
