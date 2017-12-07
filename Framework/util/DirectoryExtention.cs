using System.IO;

namespace WebdriverFramework.Framework.Util
{
    public static class DirectoryExtension
    {
        /*public static IEnumerable<string> CopyDirectory(string sourceDirName, string destDirName)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(sourceDirName, "sourceDirName must not be null or white space");
            //Guard.ArgumentIsNotNullOrWhiteSpace(destDirName, "destDirName must not be null or white space");
            List<string> list = new List<string>();
            DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] directories = directoryInfo1.GetDirectories();
            if (!directoryInfo1.Exists)
                throw new DirectoryNotFoundException(string.Format("Source directory does not exist or could not be found: {0}.", (object)sourceDirName));
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);
            foreach (FileInfo fileInfo in directoryInfo1.GetFiles())
            {
                string destFileName = Path.Combine(destDirName, fileInfo.Name);
                fileInfo.Refresh();
                if (fileInfo.Exists)
                {
                    fileInfo.CopyTo(destFileName, true);
                    list.Add(destFileName);
                }
            }
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
                IEnumerable<string> collection = DirectoryExtension.CopyDirectory(directoryInfo2.FullName, destDirName1);
                list.AddRange(collection);
            }
            return (IEnumerable<string>)list;
        }

        public static void DeleteDirectory(string path)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(path, "path must not be null or white space");
            if (!Directory.Exists(path))
                return;
            Directory.Delete(path, true);
        }

        public static bool TryDeleteDirectory(string path)
        {
            // Guard.ArgumentIsNotNullOrWhiteSpace(path, "path must not be null or white space");
            try
            {
                DirectoryExtension.ForceDeleteDirectory(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ForceDeleteDirectory(string path)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(path, "path must not be null or white space");
            if (!Directory.Exists(path))
                return;
            DirectoryInfo directoryInfo1 = new DirectoryInfo(path);
            directoryInfo1.Attributes = directoryInfo1.Attributes & ~(FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive);
            foreach (DirectoryInfo directoryInfo2 in directoryInfo1.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                int num = (int)(directoryInfo2.Attributes & ~(FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive));
                directoryInfo2.Attributes = (FileAttributes)num;
            }
            foreach (FileInfo fileInfo in directoryInfo1.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                int num = (int)(fileInfo.Attributes & ~(FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive));
                fileInfo.Attributes = (FileAttributes)num;
            }
            directoryInfo1.Delete(true);
        }

        public static bool AreFoldersEqual(string lhsPath, string rhsPath)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            //Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            DirectoryInfo directoryInfo1 = new DirectoryInfo(lhsPath);
            DirectoryInfo directoryInfo2 = new DirectoryInfo(rhsPath);
            string searchPattern = "*.*";
            int num = 1;
            return Enumerable.SequenceEqual<FileInfo>((IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo1.GetFiles(searchPattern, (SearchOption)num), (Func<FileInfo, string>)(info => info.FullName)), (IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo2.GetFiles("*.*", SearchOption.AllDirectories), (Func<FileInfo, string>)(info => info.FullName)), (IEqualityComparer<FileInfo>)DirectoryExtension.GetFileInfoComparerByNameAndLength());
        }

        public static bool AreFoldersEqual(string lhsPath, string rhsPath, out string differencesLog)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            //Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            differencesLog = string.Empty;
            DirectoryInfo directoryInfo1 = new DirectoryInfo(lhsPath);
            DirectoryInfo directoryInfo2 = new DirectoryInfo(rhsPath);
            string searchPattern = "*.*";
            int num1 = 1;
            List<FileInfo> list1 = Enumerable.ToList<FileInfo>((IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo1.GetFiles(searchPattern, (SearchOption)num1), (Func<FileInfo, string>)(info => info.FullName)));
            List<FileInfo> list2 = Enumerable.ToList<FileInfo>((IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo2.GetFiles("*.*", SearchOption.AllDirectories), (Func<FileInfo, string>)(info => info.FullName)));
            List<FileInfo> list3 = new List<FileInfo>();
            List<FileInfo> list4 = new List<FileInfo>();
            List<FileInfo> list5 = new List<FileInfo>();
            List<FileInfo> list6 = new List<FileInfo>();
            int num2 = 0;
            bool flag = false;
            for (int index1 = 0; index1 < list1.Count; ++index1)
            {
                for (int index2 = 0; index2 < list2.Count; ++index2)
                {
                    if (list1[index1].FullName.Remove(0, lhsPath.Length).Equals(list2[index2].FullName.Remove(0, rhsPath.Length), StringComparison.OrdinalIgnoreCase))
                    {
                        if (list1[index1].Length != list2[index2].Length)
                            list5.Add(list1[index1]);
                        else
                            list6.Add(list1[index1]);
                        for (int index3 = num2; index3 < index2; ++index3)
                            list4.Add(list2[index3]);
                        num2 = index2 + 1;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    list3.Add(list1[index1]);
                flag = false;
            }
            if (!Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list5) && !Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list3) && (!Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list4) && list1.Count == list2.Count))
                return true;
            StringBuilder stringBuilder = new StringBuilder();
            if (Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list5))
                stringBuilder.AppendLine(string.Format("Files have different size: {0}. ", (object)string.Join<FileInfo>("; ", (IEnumerable<FileInfo>)list5)));
            if (Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list3))
                stringBuilder.AppendLine(string.Format("Only in the first folder: {0}. ", (object)string.Join("; ", Enumerable.Select<FileInfo, string>((IEnumerable<FileInfo>)list3, (Func<FileInfo, string>)(f => f.FullName)))));
            if (Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list4))
                stringBuilder.AppendLine(string.Format("Only in the second folder: {0}. ", (object)string.Join("; ", Enumerable.Select<FileInfo, string>((IEnumerable<FileInfo>)list4, (Func<FileInfo, string>)(f => f.FullName)))));
            if (Enumerable.Any<FileInfo>((IEnumerable<FileInfo>)list6))
                stringBuilder.AppendLine(string.Format("Common files: {0}. ", (object)string.Join("; ", Enumerable.Select<FileInfo, string>((IEnumerable<FileInfo>)list6, (Func<FileInfo, string>)(f => f.FullName)))));
            if (list1.Count != list2.Count)
                stringBuilder.AppendLine(string.Format("Different files count: first folder: {0}, second folder: {1}.", (object)list1.Count, (object)list2.Count));
            differencesLog = stringBuilder.ToString();
            return false;
        }

        public static bool AreFoldersEqual(string lhsPath, string rhsPath, out List<string> commonFiles, out List<string> onlyInLeftFiles, out List<string> onlyInRightFiles)
        {
            //Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            //Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            //Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            DirectoryInfo directoryInfo1 = new DirectoryInfo(lhsPath);
            DirectoryInfo directoryInfo2 = new DirectoryInfo(rhsPath);
            string searchPattern = "*.*";
            int num = 1;
            List<FileInfo> list1 = Enumerable.ToList<FileInfo>((IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo1.GetFiles(searchPattern, (SearchOption)num), (Func<FileInfo, string>)(info => info.FullName)));
            List<FileInfo> list2 = Enumerable.ToList<FileInfo>((IEnumerable<FileInfo>)Enumerable.OrderBy<FileInfo, string>((IEnumerable<FileInfo>)directoryInfo2.GetFiles("*.*", SearchOption.AllDirectories), (Func<FileInfo, string>)(info => info.FullName)));
            GenericComparer<FileInfo> comparerByNameAndLength = DirectoryExtension.GetFileInfoComparerByNameAndLength();
            if (Enumerable.SequenceEqual<FileInfo>((IEnumerable<FileInfo>)list1, (IEnumerable<FileInfo>)list2, (IEqualityComparer<FileInfo>)comparerByNameAndLength))
            {
                commonFiles = new List<string>();
                onlyInLeftFiles = new List<string>();
                onlyInRightFiles = new List<string>();
                return true;
            }
            IEnumerable<FileInfo> source1 = Enumerable.Intersect<FileInfo>((IEnumerable<FileInfo>)list1, (IEnumerable<FileInfo>)list2, (IEqualityComparer<FileInfo>)comparerByNameAndLength);
            commonFiles = Enumerable.ToList<string>(Enumerable.Select<FileInfo, string>(source1, (Func<FileInfo, string>)(file => file.FullName)));
            IEnumerable<FileInfo> source2 = Enumerable.Except<FileInfo>(Enumerable.Select<FileInfo, FileInfo>((IEnumerable<FileInfo>)list1, (Func<FileInfo, FileInfo>)(file => file)), (IEnumerable<FileInfo>)list2, (IEqualityComparer<FileInfo>)comparerByNameAndLength);
            onlyInLeftFiles = Enumerable.ToList<string>(Enumerable.Select<FileInfo, string>(source2, (Func<FileInfo, string>)(file => file.FullName)));
            IEnumerable<FileInfo> source3 = Enumerable.Except<FileInfo>(Enumerable.Select<FileInfo, FileInfo>((IEnumerable<FileInfo>)list2, (Func<FileInfo, FileInfo>)(file => file)), (IEnumerable<FileInfo>)list1, (IEqualityComparer<FileInfo>)comparerByNameAndLength);
            onlyInRightFiles = Enumerable.ToList<string>(Enumerable.Select<FileInfo, string>(source3, (Func<FileInfo, string>)(file => file.FullName)));
            return false;
        }

       /* public static bool AreFoldersEqualWithTimeout(string lhsPath, string rhsPath, TimeSpan timeoutTimeSpan)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            return DirectoryExtension.AreFoldersEqualWithTimeout(lhsPath, rhsPath, timeoutTimeSpan, TimeSpan.FromSeconds(5.0));
        }#1#

       /* public static bool AreFoldersEqualWithTimeout(string lhsPath, string rhsPath, TimeSpan timeoutTimeSpan, TimeSpan timeBetweenAttempts)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            return Executor.SpinWait((Func<bool>)(() => DirectoryExtension.AreFoldersEqual(lhsPath, rhsPath)), timeoutTimeSpan, timeBetweenAttempts);
        }#1#

        public static bool AreFoldersNotEqualWithTimeout(string lhsPath, string rhsPath, TimeSpan timeout)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            return DirectoryExtension.AreFoldersNotEqualWithTimeout(lhsPath, rhsPath, timeout, TimeSpan.FromSeconds(5.0));
        }

        public static bool AreFoldersNotEqualWithTimeout(string lhsPath, string rhsPath, TimeSpan timeout, TimeSpan timeBetweenAttempts)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(lhsPath, "lhsPath must not be null or white space");
            Guard.ArgumentIsNotNullOrWhiteSpace(rhsPath, "rhsPath must not be null or white space");
            Guard.Argument((DirectoryExtension.IsDirectoryExists(lhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)lhsPath);
            Guard.Argument((DirectoryExtension.IsDirectoryExists(rhsPath, false) ? 1 : 0) != 0, "{0} must exists", (object)rhsPath);
            return Executor.SpinWait((Func<bool>)(() => !DirectoryExtension.AreFoldersEqual(lhsPath, rhsPath)), timeout, timeBetweenAttempts);
        }

        public static string[] SafeGetFiles(string filePath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(filePath, "filePath must not be null or white space");
            if (Directory.Exists(filePath))
                return Directory.GetFiles(filePath, searchPattern, searchOption);
            return new string[0];
        }

        public static IEnumerable<string> GetFiles(string root, List<string> foldersWithoutAccess)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                string path = pending.Pop();
                foreach (string str in Directory.EnumerateFiles(path))
                    yield return str;
                foreach (string path1 in Directory.GetDirectories(path))
                {
                    if (!foldersWithoutAccess.Contains(Path.GetFileName(path1)))
                        pending.Push(path1);
                }
                path = (string)null;
            }
        }

        public static void DeleteFiles(string path, string searchPattern)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(path, "path must not be null or white space");
            Guard.ArgumentIsNotNullOrWhiteSpace(searchPattern, "searchPattern must not be null or white space");
            Enumerable.ToList<string>((IEnumerable<string>)Directory.GetFiles(path, searchPattern)).ForEach(new Action<string>(DirectoryExtension.DeleteFileWithUnlock));
        }

        public static void DeleteFileWithUnlock(string path)
        {
            Guard.ArgumentIsNotNullOrWhiteSpace(path, "path must not be null or white space");
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        private static GenericComparer<FileInfo> GetFileInfoComparerByNameAndLength()
        {
            Func<FileInfo, FileInfo, bool> func = (Func<FileInfo, FileInfo, bool>)((etalonInfo, resultInfo) =>
            {
                if (etalonInfo.Name.Equals(resultInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    return etalonInfo.Length == resultInfo.Length;
                return false;
            });
            Func<FileInfo, FileInfo, bool> comparator;
            return new GenericComparer<FileInfo>(comparator, (Func<FileInfo, int>)(fileInfo => string.Format("{0}{1}", (object)fileInfo.Name, (object)fileInfo.Length).GetHashCode()));
        }*/

        public static string CreateTempDirectory()
        {
            string path = Path.Combine("C:\\", Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        public static string CreateTempDirectory(string folderPath)
        {
            string path = Path.Combine(folderPath, Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }/*

        public static bool PathEqual(string path1, string path2)
        {
            path1 = path1.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            path2 = path2.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsDirectoryExists(string path, bool withoutRedirection = false)
        {
            if (!withoutRedirection)
                return Directory.Exists(path);
            using (new Wow64FsRedirectionDisabler())
                return Directory.Exists(path);
        }

        public static string CopyDirectoryToTempDirectory(string directory)
        {
            Guard.ArgumentIsNotNullOrEmpty(directory, "directory must not be null or empty");
            string tempDirectory = DirectoryExtension.CreateTempDirectory();
            DirectoryExtension.CopyDirectory(directory, tempDirectory);
            return tempDirectory;
        }

        public static void SetPermissionsForAllFilesInDirectory(string directory, FileAttributes attributes)
        {
            Guard.ArgumentIsNotNullOrEmpty(directory, "directory must not be null or empty");
            Enumerable.ToList<string>((IEnumerable<string>)Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)).ForEach((Action<string>)(file => File.SetAttributes(file, attributes)));
        }

        public static void RemoveAttributeFromFiles(string directory, FileAttributes attribute)
        {
            Guard.ArgumentIsNotNullOrEmpty(directory, "directory must not be null or empty");
            Enumerable.ToList<string>((IEnumerable<string>)Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)).ForEach((Action<string>)(file =>
            {
                FileAttributes fileAttributes = File.GetAttributes(file) & ~attribute;
                File.SetAttributes(file, fileAttributes);
            }));
        }*/
    }
}
