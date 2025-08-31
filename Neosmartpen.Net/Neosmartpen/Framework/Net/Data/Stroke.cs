using System;
using System.Collections.Generic;
using System.IO;

namespace Neosmartpen.Net
{
    /// <summary>
    /// Represents coordinate of stroke.
    /// It consist of Dot's Collection.
    /// </summary>
    public class Stroke : List<Dot>
    {
        /// <summary>
        /// Gets the Section Id of the NCode paper
        /// </summary>
        public int Section { get; private set; }

        /// <summary>
        /// Gets the Owner Id of the NCode paper
        /// </summary>
        public int Owner { get; private set; }

        /// <summary>
        /// Gets the Note Id of the NCode paper
        /// </summary>
        public int Note { get; private set; }

        /// <summary>
        /// Gets the Page Number of the NCode paper
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// Gets the color of the stroke
        /// </summary>
        public int Color { get; private set; }

        /// <summary>
        /// Gets the timestamp of start point
        /// </summary>
        public long TimeStart { get; private set; }

        /// <summary>
        /// Gets the timestamp of end point
        /// </summary>
        public long TimeEnd { get; private set; }

        /// <summary>
        /// A constructor that constructs a Stroke object
        /// </summary>
        /// <param name="section">The Section Id of the NCode paper</param>
        /// <param name="owner">The Owner Id of the NCode paper</param>
        /// <param name="note">The Note Id of the NCode paper</param>
        /// <param name="page">The Page Number of the NCode paper</param>
        public Stroke( int section, int owner, int note, int page )
        {
            Section = section;
            Owner = owner;
            Note = note;
            Page = page;
        }

        /// <summary>
        /// Adds a new Dot to the current Stroke object.
        /// </summary>
        /// <param name="dot">Dot object</param>
        public new void Add( Dot dot )
        {
            if ( base.Count <= 0 )
            {
                TimeStart = dot.Timestamp;
                Color = dot.Color;
            }

            TimeEnd = dot.Timestamp;

            base.Add( dot );
        }

        /// <summary>
        /// Convert to standard Dot Byte serialization format.
        /// </summary>
        public string DotsToBase64()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    foreach (var dot in this)
                    {
                        binaryWriter.Write((short)dot.Timestamp);   // 2 bytes
                        binaryWriter.Write(dot.Force);              // 4 bytes
                        binaryWriter.Write(dot.X);                  // 4 bytes
                        binaryWriter.Write(dot.Y);                  // 4 bytes
                        binaryWriter.Write((byte)dot.TiltX);        // 1 byte
                        binaryWriter.Write((byte)dot.TiltY);        // 1 byte
                        binaryWriter.Write((byte)dot.Twist);        // 1 byte
                    }
                }
                byte[] byteArray = memoryStream.ToArray();
                return Convert.ToBase64String(byteArray);
            }
        }

        public override string ToString()
        {
            return String.Format( "Stroke => sectionId : {0}, ownerId : {1}, noteId : {2}, pageId : {3}, timeStart : {4}, timeEnd : {5}", Section, Owner, Note, Page, TimeStart, TimeEnd );
        }
    }
}
