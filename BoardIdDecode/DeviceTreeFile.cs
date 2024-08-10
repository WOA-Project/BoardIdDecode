using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoardIdDecode
{
    internal class DeviceTreeFile
    {
        public string FileName
        {
            get; set;
        }

        public BoardIDData[] BoardIDDatas
        {
            get; set;
        }

        public MSMIDData[] MSMIDDatas
        {
            get; set;
        }

        public string[] Includes
        {
            get; set;
        }

        public DeviceTreeFile(string fileName, bool autoReplaceIncludes = true)
        {
            string content = File.ReadAllText(fileName);
            string strippedContent = StripComments(content);
            string[] lines = strippedContent.Split("\n");

            if (autoReplaceIncludes)
            {
                Dictionary<string, string> includeReplacements = new Dictionary<string, string>();

                do
                {
                    foreach (KeyValuePair<string, string> item in includeReplacements)
                    {
                        strippedContent = strippedContent.Replace(item.Key, item.Value);
                    }

                    lines = strippedContent.Split("\n");

                    includeReplacements = [];
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();

                        if (!(trimmedLine.Contains("#include ") && trimmedLine.Contains(".dts")))
                        {
                            continue;
                        }

                        string valuePart = trimmedLine.Split("\"")[1].Trim().Replace('/', Path.DirectorySeparatorChar);
                        string directoryOfCurrentFile = Path.GetDirectoryName(fileName)!;
                        string includeFileLocation = Path.Combine(directoryOfCurrentFile, valuePart);

                        if (File.Exists(includeFileLocation))
                        {
                            if (!includeReplacements.ContainsKey(line))
                            {
                                string replacementText = File.ReadAllText(includeFileLocation);
                                replacementText = replacementText.Replace("#include \"", $"#include \"{Path.GetDirectoryName(includeFileLocation).Replace(Path.DirectorySeparatorChar, '/')}/");
                                replacementText = StripComments(replacementText);
                                includeReplacements.Add(line, replacementText);
                            }
                        }
                    }
                }
                while (includeReplacements.Count > 0);

                File.WriteAllText($"{fileName}.filled.txt", strippedContent);
            }

            FileName = fileName;

            List<BoardIDData> listOfBoardIDs = new List<BoardIDData>();
            List<MSMIDData> listOfMSMIDs = new List<MSMIDData>();
            List<string> listOfIncludes = new List<string>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                BoardIDData[] data = DecodeIdFromString(trimmedLine);
                if (data.Length > 0)
                {
                    listOfBoardIDs = data.ToList();
                }

                MSMIDData[] data2 = DecodeIdFromString2(trimmedLine);
                if (data2.Length > 0)
                {
                    listOfMSMIDs = data2.ToList();
                }

                if (!(trimmedLine.Contains("#include ") && trimmedLine.Contains(".dts")))
                {
                    continue;
                }

                string valuePart = trimmedLine.Split("\"")[1].Trim().Replace('/', Path.DirectorySeparatorChar);
                string directoryOfCurrentFile = Path.GetDirectoryName(fileName)!;
                string includeFileLocation = Path.Combine(directoryOfCurrentFile, valuePart);

                listOfIncludes.Add(Path.GetFullPath(includeFileLocation));
            }

            BoardIDDatas = listOfBoardIDs.ToArray();
            MSMIDDatas = listOfMSMIDs.ToArray();
            Includes = listOfIncludes.ToArray();
        }

        private static string StripComments(string input)
        {
            string blockComments = @"/\*(.*?)\*/";
            string lineComments = @"//(.*?)\r?\n";
            string strings = @"""((\\[^\n]|[^""\n])*)""";
            string verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(input,
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me => {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);

            return noComments;
        }

        public static MSMIDData[] DecodeIdFromString2(string line)
        {
            List<MSMIDData> mSMIDDatas = new();

            if (!line.Contains("qcom,msm-id"))
            {
                return mSMIDDatas.ToArray();
            }

            string QCOMMsmIdRawVal = line.Split('=')[1].Replace(";", "").Trim();
            string[] elements = QCOMMsmIdRawVal.Replace("> ,", ">,").Split(">,").Select(x => x.Replace("<", "").Replace(">", "").Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            foreach (string element in elements)
            {
                string[] values;
                if (element.Contains(','))
                {
                    values = element.Split(',');
                }
                else
                {
                    values = element.Split(' ');
                }

                string QCOMMsmIdLeftVal = values[0].Trim();

                uint msmId;
                if (QCOMMsmIdLeftVal.ToLower().Contains("0x"))
                {
                    msmId = Convert.ToUInt32(QCOMMsmIdLeftVal.ToLower().Replace("0x", ""), 16);
                }
                else
                {
                    msmId = Convert.ToUInt32(QCOMMsmIdLeftVal, 10);
                }

                uint revId = 0;
                uint platformId = 0;

                if (values.Length == 2)
                {
                    string QCOMBoardIdRightVal = values[1].Trim();

                    if (QCOMBoardIdRightVal.ToLower().Contains("0x"))
                    {
                        revId = Convert.ToUInt32(QCOMBoardIdRightVal.ToLower().Replace("0x", ""), 16);
                    }
                    else
                    {
                        revId = Convert.ToUInt32(QCOMBoardIdRightVal, 10);
                    }
                }
                else if (values.Length == 3)
                {
                    string QCOMBoardIdMidVal = values[2].Trim();

                    if (QCOMBoardIdMidVal.ToLower().Contains("0x"))
                    {
                        platformId = Convert.ToUInt32(QCOMBoardIdMidVal.ToLower().Replace("0x", ""), 16);
                    }
                    else
                    {
                        platformId = Convert.ToUInt32(QCOMBoardIdMidVal, 10);
                    }

                    string QCOMBoardIdRightVal = values[3].Trim();

                    if (QCOMBoardIdRightVal.ToLower().Contains("0x"))
                    {
                        revId = Convert.ToUInt32(QCOMBoardIdRightVal.ToLower().Replace("0x", ""), 16);
                    }
                    else
                    {
                        revId = Convert.ToUInt32(QCOMBoardIdRightVal, 10);
                    }
                }

                mSMIDDatas.Add(DecodeId2(msmId, platformId, revId));
            }

            return mSMIDDatas.ToArray();
        }

        public static MSMIDData DecodeId2(uint msmID, uint platformID, uint revID)
        {
            uint reserved = (msmID & 0xFF000000) >> 24;
            uint foundryID = (msmID & 0xFF0000) >> 16;
            uint chipID = (msmID & 0xFFFF);

            uint MajorRevisionID = (revID & 0xFF0000) >> 16;
            uint MinorRevisionID = (revID & 0xFFFF);

            return new MSMIDData()
            {
                ChipID = chipID,
                PlatformID = platformID,
                MajorRevisionID = MajorRevisionID,
                MinorRevisionID = MinorRevisionID,
                FoundryID = foundryID,
                Reserved = reserved
            };
        }

        public static BoardIDData[] DecodeIdFromString(string line)
        {
            List<BoardIDData> boardIDDatas = new();

            if (!line.Contains("qcom,board-id"))
            {
                return boardIDDatas.ToArray();
            }

            string QCOMBoardIdRawVal = line.Split('=')[1].Replace(";", "").Trim();
            string[] elements = QCOMBoardIdRawVal.Replace("> ,", ">,").Split(">,").Select(x => x.Replace("<", "").Replace(">", "").Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            foreach (string element in elements)
            {
                string[] values;
                if (element.Contains(','))
                {
                    values = element.Split(',');
                }
                else
                {
                    values = element.Split(' ');
                }

                string QCOMBoardIdLeftVal = values[0].Trim();

                uint boardId;
                if (QCOMBoardIdLeftVal.ToLower().Contains("0x"))
                {
                    boardId = Convert.ToUInt32(QCOMBoardIdLeftVal.ToLower().Replace("0x", ""), 16);
                }
                else
                {
                    boardId = Convert.ToUInt32(QCOMBoardIdLeftVal, 10);
                }

                string QCOMBoardIdRightVal = values[1].Trim();

                uint reserved;
                if (QCOMBoardIdRightVal.ToLower().Contains("0x"))
                {
                    reserved = Convert.ToUInt32(QCOMBoardIdRightVal.ToLower().Replace("0x", ""), 16);
                }
                else
                {
                    reserved = Convert.ToUInt32(QCOMBoardIdRightVal, 10);
                }

                boardIDDatas.Add(DecodeId(boardId, reserved));
            }

            return boardIDDatas.ToArray();
        }

        public static BoardIDData DecodeId(uint boardID, uint reserved)
        {
            uint PlatformSubTypeID = (boardID & 0xFF000000) >> 24;
            uint PlatformMajorVersion = (boardID & 0xFF0000) >> 16;
            uint PlatformMinorVersion = (boardID & 0xFF00) >> 8;
            uint PlatformTypeID = (boardID & 0xFF);

            uint ReservedBits = (reserved & 0xFFFFE000) >> 13;
            uint PanelDetection = (reserved & 0x1800) >> 11;
            uint DDRSize = (reserved & 0x0700) >> 8;
            uint PlatformSubType = (reserved & 0xFF);

            return new BoardIDData()
            {
                DDRSize = DDRSize,
                Reserved = ReservedBits,
                PanelDetection = PanelDetection,
                PlatformSubTypeID = PlatformSubTypeID,
                PlatformTypeID = PlatformTypeID,
                PlatformSubType = PlatformSubType,
                PlatformMajorVersion = PlatformMajorVersion,
                PlatformMinorVersion = PlatformMinorVersion,
            };
        }
    }
}
