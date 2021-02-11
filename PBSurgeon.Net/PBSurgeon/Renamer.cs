using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tab = Microsoft.AnalysisServices.Tabular;

namespace PBSurgeon
{
    /// <summary>
    /// A class to update the model.
    /// </summary>
    public static class Renamer
    {
        /// <summary>
        /// Must be set before any of the methods are called.
        /// </summary>
        public static string ConnectionString { get; set; }
        public static bool DryRun { get; set; }
        public static bool Verbose { get; set; }
        static Renamer()
        {
            DryRun = false;
            Verbose = false;
        }

        public static void RenameTable(string tableName, string newTableName)
        {
            var srv = new Tab.Server();
            try
            {
                srv.Connect(ConnectionString);
            }
            catch
            {
                throw new Exception("Unable to connect to the model");
            }
            var model = srv.Databases[0].Model;
            Tab.Table tab;
            try
            {
                tab = model.Tables[tableName];
            }
            catch
            {
                throw new Exception($"Table not found, {tableName}");
            }
            if (DryRun)
            {
                if (Verbose) Console.WriteLine($"Table name will be changed from {tableName} to {newTableName}.");
                return;
            }
            tab.RequestRename(newTableName);
            try
            {
                model.SaveChanges();
                Console.WriteLine($"Table name changed from {tableName} to {newTableName}.");
            }
            catch (Exception ex)
            {
                model.UndoLocalChanges();
                throw ex;
            }
        }

        public static void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            var srv = new Tab.Server();
            try
            {
                srv.Connect(ConnectionString);
            }
            catch
            {
                throw new Exception("Unable to connect to the model");
            }
            var model = srv.Databases[0].Model;
            Tab.Table tab;
            try
            {
                tab = model.Tables[tableName];
            }
            catch
            {
                throw new Exception($"Table not found, {tableName}");
            }
            if (!tab.Columns.Contains(columnName)) throw new Exception($"Column {columnName} not found in table {tableName}");
            if (DryRun)
            {
                if (Verbose) Console.WriteLine($"Column name will be changed from {columnName} to {newColumnName} in table {tableName}.");
                return;
            }
            var col = tab.Columns[columnName];
            col.Name = newColumnName;
            try
            {
                model.SaveChanges();
                Console.WriteLine($"Column name changed from {columnName} to {newColumnName} in table {tableName}.");
            }
            catch (Exception ex)
            {
                model.UndoLocalChanges();
                throw ex;
            }
        }
    }
}
