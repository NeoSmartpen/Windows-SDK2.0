namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PenProfileInfoEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileInfoEventArgs()
		{
			Type = PenProfileType.Info;
			Result = ResultType.Success;
		}
		internal PenProfileInfoEventArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
		internal PenProfileInfoEventArgs(string profileName, int status, int totalSectionCount, int sectionSize, int useSectionCount, int useKeyCount) : this()
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
