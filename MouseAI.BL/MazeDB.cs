using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MouseAI.PL;

namespace MouseAI.BL
{
    public class MazeDb
    {
        private readonly Database db;
        private string db_file;
        private const string DB_NAME = "stats.db";
        private string err = string.Empty;

        public MazeDb()
        {
            try
            {
                db_file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + DB_NAME;
                db = new Database(db_file);
            }
            catch (Exception e)
            {
                throw new Exception("Error Loading Db:" + e.Message);
            }
        }

        public bool InsertStats(DbTable_Stats tbl)
        {
            string columns = "Guid, Success, Failure, LastUsed";
            string values = string.Format("'{0}', {1}, {2}, '{3}'", tbl.Guid, tbl.Success, tbl.Failure, tbl.LastUsed);

            return db.Insert(DbTable_Stats.TABLE, DbTable_Stats.COLUMNS, values);
        }

        public string GetError()
        {
            return err;
        }
    }

    public class DbTable_Stats
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
        public string LastUsed { get; set; }

        public static readonly string TABLE = "stats";
        public static readonly string COLUMNS = "Guid, Success, Failure, LastUsed";

    }
}
