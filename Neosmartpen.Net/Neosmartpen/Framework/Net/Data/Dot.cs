using System;

namespace Neosmartpen.Net
{
    /// <summary>
    /// Type of Dot object.
    /// </summary>
    public enum DotTypes
    {
        PEN_DOWN, PEN_MOVE, PEN_UP, PEN_HOVER, PEN_ERROR
    };
    
    /// <summary>
    /// Represents coordinate information is detected by sensor of pen.
    /// </summary>
    public class Dot
    {
        /// <summary>
        /// The Section Id of the NCode paper
        /// </summary>
        public int Section { get;  set; }

        /// <summary>
        /// The Owner Id of the NCode paper
        /// </summary>
        public int Owner { get;   set; }

        /// <summary>
        /// The Note Id of the NCode paper
        /// </summary>
        public int Note { get;   set; }

        /// <summary>
        /// The Page Number of the NCode paper
        /// </summary>
        public int Page { get;  set; }

        /// <summary>
        /// Gets or sets the x coordinates of NCode cell.
        /// ( NCode's cell size is 2.371mm )
        /// </summary>
        public float X { get;  set; }

        /// <summary>
        /// Gets or sets the y coordinates of NCode cell.
        /// ( NCode's cell size is 2.371mm )
        /// </summary>
        public float Y { get;  set; }

        /// <summary>
        /// Gets the tilt x of the pen
        /// </summary>
        public int TiltX { get;  set; }

        /// <summary>
        /// Gets the tilt x of the pen
        /// </summary>
        public int TiltY { get;  set; }

        /// <summary>
        /// Gets the twist degree of the pen
        /// </summary>
        public int Twist { get;  set; }

        /// <summary>
        /// Gets or sets the pressure of the dot
        /// </summary>
        public int Force { get;  set; }

        /// <summary>
        /// Gets or sets the color of the dot
        /// </summary>
        public int Color { get;  set; }

        /// <summary>
        /// Gets or sets the timestamp of the dot
        /// </summary>
        public long Timestamp { get;  set; }

        /// <summary>
        /// Gets or sets the type of the dot
        /// </summary>
        public DotTypes DotType { get;  set; }

        /// <summary>
        /// A constructor that constructs a Dot object
        /// </summary>
        public Dot()
        {
        }

        public Dot Clone()
        {
            Dot newDot = new Net.Dot();

            newDot.Owner = Owner;
            newDot.Section = Section;
            newDot.Note = Note;
            newDot.Page = Page;
            newDot.X = X;
            newDot.Y = Y;
            newDot.Force = Force;
            newDot.Timestamp = Timestamp;
            newDot.DotType = DotType;
            newDot.Color = Color;
            newDot.TiltX = TiltX;
            newDot.TiltY = TiltY;
            newDot.Twist = Twist;

            return newDot;
        }

        /// <summary>
        /// A constructor that constructs a Dot object
        /// </summary>
        /// <param name="owner">The Owner Id of the NCode paper</param>
        /// <param name="section">The Section Id of the NCode paper</param>
        /// <param name="note">The Note Id of the NCode paper</param>
        /// <param name="page">The Page Number of the NCode paper</param>
        /// <param name="timestamp">the timestamp of the dot</param>
        /// <param name="x">the x coordinates of NCode cell</param>
        /// <param name="y">the y coordinates of NCode cell</param>
        /// <param name="force">the pressure of the dot</param>
        /// <param name="type">the type of the dot</param>
        /// <param name="color">the color of the dot</param>
        public Dot( int owner, int section, int note, int page, long timestamp, float x, float y, int force, DotTypes type, int color )
        {
            Owner = owner;
            Section = section;
            Note = note;
            Page = page;
            X = x;
            Y = y;
            Force = force;
            Timestamp = timestamp;
            DotType = type;
            Color = color;
        }

        /// <summary>
        /// A constructor that constructs a Dot object
        /// </summary>
        /// <param name="owner">The Owner Id of the NCode paper</param>
        /// <param name="section">The Section Id of the NCode paper</param>
        /// <param name="note">The Note Id of the NCode paper</param>
        /// <param name="page">The Page Number of the NCode paper</param>
        /// <param name="timestamp">the timestamp of the dot</param>
        /// <param name="x">the x coordinates of NCode cell</param>
        /// <param name="y">the y coordinates of NCode cell</param>
        /// <param name="tiltX"></param>
        /// <param name="tiltY"></param>
        /// <param name="twist"></param>
        /// <param name="force">the pressure of the dot</param>
        /// <param name="type">the type of the dot</param>
        /// <param name="color">the color of the dot</param>
        public Dot( int owner, int section, int note, int page, long timestamp, float x, float y, int tiltX, int tiltY, int twist, int force, DotTypes type, int color )
            : this( owner, section, note, page, timestamp, x, y, force,type,color)
        {
            TiltX = tiltX;
            TiltY = tiltY;
            Twist = twist;
        }

        public override string ToString()
        {
            return String.Format( "o:{0}, s:{1}, b:{2}, p:{3}, time:{4}, x:{5}, y:{6}, fx:{7}, fy:{8}, force:{9}, type:{10}", Owner, Section, Note, Page, Timestamp, X, Y, Force, DotType.ToString() );
        }

        public class Builder
        {
            private Dot mDot;
			private int RefindMaxForce = 1023;
			private int maxForce = -1;
			private float scale = -1;

            public Builder()
            {
                mDot = new Dot();
            }

			public Builder(int maxForce) : this()
			{
				this.maxForce = maxForce;
				scale = RefindMaxForce / maxForce;
			}

            public Builder owner( int owner )
            {
                mDot.Owner = owner;
                return this;
            }

            public Builder section( int section )
            {
                mDot.Section = section;
                return this;
            }

            public Builder note( int note )
            {
                mDot.Note = note;
                return this;
            }

            public Builder page( int page )
            {
                mDot.Page = page;
                return this;
            }

            public Builder timestamp( long timestamp )
            {
                mDot.Timestamp = timestamp;
                return this;
            }

            public Builder coord( float x, float y )
            {
                mDot.X = x;
                mDot.Y = y;
                return this;
            }

            public Builder tilt( int x, int y )
            {
                mDot.TiltX = x;
                mDot.TiltY = y;
                return this;
            }

            public Builder twist( int twist )
            {
                mDot.Twist = twist;
                return this;
            }

            public Builder force( int force )
            {
				if ( maxForce == -1 )
					mDot.Force = force;
				else
				{
					mDot.Force = (int)((force * scale)+0.5); // 반올림

					if (Support.PressureCalibration.Instance.Factor != null)
						mDot.Force = (int)Support.PressureCalibration.Instance.Factor[mDot.Force];
				}

				return this;
            }

            public Builder dotType( DotTypes dotType )
            {
                mDot.DotType = dotType;
                return this;
            }

            public Builder color( int color )
            {
                mDot.Color = color;
                return this;
            }

            public Dot Build()
            {
                return mDot;
            }
        }
    }
}
