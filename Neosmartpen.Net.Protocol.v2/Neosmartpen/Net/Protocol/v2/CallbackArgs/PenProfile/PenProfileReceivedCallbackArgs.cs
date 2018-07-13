namespace Neosmartpen.Net.Protocol.v2
{
	public class PenProfileReceivedCallbackArgs
	{
		internal PenProfileReceivedCallbackArgs() { }
		internal PenProfileReceivedCallbackArgs(ResultType result)
		{
			Result = result;
		}

		public ResultType Result { get; internal set; }
		public enum ResultType
		{
			Success, 
			Failed
		}
		public string ProfileName { get; internal set; }
		public int Status { get; internal set; }

		public PenProfileType Type { get; protected set; }
		public enum PenProfileType
		{
			Create, 
			Delete,
			Info,
			ReadValue,
			WriteValue,
			DeleteValue
		}
	}
}
