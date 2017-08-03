using System;

namespace PerfTool
{
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

            // read the performance result.
            PerformanceTest basePerformance = new PerformanceTest(processer.BaseFile);
            PerformanceTest currPerformance = new PerformanceTest(processer.TestFile);

            if (basePerformance.BuildId != currPerformance.BuildId ||
                basePerformance.TestType != currPerformance.TestType ||
                basePerformance.CreateDate != currPerformance.CreateDate ||
                basePerformance.Items.Count != currPerformance.Items.Count)
            {
                Console.WriteLine("The Performance results between Base and Test are not matched.");
                return -1;
            }

            PerfMarkdown markdown = null;
            if (processer.PerfType == PerfType.All)
            {
                markdown = new PerfMarkdownAll(basePerformance, currPerformance, processer.BaseVersion, processer.Threshold);
            }
            else if(processer.PerfType == PerfType.Regression)
            {
                markdown = new PerfMarkdownRegression(basePerformance, currPerformance, processer.BaseVersion, processer.Threshold);
            }
            else
            {
                return -1;
            }
            markdown.CreateMarkdown();

            return 0;
        }
    }
}
