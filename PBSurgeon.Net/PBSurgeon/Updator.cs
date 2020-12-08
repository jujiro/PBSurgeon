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
        public static void UpdateMeasure(Measure m, bool overwriteExisting=true)
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
                tab = model.Tables[m.TableName];
            }
            catch
            {
                throw new Exception($"Table not found, {m.TableName}");
            }
            if (tab.Measures.Contains(m.Name) && DryRun)
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
                if (!string.IsNullOrWhiteSpace(m.FormatMask)) mx.FormatString = m.FormatMask; //"#,0.00"  "0.0000%;-0.0000%;0.0000%"
                if (Verbose) Console.WriteLine($"Measure, {m.Name} was updated.");
            }
            else
            {
                var mz = new Tab.Measure();
                mz.Name = m.Name;
                mz.Expression = m.Expression;
                if (!string.IsNullOrWhiteSpace(m.DisplayFolder)) mz.DisplayFolder = m.DisplayFolder;
                if (!string.IsNullOrWhiteSpace(m.FormatMask)) mz.FormatString = m.FormatMask;
                if (!string.IsNullOrWhiteSpace(m.Annotations)) mz.Annotations.Add(new Tab.Annotation() { Value = m.Annotations });
                tab.Measures.Add(mz);
                if (Verbose) Console.WriteLine($"Measure, {m.Name} was created.");
            }
            try 
            {
                model.SaveChanges();
            }
            catch(Exception ex)
            {
                model.UndoLocalChanges();
                throw ex;
            }            
        }
    }
}
