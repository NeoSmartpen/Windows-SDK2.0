using System;

namespace Neosmartpen.Net.Filter
{
	public class FilterForPaper
	{
		#region Constants
		private static readonly int DELTA = 10;
		private static readonly int MAX_X = 15070;
		private static readonly int MAX_Y = 8480;
		private static readonly int MAX_OWNER = 1024;
		private static readonly int MAX_NOTE_ID = 16384;
		private static readonly int MAX_PAGE_ID = 262143;
		#endregion

		#region Variables
		private Dot dot1, dot2;
		private Dot makeDownDot, makeMoveDot;
		private bool secondCheck = true, thirdCheck = true;
		#endregion

		#region Delegates
		public delegate void FilteredDot(Dot dot, object obj);
		private FilteredDot filteredDot;
		#endregion

		public FilterForPaper(FilteredDot func)
		{
			filteredDot = func;
		}

		public void Put(Dot dot, object obj)
		{
			if (!ValidateCode(dot))
			{
				return;
			}

			if (dot.DotType == DotTypes.PEN_DOWN)
			{
				dot1 = dot;
			}
			else if (dot.DotType == DotTypes.PEN_MOVE)
			{
				if ( secondCheck )
				{
					dot2 = dot;
					secondCheck = false;
				}
				else if ( thirdCheck )
				{
					if ( ValidateStartDot(dot1, dot2, dot) )
					{
						filteredDot(dot1, obj);

						if ( ValidateMiddleDot(dot1, dot2, dot))
						{
							filteredDot(dot2, obj);
							dot1 = dot2;
							dot2 = dot;
						}
						else
						{
							dot2 = dot;
						}
					}
					else
					{
						dot1 = dot2;
						dot2 = dot;
					}

					thirdCheck = false;
				}
				else
				{
					if (ValidateMiddleDot(dot1, dot2, dot))
					{
						filteredDot(dot2, obj);
						dot1 = dot2;
						dot2 = dot;
					}
					else
					{
						dot2 = dot;
					}
				}
			}
			else if ( dot.DotType == DotTypes.PEN_UP)
			{
				bool validateStartDot = true;
				bool validateMiddleDot = true;
				if ( secondCheck)
				{
					dot2 = dot1;
				}
				if ( thirdCheck && dot.DotType == DotTypes.PEN_DOWN)
				{
					if (ValidateStartDot(dot1, dot2, dot))
					{
						filteredDot(dot1, obj);
					}
					else
					{
						validateStartDot = false;
					}
				}

				if (ValidateMiddleDot(dot1, dot2, dot))
				{

					if (!validateStartDot)
					{
						makeDownDot = new Dot(dot2.Owner, dot2.Section, dot2.Note, dot2.Page, dot2.Timestamp, dot2.X, dot2.Y, dot2.Fx, dot2.Fy, dot2.TiltX, dot2.TiltY, dot2.Twist, dot2.Force, DotTypes.PEN_DOWN, dot2.Color);
						filteredDot(makeDownDot, obj);
					}

					filteredDot(dot2, obj);
				}
				else
				{
					validateMiddleDot = false;
				}

				// 마지막 Dot 검증
				if (ValidateEndDot(dot1, dot2, dot))
				{
					if (!validateStartDot && !validateMiddleDot)
					{
						makeDownDot = new Dot(dot.Owner, dot.Section, dot.Note, dot.Page, dot.Timestamp, dot.X, dot.Y, dot.Fx, dot.Fy, dot.TiltX, dot.TiltY, dot.Twist, dot.Force, DotTypes.PEN_DOWN, dot.Color);
						filteredDot(makeDownDot, obj);
					}
					if (thirdCheck && !validateMiddleDot)
					{
						makeMoveDot= new Dot(dot.Owner, dot.Section, dot.Note, dot.Page, dot.Timestamp, dot.X, dot.Y, dot.Fx, dot.Fy, dot.TiltX, dot.TiltY, dot.Twist, dot.Force, DotTypes.PEN_MOVE, dot.Color);
						filteredDot(makeMoveDot, obj);
					}
					filteredDot(dot, obj);
				}
				else
				{
					dot2.DotType = DotTypes.PEN_UP;
					filteredDot(dot2, obj);
				}

				// Dot 및 변수 초기화
				dot1 = new Dot();
				dot2 = new Dot();
				secondCheck = true;
				thirdCheck = true;
			}
			else if ( dot.DotType == DotTypes.PEN_HOVER )
			{
				filteredDot(dot, obj);
			}
		}

		private bool ValidateCode(Dot dot)
		{
			return !(MAX_NOTE_ID < dot.Note || MAX_PAGE_ID < dot.Page);
		}

		private bool ValidateStartDot(Dot dot1, Dot dot2, Dot dot3)
		{
			if (dot1 == null || dot2 == null || dot3 == null)
				return false;

			float dot1X = dot1.X + (dot1.Fx * 0.01f);
			float dot1Y = dot1.Y + (dot1.Fy * 0.01f);
			float dot2X = dot2.X + (dot2.Fx * 0.01f);
			float dot2Y = dot2.Y + (dot2.Fy * 0.01f);
			float dot3X = dot3.X + (dot3.Fx * 0.01f);
			float dot3Y = dot3.Y + (dot3.Fy * 0.01f);

			if (dot1X > MAX_X || dot1X < 1)
				return false;
			if (dot1Y > MAX_Y || dot1Y < 1)
				return false;
			if ((dot3X - dot1X) * (dot2X - dot1X) > 0 && Math.Abs(dot3X - dot1X) > DELTA && Math.Abs(dot1X - dot2X) > DELTA)
			{
				return false;
			}
			else if ((dot3Y - dot1Y) * (dot2Y - dot1Y) > 0 && Math.Abs(dot3Y - dot1Y) > DELTA && Math.Abs(dot1Y - dot2Y) > DELTA)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private bool ValidateMiddleDot(Dot dot1, Dot dot2, Dot dot3)
		{
			if (dot1 == null || dot2 == null || dot3 == null)
				return false;

			float dot1X = dot1.X + (dot1.Fx * 0.01f);
			float dot1Y = dot1.Y + (dot1.Fy * 0.01f);
			float dot2X = dot2.X + (dot2.Fx * 0.01f);
			float dot2Y = dot2.Y + (dot2.Fy * 0.01f);
			float dot3X = dot3.X + (dot3.Fx * 0.01f);
			float dot3Y = dot3.Y + (dot3.Fy * 0.01f);

			if (dot2X > MAX_X || dot2X < 1)
				return false;
			if (dot2Y > MAX_Y || dot2Y < 1)
				return false;
			if ((dot1X - dot2X) * (dot3X - dot2X) > 0 && Math.Abs(dot1X - dot2X) > DELTA && Math.Abs(dot3X - dot2X) > DELTA)
			{
				return false;
			}
			else if ((dot1Y - dot2Y) * (dot3Y - dot2Y) > 0 && Math.Abs(dot1Y - dot2Y) > DELTA && Math.Abs(dot3Y - dot2Y) > DELTA)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private bool ValidateEndDot(Dot dot1, Dot dot2, Dot dot3)
		{
			if (dot1 == null || dot2 == null || dot3 == null)
				return false;

			float dot1X = dot1.X + (dot1.Fx * 0.01f);
			float dot1Y = dot1.Y + (dot1.Fy * 0.01f);
			float dot2X = dot2.X + (dot2.Fx * 0.01f);
			float dot2Y = dot2.Y + (dot2.Fy * 0.01f);
			float dot3X = dot3.X + (dot3.Fx * 0.01f);
			float dot3Y = dot3.Y + (dot3.Fy * 0.01f);

			if (dot3X > MAX_X || dot3X < 1)
				return false;
			if (dot3Y > MAX_Y || dot3Y < 1)
				return false;
			if ((dot3X - dot1X) * (dot3X - dot2X) > 0 && Math.Abs(dot3X - dot1X) > DELTA && Math.Abs(dot3X - dot2X) > DELTA)
			{
				return false;
			}
			else if ((dot3Y - dot1Y) * (dot3Y - dot2Y) > 0 && Math.Abs(dot3Y - dot1Y) > DELTA && Math.Abs(dot3Y - dot2Y) > DELTA)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
