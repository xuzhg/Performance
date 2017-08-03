using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfTool
{
    static class PerfImage
    {
        public const int Width = 500;
        public const int Height = 400;

        private const int Margin = 30;
        private const int YMargin = 12; // write the percentage (%)
        private const int ArrawLen = 15;
        private const int ArrawWidth = 6;
        private const int YScale = 6;// means draw top 60%

        public static void Draw(Graphics gph, IEnumerable<double> percentages, int threshold)
        {
            PointF cpt = new PointF(Margin, Height/2);//center point & start point

            double maxPercentage = percentages.Max(a => Math.Abs(a));
            int scaleNum = YScale; // at least 60%
            if (maxPercentage > (YScale * 10))
            {
                scaleNum = (int)((maxPercentage / 10.0) + 0.5);
                if (scaleNum > 10)
                {
                    scaleNum = 10; // at most 60%
                }
            }

            DrawAxis(gph, cpt, scaleNum);
            // DrawThreshold(gph, cpt, threshold, scaleNum);

            // draw the data
            int index = 1;
            float x = cpt.X + 4; // start x

            int num = percentages.Count(); // number
            float xDelta = (Width - Margin - ArrawLen - 4 - 2) / (num - 1);
            float scale = (float)(scaleNum * 10.0);

            float bottomY = Height - 12 - ArrawLen - 5;
            float deltaY = (bottomY - cpt.Y) / scale;

            Pen myPen = new Pen(Color.Blue, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            float x0 = 0, y0 = 0;
            foreach (double percent in percentages)
            {
                float y = (float)(cpt.Y - deltaY * percent);

                if (Math.Abs(percent) > threshold)
                {
                    gph.DrawEllipse(Pens.Cyan, x - 2.0f, y - 2.0f, 4, 4);
                    gph.FillEllipse(new SolidBrush(Color.Cyan), x - x - 2.0f, y - 2.0f, 4, 4);
                }
                else
                {
                    gph.DrawEllipse(Pens.Black, x - 1.5f, y - 1.5f, 3, 3);
                    gph.FillEllipse(new SolidBrush(Color.Black), x - 1.5f, y - 1.5f, 3, 3);
                }

                gph.DrawLine(myPen, x, y, x, cpt.Y); // draw blue vertical line

                if (index > 1)
                {
                    gph.DrawLine(Pens.Red, x0, y0, x, y);
                }

                x0 = x;
                y0 = y;

                x += xDelta;
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

        private static void DrawAxis(Graphics gph, PointF cpt, int scaleNum)
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
            gph.DrawString("Test", new Font("Calibri", 12), Brushes.Black, new PointF(xEnd.X - 15 , xEnd.Y + 9));

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
            gph.DrawString("Percentage (%)", new Font("Calibri", 12), Brushes.Black, new PointF(0, 0 - 4));

            DrawYScale(gph, cpt, new PointF(yEnd.X, yEnd.Y + 5), scaleNum);

            // draw -y
            yEnd = new PointF(cpt.X, Height - ArrawLen - YMargin);
            ypt = new PointF[3]
            {
                new PointF(yEnd.X, yEnd.Y + ArrawLen),
                new PointF(yEnd.X - ArrawWidth, yEnd.Y),
                new PointF(yEnd.X + ArrawWidth, yEnd.Y)
            };//-y triangle
            gph.DrawLine(Pens.Black, cpt.X, cpt.Y, yEnd.X, yEnd.Y);
            gph.DrawPolygon(Pens.Black, ypt);
            gph.FillPolygon(new SolidBrush(Color.Black), ypt);
            gph.DrawString("Percentage (-%)", new Font("Calibri", 12), Brushes.Black, new PointF(0, Height - 18));

            DrawYScale(gph, cpt, new PointF(yEnd.X, yEnd.Y - 5), scaleNum);
        }

        private static void DrawYScale(Graphics gph, PointF start, PointF end, int scaleNum)
        {
            float delta = (end.Y - start.Y) / scaleNum;
            float y = start.Y;
            float x = start.X;

            Pen myPen = new Pen(Color.LightGray, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            for (int i = 1; i <= scaleNum; i++)
            {
                float deltaY = i * delta;
                float tmp = 20;
                if (i == 10)
                {
                    tmp = 25;
                }

                gph.DrawString((i * 10).ToString(), new Font("Calibri", 10), Brushes.Black, new PointF(start.X - tmp, y - deltaY - 8));
                gph.DrawLine(Pens.Black, x - 3, y - deltaY, x, y - deltaY);

                gph.DrawLine(myPen, x, y - deltaY, Width - 5, y - deltaY);
            }
        }

        private static void DrawThreshold(Graphics gph, PointF cpt, int threshold, int scaleNum)
        {
            float topY = 12 + ArrawLen + 5;
            float x = cpt.X;
            float scale = (float)(scaleNum * 10.0);

            double y = cpt.Y - ((topY - cpt.Y) / scale) * threshold;

            Pen myPen = new Pen(Color.Cyan, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            gph.DrawLine(myPen, x, (float)y, Width - 5 , (float)y );

            topY = Height - 12 - ArrawLen - 5;

            y = cpt.Y - ((topY - cpt.Y) / scale) * threshold;

            gph.DrawLine(myPen, x, (float)y, Width - 5, (float)y);
        }
    }
}
