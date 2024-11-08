using System;

namespace Neosmartpen.Net.Filter
{
    /// <exclude />
    class FilterForPaper
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
		private Dot makeDownDot = null, makeMoveDot = null;
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
						makeDownDot = MakeDot(dot2.Owner, dot2.Section, dot2.Note, dot2.Page, dot2.Timestamp, dot2.X, dot2.Y, dot2.TiltX, dot2.TiltY, dot2.Twist, dot2.Force, DotTypes.PEN_DOWN, dot2.Color);
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
						makeDownDot = MakeDot(dot.Owner, dot.Section, dot.Note, dot.Page, dot.Timestamp, dot.X, dot.Y, dot.TiltX, dot.TiltY, dot.Twist, dot.Force, DotTypes.PEN_DOWN, dot.Color);
						filteredDot(makeDownDot, obj);
					}
					if (thirdCheck && !validateMiddleDot)
					{
						makeMoveDot = MakeDot(dot.Owner, dot.Section, dot.Note, dot.Page, dot.Timestamp, dot.X, dot.Y, dot.TiltX, dot.TiltY, dot.Twist, dot.Force, DotTypes.PEN_MOVE, dot.Color);
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
		}

		private bool ValidateCode(Dot dot)
		{
			return !(MAX_NOTE_ID < dot.Note || MAX_PAGE_ID < dot.Page);
		}

		private bool ValidateStartDot(Dot dot1, Dot dot2, Dot dot3)
		{
			if (dot1 == null || dot2 == null || dot3 == null)
				return false;

			if (dot1.X > MAX_X || dot1.X < 1)
				return false;
			if (dot1.Y > MAX_Y || dot1.Y < 1)
				return false;
			if ((dot3.X - dot1.X) * (dot2.X - dot1.X) > 0 && Math.Abs(dot3.X - dot1.X) > DELTA && Math.Abs(dot1.X - dot2.X) > DELTA)
			{
				return false;
			}
			else if ((dot3.Y - dot1.Y) * (dot2.Y - dot1.Y) > 0 && Math.Abs(dot3.Y - dot1.Y) > DELTA && Math.Abs(dot1.Y - dot2.Y) > DELTA)
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

			if (dot2.X > MAX_X || dot2.X < 1)
				return false;
			if (dot2.Y > MAX_Y || dot2.Y < 1)
				return false;
			if ((dot1.X - dot2.X) * (dot3.X - dot2.X) > 0 && Math.Abs(dot1.X - dot2.X) > DELTA && Math.Abs(dot3.X - dot2.X) > DELTA)
			{
				return false;
			}
			else if ((dot1.Y - dot2.Y) * (dot3.Y - dot2.Y) > 0 && Math.Abs(dot1.Y - dot2.Y) > DELTA && Math.Abs(dot3.Y - dot2.Y) > DELTA)
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

			if (dot3.X > MAX_X || dot3.X < 1)
				return false;
			if (dot3.Y > MAX_Y || dot3.Y < 1)
				return false;
			if ((dot3.X - dot1.X) * (dot3.X - dot2.X) > 0 && Math.Abs(dot3.X - dot1.X) > DELTA && Math.Abs(dot3.X - dot2.X) > DELTA)
			{
				return false;
			}
			else if ((dot3.Y - dot1.Y) * (dot3.Y - dot2.Y) > 0 && Math.Abs(dot3.Y - dot1.Y) > DELTA && Math.Abs(dot3.Y - dot2.Y) > DELTA)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private Dot MakeDot(int owner, int section, int note, int page, long timestamp, float x, float y, int tiltX, int tiltY, int twist , int force, DotTypes type, int color)
		{
			Dot.Builder builder = new Dot.Builder();

			builder.owner(owner)
				.section(section)
				.note(note)
				.page(page)
				.timestamp(timestamp)
				.coord(x, y)
				.tilt(tiltX, tiltY)
				.twist(twist)
				.force(force)
				.dotType(type)
				.color(color);
			return builder.Build();
		}
	}
}
