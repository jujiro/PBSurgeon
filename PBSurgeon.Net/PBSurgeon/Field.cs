using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tab = Microsoft.AnalysisServices.Tabular;

namespace PBSurgeon
{
    /// <summary>
    /// A field is a column, computed column, or a measure
    /// </summary>
    public class Field
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string SourceColumnName { get; set; }
        public string FormatString { get; set; }
        public string DisplayFolder { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public int OrdinalPosition { get; set; }
        public string FieldType { get; set; }
        public string Type { get; set; }
        public string SortByColumn { get; set; }
        /// <summary>
        /// Applies to calculated column and measures only.
        /// </summary>
        public string Expression { get; set; }
        public Field()
        {
            Description = "";
            FormatString = "";
            DisplayFolder = "";
        }
    }
}
