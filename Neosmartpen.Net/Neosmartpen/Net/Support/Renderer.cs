using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Neosmartpen.Net.Support
{
    public class Renderer
    {

        public static void draw(Bitmap bitmap, Stroke[] stroke, float scalex, float scaley, float offset_x, float offset_y, int width, Color color)
        {
            foreach (Stroke m in stroke)
            {
                draw(bitmap, m, scalex, scaley, offset_x, offset_y, width, color);
            }
        }

        public static void draw(Bitmap bitmap, Stroke stroke,  float scalex, float scaley, float offset_x, float offset_y, int width, Color color)
        {
            RenderStroke m= StroketoRenderStroke(stroke);
            draw(bitmap, m, scalex, scaley, offset_x, offset_y, width, color);

        }


        private static void draw(Bitmap bitmap, RenderStroke neoStroke, float scalex, float scaley, float offset_x, float offset_y, int width, Color color)
        {
            bool outline = true;
            if (neoStroke == null || neoStroke.dotCount == 0)
                return;

            ///////////////////////////////////////////////
            // 스트로크 랜더링을 위한 준비작업
            int ColorA = Math.Min(10*width,(int)(color.A*0.1));

            Pen blackPen = new Pen(Color.FromArgb(ColorA, color), 1);

            SolidBrush mBrush = new SolidBrush(color);

            GraphicsPath graphPath = new GraphicsPath();

            PointF m_ptOrigin = new PointF(offset_x, offset_y);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float scale = Math.Min(scalex, scaley);

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
                    x[i] = dotArray.fX[i] * scalex + (float)m_ptOrigin.X;
                    y[i] = dotArray.fY[i] * scaley + (float)m_ptOrigin.Y;
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
                x0 = dotArray.fX[0] * scalex + (float)m_ptOrigin.X + 0.1f;
                y0 = dotArray.fY[0] * scaley + (float)m_ptOrigin.Y;
                p0 = Math.Max(fMin, dotArray.fPressureRate[0] * thick);

                x1 = dotArray.fX[1] * scalex + (float)m_ptOrigin.X + 0.1f;
                y1 = dotArray.fY[1] * scaley + (float)m_ptOrigin.Y;
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

                    x3 = dotArray.fX[i] * scalex + (float)m_ptOrigin.X;
                    y3 = dotArray.fY[i] * scaley + (float)m_ptOrigin.Y;
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
                    //graphics.Render(strockedLineToDraw, color);

                    x0 = x2; y0 = y2; p0 = p2;
                    x1 = x3; y1 = y3; p1 = p3;
                    vx01 = -vx21; vy01 = -vy21;
                    n_x0 = n_x2; n_y0 = n_y2;
                }

                x2 = dotArray.fX[count - 1] * scalex + (float)m_ptOrigin.X;
                y2 = dotArray.fY[count - 1] * scaley + (float)m_ptOrigin.Y;
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
                //              graphics.Render(strockedLineToDraw, color);
                mBrush.Dispose();
                blackPen.Dispose();
                graphics.Dispose();
            }
        }       
    

    private static RenderStroke StroketoRenderStroke(Stroke mStroke)
    {
        RenderStroke ms = new RenderStroke();
        ms.createPointArray(mStroke.Count);
        int i = 0;
        foreach (Dot m in mStroke)
        {
            ms.fX[i] = m.X  + m.Fx * 0.01f;
            ms.fY[i] = m.Y  + m.Fy * 0.01f;
            ms.fPressureRate[i] =(float) m.Force/255;
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

        public void createPointArray(int count)
        {
            fX = new float[count];
            fY = new float[count];
            fPressureRate = new float[count];
            timestampDelta = new byte[count];
            dotCount = count;
        }

        public void clear()
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
        public float base_thickness;
        public static float PenBallSizeInch = 0.7f; //0.7mm

        public RenderParam()
        {
            dpi = 72.0f;
            scale = 0.01f;//0.01f;
            setThickness(2f); //2가 적당함
        }

        public RenderParam(float d, float s, float t)
        {
            dpi = d;
            scale = s;
            setThickness(t);
        }

        public void setThickness(float thickness)
        {
            base_thickness = PenBallSizeInch * thickness * dpi;
        }
    }


}
