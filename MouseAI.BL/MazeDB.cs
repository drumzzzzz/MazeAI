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

        public bool InsertMaze(DbTable_Mazes tbl)
        {
            string values = string.Format("'{0}', {1}, {2}, '{3}'", tbl.Guid, tbl.Success, tbl.Failure, tbl.LastUsed);

            return db.Insert(DbTable_Mazes.TABLE, DbTable_Mazes.COLUMNS, values);
        }

        public DbTable_Mazes ReadMazes(string guid)
        {
            return (DbTable_Mazes) db.Read(DbTable_Mazes.TABLE, "Guid", guid, new DbTable_Mazes());
        }

        public bool InsertProject(DbTable_Projects tbl)
        {
            string values = string.Format("'{0}','{1}',{2},'{3}','{4}', '{5}'", 
                tbl.Guid, tbl.Accuracy, tbl.Epochs, tbl.Start, tbl.End, tbl.Log);

            return db.Insert(DbTable_Projects.TABLE, DbTable_Projects.COLUMNS, values);
        }

        public string GetError()
        {
            return err;
        }
    }

    public class DbTable_Mazes
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public long Success { get; set; }
        public long Failure { get; set; }
        public string LastUsed { get; set; }

        public static readonly string TABLE = "mazes";
        public static readonly string COLUMNS = "Guid, Success, Failure, LastUsed";
    }

    public class DbTable_Projects
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public double Accuracy { get; set; }
        public int Epochs { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Log { get; set; }

        public static readonly string TABLE = "projects";
        public static readonly string COLUMNS = "Guid, Accuracy, Epochs, Start, End, Log";
    }
}
