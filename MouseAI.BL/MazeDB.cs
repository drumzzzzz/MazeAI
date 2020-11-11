#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MouseAI.PL;

#endregion

namespace MouseAI.BL
{
    public class MazeDb
    {
        private readonly Database db;
        private const string DB_NAME = "stats.db";

        public MazeDb()
        {
            try
            {
                string dbFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + DB_NAME;
                db = new Database(dbFile);
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
            return (DbTable_Mazes) db.ReadRow(DbTable_Mazes.TABLE, "Guid", guid, new DbTable_Mazes());
        }

        public bool InsertProject(DbTable_Projects tbl)
        {
            string values = string.Format("'{0}','{1}',{2},'{3}','{4}', '{5}'", 
                tbl.Guid, tbl.Accuracy, tbl.Epochs, tbl.Start, tbl.End, tbl.Log);

            return db.Insert(DbTable_Projects.TABLE, DbTable_Projects.COLUMNS, values);
        }

        public bool UpdateModelLast(string guid, string starttime)
        {
            string[] columns1 = {"Guid"};
            string[] values1 = {guid};
            if (!db.Update(DbTable_Projects.TABLE, columns1, values1, "isLast", "0"))
                return false;
            
            string[] columns2 = {"Guid", "Log"};
            string[] values2 = {guid, starttime};

            return db.Update(DbTable_Projects.TABLE, columns2, values2, "isLast", "1");
        }

        public string ReadModelLast(string guid)
        {
            string[] columns = {"Guid", "isLast"};
            string[] values = {guid, "1"};

            DbTable_Projects dbTable = 
                (DbTable_Projects)db.ReadRows(DbTable_Projects.TABLE, columns, values, new DbTable_Projects()).FirstOrDefault();

            return (dbTable != null && dbTable.isLast == "1" && !string.IsNullOrEmpty(dbTable.Log))
                ? dbTable.Log
                : string.Empty;
        }

        public List<object> ReadProjectGuids(string guid)
        {
            return db.ReadRows(DbTable_Projects.TABLE, "Guid", guid, new DbTable_Projects());
        }

        public int GetMazeCounts(List<string> guids)
        {
            return db.RowCount(DbTable_Mazes.TABLE, "Guid", guids);
        }

        public void DeleteMazeRecords(string guid)
        {
            db.DeleteRows(DbTable_Mazes.TABLE, "Guid", guid);
        }

        public void DeleteProjectRecords(string guid)
        {
            db.DeleteRows(DbTable_Projects.TABLE, "Guid", guid);
        }

        public int GetProjectCounts(string guid)
        {
            return db.RowCount(DbTable_Mazes.TABLE, "Guid", guid);
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
        public string isLast { get; set; }

        public static readonly string TABLE = "projects";
        public static readonly string COLUMNS = "Guid, Accuracy, Epochs, Start, End, Log, isLast";
    }
}
