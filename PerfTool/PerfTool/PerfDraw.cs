using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfTool
{
    static class PerfDraw
    {
        private const int Width = 500;
        private const int Height = 500;
        private const int Margin = 30;
        private const int YMargin = 12; // write the percentage (%)
        private const int ArrawLen = 15;
        private const int ArrawWidth = 6;
        private const int YScale = 6;// means draw top 60%
        public static void Draw(IList<DiffItem> items, string fileName, int threshold, PerformanceTest perf)
        {
            Bitmap bmap = new Bitmap(Width, Height);
            Graphics gph = Graphics.FromImage(bmap);
            gph.Clear(Color.White);

            PointF cpt = new PointF(Margin, Height/2);//center point & start point
            PointF topPt = new PointF(Margin, YMargin); //  /|\
            PointF bottomPt = new PointF(Margin, Height - YMargin);  // \|/

            double maxPercentage = items.Max(c => c.Percentage < 0 ? c.Percentage * -1 : c.Percentage);
            int scaleNum = (int)((maxPercentage / 10.0) + 0.5);
            scaleNum = scaleNum > YScale ? scaleNum : YScale; // Y aliax can draw max percentage (60%)

            DrawAliax(gph, cpt, scaleNum);
            DrawThreshold(gph, cpt, threshold, scaleNum);

            // draw the data
            int index = 1;
            float x = cpt.X + 4; // start x

            int num = items.Count; // number
            float xDelta = (Height - Margin - ArrawLen - 1) / num;
            float scale = (float)(scaleNum * 10.0);

            float bottomY = Height - 12 - ArrawLen - 5;
            float deltaY = (bottomY - cpt.Y) / scale;

            Pen myPen = new Pen(Color.Blue, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            float x0 = 0, y0 = 0;
            foreach (DiffItem item in items)
            {
                float y = (float)(cpt.Y - deltaY * item.Percentage);

                gph.DrawEllipse(Pens.Black, x - 1.5f, y - 1.5f, 3, 3);
                gph.FillEllipse(new SolidBrush(Color.Black), x - 1.5f, y - 1.5f, 3, 3);

                // gph.DrawString(index.ToString(), new Font("Calibri", 11), Brushes.Black, new PointF(x, y));

                gph.DrawLine(myPen, x, y, x, cpt.Y);
                // gph.DrawLine(myPen, x, yy, cpt.X, yy);

                if (index > 1)
                {
                    gph.DrawLine(Pens.Red, x0, y0, x, y);
                }

                x0 = x;
                y0 = y;

                x += xDelta;
                index++;
            }

            // Draw Lenge
            x = Width / 2 - 20;
            string legen = perf.TestType + " / " + perf.CreateDate + " / " + perf.BuildId;
            
            gph.DrawString(legen, new Font("Calibri", 11), Brushes.DarkBlue, new PointF(x, 0));

            bmap.Save(fileName, ImageFormat.Png);
        }

        private static void DrawAliax(Graphics gph, PointF cpt, int scaleNum)
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
            float delta = (float)((end.Y - start.Y) / scaleNum);
            // float extra = delta < 0 ? 6 : -6;
            float y = start.Y;
            float x = start.X;

            Pen myPen = new Pen(Color.LightGray, 1.0F);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            for (int i = 1; i <= scaleNum; i++)
            {
                float deltaY = i * delta;
                gph.DrawString((i * 10).ToString(), new Font("Calibri", 10), Brushes.Black, new PointF(start.X - 20, (float)(y - deltaY - 8)));
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
