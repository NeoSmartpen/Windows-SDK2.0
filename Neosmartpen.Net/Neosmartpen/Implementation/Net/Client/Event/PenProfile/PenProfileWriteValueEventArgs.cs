using System.Collections.Generic;

namespace Neosmartpen.Net
{
	public sealed class PenProfileWriteValueEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileWriteValueEventArgs()
		{
			Type = PenProfileType.WriteValue;
			Result = ResultType.Success;
		}
		internal PenProfileWriteValueEventArgs(string profileName) : this()
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
