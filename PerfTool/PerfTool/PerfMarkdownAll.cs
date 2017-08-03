using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfTool
{
    class PerfMarkdownAll : PerfMarkdown
    {
        public PerfMarkdownAll(PerformanceTest bench, PerformanceTest latest, string odlVersion, int threshold)
            : base(bench, latest, odlVersion, threshold)
        {
            string prefix = odlVersion + "." + bench.TestType + "." + bench.CreateDate + "." + bench.BuildId;
            ImageFileName = prefix;
            LogFileName = prefix + ".md";
            LatestFile = odlVersion + ".latest.md";
        }
    }
}
