using System.Collections.Generic;

namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileWriteValueCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileWriteValueCallbackArgs()
		{
			Type = PenProfileType.WriteValue;
			Result = ResultType.Success;
		}
		internal PenProfileWriteValueCallbackArgs(string profileName) : this()
		{
			ProfileName = profileName;
			Data = new List<WriteValueResult>();
		}

		public List<WriteValueResult> Data { get; internal set; }

		public class WriteValueResult
		{
			public int Status { get; internal set; }
			public string Key { get; internal set; }
		}
	}
}
