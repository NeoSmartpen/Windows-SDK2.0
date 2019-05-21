using Neosmartpen.Net.Metadata.Model;
using System.Collections.Generic;

namespace Neosmartpen.Net.Metadata
{
    /// <summary>
    /// An interface that defines the functionality of the MetadataManager that is responsible for handling metadata.
    /// </summary>
    public interface IMetadataManager
    {
        /// <summary>
        /// Associated metadata parser
        /// </summary>
        IMetadataParser Parser { get; set; }

        /// <summary>
        /// Load the metadata file.
        /// </summary>
        /// <param name="metadataFilePath">Path to the metadata file</param>
        void Load(string nprojPath);

        /// <summary>
        /// Get the Symbols from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <param name="pageNumber">Number of Page</param>
        /// <returns>A collection of Symbol</returns>
        List<Symbol> GetSymbols(int section, int owner, int book, int pageNumber);

        /// <summary>
        /// Get the Page from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <returns>A class representing a Page in metadata</returns>
        Page GetPage(int section, int owner, int book, int pageNumber);

        /// <summary>
        /// Get the Book from the metadata.
        /// </summary>
        /// <param name="section">Section code value</param>
        /// <param name="owner">Owner code value</param>
        /// <param name="book">Book code value</param>
        /// <returns>A class representing a Book in metadata</returns>
        Book GetBook(int section, int owner, int book);

        /// <summary>
        /// Functions for finding symbols over strokes
        /// </summary>
        /// <param name="stroke">Stroke class with multiple dots</param>
        /// <returns>A collection of applicable Symbol</returns>
        List<Symbol> FindApplicableSymbols(Stroke stroke);
    }
}
