using SoulsFormats;
using System;

namespace AddChr
{
    class Program
    {
        static void Main(string[] args)
        {
            var msb = MSB3.Read(args[0]);
            var chr = new MSB3.Part.Enemy();
            chr.Name = "c1280_0000";
            chr.ModelName = "c0000";
            chr.MapStudioLayer = 0xFFFFFFFF;
            chr.ThinkParamID = 128010;
            chr.NPCParamID = 128010;
            chr.CharaInitID = -1;
            chr.BackupEventAnimID = -1;
            chr.EntityID = -1;
            chr.LodParamID = -1;
            chr.UnkE0E = -1;

            msb.Parts.Enemies.Add(chr);

            msb.Write(args[0]);
        }
    }
}
