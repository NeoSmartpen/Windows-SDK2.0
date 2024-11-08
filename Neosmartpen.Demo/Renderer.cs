using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Neosmartpen.Net.Support
{
    /// <summary>
    /// Utility class for drawing Stroke
    /// </summary>
    public class Renderer
    {
        private const float Margin = 20;


        /// <summary>
        /// Draw a stroke on the Bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to which the stroke will be drawn</param>
        /// <param name="stroke">Stroke to draw</param>
        /// <param name="scaleX">x scale</param>
        /// <param name="scaleY">y scale</param>
        /// <param name="offsetX">x offset</param>
        /// <param name="offsetY">y offset</param>
        /// <param name="width">Thickness of Stroke</param>
        /// <param name="color">Color of Stroke</param>
        public static void Draw(Bitmap bitmap, Stroke stroke,  float scaleX, float scaleY, float offsetX, float offsetY, int width, Color color)
        {
            RenderStroke m= StroketoRenderStroke(stroke);
            RectangleF rect = RectangleF.Empty;
            Draw(bitmap, m, scaleX, scaleY, offsetX, offsetY, width, color, ref rect);
        }

        private static void Draw(Bitmap bitmap, RenderStroke neoStroke, float scaleX, float scaleY, float offsetX, float offsetY, int width, Color color, ref RectangleF bound)
        {
            bool outline = false;

            if (neoStroke == null || neoStroke.dotCount == 0)
                return;

            ///////////////////////////////////////////////
            // 스트로크 랜더링을 위한 준비작업
            int ColorA = Math.Min(10*width,(int)(color.A*0.1));

            Pen blackPen = new Pen(Color.FromArgb(ColorA, color), 1);

            SolidBrush mBrush = new SolidBrush(color);

            GraphicsPath graphPath = new GraphicsPath();

            PointF m_ptOrigin = new PointF(offsetX, offsetY);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float scale = Math.Min(scaleX, scaleY);

            float fMin = 1f;

            ///////////////////////////////////////////////
            // 스트로크 그리기

            int count = neoStroke.dotCount;
            RenderStroke dotArray = neoStroke;

            if (count <= 2)
            {
                float[] x, y, p;
                x = new float[count + 1];
                y = new float[count + 1];
                p = new float[count + 1];

                for (int i = 0; i < count; i++)
                {
                    x[i] = (dotArray.fX[i]) * scaleX + (float)m_ptOrigin.X;
                    y[i] = (dotArray.fY[i]) * scaleY + (float)m_ptOrigin.Y;
                    p[i] = dotArray.fPressureRate[i];
                }

                if (count == 1)
                {
                    graphics.DrawEllipse(blackPen, x[0] - 1, y[0] - 1, 2, 2);
                }
                else if (count == 2)
                {
                    graphics.DrawLine(blackPen, x[0], y[0], x[1], y[1]);
                }
            }
            else
            {
                float x0, x1, x2, x3, y0, y1, y2, y3, p0, p1, p2, p3;
                float vx01, vy01, vx21, vy21;  // unit tangent vectors 0->1 and 1<-2
                float norm;
                float n_x0, n_y0, n_x2, n_y2; // the normals 

                float thick = width;

                // the first actual point is treated as a midpoint
                x0 = (dotArray.fX[0]) * scaleX + (float)m_ptOrigin.X + 0.1f;
                y0 = (dotArray.fY[0]) * scaleY + (float)m_ptOrigin.Y;
                p0 = Math.Max(fMin, dotArray.fPressureRate[0] * thick);

                x1 = (dotArray.fX[1]) * scaleX + (float)m_ptOrigin.X + 0.1f;
                y1 = (dotArray.fY[1]) * scaleY + (float)m_ptOrigin.Y;
                p1 = Math.Max(fMin, dotArray.fPressureRate[1] * thick);

                vx01 = x1 - x0;
                vy01 = y1 - y0;

                norm = (float)Math.Sqrt(vx01 * vx01 + vy01 * vy01 + 0.0001f) * 2.0f;
                vx01 = vx01 / norm * p0;
                vy01 = vy01 / norm * p0;
                n_x0 = vy01;
                n_y0 = -vx01;

                for (int i = 2; i < count - 1; i++)
                {
                    x3 = (dotArray.fX[i]) * scaleX + (float)m_ptOrigin.X;
                    y3 = (dotArray.fY[i]) * scaleY + (float)m_ptOrigin.Y;
                    p3 = Math.Max(fMin, dotArray.fPressureRate[i] * thick);

                    x2 = (x1 + x3) / 2.0f;
                    y2 = (y1 + y3) / 2.0f;
                    p2 = (p1 + p3) / 2.0f;
                    vx21 = x1 - x2;
                    vy21 = y1 - y2;
                    norm = (float)Math.Sqrt(vx21 * vx21 + vy21 * vy21 + 0.0001f) * 2.0f;
                    vx21 = vx21 / norm * p2;
                    vy21 = vy21 / norm * p2;
                    n_x2 = -vy21;
                    n_y2 = vx21;

                    graphPath.Reset();
                    //agg_path.MoveTo(x0 + n_x0, y0 + n_y0);

                    // Draw a curve : 일단 한 선만 죽 그리면서 간다.
                    // The + boundary of the stroke

                    graphPath.AddBezier(x0 + n_x0, y0 + n_y0, (x1 + n_x0), (y1 + n_y0), (x1 + n_x2), (y1 + n_y2), (x2 + n_x2), (y2 + n_y2));

                    // round out the cap
                    //graphPath.AddBezier((x2 + n_x2), (y2 + n_y2), x2 + n_x2 - vx21, y2 + n_y2 - vy21, x2 - n_x2 - vx21, y2 - n_y2 - vy21, x2 - n_x2, y2 - n_y2);

                    // THe - boundary of the stroke
                    graphPath.AddBezier(x2 - n_x2, y2 - n_y2, x1 - n_x2, y1 - n_y2, x1 - n_x0, y1 - n_y0, x0 - n_x0, y0 - n_y0);

                    // round out the other cap
                    if (i == 2)
                        graphPath.AddBezier(x0 - n_x0, y0 - n_y0, x0 - n_x0 - vx01, y0 - n_y0 - vy01, x0 + n_x0 - vx01, y0 + n_y0 - vy01, x0 + n_x0, y0 + n_y0);

                    graphics.FillPath(mBrush, graphPath);
                    if (outline) graphics.DrawPath(blackPen, graphPath);
                    MergeBound(ref bound, graphPath.GetBounds());
                    //graphics.Render(strockedLineToDraw, color);

                    x0 = x2; y0 = y2; p0 = p2;
                    x1 = x3; y1 = y3; p1 = p3;
                    vx01 = -vx21; vy01 = -vy21;
                    n_x0 = n_x2; n_y0 = n_y2;
                }
                
                x2 = (dotArray.fX[count - 1]) * scaleX + (float)m_ptOrigin.X;
                y2 = (dotArray.fY[count - 1]) * scaleY + (float)m_ptOrigin.Y;
                p2 = Math.Max(fMin, dotArray.fPressureRate[count - 1] * thick);

                vx21 = x1 - x2;
                vy21 = y1 - y2;
                norm = (float)Math.Sqrt(vx21 * vx21 + vy21 * vy21 + 0.0001f) * 2.0f;
                vx21 = vx21 / norm * p2;
                vy21 = vy21 / norm * p2;
                n_x2 = -vy21;
                n_y2 = vx21;

                graphPath.Reset();
                graphPath.AddBezier(x0 + n_x0, y0 + n_y0, x1 + n_x0, y1 + n_y0, x1 + n_x2, y1 + n_y2, x2 + n_x2, y2 + n_y2);
                graphPath.AddBezier(x2 + n_x2, y2 + n_y2, x2 + n_x2 - vx21, y2 + n_y2 - vy21, x2 - n_x2 - vx21, y2 - n_y2 - vy21, x2 - n_x2, y2 - n_y2);
                graphPath.AddBezier(x2 - n_x2, y2 - n_y2, x1 - n_x2, y1 - n_y2, x1 - n_x0, y1 - n_y0, x0 - n_x0, y0 - n_y0);

                //graphPath.AddBezier(x0 - n_x0, y0 - n_y0, x0 - n_x0 - vx01, y0 - n_y0 - vy01, x0 + n_x0 - vx01, y0 + n_y0 - vy01, x0 + n_x0, y0 + n_y0);

                //System.Console.WriteLine("Boundary1 {0},{1},{2},{3},{4},{5},{6},{7}", x0 + n_x0, y0 + n_y0, x1 + n_x0, y1 + n_y0, x1 + n_x2, y1 + n_y2, x2 + n_x2, y2 + n_y2);
                //System.Console.WriteLine("Round cap1 {0},{1},{2},{3},{4},{5},{6},{7}", x2 + n_x2, y2 + n_y2, x2 + n_x2 - vx21, y2 + n_y2 - vy21, x2 - n_x2 - vx21, y2 - n_y2 - vy21, x2 - n_x2, y2 - n_y2);
                //System.Console.WriteLine("Boundary2 {0},{1},{2},{3},{4},{5},{6},{7}", x2 - n_x2, y2 - n_y2, x1 - n_x2, y1 - n_y2, x1 - n_x0, y1 - n_y0, x0 - n_x0, y0 - n_y0);
                //System.Console.WriteLine("cap2 {0},{1},{2},{3},{4},{5},{6},{7}", x0 - n_x0, y0 - n_y0, x0 - n_x0 - vx01, y0 - n_y0 - vy01, x0 + n_x0 - vx01, y0 + n_y0 - vy01, x0 + n_x0, y0 + n_y0);

                graphics.FillPath(mBrush, graphPath);
                if (outline) graphics.DrawPath(blackPen, graphPath);
                MergeBound(ref bound, graphPath.GetBounds());
                //              graphics.Render(strockedLineToDraw, color);
                mBrush.Dispose();
                blackPen.Dispose();
                graphics.Dispose();
            }
        }      
        
        private static RectangleF MergeBound(ref RectangleF selectedRect, RectangleF boundingBox)
        {
            if (selectedRect == RectangleF.Empty)
            {
                selectedRect.X = boundingBox.X;
                selectedRect.Y = boundingBox.Y;
                selectedRect.Width = boundingBox.Width;
                selectedRect.Height = boundingBox.Height;

                return selectedRect;
            }

            if (boundingBox.X < selectedRect.X)
            {
                if (selectedRect.Width != float.NegativeInfinity)
                    selectedRect.Width += selectedRect.X - boundingBox.X;
                selectedRect.X = boundingBox.X;
            }
            if (boundingBox.Right > selectedRect.Right) selectedRect.Width = boundingBox.Right - selectedRect.Left;
            if (boundingBox.Y < selectedRect.Y)
            {
                if (selectedRect.Height != float.NegativeInfinity)
                    selectedRect.Height += selectedRect.Y - boundingBox.Y;
                selectedRect.Y = boundingBox.Y;
            }
            if (boundingBox.Bottom > selectedRect.Bottom) selectedRect.Height = boundingBox.Bottom - selectedRect.Top;

            return selectedRect;
        }

        private static RenderStroke StroketoRenderStroke(Stroke stroke)
        {
            RenderStroke ms = new RenderStroke();
            ms.CreatePointArray(stroke.Count);
            int i = 0;
            foreach (Dot m in stroke)
            {
                ms.fX[i] = m.X;
                ms.fY[i] = m.Y;
                ms.fPressureRate[i] =(float) m.Force/1023;
                i++;
            }
            return ms;
        }
    }

    class RenderStroke
    {
        public int dotCount = 0;
        public float[] fX;
        public float[] fY;			    // pixel 좌표 값 (inch 값이며, 고정 값이다. 이 값으로 nX, nY가 계산된다)
        public float[] fPressureRate;	// 필압의 비율이다. (이 값으로 nDotSize가 계산된다)
        public byte[] timestampDelta;	// 스트로크 시작 시점의 timestamp(64비트)를 기준으로, 추가되는 점들의 시간 차이값 (밀리초 단위)

        public void CreatePointArray(int count)
        {
            fX = new float[count];
            fY = new float[count];
            fPressureRate = new float[count];
            timestampDelta = new byte[count];
            dotCount = count;
        }

        public void Clear()
        {
            fX = null;
            fY = null;
            fPressureRate = null;
            timestampDelta = null;
            dotCount = 0;
        }
    }

    class RenderParam
    {
        public float dpi;
        public float scale;
        public float baseThickness;
        public static float PenBallSizeInch = 0.7f; //0.7mm

        public RenderParam()
        {
            dpi = 72.0f;
            scale = 0.01f;//0.01f;
            SetThickness(2f); //2가 적당함
        }

        public RenderParam(float dpi, float scale, float thickness)
        {
            this.dpi = dpi;
            this.scale = scale;
            SetThickness(thickness);
        }

        public void SetThickness(float thickness)
        {
            baseThickness = PenBallSizeInch * thickness * dpi;
        }
    }
}
