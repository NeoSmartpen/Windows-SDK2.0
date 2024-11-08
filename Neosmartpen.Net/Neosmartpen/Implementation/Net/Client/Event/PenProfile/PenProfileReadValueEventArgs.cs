using System.Collections.Generic;

namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PenProfileReadValueEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileReadValueEventArgs()
		{
			Type = PenProfileType.ReadValue;
			Result = ResultType.Success;
		}

		internal PenProfileReadValueEventArgs(string profileName):this()
		{
			ProfileName = profileName;
			Data = new List<ReadValueResult>();
		}

		public List<ReadValueResult> Data { get; internal set; }
		
		public class ReadValueResult
		{
			public int Status { get; internal set; }
			public string Key { get; internal set; }
			public byte[] Data { get; internal set; }
		}
	}
}
