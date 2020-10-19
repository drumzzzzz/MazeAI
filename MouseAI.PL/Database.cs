using System;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace MouseAI.PL
{
    public class Database
    {
        private readonly string db_file;
        private const string DB_PATH = "URI=file:";
        public string err { get; set; }

        public Database(string db_file)
        {
            if (!FileIO.CheckFileName(db_file))
                throw new Exception(db_file + " not found");

            this.db_file = DB_PATH + db_file;
        }

        public bool Insert(string table,string columns, string values)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(db_file);
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", table, columns, values)
                };

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

        public object Read(string table, string column, string value, object obj)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(db_file);
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", table, column, value)
                };

                SQLiteDataReader rdr = cmd.ExecuteReader();
                Type ObjType = obj.GetType();
                bool isFound;

                rdr.Read();
                if (!rdr.HasRows || rdr.FieldCount <= 0)
                    throw new Exception("No Row Returned");

                foreach (FieldInfo item in ObjType.GetRuntimeFields().Where(x=>x.IsStatic == false))
                {
                    isFound = false;
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        string name = string.Format("<{0}>", rdr.GetName(i));
                        if (item.Name.Contains(string.Format("<{0}>",rdr.GetName(i))) && rdr.GetFieldType(i) == item.FieldType)
                        {
                            var v = rdr.GetValue(i);
                            item.SetValue(obj, v);
                            isFound = true;
                            break;
                        }
                    }
                    if(!isFound)
                        throw new Exception("Error Reading Fields");
                }

                return obj;
            }
            catch (Exception e)
            {
                err = e.Message;
                return null;
            }
        }
    }
}
