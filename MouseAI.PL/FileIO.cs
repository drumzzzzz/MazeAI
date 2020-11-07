#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;

#endregion

namespace MouseAI.PL
{
    public class FileIO
    {
        #region Declarations

        private static int FileCount { get; set; }

        #endregion

        #region File Dialogs

        public static string SaveFileAs_Dialog(string directory, string extension)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (!string.IsNullOrEmpty(extension))
            {
                sfd.DefaultExt = extension;
                sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*".Replace("txt", extension);
            }

            if (!string.IsNullOrEmpty(directory))
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (Directory.Exists(directory))
                    sfd.InitialDirectory = directory;
            }

            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.Cancel)
                return string.Empty;
            return result == DialogResult.OK ? sfd.FileName : null;
        }

        public static string OpenFile_Dialog(string directory, string extension)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (!string.IsNullOrEmpty(extension))
            {
                ofd.DefaultExt = extension;
                ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*".Replace("txt", extension);
            }

            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                ofd.InitialDirectory = directory;
            }

            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.Cancel)
                return string.Empty;
            return result == DialogResult.OK ? ofd.FileName : null;
        }


        #endregion

        #region File IO Stream Related

        public static List<string> GetFiles(string path)
        {
            List<string> fileNames = new List<string>();
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                fileNames.AddRange(files);
            }
            return fileNames;
        }

        public static List<string> GetFiles(string path, string filters)
        {
            List<string> fpaths = new List<string>();
            List<string> fnames = new List<string>();

            if (Directory.Exists(path))
                fpaths = filters.Split('|').SelectMany(filter => Directory.GetFiles(path, filter)).ToList();

            int index;
            foreach (string fname in fpaths)
            {
                index = fname.LastIndexOf(@"\", StringComparison.Ordinal);
                if (index != -1)
                {
                    fnames.Add(fname.Substring(index + 1));
                }
            }
            return fnames;
        }

        public static bool WriteCreateFile(string FileName, List<string> stringVals)
        {
            try
            {
                FileStream fStream = new FileStream(FileName, FileMode.OpenOrCreate);
                StreamWriter sWriter = new StreamWriter(fStream);

                foreach (string sVal in stringVals)
                    sWriter.WriteLine(sVal);

                sWriter.Close();
                fStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Write Error:" + ex.Message);
                return false;
            }
        }

        public static bool WriteCreateFile(string FileName, string value)
        {
            try
            {
                FileStream fStream = new FileStream(FileName, FileMode.OpenOrCreate);
                StreamWriter sWriter = new StreamWriter(fStream);

                sWriter.WriteLine(value);
                sWriter.Close();
                fStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Write Error:" + ex.Message);
                return false;
            }
        }

        public static bool AppendFile(string FileName,List<string> stringVals)
        {
            try
            {
                StreamWriter sw = File.AppendText(FileName);
                foreach (string sVal in stringVals)
                    sw.WriteLine(sVal);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Append Error:" + ex.Message);
                return false;
            }
        }

        public static bool AppendFile(string FileName, string stringVal)
        {
            try
            {
                StreamWriter sw = File.AppendText(FileName);
                sw.WriteLine(stringVal);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Append Error:" + ex.Message);
                return false;
            }
        }

        public static string ReadFile(string filepath) // Loads, reads and returns a files text contents
        {                                                               
            StreamReader reader = new StreamReader(filepath);
            string strValues = reader.ReadToEnd();
            reader.Close();
            return strValues;
        }

        public static bool CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
                return false;
            }
            return true;
        }

        public static bool CheckFileName(string Value)
        {
            return File.Exists(Value);
        }

        public static bool CheckDriveDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static bool CheckCreateDirectory(string path)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Directory.Exists(path);
        }

        public static void CopyFile(string sourcepath,string filename,  string destinationpath, bool isOverWrite)
        {
            File.Copy(Path.Combine(sourcepath, filename), Path.Combine(destinationpath, filename), isOverWrite);
        }

        public static int CopyFilesDirectories(string source, string target, string[] extensions, bool isOverWrite)
        {
            FileCount = 0;
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(source);
                DirectoryInfo diTarget = new DirectoryInfo(target);
                CopyFileDirectories(diSource, diTarget, extensions, isOverWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("Copy Files Directories Error:" +e.Message);
            }
            return FileCount;
        }

        public static void CopyFileDirectories(DirectoryInfo source, DirectoryInfo target, string[] extensions, bool isOverWrite )
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFileDirectories(dir, target.CreateSubdirectory(dir.Name), extensions,isOverWrite);

            CopyFiles(source, target, extensions, isOverWrite);
        }

        public static int CopyFiles(string source, string target, string[] extensions, bool isOverWrite)
        {
            FileCount = 0;
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(source);
                DirectoryInfo diTarget = new DirectoryInfo(target);
                CopyFiles(diSource, diTarget, extensions, isOverWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("Copy Files Error:" + e.Message);
            }

            return FileCount;
        }

        private static void CopyFiles(DirectoryInfo source, FileSystemInfo target, string[] extensions, bool isOverWrite)
        { 
            foreach (FileInfo file in source.EnumerateFiles().Where(file => extensions.Contains(file.Extension)))
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), isOverWrite);
                FileCount++;
            }
        }

        public static int DeleteFiles(string source, string[] extensions)
        {
            FileCount = 0;
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(source);
                DeleteFiles(diSource, extensions);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete Files Error:" + e.Message);
            }

            return FileCount;
        }

        public static int DeleteFiles(List<string> files)
        {
            int count = 0;
            try
            {
                foreach (string file in files.Where(File.Exists))
                {
                    File.Delete(file);
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Deleting File:" + e.Message);
            }

            return count;
        }

        public static int DeleteAllFiles(string source, string[] filenames)
        {
            FileCount = 0;
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(source);
                DeleteFiles(diSource, filenames);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete Files Error:" + e.Message);
            }

            return FileCount;
        }

        private static void DeleteFiles(DirectoryInfo source, string[] extensions)
        {
            foreach (FileInfo file in source.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())))
            {
                file.Delete();
                FileCount++;
            }
        }

        private static void DeleteAllFiles(DirectoryInfo source, string[] names)
        {
            foreach (FileInfo file in source.EnumerateFiles().Where(f => names.Contains(f.FullName.ToLower())))
            {
                file.Delete();
                FileCount++;
            }
        }

        #endregion

        #region XML Related

        public static void SerializeXml(object obj, string filepath) // Saves a supplied object to an XML file
        {
            Type type = obj.GetType();

            XmlSerializer serializer = new XmlSerializer(type);
            StreamWriter writer = new StreamWriter(filepath);
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static object DeSerializeXml(Type type, string filepath) // Loads and deserializes an XML file
        {
            XmlSerializer serializer = new XmlSerializer(type);
            StreamReader reader = new StreamReader(filepath);
            object obj = serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static string ReadXml(string filepath, string[] Ignore)
        {
            if (File.Exists(filepath))
            {
                StringBuilder sb = new StringBuilder();
                XDocument xdoc = XDocument.Load(filepath);
                foreach (XElement element in xdoc.Descendants())
                {
                    if (!Ignore.Contains(element.Name.ToString()))
                    {
                        sb.Append(string.Format("{0}:{1}", element.Name, element.Value) + Environment.NewLine);
                    }
                }
                return sb.ToString();
            }

            return null;
        }

        #endregion

        #region Archive Related

        public static bool CreateZipArchive(string path, List<string> files, out string result)
        {
            result = string.Empty;

            if (string.IsNullOrEmpty(path))
                return false;

            FileInfo fi;
            string filename = new FileInfo(path).Name;

            try
            {
                using (var stream = File.OpenWrite(path))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    foreach (string item in files)
                    {
                        fi = new FileInfo(item);
                        archive.CreateEntryFromFile(fi.FullName, fi.Name);
                    }
                }

                if (!File.Exists(path))
                    throw new Exception(filename);

                result = string.Format("Created Archive: {0}", new FileInfo(path).Name);

                return true;
            }
            catch (Exception e)
            {
                result = string.Format("Error creating archive: {0}", e.Message);
                Console.WriteLine(result);
                return false;
            }
        }

        #endregion
    }
}
