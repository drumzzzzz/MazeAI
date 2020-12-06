#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

#endregion

namespace MouseAI.PL
{
    public class FileIO
    {
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

        public static List<string> ReadFileAsList(string filepath) // Loads, reads and returns a file text contents as list
        {
            StreamReader reader = new StreamReader(filepath);
            List<string> strValues = new List<string>();
            
            while (!reader.EndOfStream)
            {
                strValues.Add(reader.ReadLine());
            }

            reader.Close();
            return strValues;
        }

        public static bool CheckFileName(string Value)
        {
            return File.Exists(Value);
        }

        public static bool CheckDriveDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static bool CheckCreateDirectory(string path)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Directory.Exists(path);
        }

        public static bool MoveFiles(List<string> sources, string dest)
        {
            int count = 0;
            string file_name;
            string dest_file_path;

            foreach (string source_file_path in sources)
            {
                try
                {
                    file_name = Path.GetFileName(source_file_path);

                    if (string.IsNullOrEmpty(file_name))
                        throw new Exception(string.Format("Failed getting file name for {0}", source_file_path));

                    dest_file_path = string.Format(@"{0}\{1}", dest, file_name);
                    Console.WriteLine("Moving {0} to {1}", source_file_path, dest_file_path);

                    File.Move(source_file_path, dest_file_path);
                    if (File.Exists(dest_file_path))
                        count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Moved {0} Files", count);
            return count == sources.Count;
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

        public static XDocument ReadXml(string filepath)
        {
            return File.Exists(filepath) ? XDocument.Load(filepath) : null;
        }

        #endregion

        #region JSON

        public static string ParseJson(string json)
        {
            try
            {
                Dictionary<string, string> nodes = new Dictionary<string, string>();
                ParseJson(JObject.Parse(json), nodes);
                StringBuilder sb = new StringBuilder();
                foreach (string key in nodes.Keys.Where(key => !nodes[key].Contains(",,") 
                                                               && !nodes[key].Contains("{}")
                                                               && !nodes[key].Equals(string.Empty, StringComparison.OrdinalIgnoreCase)))
                {
                    sb.Append(string.Format("{0}:{1}", key, nodes[key]));
                    sb.Append(Environment.NewLine);
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        private static void ParseJson(JToken token, IDictionary<string, string> nodes, string parent = "")
        {
            if (token.HasValues)
            {
                foreach (JToken child in token.Children())
                {
                    if (token.Type == JTokenType.Property)
                    {
                        if (parent == string.Empty)
                            parent = ((JProperty)token).Name;
                        else
                            parent += "." + ((JProperty)token).Name;
                    }
                    ParseJson(child, nodes, parent);
                }
                return;
            }

            if (nodes.ContainsKey(parent))
                nodes[parent] += "," + token;
            else
                nodes.Add(parent, token.ToString());
        }

        #endregion
    }
}
