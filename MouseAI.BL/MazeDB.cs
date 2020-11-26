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

        public bool InsertProject(DbTable_Projects tbl)
        {
            string values = string.Format("'{0}','{1}',{2},'{3}','{4}', '{5}'", 
                tbl.Guid, tbl.Accuracy, tbl.Start, tbl.End, tbl.Log, tbl.isLast);

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

        public void DeleteProjectRecords(string guid)
        {
            db.DeleteRows(DbTable_Projects.TABLE, "Guid", guid);
        }

        public int GetProjectCounts(string guid)
        {
            return db.RowCount(DbTable_Projects.TABLE, "Guid", guid);
        }
    }

    public class DbTable_Projects
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public double Accuracy { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Log { get; set; }
        public string isLast { get; set; }

        public static readonly string TABLE = "projects";
        public static readonly string COLUMNS = "Guid, Accuracy, Start, End, Log, isLast";
    }
}
