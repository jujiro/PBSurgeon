using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBSurgeon
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public Parameter()
        {
            Kind = Kinds.M;
        }
    }
}
