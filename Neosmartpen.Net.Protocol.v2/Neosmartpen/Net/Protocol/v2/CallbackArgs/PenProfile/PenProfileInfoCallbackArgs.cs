namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileInfoCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileInfoCallbackArgs()
		{
			Type = PenProfileType.Info;
			Result = ResultType.Success;
		}
		internal PenProfileInfoCallbackArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
		internal PenProfileInfoCallbackArgs(string profileName, int status, int totalSectionCount, int sectionSize, int useSectionCount, int useKeyCount) : this()
		{
			ProfileName = profileName;
			Status = status;
			TotalSectionCount = totalSectionCount;
			SectionSize = sectionSize;
			UseSectionCount = useSectionCount;
			UseKeyCount = useKeyCount;
		}

		public int TotalSectionCount { get; internal set; }
		public int SectionSize { get; internal set; }
		public int UseSectionCount { get; internal set; }
		public int UseKeyCount { get; internal set; }
	}
}
