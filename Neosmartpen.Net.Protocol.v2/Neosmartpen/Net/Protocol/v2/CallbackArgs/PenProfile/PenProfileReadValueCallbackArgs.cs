using System.Collections.Generic;

namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileReadValueCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileReadValueCallbackArgs()
		{
			Type = PenProfileType.ReadValue;
			Result = ResultType.Success;
		}

		internal PenProfileReadValueCallbackArgs(string profileName):this()
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
