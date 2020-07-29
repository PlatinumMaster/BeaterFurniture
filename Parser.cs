using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;

namespace BeaterFurniture
{
    public class OverworldParser
    {
        private BinaryReader b;
        public OverworldParser(string overworld, string output)
        {
            // Initialize the overworld we will read from.
            b = new BinaryReader(File.Open(overworld, FileMode.Open));
            ParseOverworld(overworld, output);
            b.Close();
        }

        public void ParseOverworld(string overworld, string output)
        {
            using (StreamWriter o = new StreamWriter($"{output}"))
            {
                o.WriteLine($".include \"{"B2W2.s"}\"");

                o.WriteLine($".word Extra - 4 @ File Size");
                b.BaseStream.Position = 0x4;

                byte furnitureCount = b.ReadByte();
                byte npcCount = b.ReadByte();
                byte warpCount = b.ReadByte();
                byte triggerCount = b.ReadByte();

                // Write the counts.
                o.WriteLine($".byte {furnitureCount}{Environment.NewLine}" +
                    $".byte {npcCount}{Environment.NewLine}" +
                    $".byte {warpCount}{Environment.NewLine}" +
                    $".byte {triggerCount}{Environment.NewLine}");

                if (furnitureCount > 0)
                {
                    o.WriteLine("Furniture:");
                    for (int i = 0; i < furnitureCount; i++)
                        o.WriteLine($"\tFurniture {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadInt32()}, {b.ReadInt32()}, {b.ReadInt32()}");
                }

                if (npcCount > 0)
                {
                    o.WriteLine("NPCs:");
                    for (int i = 0; i < npcCount; i++)
                        o.WriteLine($"\tNPC {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, " +
                            $"{b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, " +
                            $"{b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}");
                }

                if (warpCount > 0)
                {
                    o.WriteLine("Warps:");
                    for (int i = 0; i < warpCount; i++)
                        o.WriteLine($"\tWarp {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadByte()}, {b.ReadByte()}, {b.ReadInt32()}, {b.ReadInt32()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}");
                }

                if (triggerCount > 0)
                {
                    o.WriteLine("Triggers:");
                    for (int i = 0; i < triggerCount; i++)
                        o.WriteLine($"\tTrigger {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, " +
                            $"{b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}, {b.ReadUInt16()}");
                }

                o.WriteLine("Extra:");

                long safePos = 0;
                while (true)
                {
                    try
                    {
                        safePos = b.BaseStream.Position;
                        o.WriteLine($"\tExtra {b.ReadUInt16()}, {b.ReadInt32()}");
                    }
                    catch (EndOfStreamException)
                    {
                        long fileLength = b.BaseStream.Position;
                        b.BaseStream.Position = safePos;
                        for (int i = 0; i < fileLength - safePos; i++)
                            o.WriteLine($"\t.byte {b.ReadByte()}");
                        break;
                    }
                }
            }
        }
    }
}
