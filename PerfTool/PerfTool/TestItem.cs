using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfTool
{
    public class TestItem
    {
        public string Name { get; set; }
        public double Min { get; set; }
        public double Mean { get; set; }
        public double Max { get; set; }
        public double MarginOfError { get; set; }
        public double StdDev { get; set; }
    }
}
