using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB2
    {
        private class MapstudioBoneName : Param<BoneName>
        {
            internal override string Name => "MAPSTUDIO_BONE_NAME_STRING";
            internal override int Version => 0;

            public List<BoneName> BoneNames { get; set; }

            public MapstudioBoneName()
            {
                BoneNames = new List<BoneName>();
            }

            internal override BoneName ReadEntry(BinaryReaderEx br)
            {
                var boneName = new BoneName(br);
                BoneNames.Add(boneName);
                return boneName;
            }

            public override List<BoneName> GetEntries()
            {
                return BoneNames;
            }
        }

        internal class BoneName : NamedEntry
        {
            public BoneName(string name)
            {
                Name = name;
            }

            internal BoneName(BinaryReaderEx br)
            {
                Name = br.ReadUTF16();
            }

            internal override void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
            }
        }
    }
}
