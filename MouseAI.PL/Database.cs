using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI.PL
{
    public class Database
    {
        private string db_file;
        private const string DB_PATH = "URI=file:";
        public string err { get; set; }

        public Database(string db_file)
        {
            if (!FileIO.CheckFileName(db_file))
                throw new Exception(db_file + " not found");

            this.db_file = DB_PATH + db_file;
        }

        public bool Create(string table, string columns)
        {
            

            try
            {
                SQLiteConnection conn = new SQLiteConnection();
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);

                cmd.CommandText = "DROP TABLE IF EXISTS " + table;
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("CREATE TABLE {0} ({1})", table, columns);
                cmd.ExecuteNonQuery();

                conn.Close();

                return true;
            }
            catch (Exception e)
            {
                err = e.Message;
                return false;
            }
        }

        public bool Insert(string table,string columns, string values)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(db_file);
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);

                cmd.CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", table, columns, values);
                cmd.ExecuteNonQuery();

                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                err = e.Message;
                return false;
            }
        }
    }
}
