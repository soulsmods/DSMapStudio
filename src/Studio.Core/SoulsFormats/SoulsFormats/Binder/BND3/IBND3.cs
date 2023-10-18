﻿namespace SoulsFormats
{
    internal interface IBND3
    {
        string Version { get; set; }

        Binder.Format Format { get; set; }

        bool BigEndian { get; set; }

        bool BitBigEndian { get; set; }

        int Unk18 { get; set; }
    }
}
