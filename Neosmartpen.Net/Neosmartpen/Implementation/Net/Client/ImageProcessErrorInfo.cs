using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosmartpen.Net
{
    /// <exclude />
    public class ImageProcessErrorInfo
    {
        public long Timestamp { get; set; }

        public int Force { get; set; }

        public int Brightness { get; set; }

        public int ExposureTime { get; set; }

        public int ProcessTime { get; set; }

        public int LabelCount { get; set; }

        public int ErrorCode { get; set; }

        public int ClassType { get; set; }

        public int ErrorCount { get; set; }
    }
}
