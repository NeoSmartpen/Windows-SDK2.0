using System;

namespace Neosmartpen.Net
{ 
    /// <summary>
    /// Type of Dot object.
    /// </summary>
    public enum DotTypes
    {
        PEN_DOWN, PEN_MOVE, PEN_UP, PEN_HOVER
    };
    
    /// <summary>
    /// Represents coordinate information is detected by sensor of pen.
    /// </summary>
    public class Dot
    {
        /// <summary>
        /// The Section Id of the NCode paper
        /// </summary>
        public int Section { get; private set; }

        /// <summary>
        /// The Owner Id of the NCode paper
        /// </summary>
        public int Owner { get;  private set; }

        /// <summary>
        /// The Note Id of the NCode paper
        /// </summary>
        public int Note { get;  private set; }

        /// <summary>
        /// The Page Number of the NCode paper
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// Gets or sets the x coordinates of NCode cell.
        /// ( NCode's cell size is 2.371mm )
        /// </summary>
        public int X { get;  set; }

        /// <summary>
        /// Gets or sets the y coordinates of NCode cell.
        /// ( NCode's cell size is 2.371mm )
        /// </summary>
        public int Y { get;  set; }

        /// <summary>
        /// Gets or sets the fractional part of NCode x coordinates. ( maximum value is 100 )
        /// </summary>
        public int Fx { get;  set; }

        /// <summary>
        /// Gets or sets the fractional part of NCode y coordinates. ( maximum value is 100 )
        /// </summary>
        public int Fy { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        public int TiltX { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        public int TiltY { get;  set; }

        /// <summary>
        /// 
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
        /// <param name="fx">the fractional part of NCode x coordinates</param>
        /// <param name="fy">the fractional part of NCode y coordinates</param>
        /// <param name="force">the pressure of the dot</param>
        /// <param name="type">the type of the dot</param>
        /// <param name="color">the color of the dot</param>
        public Dot( int owner, int section, int note, int page, long timestamp, int x, int y, int fx, int fy, int force, DotTypes type, int color )
        {
            Owner = owner;
            Section = section;
            Note = note;
            Page = page;
            X = x;
            Y = y;
            Fx = fx;
            Fy = fy;
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
        /// <param name="fx">the fractional part of NCode x coordinates</param>
        /// <param name="fy">the fractional part of NCode y coordinates</param>
        /// <param name="tiltX"></param>
        /// <param name="tiltY"></param>
        /// <param name="twist"></param>
        /// <param name="force">the pressure of the dot</param>
        /// <param name="type">the type of the dot</param>
        /// <param name="color">the color of the dot</param>
        public Dot( int owner, int section, int note, int page, long timestamp, int x, int y, int fx, int fy, int tiltX, int tiltY, int twist, int force, DotTypes type, int color )
            : this( owner, section, note, page, timestamp, x, y, fx, fy, force,type,color)
        {
            TiltX = tiltX;
            TiltY = tiltY;
            Twist = twist;
        }

        public override string ToString()
        {
            return String.Format( "o:{0}, s:{1}, b:{2}, p:{3}, time:{4}, x:{5}, y:{6}, fx:{7}, fy:{8}, force:{9}, type:{10}", Owner, Section, Note, Page, Timestamp, X, Y, Fx, Fy, Force, DotType.ToString() );
        }

        public class Builder
        {
            private Dot mDot;

            public Builder()
            {
                mDot = new Dot();
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

            public Builder coord( int x, int fx, int y, int fy )
            {
                mDot.X = x;
                mDot.Fx = fx;
                mDot.Y = y;
                mDot.Fy = fy;
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
                mDot.Force = force;
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
