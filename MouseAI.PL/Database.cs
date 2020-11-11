using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MouseAI.PL
{
    public class Database
    {
        private readonly string db_file;
        private const string DB_PATH = "URI=file:";

        public Database(string db_file)
        {
            if (!FileIO.CheckFileName(db_file))
                throw new Exception(db_file + " not found");

            this.db_file = DB_PATH + db_file;
        }

        private static void CloseConnection(SQLiteConnection conn)
        {
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Cancel();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool Insert(string table,string columns, string values)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);
            
            try
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", table, columns, values),
                    CommandTimeout = 5
                };

                cmd.ExecuteNonQuery();
                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return false;
            }
        }

        public bool Update(string table, string[] columns, string[] values,string target_column, string target_value)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);

            try
            {
                if (columns == null || columns.Length == 0 || values == null || values.Length == 0 || columns.Length != values.Length)
                {
                    throw new Exception("Invalid update parameters");
                }

                if (string.IsNullOrEmpty(target_value) || string.IsNullOrEmpty(target_column))
                {
                    throw new Exception("Invalid update targets");
                }

                if (columns.Where((t, idx) => string.IsNullOrEmpty(t) || string.IsNullOrEmpty(values[idx])).Any())
                {
                    throw new Exception("Invalid update comparison parameters");
                }

                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("UPDATE {0} SET {1} = '{2}' WHERE ", table, target_column, target_value));

                for (int idx = 0; idx < columns.Length; idx++)
                {
                    if (idx > 0)
                        sb.Append(" AND ");
                    sb.Append(string.Format("{0} = '{1}'", columns[idx], values[idx]));
                }

                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = sb.ToString(),
                    CommandTimeout = 5
                };

                Console.WriteLine("Update: {0}", sb);

                cmd.ExecuteNonQuery();
                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return false;
            }
        }

        public object ReadRow(string table, string column, string value, object obj)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", table, column, value)
                };

                SQLiteDataReader rdr = cmd.ExecuteReader();
                Type ObjType = obj.GetType();
                bool isFound;
                // string name;

                rdr.Read();
                if (!rdr.HasRows || rdr.FieldCount <= 0)
                    throw new Exception("No Row Returned");

                foreach (FieldInfo item in ObjType.GetRuntimeFields().Where(x=>x.IsStatic == false))
                {
                    isFound = false;
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        // name = string.Format("<{0}>", rdr.GetName(i));
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
                rdr.Close();
                CloseConnection(conn);

                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return null;
            }
        }

        public List<object> ReadRows(string table, string column, string value, object obj)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", table, column, value)
                };

                SQLiteDataReader rdr = cmd.ExecuteReader();
                Type ObjType = obj.GetType();
                bool isFound = false;

                List<object> objList = new List<object>();

                while (rdr.Read())
                {
                    if (!rdr.HasRows || rdr.FieldCount <= 0)
                        throw new Exception("No Row Returned");

                    object instance = Activator.CreateInstance(ObjType);

                    foreach (FieldInfo item in ObjType.GetRuntimeFields().Where(x => x.IsStatic == false))
                    {
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            if (item.Name.Contains(string.Format("<{0}>", rdr.GetName(i))) &&
                                rdr.GetFieldType(i) == item.FieldType)
                            {
                                var v = rdr.GetValue(i);
                                item.SetValue(instance, v);
                                isFound = true;
                                break;
                            }
                        }
                    }

                    if (!isFound)
                         throw new Exception("Error Reading Fields");
                    objList.Add(instance);
                }
                rdr.Close();
                CloseConnection(conn);

                return objList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return null;
            }
        }

        public void DeleteRows(string table, string column, string value)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn)
                {
                    CommandText = string.Format("DELETE FROM {0} WHERE {1} = '{2}'", table, column, value)
                };

                cmd.ExecuteNonQuery();
                CloseConnection(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
            }
        }

        public int RowCount(string table, string column, List<string> values)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);
            int rowcount = 0;
            object result;

            try
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(conn);

                foreach (string value in values)
                {
                    cmd.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'", table, column, value);
                    result = cmd.ExecuteScalar();
                    rowcount += (result != null) ? Convert.ToInt32(result) : 0;
                }
                CloseConnection(conn);
                return rowcount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return -1;
            }
        }

        public int RowCount(string table, string column, string value)
        {
            SQLiteConnection conn = new SQLiteConnection(db_file);
            int rowcount = 0;
            object result;

            try
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(conn);

                cmd.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'", table, column, value);
                result = cmd.ExecuteScalar();
                rowcount += (result != null) ? Convert.ToInt32(result) : 0;
     
                CloseConnection(conn);
                return rowcount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(conn);
                return -1;
            }
        }
    }
}
