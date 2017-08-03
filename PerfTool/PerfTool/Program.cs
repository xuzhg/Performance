using System;

namespace PerfTool
{
    enum OutputType
    {
        Normal,
        Regression
    }
    class Program
    {
        
        static int Main(string[] args)
        {
            ComLineProcesser processer = new ComLineProcesser(args);
            if (!processer.Process())
            {
                return -1;
            }

            Console.WriteLine(processer);

            // input, as simple as possible
            string baseFile = args[0];
            string currFile = args[1];
            string odlVersion = args[2];
            int threshold = Int32.Parse(args[3]);
            OutputType outputType = OutputType.Normal;
            if (args.Length == 5)
            {
                outputType = OutputType.Regression;
            }

            PerformanceTest basePerformance = new PerformanceTest(baseFile);
            PerformanceTest currPerformance = new PerformanceTest(currFile);

            if (basePerformance.BuildId != currPerformance.BuildId ||
                basePerformance.TestType != currPerformance.TestType ||
                basePerformance.CreateDate != currPerformance.CreateDate)
            {
                Console.WriteLine("The Performance resultes between Base and Test are not matched.");
                return -1;
            }

            if (outputType == OutputType.Normal)
            {
                PerfMarkdown markdown = new PerfMarkdown(basePerformance, currPerformance, odlVersion, threshold);
                markdown.CreateMarkdown();
            }
            else if (outputType == OutputType.Regression)
            {
                PerfRegressionMarkdown markdown = new PerfRegressionMarkdown(basePerformance, currPerformance, odlVersion, threshold);
                markdown.CreateMarkdown();
            }

            return 0;
            /*
            IList<DiffItem> diffs = new List<DiffItem>();
            foreach (TestItem currTest in currPerformance.Items)
            {
                TestItem baseTest = basePerformance.Find(currTest.Name);
                if (baseTest == null)
                {
                    Console.WriteLine("Can not find the [" + currTest.Name + "] in the base!");
                    return -1;
                }

                DiffItem item = new DiffItem
                {
                    Name = currTest.Name,
                    Pass = true
                };

                double delta = currTest.Mean - baseTest.Mean;
                item.Percentage = (delta / baseTest.Mean) * 100.0;
                if (delta < 0.0)
                {
                    item.Pass = false;
                }

                diffs.Add(item);
            }

            PerfDraw.Draw(diffs, "a.png", threshold, currPerformance);
            return 0;

            */
        }

        
    }
}
