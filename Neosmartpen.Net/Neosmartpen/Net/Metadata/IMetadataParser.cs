using Neosmartpen.Net.Metadata.Model;

namespace Neosmartpen.Net.Metadata
{
    /// <summary>
    /// An interface that defines the functionality of the class that parses the metadata file.
    /// </summary>
    public interface IMetadataParser
    {
        /// <summary>
        /// Functions to parse the metadata file to get the Book class
        /// </summary>
        /// <param name="metadataFilePath">metadata file's path</param>
        /// <returns>A class representing a Book in metadata</returns>
        Book Parse(string metadataFilePath);
    }
}
