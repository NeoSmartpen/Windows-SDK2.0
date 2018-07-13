using System.Collections.Generic;

namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileDeleteValueCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileDeleteValueCallbackArgs()
		{
			Type = PenProfileType.DeleteValue;
			Result = ResultType.Success;
		}
		internal PenProfileDeleteValueCallbackArgs(string profileName) : this()
		{
			ProfileName = profileName;
			Data = new List<DeleteValueResult>();
		}

		public List<DeleteValueResult> Data { get; internal set; }

		public class DeleteValueResult
		{
			public string Key { get; internal set; }
			public int Status { get; internal set; }
		}
	}
}
