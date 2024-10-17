namespace Neosmartpen.Net
{
	public class Pds
	{
		internal Pds()
		{

		}
		public Pds(int section, int owner, int content, int page, float x, float y)
		{
			Section = section;
			Owner = owner;
			Content = content;
			Page = page;
			X = x;
			Y = y;
		}

		public int Section { get; set; }
		public int Owner { get; set; }
		public int Content { get; set; }
		public int Page { get; set; }
		public float X { get; set; }
		public float Y { get; set; }

		public Pds Clone()
		{
			var pds = new Pds();
			pds.Section = Section;
			pds.Owner = Owner;
			pds.Content = Content;
			pds.Page = Page;
			pds.X = X;
			pds.Y = Y;
			return pds;
		}
	}
}
