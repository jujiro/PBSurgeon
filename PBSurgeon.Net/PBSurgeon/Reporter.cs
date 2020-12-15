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
        /// Creates the raw TMSL json schema of an existing model.
        /// The detailed schema contains too much information.
        /// </summary>
        /// <returns></returns>
        public static string DumpRawSchema()
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
        /// Creates a simplified JSON schema for an existing model
        /// </summary>
        /// <returns></returns>
        public static string DumpSchema()
        {
            var srv = new Tab.Server();
            srv.Connect(ConnectionString);
            var model = srv.Databases[0].Model;
            var sch = new PBSurgeon.Schema();
            foreach (var t in model.Tables)
            {
                var tab = new PBSurgeon.Table
                {
                    Name = t.Name,
                    IsHidden = t.IsHidden,
                    ExcludeFromModelRefresh = t.ExcludeFromModelRefresh,
                    Description = t.Description
                };
                foreach (var par in t.Partitions)
                {
                    var part = new PBSurgeon.Partition
                    {
                        Name = par.Name,
                        Mode = par.Mode.ToString()
                    };
                    if (par.Source is Tab.MPartitionSource mpar)
                    {
                        part.Kind = par.SourceType.ToString();
                        part.Expression = mpar.Expression;
                    }
                    tab.Partitions.Add(part);
                }
                foreach (var c in t.Columns)
                {
                    var fl = new PBSurgeon.Field
                    {
                        Name = c.Name,
                        OrdinalPosition = c.DisplayOrdinal,
                        DisplayFolder = c.DisplayFolder,
                        FormatString = c.FormatString,
                        DataType = c.DataType.ToString(),
                        Type = c.Type.ToString(),
                        Description = c.Description,
                        SortByColumn = c.SortByColumn?.Name
                    };
                    if (c is Tab.CalculatedColumn column)
                    {
                        fl.FieldType = FieldType.CalculatedColumn;
                        fl.Expression = column.Expression;
                    }
                    else
                    {
                        fl.FieldType = FieldType.Column;
                        if (c is Tab.DataColumn cxd) fl.SourceColumnName = cxd.SourceColumn;
                    }
                    tab.Fields.Add(fl);
                }
                foreach (var m in t.Measures)
                {
                    var fl = new PBSurgeon.Field
                    {
                        Name = m.Name,
                        DisplayFolder = m.DisplayFolder,
                        FormatString = m.FormatString,
                        DataType = m.DataType.ToString(),
                        Type = m.ObjectType.ToString(),
                        Description = m.Description,
                        FieldType = FieldType.Measure,
                        Expression = m.Expression
                    };
                    tab.Fields.Add(fl);
                }
                sch.Tables.Add(tab);
            }
            foreach (var p in model.Expressions)
            {
                var param = new PBSurgeon.Parameter
                {
                    Name = p.Name,
                    Description = p.Description,
                    Expression = p.Expression,
                    Kind = p.Kind.ToString()
                };
                sch.Parameters.Add(param);
            }
            return JsonConvert.SerializeObject(sch, Formatting.Indented, new JsonSerializerSettings()
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
