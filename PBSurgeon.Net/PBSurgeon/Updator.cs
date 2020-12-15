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
        public static bool DryRun = false;
        public static bool Verbose = true;
        /// <summary>
        /// Creates or updates the measure in the model
        /// </summary>
        /// <param name="m"></param>
        public static void UpdateMeasure(Tab.Measure m, bool overwriteExisting = true)
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
                tab = model.Tables[m.Table.Name];
            }
            catch
            {
                throw new Exception($"Table not found, {m.Table.Name}");
            }
            if (DryRun)
                if (tab.Measures.Contains(m.Name))
                    if (overwriteExisting) Console.WriteLine($"Measure, {m.Name} already exists.  It will be updated.");
                    else Console.WriteLine($"Measure, {m.Name} already exists.  It will be skipped.");
                else
                    Console.WriteLine($"Measure, {m.Name} will be added to the model.");
            if (DryRun)
                return;
            if (tab.Measures.Contains(m.Name))
            {
                var mx = tab.Measures[m.Name];
                mx.Expression = m.Expression;
                if (!string.IsNullOrWhiteSpace(m.DisplayFolder)) mx.DisplayFolder = m.DisplayFolder;
                if (!string.IsNullOrWhiteSpace(m.FormatString)) mx.FormatString = m.FormatString; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
                if (Verbose) Console.WriteLine($"Measure, {m.Name} was updated.");
            }
            else
            {
                var mz = new Tab.Measure();
                mz.Name = m.Name;
                mz.Expression = m.Expression;
                if (!string.IsNullOrWhiteSpace(m.DisplayFolder)) mz.DisplayFolder = m.DisplayFolder;
                if (!string.IsNullOrWhiteSpace(m.FormatString)) mz.FormatString = m.FormatString;
                if (!string.IsNullOrWhiteSpace(m.Description)) mz.Description = m.Description;
                tab.Measures.Add(mz);
                if (Verbose) Console.WriteLine($"Measure, {m.Name} was created.");
            }
            try
            {
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                model.UndoLocalChanges();
                throw ex;
            }
        }
        //public static void UpdateColumn(PBSurgeon.Column c, bool overwriteExisting = true)
        //{
        //    var srv = new Tab.Server();
        //    try
        //    {
        //        srv.Connect(ConnectionString);
        //    }
        //    catch
        //    {
        //        throw new Exception("Unable to connect to the model");
        //    }
        //    var model = srv.Databases[0].Model;
        //    Tab.Table tab;
        //    //try
        //    //{
        //    //    //tab = model.Tables[c.TableName];
        //    //}
        //    //catch
        //    //{
        //    //    //throw new Exception($"Table not found, {c.TableName}");
        //    //}
        //    //if (DryRun)
        //    //    if (tab.Columns.Contains(c.Name))
        //    //        if (overwriteExisting) Console.WriteLine($"Column, {c.Name} already exists.  It will be updated.");
        //    //        else Console.WriteLine($"Column, {c.Name} already exists.  It will be skipped.");
        //    //    else
        //    //        Console.WriteLine($"Column, {c.Name} will be added to the model.");
        //    //if (DryRun)
        //    //    return;
        //    //if (tab.Columns.Contains(c.Name))
        //    //{
        //    //    var mx = tab.Columns[c.Name];
        //    //    mx.DataType = c.DataType;
        //    //    if (!string.IsNullOrWhiteSpace(c.Description)) mx.Description = c.Description;
        //    //    if (!string.IsNullOrWhiteSpace(c.DisplayFolder)) mx.DisplayFolder = c.DisplayFolder;
        //    //    if (!string.IsNullOrWhiteSpace(c.FormatMask)) mx.FormatString = c.FormatMask; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
        //    //    if (Verbose) Console.WriteLine($"Column, {c.Name} was updated.");
        //    //}




        //    try
        //    {
        //        model.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        model.UndoLocalChanges();
        //        throw ex;
        //    }

        //}
    }
}
