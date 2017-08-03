using System;
using System.Collections.Generic;
using System.Xml;

namespace PerfTool
{
    class PerformanceTest
    {
        public PerformanceTest(string testFile)
        {
            FileName = testFile;
            Read();
        }

        public string FileName { get; private set; }

        public IList<TestItem> Items { get; private set; }

        public string RunId { get; private set; }

        public string TestType { get; private set; }

        public string CreateDate { get; private set; }

        public int BuildId { get; private set; }

        private IDictionary<string, TestItem> _Lookup = new Dictionary<string, TestItem>();

        private void Read()
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(FileName, settings);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                XmlNode runNode = xmlDoc.SelectSingleNode("results/run");
                XmlElement runElem = (XmlElement)runNode;
                RunId = runElem.GetAttribute("id");

                Items = new List<TestItem>();
                XmlNodeList tests = runNode.ChildNodes;
                foreach (XmlNode xnl in tests)
                {
                    TestItem item = new TestItem();
                    XmlElement test = (XmlElement)xnl;
                    item.Name = test.GetAttribute("name");

                    XmlNode tempNode = xnl.SelectSingleNode("summary/Duration");
                    XmlElement duration = (XmlElement)tempNode;

                    item.Min = Double.Parse(duration.GetAttribute("min"));
                    item.Mean = Double.Parse(duration.GetAttribute("mean"));
                    item.Max = Double.Parse(duration.GetAttribute("max"));
                    item.MarginOfError = Double.Parse(duration.GetAttribute("marginOfError"));
                    item.StdDev = Double.Parse(duration.GetAttribute("stddev"));
                    Items.Add(item);
                    _Lookup.Add(item.Name, item);
                }

                reader.Close();
            }
            catch
            {
                return;
            }

            ///
            var splites = RunId.Split('.');
            TestType = splites[0];
            CreateDate = splites[1];
            BuildId = Int32.Parse(splites[2]);
        }

        public TestItem Find(string testName)
        {
            TestItem value;
            if (_Lookup.TryGetValue(testName, out value))
            {
                return value;
            }

            return null;
        }
    }
}
