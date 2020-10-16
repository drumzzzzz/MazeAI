#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Reflection;
using System.Linq;

#endregion

namespace MouseAI.PL
{
    public class FileIO
    {
        #region Declarations

        private static int FileCount { get; set; }

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
            List<string> fNames = new List<string>();
            if (Directory.Exists(path))
                fNames = filters.Split('|').SelectMany(filter => Directory.GetFiles(path, filter)).ToList();
   
            return fNames;
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

        private static void DeleteFiles(DirectoryInfo source, string[] extensions)
        {
            foreach (FileInfo file in source.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())))
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
        #endregion
    }
}
