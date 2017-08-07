using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PerfTool
{
    class ComLineProcesser
    {
        private IList<string> _args;

        public ComLineProcesser(string[] args)
        {
            _args = args;
        }

        public string BaseFile { get; private set; }

        public string BaseVersion { get; private set; }

        public string TestFile { get; private set; }

        public int Threshold { get; private set; }

        public PerfType PerfType { get; private set; }

        public bool Process()
        {
            if (_args.Count != 8 && _args.Count != 9)
            {
                Usage();
                return false;
            }

            try
            {
                for (int i = 0; i < _args.Count; i++)
                {
                    string arg = _args[i];
                    switch (arg)
                    {
                        case "-b":
                        case "-B":
                            BaseFile = _args[i + 1];
                            i++;
                            break;

                        case "-t":
                        case "-T":
                            TestFile = _args[i + 1];
                            i++;
                            break;

                        case "-v":
                        case "-V":
                            BaseVersion = _args[i + 1];
                            i++;
                            break;

                        case "-a":
                        case "-A":
                            Threshold = Int32.Parse(_args[i + 1]);
                            i++;
                            break;

                        case "-reg":
                        case "-REG":
                            if (PerfType != PerfType.None)
                            {
                                Usage();
                                return false;
                            }
                            PerfType = PerfType.Regression;
                            break;

                        case "-all":
                        case "-ALL":
                            if (PerfType != PerfType.None)
                            {
                                Usage();
                                return false;
                            }

                            PerfType = PerfType.All;
                            break;

                        case "-mean":
                        case "-Mean":
                            if (PerfType != PerfType.None)
                            {
                                Usage();
                                return false;
                            }

                            PerfType = PerfType.OnlyMean;
                            break;

                        case "-pillar":
                        case "-PILLAR":
                            if (PerfType != PerfType.None)
                            {
                                Usage();
                                return false;
                            }
                            PerfType = PerfType.OnlyMeanWithPillar;

                            break;

                        default:
                            Usage();
                            return false;
                    }
                }
            }
            catch
            {
                Usage();
                return false;
            }

            if (PerfType == PerfType.None)
            {
                PerfType = PerfType.Regression; // can be options
            }

            return ValidateArguments();
        }

        private bool ValidateArguments()
        {
            if (String.IsNullOrEmpty(BaseFile))
            {
                Console.WriteLine("[-b BaseFile] is required.");
                Usage();
                return false;
            }

            if (String.IsNullOrEmpty(TestFile))
            {
                Console.WriteLine("[-b BaseFile] is required.");
                Usage();
                return false;
            }

            if (String.IsNullOrEmpty(BaseVersion))
            {
                Console.WriteLine("[-v BaseVersion] is required.");
                Usage();
                return false;
            }

            if (Threshold <= 0 || Threshold > 100)
            {
                Console.WriteLine("Threshold value is not valid. It should be between (0,100).");
                Usage();
                return false;
            }

            if (!File.Exists(BaseFile))
            {
                Console.WriteLine("BaseFile: (" + BaseFile + ") is not existed.");
                return false;
            }

            if (!File.Exists(TestFile))
            {
                Console.WriteLine("TestFile: (" + TestFile + ") is not existed.");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("**************************************************************\n" +
                "*Running performance tools using the following parameters:\n");
            sb.Append("*\t-b : ").Append(BaseFile).Append("\n")
              .Append("*\t-t : ").Append(TestFile).Append("\n")
              .Append("*\t-v : ").Append(BaseVersion).Append("\n")
              .Append("*\t-a : ").Append(Threshold).Append("\n")
              .Append("*\t-").Append(PerfType).Append("\n**************************************************************");
            return sb.ToString();
        }

        private static void Usage()
        {
            string usage = "\nUsage:\n     PerfTool.exe -b BaseFile -t TestFile -v BaseVersion -a Threshold [-reg|-all|-mean] \n";
            Console.WriteLine(usage);
        }
    }
}
