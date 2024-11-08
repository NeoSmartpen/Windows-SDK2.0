using System;

namespace Neosmartpen.Net.Support
{
    /// <exclude />
    internal class PressureCalibration
	{
		private static PressureCalibration instance;
		private static object lockObject = new object();
		public static PressureCalibration Instance
		{
			get
			{
				if ( instance == null )
				{
					lock(lockObject)
					{
						if (instance == null)
							instance = new PressureCalibration();
					}
				}
				return instance;
			}
		}

		public PressureCalibration()
		{
		}

		#region Calculate Calibration Factor 
		public readonly int MAX_FACTOR = 1023;
		private readonly int MAX_CURVE = 2000;
		private float[] pressureCalibrationFactor;
		public float[] Factor
		{
			get
			{
				return pressureCalibrationFactor;
			}
		}
		public void Clear()
		{
			pressureCalibrationFactor = null;
		}
		public void MakeFactor(int cPX1, int cPY1, int cPX2, int cPY2, int cPX3, int cPY3)
		{
			PointF[] pCurve = new PointF[MAX_CURVE];
			pressureCalibrationFactor = new float[MAX_FACTOR + 1];

			PointF[] point = new PointF[4]
			{
				new PointF(cPX1, cPY1),
				new PointF(cPX2, cPY2),
				new PointF(cPX2, cPY2),
				new PointF(cPX3, cPY3),
			};
			ComputeBezier(point, MAX_CURVE, pCurve);

			int count = 0;
			int prevCount = 0;
			int length = pCurve.Length - 1;
			for (int i = 0; i < length; i++)
			{
				if (count < pCurve[i].X)
				{
					if (Math.Abs(pCurve[prevCount].X - count) > Math.Abs(pCurve[i].X - count))
						pressureCalibrationFactor[count] = pCurve[i].Y;
					else
						pressureCalibrationFactor[count] = pCurve[prevCount].Y;
					if (pressureCalibrationFactor[count] > MAX_FACTOR) pressureCalibrationFactor[count] = MAX_FACTOR;
					count++;
					if (count > MAX_FACTOR)
						count = MAX_FACTOR;
				}
				else
				{
					prevCount = count;
				}
			}
			pressureCalibrationFactor[MAX_FACTOR] = pCurve[MAX_CURVE - 1].Y;
			if (pressureCalibrationFactor[MAX_FACTOR] > MAX_FACTOR) pressureCalibrationFactor[MAX_FACTOR] = MAX_FACTOR;

#if DEBUG_LOG
			for (int i = 0; i < mModifyPressureFactor.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine("test mPressureFactor" + i + ":" + mModifyPressureFactor[i]);
			}
#endif
		}
		public float[] GetPressureCalibrateFactor(int cPX1, int cPY1, int cPX2, int cPY2, int cPX3, int cPY3)
		{
			MakeFactor(cPX1, cPY1, cPX2, cPY2, cPX3, cPY3);
			return pressureCalibrationFactor;
		}

		private void ComputeBezier(PointF[] cp, int numberOfPoints, PointF[] curve)
		{
			float dt = 1.0f / (numberOfPoints - 1);

			for (int i = 0; i < numberOfPoints; i++)
				curve[i] = PointOnCubicBezier(cp, i * dt);
		}

		private PointF PointOnCubicBezier(PointF[] cp, float t)
		{
			float ax, bx, cx;
			float ay, by, cy;
			float tSquared, tCubed;
			PointF result = new PointF();

			cx = 3.0f * (cp[1].X - cp[0].X);
			bx = 3.0f * (cp[2].X - cp[1].X) - cx;
			ax = cp[3].X - cp[0].X - cx - bx;

			cy = 3.0f * (cp[1].Y - cp[0].Y);
			by = 3.0f * (cp[2].Y - cp[1].Y) - cy;
			ay = cp[3].Y - cp[0].Y - cy - by;

			tSquared = t * t;
			tCubed = tSquared * t;

			result.X = (ax * tCubed) + (bx * tSquared) + (cx * t) + cp[0].X;
			result.Y = (ay * tCubed) + (by * tSquared) + (cy * t) + cp[0].Y;

			return result;
		}

		private struct PointF
		{
			public PointF(float x, float y)
			{
				X = x;
				Y = y;
			}
			public float X { get; set; }
			public float Y { get; set; }
		}
#endregion
	}
}
