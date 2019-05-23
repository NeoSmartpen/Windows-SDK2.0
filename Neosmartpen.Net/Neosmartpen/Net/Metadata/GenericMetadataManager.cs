using Neosmartpen.Net.Metadata.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosmartpen.Net.Metadata
{
    /// <summary>
    /// A generic class that handles most metadata that implements the IMetadataManager.
    /// </summary>
    public class GenericMetadataManager : IMetadataManager
    {
        private Dictionary<string, Book> Books = new Dictionary<string, Book>();

        /// <summary>
        /// Associated metadata parser
        /// </summary>
        public IMetadataParser Parser { get; set; }

        /// <summary>
        /// GenericMetadataManager Constructor
        /// </summary>
        /// <param name="parser">Associated metadata parser</param>
        public GenericMetadataManager(IMetadataParser parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Get the Book from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <returns>A class representing a Book in metadata</returns>
        public Book GetBook(int section, int owner, int book)
        {
            if (Books.ContainsKey(MakeKey(section, owner, book)))
                return Books[MakeKey(section, owner, book)];
            else
                return null;
        }

        /// <summary>
        /// Get the Page from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <param name="pageNumber">Number of Page</param>
        /// <returns>A class representing a Page in metadata</returns>
        public Page GetPage(int section, int owner, int book, int pageNumber)
        {
            return GetBook(section, owner, book)?.Pages?.Where(p => p.Number == pageNumber)?.FirstOrDefault();
        }

        /// <summary>
        /// Get the Symbols from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <param name="pageNumber">Number of Page</param>
        /// <returns>A collection of Symbol</returns>
        public List<Symbol> GetSymbols(int section, int owner, int book, int pageNumber)
        {
            return GetBook(section, owner, book)?.Symbols?.Where(s => s.Page == pageNumber)?.ToList();
        }

        /// <summary>
        /// Functions for finding symbols over strokes
        /// </summary>
        /// <param name="stroke">Target Stroke to check whether it is positioned above the Symbol</param>
        /// <returns>Symbols corresponding to the position of the Stroke</returns>
        public List<Symbol> FindApplicableSymbols(Stroke stroke)
        {
            var result = new List<Symbol>();

            var book = GetBook(stroke.Section, stroke.Owner, stroke.Note);
            var symbols = GetSymbols(stroke.Section, stroke.Owner, stroke.Note, stroke.Page);

            if (book == null || symbols == null)
            {
                return result;
            }

            foreach (var symbol in symbols)
            {
                if (symbol.Contains(stroke, (float)book.OffsetLeft, (float)book.OffsetTop))
                {
                    result.Add(symbol);
                }
            }

            return result;
        }

        /// <summary>
        /// Separates only Strokes above the Symbol area.
        /// </summary>
        /// <param name="symbol">A class representing a Symbol in metadata</param>
        /// <param name="strokes">Target Stroke to check whether it is positioned above the Symbol</param>
        /// <returns>Strokes corresponding to Symbol area</returns>
        public List<Stroke> GetStrokesInSymbol(Symbol symbol, List<Stroke> strokes)
        {
            var result = new List<Stroke>();

            var book = GetBook(symbol.Section, symbol.Owner, symbol.Book);

            if (book == null)
            {
                return result;
            }

            foreach (var stroke in strokes)
            {
                if (symbol.Contains(stroke, (float)book.OffsetLeft, (float)book.OffsetTop))
                {
                    result.Add(stroke);
                }
            }

            return result;
        }

        /// <summary>
        /// Load the metadata file.
        /// </summary>
        /// <param name="metadataFilePath">Path to the metadata file</param>
        public void Load(string metadataFilePath)
        {
            var book = Parser.Parse(metadataFilePath);
            // 북 추가
            Books.Add(MakeKey(book.Section, book.Owner, book.Code), book);
        }

        private string MakeKey(int section, int owner, int book, int page = -1)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(section);
            builder.Append("_");
            builder.Append(owner);
            builder.Append("_");
            builder.Append(book);
            if (page != -1)
                builder.Append("_").Append(page);

            return builder.ToString();
        }
    }
}
