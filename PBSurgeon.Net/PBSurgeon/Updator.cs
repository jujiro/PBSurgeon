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
    public static class Updator
    {
        /// <summary>
        /// Must be set before any of the methods are called.
        /// </summary>
        public static string ConnectionString { get; set; }
        public static bool DryRun { get; set; }
        public static bool Verbose { get; set; }
        static Updator()
        {
            DryRun = false;
            Verbose = false;
        }
        /// <summary>
        /// Creates or updates the measure in the model
        /// </summary>
        /// <param name="f"></param>
        public static void UpsertField(Field f, bool overwriteExisting = true)
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
                tab = model.Tables[f.TableName];
            }
            catch
            {
                throw new Exception($"Table not found, {f.TableName}");
            }
            if (Verbose)
            {
                switch (f.FieldType)
                {
                    case FieldType.CalculatedColumn:
                    case FieldType.Column:
                        if (tab.Columns.Contains(f.Name))
                            if (overwriteExisting) Console.WriteLine($"Column, {f.Name} already exists.  It will be updated.");
                            else Console.WriteLine($"Column, {f.Name} already exists.  It will be skipped.");
                        else
                            Console.WriteLine($"Column, {f.Name} will be added to the model.");
                        break;
                    case FieldType.Measure:
                        if (tab.Measures.Contains(f.Name))
                            if (overwriteExisting) Console.WriteLine($"Measure, {f.Name} already exists.  It will be updated.");
                            else Console.WriteLine($"Measure, {f.Name} already exists.  It will be skipped.");
                        else
                            Console.WriteLine($"Measure, {f.Name} will be added to the model.");
                        break;
                    default:
                        throw new Exception("Unrecognized field type");
                }
            }
            if (DryRun)
                return;
            var exists = false;
            switch (f.FieldType)
            {
                case FieldType.CalculatedColumn:
                    exists = tab.Columns.Contains(f.Name);
                    var mc = new Tab.CalculatedColumn();
                    if (exists) mc = (Tab.CalculatedColumn)tab.Columns[f.Name];
                    mc.Name = f.Name;
                    mc.DisplayFolder = f.DisplayFolder;
                    mc.FormatString = f.FormatString; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
                    //mc.SortByColumn = f.SortByColumn;
                    mc.Expression = f.Expression;
                    if (!exists) tab.Columns.Add(mc);
                    break;
                case FieldType.Column:
                    exists = tab.Columns.Contains(f.Name);
                    var mx = new Tab.DataColumn();
                    if (exists) mx = (Tab.DataColumn)tab.Columns[f.Name];
                    mx.Name = f.Name;
                    mx.DisplayFolder = f.DisplayFolder;
                    mx.FormatString = f.FormatString; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
                    mx.SourceColumn = string.IsNullOrWhiteSpace(f.SourceColumnName)?mx.Name:f.SourceColumnName;
                    if (exists) tab.Columns.Add(mx);
                    break;
                case FieldType.Measure:
                    exists = tab.Measures.Contains(f.Name);
                    var mr = new Tab.Measure();
                    if (exists) mr = tab.Measures[f.Name];
                    mr.Name = f.Name;
                    mr.DisplayFolder = f.DisplayFolder;
                    mr.FormatString = f.FormatString; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
                    //mc.SortByColumn = f.SortByColumn;
                    mr.Expression = f.Expression;
                    if (!exists) tab.Measures.Add(mr);
                    break;
                default:
                    throw new Exception("Unrecognized field type");
            }
            try
            {
                model.SaveChanges();
                if (exists)
                    Console.WriteLine($"{f.FieldType}, {f.Name} updated in the model.");
                else
                    Console.WriteLine($"{f.FieldType}, {f.Name} added to the model.");
            }
            catch (Exception ex)
            {
                model.UndoLocalChanges();
                throw ex;
            }
        }
    }
}
