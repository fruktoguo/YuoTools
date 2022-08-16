using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sirenix.Utilities;

namespace YuoTools.Extend.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// 获取文件（单层目录）
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="pattern">通配符</param>
        /// <returns></returns>
        public static List<string> GetFile(string path, string pattern = "*")
        {
            try
            {
                if (Directory.Exists(path))
                {
                    List<string> result = Directory.EnumerateFiles(path, pattern).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        /// <summary>
        /// 获取目录下的所有文件
        /// 防止遇到（$文件夹报错无法获取目录的错误）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public static List<string> GetAllFile(string path, string[] patterns = null)
        {
            List<string> allpath = GetAllPath(path);
            if (allpath == null) allpath = new List<string>();
            allpath.Add(path);
            return GetAllFile(allpath, patterns);
        }

        /// <summary>
        /// 获取文件（多个目录）
        /// </summary>
        /// <param name="paths">路径（支持多个路径）</param>
        /// <param name="patterns">通配符（支持多个通配符）</param>
        /// <returns></returns>
        public static List<string> GetAllFile(List<string> paths, string[] patterns = null)
        {
            List<string> result = new List<string>();
            if (!paths.IsNullOrEmpty())
            {
                foreach (var path in paths)
                {
                    if (!patterns.IsNullOrEmpty())
                    {
                        foreach (var pattern in patterns)
                        {
                            List<string> temp = GetFile(path, pattern);
                            if (!temp.IsNullOrEmpty()) result.AddRange(temp);
                        }
                    }
                    else
                    {
                        List<string> temp = GetFile(path);
                        if (!temp.IsNullOrEmpty()) result.AddRange(temp);
                    }
                }
            }

            return result;
        }

        public static void GetAllFiles(List<string> files, string dir)
        {
            var fls = Directory.GetFiles(dir);
            foreach (var fl in fls) files.Add(fl);

            var subDirs = Directory.GetDirectories(dir);
            foreach (var subDir in subDirs) GetAllFiles(files, subDir);
        }

        /// <summary>
        /// 获取文件夹下所有指定后缀名的文件
        /// </summary>
        public static List<string> GetAllFilesOfExtension(string path, string extensionName)
        {
            var list = new List<string>();
            var dirs = Directory.GetFiles(path, "*." + extensionName);
            foreach (var dir in dirs)
            {
                list.Add(dir);
            }

            return list;
        }

        public static void CleanDirectory(string dir)
        {
            foreach (var subdir in Directory.GetDirectories(dir)) Directory.Delete(subdir, true);

            foreach (var subFile in Directory.GetFiles(dir)) File.Delete(subFile);
        }

        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            var source = new DirectoryInfo(srcDir);
            var target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("父目录不能拷贝到子目录！");

            if (!source.Exists) return;

            if (!target.Exists) target.Create();

            var files = source.GetFiles();

            for (var i = 0; i < files.Length; i++)
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);

            var dirs = source.GetDirectories();

            for (var j = 0; j < dirs.Length; j++)
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
        }

        public static void ReplaceExtensionName(string srcDir, string extensionName, string newExtensionName)
        {
            if (Directory.Exists(srcDir))
            {
                var fls = Directory.GetFiles(srcDir);

                foreach (var fl in fls)
                    if (fl.EndsWith(extensionName))
                    {
                        File.Move(fl, fl.Substring(0, fl.IndexOf(extensionName)) + newExtensionName);
                        File.Delete(fl);
                    }

                var subDirs = Directory.GetDirectories(srcDir);

                foreach (var subDir in subDirs) ReplaceExtensionName(subDir, extensionName, newExtensionName);
            }
        }

        public static bool CopyFile(string sourcePath, string targetPath, bool overwrite)
        {
            string sourceText = null;
            string targetText = null;

            if (File.Exists(sourcePath)) sourceText = File.ReadAllText(sourcePath);

            if (File.Exists(targetPath)) targetText = File.ReadAllText(targetPath);

            if (sourceText != targetText && File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, overwrite);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     读取文本文件内容,不存在则会创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllText(string path)
        {
            await CheckFilePathOrCreate(path);
            return await File.ReadAllTextAsync(path);
        }

        public static async Task WriteAllText(string path, string text)
        {
            await CheckFilePathOrCreate(path);
            await File.WriteAllTextAsync(path, text);
        }

        /// <summary>
        ///     判断文件夹是否存在,不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static async Task CheckFilePathOrCreate(string path)
        {
            //判断路径文件是否存在,不存在则创建
            if (!File.Exists(path))
            {
                //创建文件夹
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(path, "");
            }
        }

        public static bool CheckFilePath(string path)
        {
            return File.Exists(path);
        }

        public static void CheckDirectoryPath(string path)
        {
            if (!File.Exists(path))
            {
                //创建文件夹
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
        }

        public static async Task<FileInfo> GetFileInfo(string path)
        {
            await CheckFilePathOrCreate(path);
            return new FileInfo(path);
        }

        /// <summary>
        ///     获取文件夹下的所有文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetAllDirectory(string path)
        {
            var list = new List<string>();
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                list.Add(dir);
                list.AddRange(GetAllDirectory(dir));
            }

            return list;
        }


        public static List<string> GetChildDirectory(string path)
        {
            return Directory.GetDirectories(path).ToList();
        }

        /// <summary>
        /// 创建文件目录（文件不存在则创建）
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// 如果文件已存在，返回true
        /// 如果文件不存在，则创建文件，成功返回true，失败返回false
        /// </returns>
        public static bool Create(string path)
        {
            if (Directory.Exists(path))
                return true;
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }

        /// <summary>
        /// 获取目录的父目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Parent(string path)
        {
            string p = path;
            if (!string.IsNullOrWhiteSpace(p))
            {
                while (p.EndsWith("\\")) p = p.Substring(0, p.Length - 1);
                if (StringHelper.SubStringCount(p, "\\") >= 1)
                {
                    try
                    {
                        return Directory.GetParent(p).ToString();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// 获取目录下的目录（一层）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetPath(string path)
        {
            if (Directory.Exists(path))
                try
                {
                    return Directory.EnumerateDirectories(path).ToList();
                }
                catch (Exception e)
                {
                }

            return null;
        }

        /// <summary>
        /// 获取目录下所有目录（递归）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetAllPath(string path)
        {
            List<string> result = GetPath(path);
            if (!result.IsNullOrEmpty())
            {
                List<string> temp = new List<string>();
                foreach (var item in result)
                {
                    List<string> t = GetAllPath(item);
                    if (!t.IsNullOrEmpty())
                        temp.AddRange(t);
                }

                result.AddRange(temp);
                return result;
            }

            return null;
        }

        /// <summary>
        /// 判断目录是否为磁盘
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDriver(string path)
        {
            if (path != null && path.Length >= 2)
            {
                if (path.Substring(1, 1) == ":")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取文件所在的目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFilePath(string filePath)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                result = filePath.Substring(0, filePath.Length - fileName.Length);
            }

            return result;
        }

        /// <summary>
        /// 连接多个string构成目录
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            if (!paths.IsNullOrEmpty())
            {
                if (paths.Length > 1)
                {
                    StringBuilder result = new StringBuilder(paths[0]);
                    for (int i = 1; i < paths.Length; i++)
                    {
                        result.Append("\\");
                        result.Append(paths[i]);
                    }

                    while (result.ToString().IndexOf("\\\\") >= 0)
                    {
                        result.Replace("\\\\", "\\");
                    }

                    return result.ToString();
                }
                else
                {
                    return paths[0];
                }
            }

            return "";
        }

        /// <summary>
        /// 路径包含关系
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns>
        /// -1：不存在包含关系
        /// 0：两个目录相同
        /// 1：path1 包含 path2（path1 大）
        /// 2：path2 包含 path1（path2 大）
        /// </returns>
        public static int Include(string path1, string path2)
        {
            if (path1 == path2) return 0; //两个目录相同

            string p1 = Combine(path1 + "\\");
            string p2 = Combine(path2 + "\\");

            if (p1 == p2) return 0; //两个目录相同（防止路径后有带\或不带\的情况）
            if (p1.Length > p2.Length && p1.Contains(p2)) return 1; //path1 包含 path2（path1 大）
            if (p2.Length > p1.Length && p2.Contains(p1)) return 2; //path2 包含 path1（path2 大）

            return -1; //不存在包含关系
        }

        public static string GetPathName(string s)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(s))
            {
                char[] c = s.ToArray();
                for (int i = c.Length - 1; i >= 0; i--)
                {
                    if (c[i] != '\\')
                    {
                        sb.Append(c[i]);
                    }
                    else
                    {
                        if (sb.Length > 0) break;
                    }
                }

                char[] mirror = sb.ToString().ToArray();
                sb.Clear();
                for (int i = mirror.Length - 1; i >= 0; i--)
                {
                    sb.Append(mirror[i]);
                }
            }

            return sb.ToString();
        }

        public static long GetPathSize(string path)
        {
            long size = 0, _s = 0;
            try
            {
                List<string> files = GetAllFile(path);
                if (!files.IsNullOrEmpty())
                {
                    foreach (var f in files)
                    {
                        if (File.Exists(f))
                        {
                            _s = Size(f);
                            if (_s >= 0) size += _s;
                        }
                    }
                }
            }
            catch
            {
            }

            return size;
        }


        /// <summary>
        /// 计算文件的 MD5 值
        /// </summary>
        /// <param name="fileName">要计算 MD5 值的文件名和路径</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string GetFileMD5(string fileName)
        {
            return HashFile(fileName, "md5");
        }

        /// <summary>
        /// 计算文件的 sha1 值
        /// </summary>
        /// <param name="fileName">要计算 sha1 值的文件名和路径</param>
        /// <returns>sha1 值16进制字符串</returns>
        public static string GetFileSHA1(string fileName)
        {
            return HashFile(fileName, "sha1");
        }

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fileName">要计算哈希值的文件名和路径</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(string fileName, string algName)
        {
            if (!System.IO.File.Exists(fileName))
                return string.Empty;

            System.IO.FileStream fs =
                new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] hashBytes = FileHashData(fs, algName);
            fs.Close();
            return ByteArrayToHexString(hashBytes);
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] FileHashData(System.IO.Stream stream, string algName)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName 不能为 null");
            }

            if (string.Compare(algName, "sha1", true) == 0)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else
            {
                if (string.Compare(algName, "md5", true) != 0)
                {
                    throw new Exception("algName 只能使用 sha1 或 md5");
                }

                algorithm = System.Security.Cryptography.MD5.Create();
            }

            return algorithm.ComputeHash(stream);
        }

        /// <summary>
        /// 字节数组转换为16进制表示的字符串
        /// </summary>
        private static string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }

        /// <summary>
        /// 判断字符串是文件路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsFile(string s)
        {
            if (File.Exists(s)) return true;
            return false;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static void Delete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 删除文件（多个）
        /// </summary>
        /// <param name="files">文件路径（支持多个文件路径）</param>
        /// <returns></returns>
        public static void Delete(string[] files)
        {
            if (!files.IsNullOrEmpty())
            {
                foreach (var file in files)
                {
                    Delete(file);
                }
            }
        }

        /// <summary>
        /// 获取文件的大小（字节数）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static long Size(string fileName)
        {
            long result = -1;
            if (File.Exists(fileName))
            {
                try
                {
                    FileInfo fi = new FileInfo(fileName);
                    result = fi.Length;
                }
                catch (Exception e)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// 获取多个文件的大小（字节数）
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static long[] Size(List<string> files)
        {
            long[] result = new long[files.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Size(files[i]);
            }

            return result;
        }

        /// <summary>
        /// 获取文件大小（根据单位换算）
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="unit">B，KB，MB，GB</param>
        /// <returns></returns>
        public static double Size(string fileName, string unit)
        {
            return ByteConvertHelper.Cvt(Size(fileName), unit);
        }

        /// <summary>
        /// 获取文件大小信息（自动适配）（如：1MB，10KB...）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string SizeFormat(string fileName)
        {
            return ByteConvertHelper.Fmt(Size(fileName));
        }

        /// <summary>
        /// 获取文件的MD5特征码
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMD5(string file)
        {
            string result = string.Empty;
            if (!File.Exists(file)) return result;

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                HashAlgorithm algorithm = MD5.Create();
                byte[] hashBytes = algorithm.ComputeHash(fs);
                result = BitConverter.ToString(hashBytes).Replace("-", "");
            }

            return result;
        }

        /// <summary>
        /// 获取多个文件的MD5特征码
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string[] GetMD5(List<string> files)
        {
            string[] result = new string[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                result[i] = GetMD5(files[i]);
            }

            return result;
        }

        public static bool Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (File.Exists(sourceFileName))
            {
                string destPath = GetFilePath(destFileName);
                if (Create(destPath))
                {
                    try
                    {
                        File.Copy(sourceFileName, destFileName, overwrite);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }
    }
}