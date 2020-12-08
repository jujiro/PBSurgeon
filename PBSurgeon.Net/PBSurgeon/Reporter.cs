using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tab = Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;

namespace PBSurgeon
{
    /// <summary>
    /// A class to read a model and report on various components.
    /// It will not update the model
    /// </summary>
    public static class Reporter
    {
        /// <summary>
        /// Must be set before any of the methods are called.
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// Create the TMSL json schema of an existing model.
        /// </summary>
        /// <returns></returns>
        public static string DumpSchema()
        {
            var srv = new Tab.Server();
            srv.Connect(ConnectionString);
            var model = srv.Databases[0].Model;
            return JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings()
                                                                            {
                                                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                                            });
        }
        /// <summary>
        /// Compares the schema supplied in schemaToCompare (JSON) and reports any differences.
        /// </summary>
        /// <param name="SchemaToCompare"></param>
        /// <returns></returns>
        public static string FindDifferences(string SchemaToCompare)
        {
            throw new NotImplementedException();
        }
    }
}
