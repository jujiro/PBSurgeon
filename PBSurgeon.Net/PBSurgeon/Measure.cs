using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBSurgeon
{
    public class Measure
    {
        public string TableName { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public string FormatMask { get; set; }
        public string DisplayFolder { get; set; }
        public string Annotations { get; set; }

        public Measure(string tableName, string measureName, string expression, string formatMask=null, string displayFolder=null, string annotations=null)
        {
            TableName = tableName;
            Name = measureName;
            Expression = expression;
            if (formatMask != null) FormatMask = formatMask;
            if (displayFolder != null) DisplayFolder = displayFolder;
            if (annotations != null) Annotations = annotations;
        }
    }
}
