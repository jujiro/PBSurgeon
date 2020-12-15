using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBSurgeon
{
    public class Schema
    {
        public List<Table> Tables { get; set; }
        public List<Parameter> Parameters { get; set; }
        public Schema()
        {
            Tables = new List<Table>();
            Parameters = new List<Parameter>();
        }
    }
}
