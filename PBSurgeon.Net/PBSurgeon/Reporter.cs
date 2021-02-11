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
        static string indent = "    ";
        static string separatorLine = "================================";
        static string parameterQuery = "IsParameterQuery=true";
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
        public static Schema ExtractSchema()
        {
            var srv = new Tab.Server();
            srv.Connect(ConnectionString);
            var model = srv.Databases[0].Model;
            var sch = new PBSurgeon.Schema();
            foreach (var t in model.Tables.OrderBy(x => x.Name).ToList())
            {
                var tab = new PBSurgeon.Table
                {
                    Name = t.Name,
                    IsHidden = t.IsHidden,
                    ExcludeFromModelRefresh = t.ExcludeFromModelRefresh,
                    Description = t.Description
                };
                foreach (var par in t.Partitions.OrderBy(x => x.Name).ToList())
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
                // Let us reorder the fields alphabetically.
                tab.Fields = tab.Fields.OrderBy(x => x.Name).ToList();
                sch.Tables.Add(tab);
            }
            foreach (var p in model.Expressions.OrderBy(x => x.Name).ToList())
            {
                if (p.Expression.Contains(parameterQuery))
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
            }
            return sch;
        }
        /// <summary>
        /// Creates a simplified JSON schema for a model
        /// </summary>
        /// <returns></returns>
        public static string DumpSchema()
        {
            var sch = ExtractSchema();
            return JsonConvert.SerializeObject(sch, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        /// <summary>
        /// Prints a simplified JSON schema
        /// </summary>
        /// <returns></returns>
        public static void PrintSchema()
        {
            var level = 0;
            var sch = ExtractSchema();
            Console.WriteLine("{0}Date: {1:MM/dd/yyyy H:mm:ss}", GetLeftIndent(level), DateTime.Now);
            Console.WriteLine("{0}Model: {1}", GetLeftIndent(level), ConnectionString);
            Console.WriteLine("{0}Summary", GetLeftIndent(level));
            level++;
            Console.WriteLine("{0}Parameters: {1}", GetLeftIndent(level), sch.Parameters.Count);
            level++;
            foreach (var p in sch.Parameters)
            {
                Console.WriteLine("{0}{1}", GetLeftIndent(level), p.Name);
            }
            level--;
            Console.WriteLine("{0}Tables: {1}", GetLeftIndent(level), sch.Tables.Count);
            level++;
            foreach (var t in sch.Tables)
            {
                Console.WriteLine("{0}{1}", GetLeftIndent(level), t.Name);
                if (t.Partitions.Count > 0)
                {
                    level++;
                    Console.WriteLine("{0}This table contains {1} partitions.", GetLeftIndent(level), t.Partitions.Count);
                    foreach (var part in t.Partitions)
                    {
                        Console.WriteLine("{0}Name: {1}", GetLeftIndent(level), part.Name);
                        Console.WriteLine("{0}Mode: {1}", GetLeftIndent(level), part.Mode);
                        Console.WriteLine("{0}Kind: {1}", GetLeftIndent(level), part.Kind);
                        Console.WriteLine("{0}Expression:", GetLeftIndent(level));
                        Console.WriteLine(AlignExpression(part.Expression, level + 1));
                        Console.WriteLine();
                    }
                    level--;
                }
            }
            level = 0;
            Console.WriteLine("{0}Details", GetLeftIndent(level));

            level = 1;
            Console.WriteLine();
            Console.WriteLine("{0}Parameters:", GetLeftIndent(level));
            level++;
            foreach (var p in sch.Parameters)
            {
                Console.WriteLine("{0}Name: {1}", GetLeftIndent(level), p.Name);
                Console.WriteLine("{0}Description: {1}", GetLeftIndent(level), p.Description);
                Console.WriteLine("{0}Kind: {1}", GetLeftIndent(level), p.Kind);
                Console.WriteLine("{0}Expression: {1}", GetLeftIndent(level), p.Expression);
                Console.WriteLine("{0}{1}", GetLeftIndent(level), separatorLine);
                Console.WriteLine();
            }
            Console.WriteLine();

            level = 1;
            Console.WriteLine("{0}Tables:", GetLeftIndent(level));
            level++;
            foreach (var t in sch.Tables)
            {
                Console.WriteLine("{0}Name: {1}", GetLeftIndent(level), t.Name);
                Console.WriteLine("{0}Description: {1}", GetLeftIndent(level), t.Description);
                Console.WriteLine("{0}IsHidden: {1}", GetLeftIndent(level), t.IsHidden);
                Console.WriteLine("{0}Exclude from model refresh: {1}", GetLeftIndent(level), t.ExcludeFromModelRefresh);
                Console.WriteLine("{0}{1}", GetLeftIndent(level), separatorLine);
                Console.WriteLine();
            }

            level = 1;
            Console.WriteLine("{0}Columns and measures:", GetLeftIndent(level));
            level++;
            foreach (var t in sch.Tables)
            {
                Console.WriteLine("{0}Table: {1}", GetLeftIndent(level), t.Name);
                level++;
                foreach (var c in t.Fields)
                {
                    Console.WriteLine("{0}{1}", GetLeftIndent(level), c.Name);
                    level++;
                    Console.WriteLine("{0}Source column: {1}", GetLeftIndent(level), c.SourceColumnName);
                    Console.WriteLine("{0}Field type: {1}", GetLeftIndent(level), c.FieldType);
                    Console.WriteLine("{0}Data type: {1}", GetLeftIndent(level), c.DataType);
                    Console.WriteLine("{0}Description: {1}", GetLeftIndent(level), c.Description);
                    Console.WriteLine("{0}Format string: {1}", GetLeftIndent(level), c.FormatString);
                    Console.WriteLine("{0}Display folder: {1}", GetLeftIndent(level), c.DisplayFolder);
                    Console.WriteLine("{0}Type: {1}", GetLeftIndent(level), c.Type);
                    if ((new List<string> { FieldType.CalculatedColumn, FieldType.Measure }).Contains(c.FieldType))
                    {
                        Console.WriteLine("{0}Expression:", GetLeftIndent(level));
                        DumpExpression(level + 1, c.Expression);
                    }

                    Console.WriteLine();
                    level--;
                }
                level--;
                Console.WriteLine("{0}{1}", GetLeftIndent(level), separatorLine);
                Console.WriteLine();
            }
            Console.WriteLine("End report");
        }
        private static void DumpExpression(int level, string expression)
        {
            var lines = (expression.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)).ToList();
            foreach (var line in lines)
            {
                Console.WriteLine("{0}{1}", GetLeftIndent(level), line);
            }
        }

        private static string GetLeftIndent(int level)
        {
            if (level == 0) return "";
            return String.Concat(Enumerable.Repeat(indent, level));
        }
        private static string AlignExpression(string expression, int level)
        {
            if (expression == null) return "";
            var ret = new StringBuilder("");
            var lines = expression.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                ret.AppendLine(GetLeftIndent(level) + line);
            }
            return ret.ToString();
        }
    }
}
