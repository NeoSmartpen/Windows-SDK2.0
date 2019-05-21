using Neosmartpen.Net.Metadata.Exceptions;
using Neosmartpen.Net.Metadata.Model;
using Neosmartpen.Net.Support;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Neosmartpen.Net.Metadata
{
    /// <summary>
    /// Parser class for handling Nproj file which is currently mainly used in metadata
    /// </summary>
    public class NProjParser : IMetadataParser
	{
		#region Constants
		//private readonly string ELEMENT_NPROJ = "nproj";
		private readonly string ATTRIBUTE_VERSION = "version";

		private readonly string ELEMENT_BOOK = "book";
		private readonly string ELEMENT_TITLE = "title";
		private readonly string ELEMENT_SECTION = "section";
		private readonly string ELEMENT_OWNER = "owner";
		private readonly string ELEMENT_CODE = "code";
		private readonly string ELEMENT_REVISION= "revision";
		private readonly string ELEMENT_START_PAGE = "start_page";
        private readonly string ATTRIBUTE_SIDE = "side";
        private readonly string ELEMENT_DOT_IS_LINE_SEGMENT = "dot_is_line_segment";
		private readonly string ELEMENT_LINE_SEGMENT_LENGTH = "line_segment_length";
		private readonly string ELEMENT_TARGET_DPI = "target_dpi";
		private readonly string ELEMENT_KIND = "kind";
        private readonly string ELEMENT_OFFSET = "offset";
        private readonly string ATTRIBUTE_LEFT = "left";
        private readonly string ATTRIBUTE_TOP = "top";
        private readonly string ELEMENT_EXTRA_INFO = "extra_info";
		// page
		private readonly string ELEMENT_PAGES = "pages";
		private readonly string ELEMENT_PAGE_ITEM = "page_item";
		private readonly string ATTRIBUTE_COUNT = "count";
		private readonly string ATTRIBUTE_NUMBER = "number";
		private readonly string ATTRIBUTE_X1 = "x1";
		private readonly string ATTRIBUTE_Y1 = "y1";
		private readonly string ATTRIBUTE_X2 = "x2";
		private readonly string ATTRIBUTE_Y2 = "y2";
		private readonly string ATTRIBUTE_CROP_MARGIN = "crop_margin";
		// symbol
		private readonly string ELEMENT_SYMBOLS = "symbols";
		private readonly string ELEMENT_SYMBOL = "symbol";
		private readonly string ATTRIBUTE_PAGE = "page";
		private readonly string ATTRIBUTE_PAGE_NAME = "page_name";
		private readonly string ATTRIBUTE_TYPE = "type";
		private readonly string ATTRIBUTE_X = "x";
		private readonly string ATTRIBUTE_Y = "y";
		private readonly string ATTRIBUTE_WIDTH = "width";
		private readonly string ATTRIBUTE_HEIGHT = "height";
		private readonly string ELEMENT_NAME = "name";
		private readonly string ELEMENT_ID = "id";
        private readonly string ELEMENT_CUSTOMSHAPE = "customshape";
        private readonly string ELEMENT_POINTS = "points";

        private readonly string ELEMENT_COMMAND = "command";
		//private readonly string ATTRIBUTE_NAME = "name";
		private readonly string ATTRIBUTE_ACTION = "action";
		private readonly string ATTRIBUTE_PARAM = "param";
		private readonly string ELEMENT_MATCHING_SYMBOLS = "matching_symbols";
		private readonly string ATTRIBUTE_PREVIOUS = "provious";
		private readonly string ATTRIBUTE_NEXT = "next";
		#endregion

		public NProjParser() { }

        /// <summary>
        /// Functions to parse the Nproj file to get the Book class
        /// </summary>
        /// <param name="nrojFilePath">Nproj file's path</param>
        /// <returns>A class representing a Book in metadata</returns>
        public Book Parse(string nrojFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(nrojFilePath);
            return Parse(xmlDoc);
        }

		private Book Parse(XmlDocument xmlDoc)
		{
            XmlElement nproj = xmlDoc.DocumentElement;
            string version = nproj.GetAttribute(ATTRIBUTE_VERSION);
			if (version == "2.2")
				return ParseV231(nproj);
			else if (version == "2.30")
				return ParseV231(nproj);
			else if (version == "2.31")
				return ParseV231(nproj);

			throw new ParseException(); 
		}

        // parser와 db를 나눈다면, 그냥 여기서 nproj를 넘기는게 최고인듯
        // missing in function - id, updateddate, page(count), cover, pdf file path
		private Book ParseV231(XmlElement nproj)
		{
			System.Diagnostics.Debug.WriteLine("Come in to Parse V2.31");

            Book book = new Book();

            try
            {
                //
                // parse book element
                var xmlBook = nproj.SelectSingleNode(ELEMENT_BOOK);
                if (xmlBook == null)
                    return null;

                #region Parse Book
                // title
                var node = xmlBook.SelectSingleNode(ELEMENT_TITLE);
                book.Title = node?.InnerText;
                // nproj version
                book.Version = nproj.GetAttribute(ATTRIBUTE_VERSION);
                // section
                node = xmlBook.SelectSingleNode(ELEMENT_SECTION);
                book.Section = NConvert.ToInt32(node?.InnerText);
                // owner
                node = xmlBook.SelectSingleNode(ELEMENT_OWNER);
                book.Owner = NConvert.ToInt32(node?.InnerText);
                // code
                node = xmlBook.SelectSingleNode(ELEMENT_CODE);
                book.Code = NConvert.ToInt32(node?.InnerText);
                // revision
                node = xmlBook.SelectSingleNode(ELEMENT_REVISION);
                book.Revision = Convert.ToInt32(node?.InnerText);
                // start_page
                node = xmlBook.SelectSingleNode(ELEMENT_START_PAGE);
                book.StartPage = string.IsNullOrWhiteSpace(node?.InnerText) ? 1 : NConvert.ToInt32(node?.InnerText);
                // start_page_side
                book.StartPageSide = node?.Attributes.GetNamedItem(ATTRIBUTE_SIDE)?.InnerText;
                if (book.StartPageSide == null || book.StartPageSide.Length == 0)
                    book.StartPageSide = "right";
                // dot is line segment
                node = xmlBook.SelectSingleNode(ELEMENT_DOT_IS_LINE_SEGMENT);
                book.isLine = NConvert.ToBoolean(node?.InnerText);
                // line segment length
                node = xmlBook.SelectSingleNode(ELEMENT_LINE_SEGMENT_LENGTH);
                book.LineSegmentLength = NConvert.ToInt32(node?.InnerText);
                // target dpi
                node = xmlBook.SelectSingleNode(ELEMENT_TARGET_DPI);
                book.Dpi = NConvert.ToInt32(node?.InnerText);
                // kind (isMoleskine)
                node = xmlBook.SelectSingleNode(ELEMENT_KIND);
                book.Kind = NConvert.ToInt32(node?.InnerText);

                // offset (isMoleskine)
                node = xmlBook.SelectSingleNode(ELEMENT_OFFSET);

                if (node != null)
                {
                    book.OffsetLeft = NConvert.ToDouble(node.Attributes.GetNamedItem(ATTRIBUTE_LEFT)?.InnerText.Replace("f", ""));
                    book.OffsetTop = NConvert.ToDouble(node.Attributes.GetNamedItem(ATTRIBUTE_TOP)?.InnerText.Replace("f", ""));
                }
                else
                {
                    book.OffsetLeft = 0;
                    book.OffsetTop = 0;
                }

                // extra info
                node = xmlBook.SelectSingleNode(ELEMENT_EXTRA_INFO);
                book.ExtraInfo = node?.InnerText;
                #endregion

                #region Parse Page
                // 
                // book insert후 book id를 알아야한다.
                // 그렇지 않고 db와 분리하고 싶다면... db insert후 정리하는걸로...
                // pages
                var pages = nproj.SelectSingleNode(ELEMENT_PAGES);
                if (pages == null)
                    return null;
                string value = pages.Attributes.GetNamedItem(ATTRIBUTE_COUNT)?.InnerText;
                book.TotalPageCount = NConvert.ToInt32(value);
                List<Page> pageList = new List<Page>();
                var nodes = pages.SelectNodes(ELEMENT_PAGE_ITEM);

                foreach (XmlNode n in nodes)
                {
                    Page p = new Page();
                    //p.BookId = book.Id;
                    // page number
                    p.Section = book.Section;
                    p.Owner = book.Owner;
                    p.Book = book.Code;

                    value = n.Attributes.GetNamedItem(ATTRIBUTE_NUMBER)?.InnerText;
                    // -1 is error
                    p.Number = NConvert.ToInt32(value) + book.StartPage;
                    // X1
                    value = n.Attributes.GetNamedItem(ATTRIBUTE_X1)?.InnerText;
                    p.X1 = NConvert.ToSingle(value);
                    // Y1
                    value = n.Attributes.GetNamedItem(ATTRIBUTE_Y1)?.InnerText;
                    p.Y1 = NConvert.ToSingle(value);
                    // X2
                    value = n.Attributes.GetNamedItem(ATTRIBUTE_X2)?.InnerText;
                    p.X2 = NConvert.ToSingle(value);
                    // Y2
                    value = n.Attributes.GetNamedItem(ATTRIBUTE_Y2)?.InnerText;
                    p.Y2 = NConvert.ToSingle(value);
                    // Y2
                    value = n.Attributes.GetNamedItem(ATTRIBUTE_CROP_MARGIN)?.InnerText;

                    if (!string.IsNullOrEmpty(value))
                    {
                        // margin order Left, Right, Top, Bottom
                        string[] margin = value.Split(',');
                        p.MarginL = NConvert.ToSingle(margin[0]);
                        p.MarginR = NConvert.ToSingle(margin[1]);
                        p.MarginT = NConvert.ToSingle(margin[2]);
                        p.MarginB = NConvert.ToSingle(margin[3]);
                    }

                    pageList.Add(p);
                }
                #endregion

                #region Parse Symbol
                //
                // 똑같지만 page id를 알아야 한다.
                // Symbols
                var symbols = nproj.SelectSingleNode(ELEMENT_SYMBOLS);

                if (symbols == null)
                    return null;

                List<Symbol> symbolList = new List<Symbol>();

                nodes = symbols.SelectNodes(ELEMENT_SYMBOL);

                float x, y, width, height;

                int iValue;

                foreach (XmlNode n in nodes)
                {
                    node = n.SelectSingleNode(ELEMENT_COMMAND);
                    value = node?.Attributes.GetNamedItem(ATTRIBUTE_PARAM)?.InnerText;

                    if (value == "crop_area")
                    {
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_PAGE)?.InnerText;
                        iValue = NConvert.ToInt32(value) + book.StartPage;

                        Page page = pageList.Find(pp => pp.Number == iValue);

                        if (page == null)
                        {
                            // error?
                            continue;
                        }

                        // crop margin
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_X)?.InnerText;
                        x = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_Y)?.InnerText;
                        y = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_WIDTH)?.InnerText;
                        width = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_HEIGHT)?.InnerText;
                        height = NConvert.ToSingle(value);

                        Page p = page as Page;
                        p.MarginL = x;
                        p.MarginT = y;
                        p.MarginR = p.X2 - (x + width);
                        p.MarginB = p.Y2 - (y + height);

                    }
                    else if (value == "crop_area_common")
                    {
                        // crop area common
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_X)?.InnerText;
                        x = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_Y)?.InnerText;
                        y = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_WIDTH)?.InnerText;
                        width = NConvert.ToSingle(value);
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_HEIGHT)?.InnerText;
                        height = NConvert.ToSingle(value);

                        foreach (var page in pageList)
                        {
                            Page p = page as Page;
                            p.MarginL = x;
                            p.MarginT = y;
                            p.MarginR = p.X2 - (x + width);
                            p.MarginB = p.Y2 - (y + height);
                        }
                    }
                    else
                    {
                        // 
                        // real symbol
                        Symbol symbol = new Symbol();

                        symbol.Section = book.Section;
                        symbol.Owner = book.Owner;
                        symbol.Book = book.Code;

                        // attribute
                        // page
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_PAGE)?.InnerText;
                        symbol.Page = NConvert.ToInt32(value) + book.StartPage;
                        // page_name
                        symbol.PageName = n.Attributes.GetNamedItem(ATTRIBUTE_PAGE_NAME)?.InnerText;
                        // type
                        symbol.Type = n.Attributes.GetNamedItem(ATTRIBUTE_TYPE)?.InnerText;
                        // X
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_X)?.InnerText;
                        symbol.X = NConvert.ToSingle(value);
                        // Y
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_Y)?.InnerText;
                        symbol.Y = NConvert.ToSingle(value);
                        //width 
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_WIDTH)?.InnerText;
                        symbol.Width = NConvert.ToSingle(value);
                        // height  
                        value = n.Attributes.GetNamedItem(ATTRIBUTE_HEIGHT)?.InnerText;
                        symbol.Height = NConvert.ToSingle(value);
                        //
                        // child node
                        // name
                        symbol.Name = n.SelectSingleNode(ELEMENT_NAME)?.InnerText;
                        // id
                        symbol.Id = n.SelectSingleNode(ELEMENT_ID)?.InnerText;
                        // customshape - points
                        symbol.CustomShapePoints = n.SelectSingleNode(ELEMENT_CUSTOMSHAPE)?.SelectSingleNode(ELEMENT_POINTS)?.InnerText;

                        // action
                        node = n.SelectSingleNode(ELEMENT_COMMAND);

                        if (node != null)
                        {
                            symbol.Action = node.Attributes.GetNamedItem(ATTRIBUTE_ACTION)?.InnerText;
                            symbol.Param = node.Attributes.GetNamedItem(ATTRIBUTE_PARAM)?.InnerText;
                        }

                        node = n.SelectSingleNode(ELEMENT_MATCHING_SYMBOLS);

                        if (node != null)
                        {
                            symbol.PreviousLink = node.Attributes.GetNamedItem(ATTRIBUTE_PREVIOUS)?.InnerText;
                            symbol.NextLink = node.Attributes.GetNamedItem(ATTRIBUTE_NEXT)?.InnerText;
                        }

                        symbolList.Add(symbol);
                    }
                }
                #endregion

                book.Pages = pageList;
                book.Symbols = symbolList;
            }
            catch
            {
                throw new ParseException();
            }

			return book;
		}
	}
}