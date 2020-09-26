using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBodyQualityLibrary : hkReferencedObject
    {
        public hknpBodyQuality m_qualities_0;
        public hknpBodyQuality m_qualities_1;
        public hknpBodyQuality m_qualities_2;
        public hknpBodyQuality m_qualities_3;
        public hknpBodyQuality m_qualities_4;
        public hknpBodyQuality m_qualities_5;
        public hknpBodyQuality m_qualities_6;
        public hknpBodyQuality m_qualities_7;
        public hknpBodyQuality m_qualities_8;
        public hknpBodyQuality m_qualities_9;
        public hknpBodyQuality m_qualities_10;
        public hknpBodyQuality m_qualities_11;
        public hknpBodyQuality m_qualities_12;
        public hknpBodyQuality m_qualities_13;
        public hknpBodyQuality m_qualities_14;
        public hknpBodyQuality m_qualities_15;
        public hknpBodyQuality m_qualities_16;
        public hknpBodyQuality m_qualities_17;
        public hknpBodyQuality m_qualities_18;
        public hknpBodyQuality m_qualities_19;
        public hknpBodyQuality m_qualities_20;
        public hknpBodyQuality m_qualities_21;
        public hknpBodyQuality m_qualities_22;
        public hknpBodyQuality m_qualities_23;
        public hknpBodyQuality m_qualities_24;
        public hknpBodyQuality m_qualities_25;
        public hknpBodyQuality m_qualities_26;
        public hknpBodyQuality m_qualities_27;
        public hknpBodyQuality m_qualities_28;
        public hknpBodyQuality m_qualities_29;
        public hknpBodyQuality m_qualities_30;
        public hknpBodyQuality m_qualities_31;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt64();
            m_qualities_0 = new hknpBodyQuality();
            m_qualities_0.Read(des, br);
            m_qualities_1 = new hknpBodyQuality();
            m_qualities_1.Read(des, br);
            m_qualities_2 = new hknpBodyQuality();
            m_qualities_2.Read(des, br);
            m_qualities_3 = new hknpBodyQuality();
            m_qualities_3.Read(des, br);
            m_qualities_4 = new hknpBodyQuality();
            m_qualities_4.Read(des, br);
            m_qualities_5 = new hknpBodyQuality();
            m_qualities_5.Read(des, br);
            m_qualities_6 = new hknpBodyQuality();
            m_qualities_6.Read(des, br);
            m_qualities_7 = new hknpBodyQuality();
            m_qualities_7.Read(des, br);
            m_qualities_8 = new hknpBodyQuality();
            m_qualities_8.Read(des, br);
            m_qualities_9 = new hknpBodyQuality();
            m_qualities_9.Read(des, br);
            m_qualities_10 = new hknpBodyQuality();
            m_qualities_10.Read(des, br);
            m_qualities_11 = new hknpBodyQuality();
            m_qualities_11.Read(des, br);
            m_qualities_12 = new hknpBodyQuality();
            m_qualities_12.Read(des, br);
            m_qualities_13 = new hknpBodyQuality();
            m_qualities_13.Read(des, br);
            m_qualities_14 = new hknpBodyQuality();
            m_qualities_14.Read(des, br);
            m_qualities_15 = new hknpBodyQuality();
            m_qualities_15.Read(des, br);
            m_qualities_16 = new hknpBodyQuality();
            m_qualities_16.Read(des, br);
            m_qualities_17 = new hknpBodyQuality();
            m_qualities_17.Read(des, br);
            m_qualities_18 = new hknpBodyQuality();
            m_qualities_18.Read(des, br);
            m_qualities_19 = new hknpBodyQuality();
            m_qualities_19.Read(des, br);
            m_qualities_20 = new hknpBodyQuality();
            m_qualities_20.Read(des, br);
            m_qualities_21 = new hknpBodyQuality();
            m_qualities_21.Read(des, br);
            m_qualities_22 = new hknpBodyQuality();
            m_qualities_22.Read(des, br);
            m_qualities_23 = new hknpBodyQuality();
            m_qualities_23.Read(des, br);
            m_qualities_24 = new hknpBodyQuality();
            m_qualities_24.Read(des, br);
            m_qualities_25 = new hknpBodyQuality();
            m_qualities_25.Read(des, br);
            m_qualities_26 = new hknpBodyQuality();
            m_qualities_26.Read(des, br);
            m_qualities_27 = new hknpBodyQuality();
            m_qualities_27.Read(des, br);
            m_qualities_28 = new hknpBodyQuality();
            m_qualities_28.Read(des, br);
            m_qualities_29 = new hknpBodyQuality();
            m_qualities_29.Read(des, br);
            m_qualities_30 = new hknpBodyQuality();
            m_qualities_30.Read(des, br);
            m_qualities_31 = new hknpBodyQuality();
            m_qualities_31.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            m_qualities_0.Write(bw);
            m_qualities_1.Write(bw);
            m_qualities_2.Write(bw);
            m_qualities_3.Write(bw);
            m_qualities_4.Write(bw);
            m_qualities_5.Write(bw);
            m_qualities_6.Write(bw);
            m_qualities_7.Write(bw);
            m_qualities_8.Write(bw);
            m_qualities_9.Write(bw);
            m_qualities_10.Write(bw);
            m_qualities_11.Write(bw);
            m_qualities_12.Write(bw);
            m_qualities_13.Write(bw);
            m_qualities_14.Write(bw);
            m_qualities_15.Write(bw);
            m_qualities_16.Write(bw);
            m_qualities_17.Write(bw);
            m_qualities_18.Write(bw);
            m_qualities_19.Write(bw);
            m_qualities_20.Write(bw);
            m_qualities_21.Write(bw);
            m_qualities_22.Write(bw);
            m_qualities_23.Write(bw);
            m_qualities_24.Write(bw);
            m_qualities_25.Write(bw);
            m_qualities_26.Write(bw);
            m_qualities_27.Write(bw);
            m_qualities_28.Write(bw);
            m_qualities_29.Write(bw);
            m_qualities_30.Write(bw);
            m_qualities_31.Write(bw);
        }
    }
}
