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
            return Books[MakeKey(section, owner, book)];
        }

        /// <summary>
        /// Get the Page from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <returns>A class representing a Page in metadata</returns>
        public Page GetPage(int section, int owner, int book, int pageNumber)
        {
            return Books[MakeKey(section, owner, book)]?.Pages?.Where(p => p.Number == pageNumber)?.FirstOrDefault();
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
            return Books[MakeKey(section, owner, book)]?.Symbols?.Where(s => s.Page == pageNumber)?.ToList();
        }

        /// <summary>
        /// Functions for finding symbols over strokes
        /// </summary>
        /// <param name="stroke">Stroke class with multiple dots</param>
        /// <returns>A collection of applicable Symbol</returns>
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
