namespace Neosmartpen.Net.Metadata.Model
{
    /// <summary>
    /// A class representing a Page in metadata
    /// </summary>
    public class Page
	{
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
        public int Book { get; set; }
        /// <summary>
        /// Page Number
        /// </summary>
        public int Number { get; set; }
		/// <summary>
		/// coord X1 (Start Point)
		/// </summary>
		public float X1 { get; set; }
		/// <summary>
		/// coord Y1 (Start Point)
		/// </summary>
		public float Y1 { get; set; }
		/// <summary>
		/// coord X2 (End Point)
		/// </summary>
		public float X2 { get; set; }
		/// <summary>
		/// coord Y2 (End Point)
		/// </summary>
		public float Y2 { get; set; }
		/// <summary>
		/// Crop Margin Left
		/// </summary>
		public float MarginL { get; set; }
		/// <summary>
		/// Crop Margin Right
		/// </summary>
		public float MarginR { get; set; }
		/// <summary>
		/// Crop Margin Top
		/// </summary>
		public float MarginT { get; set; }
		/// <summary>
		/// Crop Margin Bottom
		/// </summary>
		public float MarginB { get; set; }
	}
}
