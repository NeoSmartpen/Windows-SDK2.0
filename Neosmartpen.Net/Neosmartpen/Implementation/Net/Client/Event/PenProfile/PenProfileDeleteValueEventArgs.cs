using System.Collections.Generic;

namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PenProfileDeleteValueEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileDeleteValueEventArgs()
		{
			Type = PenProfileType.DeleteValue;
			Result = ResultType.Success;
		}
		internal PenProfileDeleteValueEventArgs(string profileName) : this()
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
