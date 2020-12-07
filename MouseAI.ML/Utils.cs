// Utils Class:
// Misc formatting helpers

using System;

namespace MouseAI.ML
{
    public class Utils
    {
        public static string GetDateTime_Formatted()
        {
            return DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
        }

        public static string GetFileWithExtension(string path, string filename, string extension)
        {
            return string.Format(@"{0}\{1}.{2}", path, filename, extension);
        }

        public static string GetFileWithoutExtension(string path, string filename)
        {
            return string.Format(@"{0}\{1}.", path, filename);
        }
    }
}
