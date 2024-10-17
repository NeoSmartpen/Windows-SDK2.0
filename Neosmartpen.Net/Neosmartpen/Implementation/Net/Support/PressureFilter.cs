using System;

namespace Neosmartpen.Net.Support
{
    public class PressureFilter
    {
        bool m_bFirst;

        int m_nPrevValue;

        float MAX_PRESSURE_DELTA = 0.1f;

        public readonly int RANGE_MIN=45, RANGE_MAX;

        public readonly double RANGE;

        public PressureFilter(int max)
        {
            this.m_bFirst = true;

            this.RANGE_MAX = max;

            this.RANGE = (double)( RANGE_MAX - RANGE_MIN );
        }

        private int FitRange( int p )
        {
            if ( p > RANGE_MAX )
            {
                p = RANGE_MAX;
            }
            else if ( p < RANGE_MIN )
            {
                p = RANGE_MIN;
            }

            return p;
        }

        public int Filter( int nPressure )
        {
            nPressure = FitRange( nPressure );

            if ( m_bFirst )
            {
                this.m_bFirst = false;
                this.m_nPrevValue = nPressure;

                return nPressure;
            }

            float diffRate = 1.0f - (float)nPressure / (float)m_nPrevValue;

            if ( Math.Abs( diffRate ) > MAX_PRESSURE_DELTA )
            {
                float newRate = ( diffRate < 0 ) ? MAX_PRESSURE_DELTA : -( MAX_PRESSURE_DELTA );
                nPressure = (int)( (float)m_nPrevValue * ( 1.0 + newRate ) + 0.5f );
            }

            nPressure = FitRange( nPressure );

            this.m_nPrevValue = nPressure;

            return nPressure;
        }

        public void Reset()
        {
            m_bFirst = true;
        }

        public double ToRate( int nPressure )
        {
	        double fPressureRate = 0;

	        // 필압 강도 측정 (nRangeMin ~ nRangeMax 사이의 값)
            if ( nPressure < RANGE_MIN )
            {
                nPressure = RANGE_MIN;
            }

	        if( nPressure == 0 ) 
            {
		        //압력값(퍼센트)를 가져온다.
		        fPressureRate = 1;
	        }
	        else 
            {
		        //압력값을 퍼센트로 환산한다.
                fPressureRate = (double)( nPressure - RANGE_MIN + 1 ) / RANGE; 
	        }

            if ( fPressureRate > 1.0 )
            {
                fPressureRate = 1.0;
            }
            else if ( fPressureRate < 0.01 )
            {
                fPressureRate = 0.01;
            }

            return fPressureRate;
        }
    }
}
