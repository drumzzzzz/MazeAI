using System;
using System.IO;
using System.Xml.Serialization;
using MouseAI.PL;

namespace MouseAI
{
    public class Settings
    {
        // Non Serialized
        [XmlIgnore]
        public const string SETTINGS_FILE = "Settings.xml";

        [XmlIgnore] 
        public static string Error;

        // Serialized Values
        public string LastFileName { get; set; }
        public string Guid { get; set; }
        public bool isLoadLast { get; set; }
        public bool isAutoRun { get; set; }
        public bool isDebugConsole { get; set; }
        public bool isMazeSegments { get; set; }
        public string PythonPath { get; set; }

        public static bool isSettings()
        {
            return FileIO.CheckFileName(SETTINGS_FILE);
        }

        public static Settings Load()
        {
            try
            {
                return (Settings)FileIO.DeSerializeXml(typeof(Settings), Settings.SETTINGS_FILE);
            }
            catch (Exception e)
            {
                Error = string.Format("Error Loading Settings: {0}", e.Message);
                return null;
            }
        }

        public static Settings Create()
        {
            try
            {
                Settings oSettings = new Settings()
                {
                    LastFileName = string.Empty,
                    isLoadLast = true,
                    isAutoRun = true
                };
                FileIO.SerializeXml(oSettings, SETTINGS_FILE);
                return (Settings)FileIO.DeSerializeXml(typeof(Settings), Settings.SETTINGS_FILE);
            }
            catch (Exception e)
            {
                Error = string.Format("Error Creating Settings: {0}", e.Message);
                return null;
            }
        }

        public static Settings Update(Settings oSettings)
        {
            try
            {
                FileIO.SerializeXml(oSettings, SETTINGS_FILE);
                return (Settings)FileIO.DeSerializeXml(typeof(Settings), Settings.SETTINGS_FILE);
            }
            catch (Exception e)
            {
                Error = string.Format("Error Updating Settings: {0}", e.Message);
                return null;
            }
        }
    }
}
