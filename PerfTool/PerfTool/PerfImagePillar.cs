using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PerfTool
{
    static class PerfImagePillar
    {
        public static int Width = 450;
        public static int Height = 400;

      //  public const int PillarWidth = 4;
        public const int PillarPedding = 10;
        public const int Margin = 25;
        public const int YMargin = 12; // write the percentage (%)
        public const int ArrawLen = 15;
        public const int ArrawWidth = 6;
        public const int YScale = 10;// means draw top 60%

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
            int left = Width - Margin - 2 - ArrawLen - 4 - 4;
            left -= (PillarPedding * (number - 1));
            return left / (float)(number * 2);
        }

        public static void Draw(Graphics gph, IList<KeyValuePair<TestItem, TestItem>> tests, int threshold)
        {
            PointF cpt = new PointF(Margin, Height - Margin);//center point & start point

            double maxMs = Math.Max(tests.Max(a => a.Key.Mean), tests.Max(a => a.Value.Mean));
            maxMs = RoundUpper(maxMs);
            double minMs = Math.Min(tests.Min(a => a.Key.Mean), tests.Min(a => a.Value.Mean));
            minMs = RoundLower(minMs);

            DrawAxis(gph, cpt, minMs, maxMs);

            // draw the data
            //Pen myPen = new Pen(Color.Blue, 1.0F);
            //myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            double topY = YMargin + ArrawLen + 5;
            double yDelta = (cpt.Y - topY) / YScale;
            double bottomY = cpt.Y - yDelta;

            int index = 0;
            double maxmin = (maxMs - minMs);
            double ymaxmin = (topY - bottomY);
            float x = cpt.X + 4; // start x
            float pillarWith = CalcPillarWidth(tests.Count);
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

                gph.DrawPolygon(Pens.Black, xpt);
                gph.FillPolygon(new SolidBrush(Color.Black), xpt);
                x += pillarWith;

                gph.DrawString((index + 1).ToString(), new Font("Calibri", 12), Brushes.Black, new PointF(x - pillarWith/2, cpt.Y));

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
                    gph.DrawPolygon(Pens.Red, xpt);
                    gph.FillPolygon(new SolidBrush(Color.Red), xpt);
                }

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

            /*
            if (Math.Abs(maxPercentage) > threshold)
            {
                Image image = Image.FromFile(".\\failed.png");
                gph.DrawImage(image, new Point(Width - 100, 0));
            }
            else
            {
                Image image = Image.FromFile(".\\pass.png");
                gph.DrawImage(image, new Point(Width - 100, 0));
            }*/
        }

        private static void DrawAxis(Graphics gph, PointF cpt, double min, double max)
        {
            //   ------------------*->
            PointF xEnd = new PointF(Width - ArrawLen - 2, cpt.Y);
            PointF[] xpt = new PointF[3]
            {
                new PointF(xEnd.X + ArrawLen, xEnd.Y),
                new PointF(xEnd.X, xEnd.Y - ArrawWidth),
                new PointF(xEnd.X, xEnd.Y + ArrawWidth)
            };//x triangle

            //draw x
            gph.DrawLine(Pens.Black, cpt.X, cpt.Y, xEnd.X, xEnd.Y);
            gph.DrawPolygon(Pens.Black, xpt);
            gph.FillPolygon(new SolidBrush(Color.Black), xpt);
           // gph.DrawString("Test", new Font("Calibri", 12), Brushes.Black, new PointF(xEnd.X - 15 , xEnd.Y + 5));

            //draw y
            PointF yEnd = new PointF(cpt.X, YMargin + ArrawLen);
            PointF[] ypt = new PointF[3]
            {
                new PointF(yEnd.X, yEnd.Y - ArrawLen),
                new PointF(yEnd.X - ArrawWidth, yEnd.Y),
                new PointF(yEnd.X + ArrawWidth, yEnd.Y) };//y triangle

            gph.DrawLine(Pens.Black, cpt.X, cpt.Y, yEnd.X, yEnd.Y);
            gph.DrawPolygon(Pens.Black, ypt);
            gph.FillPolygon(new SolidBrush(Color.Black), ypt);
            gph.DrawString("Time Duration (ms)", new Font("Calibri", 12), Brushes.Black, new PointF(0, 0 - 4));

            DrawYScale(gph, cpt, new PointF(yEnd.X, yEnd.Y + 5), min, max);
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
                // float deltaY = i * delta;
                y -= delta;
                /*float tmp = 20;
                if (i == 10)
                {
                    tmp = 25;
                }

                // gph.DrawString((i * 10).ToString(), new Font("Calibri", 10), Brushes.Black, new PointF(start.X - tmp, y + deltaY - 8));
                */
                gph.DrawLine(Pens.Black, x - 3, y, x, y);

                gph.DrawLine(myPen, x, y, Width - 5, y);
            }
        }
    }
}
