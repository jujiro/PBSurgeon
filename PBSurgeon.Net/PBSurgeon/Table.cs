using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBSurgeon
{
    public class Table
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public bool ExcludeFromModelRefresh { get; set; }
        public List<Field> Fields { get; set; }
        public List<Partition> Partitions { get; set; }
        public Table()
        {
            IsHidden = false;
            ExcludeFromModelRefresh = false;
            Fields = new List<Field>();
            Partitions = new List<Partition>();
        }
    }
}
