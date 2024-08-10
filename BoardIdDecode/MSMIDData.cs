namespace BoardIdDecode
{
    public struct MSMIDData
    {
        public uint ChipID
        {
            get; set;
        }

        public uint FoundryID
        {
            get; set;
        }

        public uint Reserved
        {
            get; set;
        }

        public uint PlatformID
        {
            get; set;
        }

        public uint MajorRevisionID
        {
            get; set;
        }

        public uint MinorRevisionID
        {
            get; set;
        }
    }
}