using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PerfTool
{
    class PerfMarkdownRegression : PerfMarkdown
    {
        public PerfMarkdownRegression(PerformanceTest bench, PerformanceTest latest, string odlVersion, int threshold)
            : base(bench, latest, odlVersion, threshold)
        {
            string prefix = odlVersion + "." + bench.TestType + "." + bench.CreateDate + "." + bench.BuildId;
            ImageFileName = "OData." + bench.TestType; // always save the latest one
            LogFileName = prefix + ".md";
            LatestFile = "OData." + bench.TestType + ".latest.md";
            HistoryFileName = "OData."  + bench.TestType + ".History.md";
        }

        public string HistoryFileName { get; private set; }

        public override void CreateMarkdown()
        {
            base.CreateMarkdown();

            UpdateHistory();
        }

        private void UpdateHistory()
        {
            IList<string> histories = null;
            if (File.Exists(HistoryFileName))
            {
                histories = ReadHistory();
            }

            if (histories == null)
            {
                histories = new List<string>();
            }

            const int MaxHistoryCount = 25;
            while (true)
            {
                if (histories.Count <= MaxHistoryCount)
                {
                    break;
                }

                // remove the last one
                histories.RemoveAt(histories.Count - 1);
            }
            histories.Insert(0, "- [" + Bench.CreateDate + "](./logs/" + LogFileName + ")");

            FileStream fs = new FileStream(HistoryFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("History:\n---");
            foreach(string history in histories)
            {
                sw.WriteLine(history);
            }

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private IList<string> ReadHistory()
        {
            IList<string> newReturns = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(HistoryFileName, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("- ["))
                    {
                        newReturns.Add(line);
                    }
                }

                sr.Close();
            }
            catch
            {
                return newReturns;
            }

            return newReturns;
        }
    }
}
