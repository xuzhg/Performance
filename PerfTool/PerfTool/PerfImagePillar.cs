using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PerfTool
{
    static class PerfImagePillar
    {
        public static int Width = 450;
        public static int Height = 400;

        public const int DefaultPillarWidth = 30;
        public const int PillarPedding = 10;
        public const int Margin = 30;
        public const int YMargin = 12; // write the percentage (%)

        public const int ArrowLen = 15;
        public const int ArrowWidth = 6;
        public const int ArrowMargin = 2;

        public const int YScale = 10;// means draw top 60%

        public static void Draw(Graphics gph, IList<KeyValuePair<TestItem, TestItem>> tests, int threshold)
        {
            PointF cpt = new PointF(Margin, Height - Margin - YMargin);//center point & start point

            double maxMs = Math.Max(tests.Max(a => a.Key.Mean), tests.Max(a => a.Value.Mean));
            maxMs = RoundUpper(maxMs);
            double minMs = Math.Min(tests.Min(a => a.Key.Mean), tests.Min(a => a.Value.Mean));
            minMs = RoundLower(minMs);

            DrawAxis(gph, cpt, minMs, maxMs);

            // draw the data
            double topY = YMargin + ArrowLen + 5;
            double yDelta = (cpt.Y - topY) / YScale;
            double bottomY = cpt.Y - yDelta;

            Font myFont = new Font("Calibri", 12);
            int index = 0;
            double maxmin = (maxMs - minMs);
            double ymaxmin = (topY - bottomY);
            float x = cpt.X + 4; // start x
            float pillarWith = CalcPillarWidth(tests.Count);
            if (pillarWith > DefaultPillarWidth)
            {
                pillarWith = DefaultPillarWidth;
            }

            foreach (KeyValuePair<TestItem, TestItem> test in tests)
            {
                x = cpt.X + 4 + (pillarWith * 2 + PillarPedding) * index;

                double y = topY - (maxMs - test.Key.Mean) * ymaxmin / maxmin;
                PointF[] xpt = new PointF[4]
                {
                    new PointF(x, cpt.Y),
                    new PointF(x + pillarWith, cpt.Y),
                    new PointF(x + pillarWith, (float)y),
                    new PointF(x, (float)y),
                };//base pillar

                gph.DrawPolygon(Pens.DarkGray, xpt);
                gph.FillPolygon(new SolidBrush(Color.DarkGray), xpt);
                x += pillarWith;

                gph.DrawString((index + 1).ToString(), myFont, Brushes.Black, new PointF(x - pillarWith/2, cpt.Y));

                float maxY = (float)y;

                // test
                y = topY - (maxMs - test.Value.Mean) * ymaxmin / maxmin;
                xpt = new PointF[4]
                {
                    new PointF(x, cpt.Y),
                    new PointF(x + pillarWith, cpt.Y),
                    new PointF(x + pillarWith, (float)y),
                    new PointF(x, (float)y),
                };//test pillar

                if (test.Key.Mean >= test.Value.Mean)
                {
                    gph.DrawPolygon(Pens.Green, xpt);
                    gph.FillPolygon(new SolidBrush(Color.Green), xpt);
                }
                else
                {
                    gph.DrawPolygon(Pens.IndianRed, xpt);
                    gph.FillPolygon(new SolidBrush(Color.IndianRed), xpt);
                }

                string percentage = GetPercentage(test.Key.Mean, test.Value.Mean);
                if (maxY > (float)y)
                {
                    maxY = (float)y;
                }


                SizeF textSize = gph.MeasureString(percentage, myFont);
                gph.DrawString(percentage, myFont, Brushes.Black, new PointF(x - textSize.Width / 2, maxY - textSize.Height));

                /*
                double testY = topY - (maxMs - test.Value.Mean) * ymaxmin / maxmin;
                if (test.Key.Mean >= test.Value.Mean)
                {
                    xpt = new PointF[4]
                    {
                        new PointF(x, cpt.Y),
                        new PointF(x + pillarWith, cpt.Y),
                        new PointF(x + pillarWith, (float)testY),
                        new PointF(x, (float)testY),
                    };//base pillar

                    gph.DrawPolygon(Pens.Green, xpt);
                    gph.FillPolygon(new SolidBrush(Color.Green), xpt);
                }
                else
                {
                    xpt = new PointF[4]
                    {
                        new PointF(x, cpt.Y),
                        new PointF(x + pillarWith, cpt.Y),
                        new PointF(x + pillarWith, (float)y),
                        new PointF(x, (float)y),
                    };

                    gph.DrawPolygon(Pens.Blue, xpt);
                    gph.FillPolygon(new SolidBrush(Color.Blue), xpt);

                    xpt = new PointF[4]
                    {
                        new PointF(x, (float)y),
                        new PointF(x + pillarWith, (float)y),
                        new PointF(x + pillarWith, (float)testY),
                        new PointF(x, (float)testY),
                    };

                    gph.DrawPolygon(Pens.Red, xpt);
                    gph.FillPolygon(new SolidBrush(Color.Red), xpt);
                }
                */

                index++;
            }


            Image legendImg = GetImageFromResource("Legend.png");
            gph.DrawImage(legendImg, new PointF(Width - legendImg.Width, 0));
        }

        public static string GetPercentage(double b, double c)
        {
            double d = (c - b) / b;
            d *= 100.0;
            return d.ToString("F0") + "%";
        }

        private static void DrawAxis(Graphics gph, PointF cpt, double min, double max)
        {
            //   -------------------->*
            PointF xEnd = new PointF(Width - ArrowMargin, cpt.Y);
            PointF[] xpt = new PointF[3]
            {
                new PointF(xEnd.X, xEnd.Y),
                new PointF(xEnd.X - ArrowLen, xEnd.Y - ArrowWidth),
                new PointF(xEnd.X - ArrowLen, xEnd.Y + ArrowWidth)
            };//x triangle

            //draw x
            gph.DrawLine(Pens.DarkGray, cpt.X, cpt.Y, xEnd.X - ArrowLen, cpt.Y);
            gph.DrawPolygon(Pens.Black, xpt);
            gph.FillPolygon(new SolidBrush(Color.Black), xpt);

            Image xLabelImg = GetImageFromResource("XLabel.png");
            gph.DrawImage(xLabelImg, new PointF(Width /2 - xLabelImg.Width / 2, Height - xLabelImg.Height));

            //draw y
            PointF yEnd = new PointF(cpt.X, ArrowMargin);
            PointF[] ypt = new PointF[3]
            {
                new PointF(yEnd.X, yEnd.Y),
                new PointF(yEnd.X - ArrowWidth, yEnd.Y + ArrowLen),
                new PointF(yEnd.X + ArrowWidth, yEnd.Y + ArrowLen) };//y triangle

            gph.DrawLine(Pens.Black, cpt.X, cpt.Y, cpt.X, yEnd.Y + ArrowLen);
            gph.DrawPolygon(Pens.Black, ypt);
            gph.FillPolygon(new SolidBrush(Color.Black), ypt);

            DrawYScale(gph, cpt, new PointF(yEnd.X, yEnd.Y + ArrowLen + 5), min, max);

            Image yLabelImg = GetImageFromResource("YLabel.png");
            float tmp = (Height - ArrowMargin - YMargin - Margin) / 2;
            gph.DrawImage(yLabelImg, new PointF(0, ArrowMargin + tmp - yLabelImg.Height / 2));
        }

        private static void DrawYScale(Graphics gph, PointF start, PointF end, double min, double max)
        {
            float delta = (start.Y - end.Y) / YScale;
            float y = start.Y;
            float x = start.X;

            Pen myPen = new Pen(Color.LightGray, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            for (int i = 1; i <= YScale; i++)
            {
                y -= delta;
                gph.DrawLine(Pens.Black, x - 3, y, x, y);
                gph.DrawLine(myPen, x, y, Width - 5, y);
            }
        }

        private static Image GetImageFromResource(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("PerfTool." + fileName);
            return Image.FromStream(imageStream);
        }

        private static double RoundLower(double x)
        {
            return ((int)(x / 10.0)) * 10.0;
        }

        private static double RoundUpper(double x)
        {
            return ((int)((x + 5) / 10.0)) * 10.0;
        }

        private static float CalcPillarWidth(int number)
        {
            int left = Width - Margin - 2 - ArrowLen - 4 - 4;
            left -= (PillarPedding * (number - 1));
            return left / (float)(number * 2);
        }
    }
}
