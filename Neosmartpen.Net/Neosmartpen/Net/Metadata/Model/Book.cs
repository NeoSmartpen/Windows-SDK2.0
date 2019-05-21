using System.Collections.Generic;

namespace Neosmartpen.Net.Metadata.Model
{
    /// <summary>
    /// A class representing a Book in metadata
    /// </summary>
    public class Book
	{
		/// <summary>
		/// Book Title
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// .Nproj file's Version
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// Section
		/// </summary>
		public int Section { get; set; }
		/// <summary>
		/// Owner
		/// </summary>
		public int Owner { get; set; }
		/// <summary>
		/// Book Code(NoteID)
		/// </summary>
		public int Code { get; set; }

        /// <summary>
        /// Total page of Book
        /// </summary>
		public int TotalPageCount { get; set; }

		/// <summary>
		/// Start Page
		/// </summary>
		public int StartPage { get; set; }
        /// <summary>
        /// StartPageSide (left, right, center)
        /// </summary>
        public string StartPageSide { get; set; }
        /// <summary>
        /// .NProj의 revision 역시 업데이트시 체크
        /// </summary>
        public int Revision { get; set; }
		/// <summary>
		/// true is line, false is dot
		/// </summary>
		public bool isLine { get; set; }
        /// <summary>
        /// How many dots were made of the line
        /// 3(=600DPI) or 5(=1200DPI)
        /// </summary>
        public int LineSegmentLength { get; set; }
		/// <summary>
		/// 600 or 1200
		/// </summary>
		public int Dpi { get; set; }
		/// <summary>
		/// 1 is Moleskine, 100 is unknown note, 0 is otherwise
		/// </summary>
		public int Kind { get; set; }
		/// <summary>
		/// Extra Info
		/// </summary>
		public string ExtraInfo { get; set; }

        /// <summary>
        /// Left offset
        /// </summary>
        public double OffsetLeft { get; set; }

        /// <summary>
        /// Top offset
        /// </summary>
        public double OffsetTop { get; set; }

        /// <summary>
        /// List of Page included in Book
        /// </summary>
        public List<Page> Pages { get; set; }

        /// <summary>
        /// List of Symbol included in Book
        /// </summary>
        public List<Symbol> Symbols { get; set; }
	}
}