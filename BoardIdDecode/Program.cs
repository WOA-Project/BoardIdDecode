namespace BoardIdDecode
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            string[] mainFiles = Directory.GetFiles(args[0], "*.dts", SearchOption.AllDirectories);

            Console.WriteLine();
            Console.WriteLine($"| Device Tree Source File Name | PlatformTypeID | PlatformSubTypeID | PlatformSubType | Platform Version | ReservedBits | PanelDetection | DDRSize |");
            Console.WriteLine($"|------------------------------|----------------|-------------------|-----------------|------------------|--------------|----------------|---------|");
            foreach (string mainFile in mainFiles)
            {
                DeviceTreeFile deviceTreeFile = new DeviceTreeFile(mainFile);
                BoardIDData[] boardIDs = deviceTreeFile.BoardIDDatas;

                if (boardIDs.Length > 0)
                {
                    foreach (BoardIDData boardID in boardIDs)
                    {
                        Console.WriteLine($"| {Path.GetFileName(mainFile)} | {boardID.PlatformTypeID} ({Enum.GetName(typeof(PlatformInfoType), boardID.PlatformTypeID)}) | {boardID.PlatformSubTypeID} | {boardID.PlatformSubType} | {boardID.PlatformMajorVersion}.{boardID.PlatformMinorVersion} | {boardID.Reserved} | {boardID.PanelDetection} | {boardID.DDRSize} |");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine($"| Device Tree Source File Name | Chip ID                        | Foundry ID | Reserved | PlatformID | RevID         |");
            Console.WriteLine($"|------------------------------|--------------------------------|------------|----------|------------|---------------|");
            foreach (string mainFile in mainFiles)
            {
                DeviceTreeFile deviceTreeFile = new DeviceTreeFile(mainFile);
                MSMIDData[] msmIDs = deviceTreeFile.MSMIDDatas;

                if (msmIDs.Length > 0)
                {
                    foreach (MSMIDData msmID in msmIDs)
                    {
                        Console.WriteLine($"| {Path.GetFileName(mainFile)} | {msmID.ChipID} ({Enum.GetName(typeof(ChipInfoIdType), msmID.ChipID)}) | {msmID.FoundryID} | {msmID.Reserved} | {msmID.PlatformID} | {msmID.MajorRevisionID}.{msmID.MinorRevisionID} |");
                    }
                }
            }
            Console.WriteLine();
        }

        private static void PrintIncludes(Dictionary<string, DeviceTreeFile> mappedDeviceTrees, DeviceTreeFile dtFile, int level)
        {
            if (dtFile.Includes.Length > 0)
            {
                string spaces = new(' ', level + 1);

                foreach (string includedFile in dtFile.Includes)
                {
                    if (mappedDeviceTrees.ContainsKey(includedFile))
                    {
                        Console.WriteLine($"{spaces}{Path.GetFileName(includedFile)}");
                        DeviceTreeFile deviceTreeFileForInclude = mappedDeviceTrees[includedFile];
                        PrintIncludes(mappedDeviceTrees, deviceTreeFileForInclude, level + 1);
                    }
                    else
                    {
                        ConsoleColor backup = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{spaces}{Path.GetFileName(includedFile)} (NOT FOUND!)");
                        Console.ForegroundColor = backup;
                    }
                }
            }
        }

        private static MSMIDData[] GetMSMIDData(Dictionary<string, DeviceTreeFile> mappedDeviceTrees, DeviceTreeFile dtFile)
        {
            if (dtFile.MSMIDDatas.Length > 0)
            {
                return dtFile.MSMIDDatas;
            }

            if (dtFile.Includes.Length > 0)
            {
                foreach (string includedFile in dtFile.Includes)
                {
                    if (mappedDeviceTrees.ContainsKey(includedFile))
                    {
                        DeviceTreeFile deviceTreeFileForInclude = mappedDeviceTrees[includedFile];
                        return GetMSMIDData(mappedDeviceTrees, deviceTreeFileForInclude);
                    }
                }
            }

            return [];
        }

        private static BoardIDData[] GetBoardIDData(Dictionary<string, DeviceTreeFile> mappedDeviceTrees, DeviceTreeFile dtFile)
        {
            if (dtFile.BoardIDDatas.Length > 0)
            {
                return dtFile.BoardIDDatas;
            }

            if (dtFile.Includes.Length > 0)
            {
                foreach (string includedFile in dtFile.Includes)
                {
                    if (mappedDeviceTrees.ContainsKey(includedFile))
                    {
                        DeviceTreeFile deviceTreeFileForInclude = mappedDeviceTrees[includedFile];
                        return GetBoardIDData(mappedDeviceTrees, deviceTreeFileForInclude);
                    }
                }
            }

            return [];
        }
    }
}