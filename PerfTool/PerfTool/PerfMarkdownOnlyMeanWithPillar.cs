using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PerfTool
{
    class PerfMarkdownOnlyMeanWithPillar : PerfMarkdown
    {
        public PerfMarkdownOnlyMeanWithPillar(PerformanceTest bench, PerformanceTest latest, string odlVersion, int threshold)
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

        protected override void WriteImageFile()
        {
            PerfImagePillar.Width = 1500;
            PerfImagePillar.Height = 500;
            string meanImage = ImageFileName + ".mean.pillar.png";

            Bitmap bmap = new Bitmap(PerfImagePillar.Width, PerfImagePillar.Height);
            Graphics gph = Graphics.FromImage(bmap);
            gph.Clear(Color.White);

            PerfImagePillar.Draw(gph, Matched, Threshold);

            // Draw Legend
            string legend = " [ V" + BaseVersion + "] / " + Bench.TestType + " / " + Bench.CreateDate + " / " + Bench.BuildId + " / " + Threshold + " %";
            gph.DrawString(legend, new Font("Calibri", 12), Brushes.DarkBlue, new PointF(PerfImagePillar.Width / 2 - 110, 0));

            bmap.Save(meanImage, ImageFormat.Png);
        }

        protected override void WriteLatestFile()
        {
            FileStream fs = new FileStream(LatestFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string head = "| VER Mean |".Replace("VER", BaseVersion);
            sw.WriteLine(head);
            sw.WriteLine("|:---:|");

            string meanImage = ImageFileName + ".mean.pillar.png";
            sw.WriteLine("|![image](./images/" + meanImage + ")|");

            sw.WriteLine("---");
            WriteData(sw);

            sw.Flush();
            sw.Close();
            fs.Close();
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

            string showStr = BaseVersion + "." + Bench.TestType + "." + Bench.CreateDate + "." + Bench.BuildId;
            histories.Insert(0, "- [" + showStr + "](./logs/" + LogFileName + ")");

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
