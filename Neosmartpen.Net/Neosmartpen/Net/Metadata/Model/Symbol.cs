using Neosmartpen.Net.Support;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Neosmartpen.Net.Metadata.Model
{
    /// <summary>
    /// A class representing a Symbol in metadata
    /// </summary>
    public class Symbol
	{
        public const float Pixel2DotScaleFactor = 600f / 72f / 56f;

        /// <summary>
        /// Symbol ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Section
        /// </summary>
        public int Section { get; set; }
        /// <summary>
        /// Owner
        /// </summary>
        public int Owner { get; set; }
        /// <summary>
        /// Book Code
        /// </summary>
        public int Book { get; set; }
        /// <summary>
        /// The number of the page where the symbol is located
        /// </summary>
        public int Page { get; set; }
		/// <summary>
		/// Symbol Type = Rectangle, Trangle, Ellipse, Polygon etc.
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// Symbol's Name
		/// </summary>
		public string Name { get; set; }

		public string PageName { get; set; }
		/// <summary>
		/// coord X
		/// </summary>
		public float X { get; set; }
		/// <summary>
		/// coord Y
		/// </summary>
		public float Y { get; set; }
		/// <summary>
		/// Width
		/// </summary>
		public float Width { get; set; }
		/// <summary>
		/// Height
		/// </summary>
		public float Height { get; set; }
		/// <summary>
		/// Action : Email etc.
		/// </summary>
		public string Action { get; set; }
        /// <summary>
        /// Polygon Points
        /// </summary>
        public string CustomShapePoints { get; set; }

        /// <summary>
        /// Previous Linked Symbol ID
        /// </summary>
        public string PreviousLink { get; set; }
		/// <summary>
		/// Next Linked Symbol ID
		/// </summary>
		public string NextLink { get; set; }
        /// <summary>
        /// param (Used only in Nproj 2.2 ver)
        /// </summary>
        public string Param { get; set; }

        private PointF[] GetCustomShapeDotPoints()
        {
            try
            {
                if (CustomShapePoints == null)
                {
                    return null;
                }

                List<PointF> result = new List<PointF>();

                var points = CustomShapePoints.Split(',');

                for (int i = 0; i < points.Length; i = i + 2)
                {
                    float px = NConvert.ToSingle(points[i]);
                    float py = NConvert.ToSingle(points[i + 1]);

                    float spx = (X + (px * Width)) * Pixel2DotScaleFactor;
                    float spy = (Y + (py * Height)) * Pixel2DotScaleFactor;

                    result.Add(new PointF(spx, spy));
                }

                return result.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private bool Contains(float dotX, float dotY)
        {
            if (Type == "Rectangle")
            {
                return
                    (this.X * Pixel2DotScaleFactor) <= dotX &&
                    (this.Y * Pixel2DotScaleFactor) <= dotY &&
                    (this.X * Pixel2DotScaleFactor) + (this.Width * Pixel2DotScaleFactor) >= dotX &&
                    (this.Y * Pixel2DotScaleFactor) + (this.Height * Pixel2DotScaleFactor) > dotY;
            }
            else if (Type == "Ellipse")
            {
                return CheckPtInEllipse(dotX, dotY, X * Pixel2DotScaleFactor, (X+Width) * Pixel2DotScaleFactor, Y * Pixel2DotScaleFactor, (Y+Height) * Pixel2DotScaleFactor);
            }
            else if (Type == "Triangle")
            {
                return CheckPtInTriangle(dotX, dotY, X * Pixel2DotScaleFactor, Y * Pixel2DotScaleFactor, Width * Pixel2DotScaleFactor, Height * Pixel2DotScaleFactor);
            }
            else if (Type == "Custom")
            {
                var points = GetCustomShapeDotPoints();

                if (points == null || points.Count() <= 0)
                {
                    return false;
                }

                return IsInPolygon(points, new PointF(dotX, dotY));
            }
            else
            {
                return false;
            }
        }

        private bool CheckPtInTriangle(float x, float y, float sx, float sy, float width, float height)
        {
            float x1 = sx + width / 2;
            float y1 = sy;
            float x2 = sx;
            float y2 = sy + height;
            float x3 = sx + width;
            float y3 = sy + height;

            //return CheckPtInTriangle(x, y, x1, x2, x3, y1, y2, y3);
            return IsInPolygon(new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) }, new PointF(x, y));
        }

        private bool CheckPtInTriangle(float x, float y, float x1, float x2, float x3, float y1, float y2, float y3)
        {
            double a = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            double b = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            double c = 1 - a - b;

            if (a == 0 || b == 0 || c == 0) return true;
            else if (a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1) return true;
            else return false;
        }

        /*
        private bool CheckPtInTriangle(int x, int y, int x1, int x2, int y1, int y2)
        {
            int t = 0;

            //좌우 대칭이므로, 삼각형을 반으로 쪼개서 빗변의 기울기를 구한다.
            int halfWidth = (x2 - x1) >> 1;
            int centerX = x1 + halfWidth;
            int height = y2 - y1;
            float degree = (float)height / (float)halfWidth;

            //상단 y 값을 기준으로, y 좌표의 offset 좌표 값을 구한다.
            int yOffset = y - y1;

            //Y offset 좌표에 기울기를 적용하여, 비율에 따라 Y 좌표 시점일 때의 폭을 구한다.
            int xWidth = (int)((float)yOffset / degree);

            //x가 삼각형 중앙을 기준으로 왼쪽에 위치하면
            if (x < centerX)
            {
                //x가 왼쪽 빗변(선) 바깥에 위치하면 0, 아니면 1
                t = x < centerX - xWidth ? 0 : 1;
            }
            //x가 삼각형 중앙을 기준으로 오른쪽에 위치하면
            else
            {
                //x가 오른쪽 빗변(선) 바깥에 위치하면 0, 아니면 1
                t = x > centerX + xWidth ? 0 : 1;
            }

            //이 함수 호출 직전에 y 값의 range 체크가 이루어진 상태이므로 여기선 추가적인 검사는 하지 않는다.
            //if( t )
            //    t = ( y1 <= y && y <= y2 ) ? 1 : 0;

            return t == 1;
        }
        */

        private bool CheckPtInEllipse(float x, float y, float x1, float x2, float y1, float y2)
        {
            float a = (x2 - x1) / 2;
            float b = (y2 - y1) / 2;

            float cx = x1 + a;
            float cy = y1 + b;

            float dx = x - cx;
            float dy = y - cy;

            float result = (float)(dx * dx) / (float)(a * a) + (float)(dy * dy) / (float)(b * b);

            if (result <= 1.0f)
                return true;
            else
                return false;
        }

        private bool IsInPolygon(PointF[] poly, PointF pnt)
        {
            int i, j;
            int nvert = poly.Length;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((poly[i].Y > pnt.Y) != (poly[j].Y > pnt.Y)) &&
                 (pnt.X < (poly[j].X - poly[i].X) * (pnt.Y - poly[i].Y) / (poly[j].Y - poly[i].Y) + poly[i].X))
                    c = !c;
            }
            return c;
        }

        private RectangleF GetRect()
        {
            return new RectangleF(X * Pixel2DotScaleFactor, Y * Pixel2DotScaleFactor, Width * Pixel2DotScaleFactor, Height * Pixel2DotScaleFactor);
        }

        private bool Intersect(Stroke stroke, float offsetX, float offsetY)
        {
            RectangleF result = GetRect();
            var srect = stroke.GetRect();

            RectangleF target = new RectangleF(srect.Left + offsetX, srect.Top + offsetY, srect.Width, srect.Height);

            result.Intersect(target);

            return result != RectangleF.Empty;
        }

        /// <summary>
        /// Function to check whether strokes and symbols overlap
        /// </summary>
        /// <param name="stroke">Stroke with multiple dots</param>
        /// <param name="offsetX">Offset x coordinate</param>
        /// <param name="offsetY">Offset y coordinate</param>
        /// <returns>True if stroke and symbol overlap, false if not</returns>
        public bool Contains(Stroke stroke, float offsetX = 0, float offsetY = 0)
        {
            if ( Section == stroke.Section && Owner == stroke.Owner && Book == stroke.Note && Page == stroke.Page && Intersect(stroke, offsetX, offsetY) )
            {
                for (int i = 0; i < stroke.Count; i++)
                {
                    var dot = stroke[i];

                    // 점으로 비교
                    if (Contains(dot.X + offsetX, dot.Y + offsetY))
                    {
                        return true;
                    }

                    // 선으로 비교
                    if (i != 0 && Type == "Rectangle")
                    {
                        var pdot = stroke[i-1];

                        if (Is2DotSymbolCross(pdot, dot, this, offsetX, offsetY))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Function to check whether point and symbols overlap
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <param name="page">The number of the page where the symbol is located</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="offsetX">Offset x coordinate</param>
        /// <param name="offsetY">Offset y coordinate</param>
        /// <returns>True if stroke and symbol overlap, false if not</returns>
        public bool Contains(int section, int owner, int book, int page, float x, float y, float offsetX = 0, float offsetY = 0)
        {
            return Section == section && Owner == owner && Book == book && Page == page && Contains(x + offsetX, y + offsetY);
        }

        /// <summary>
        /// Function to check whether point and symbols overlap
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="offsetX">Offset x coordinate</param>
        /// <param name="offsetY">Offset y coordinate</param>
        /// <returns>True if stroke and symbol overlap, false if not</returns>
        public bool Contains(float x, float y, float offsetX = 0, float offsetY = 0)
        {
            return Contains(x + offsetX, y + offsetY);
        }

        private bool Is2DotSymbolCross(Dot dot1, Dot dot2, Symbol symbol, float offsetX, float offsetY)
        {
            RectangleF rect = symbol.GetRect();

            PointF ap1 = new Point();
            ap1.X = dot1.X + offsetX;
            ap1.Y = dot1.Y + offsetY;

            PointF ap2 = new Point();
            ap2.X = dot2.X + offsetX;
            ap2.Y = dot2.Y + offsetY;

            PointF bp1 = new Point();
            bp1.X = rect.Left;
            bp1.Y = rect.Top;

            PointF bp2 = new Point();
            bp2.X = rect.Right;
            bp2.Y = rect.Top;

            PointF bp3 = new Point();
            bp3.X = rect.Right;
            bp3.Y = rect.Bottom;

            PointF bp4 = new Point();
            bp4.X = rect.Left;
            bp4.Y = rect.Bottom;

            if (IsLineCross(ap1, ap2, bp1, bp2))
                return true;
            else if (IsLineCross(ap1, ap2, bp2, bp3))
                return true;
            else if (IsLineCross(ap1, ap2, bp3, bp4))
                return true;
            else if (IsLineCross(ap1, ap2, bp4, bp1))
                return true;
            else
                return false;

        }

        private bool IsLineCross(PointF ap1, PointF ap2, PointF bp1, PointF bp2)
        {
            double t;
            double s;
            double under = (bp2.Y - bp1.Y) * (ap2.X - ap1.X) - (bp2.X - bp1.X) * (ap2.Y - ap1.Y);
            if (under == 0) return false;

            double _t = (bp2.X - bp1.X) * (ap1.Y - bp1.Y) - (bp2.Y - bp1.Y) * (ap1.X - bp1.X);
            double _s = (ap2.X - ap1.X) * (ap1.Y - bp1.Y) - (ap2.Y - ap1.Y) * (ap1.X - bp1.X);

            t = _t / under;
            s = _s / under;

            if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0) return false;
            if (_t == 0 && _s == 0) return false;

            // 교차 지점을 구하는 공식
            // P.x = (int) (ap1.x + t * (double) (ap2.x - ap1.x));
            // P.y = (int) (ap1.y + t * (double) (ap2.y - ap1.y));

            return true;
        }
    }
}
