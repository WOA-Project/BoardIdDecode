namespace BoardIdDecode
{
    public struct BoardIDData
    {
        public uint PlatformTypeID
        {
            get; set;
        }

        public uint PlatformSubTypeID
        {
            get; set;
        }

        public uint PlatformSubType
        {
            get; set;
        }

        public uint PlatformMajorVersion
        {
            get; set;
        }

        public uint PlatformMinorVersion
        {
            get; set;
        }

        public uint Reserved
        {
            get; set;
        }

        public uint PanelDetection
        {
            get; set;
        }

        public uint DDRSize
        {
            get; set;
        }
    }
}