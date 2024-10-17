namespace Neosmartpen.Net
{
    public class ImageProcessingInfo
    {
        public int DotCount { get; set; }
        /// <summary>
        /// Number of images received from the image sensor
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Number of images delivered to the data processor
        /// </summary>
        public int Processed { get; set; }

        /// <summary>
        /// Number of images processed by the data processor
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// Number of images last used for coordinate recognition
        /// </summary>
        public int Transferred { get; set; }
    }
}
