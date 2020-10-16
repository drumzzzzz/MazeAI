using System.Xml.Serialization;

namespace MouseAI
{
    public class Settings
    {
        // Non Serialized
        [XmlIgnore]
        public const string SETTINGS_FILE = "Settings.xml";
        
        // Serialized Values
        public string LastFileName { get; set; }
        public bool isLoadLast { get; set; }
        public bool isDebugConsole { get; set; }
        public int DebugLevel { get; set; }
    }
}
