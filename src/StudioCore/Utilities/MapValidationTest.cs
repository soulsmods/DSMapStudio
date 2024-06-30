using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Utilities;
public static class MapValidationTest
{

    public static bool HasFinished = false;

    public static bool TargetProject = false;

    public static List<AssetDescription> resMaps = new List<AssetDescription>();

    public static void ValidateMSB()
    {
        HasFinished = false;

        var mapDir = $"{Locator.ActiveProject.Settings.GameRoot}/map/mapstudio/";

        if (TargetProject)
        {
            mapDir = $"{Locator.ActiveProject.AssetLocator.RootDirectory}/map/mapstudio/";
        }

        foreach (var entry in Directory.EnumerateFiles(mapDir))
        {
            if (entry.Contains(".msb.dcx"))
            {
                var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(entry));
                AssetDescription ad = Locator.ActiveProject.AssetLocator.GetMapMSB(name);
                if (ad.AssetPath != null)
                {
                    resMaps.Add(ad);
                }
            }
        }

        if (Locator.ActiveProject.Type is GameType.DemonsSouls)
        {
            foreach (var res in resMaps)
            {
                var msb = MSBD.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.DarkSoulsPTDE or GameType.DarkSoulsRemastered)
        {
            foreach (var res in resMaps)
            {
                var msb = MSB1.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.DarkSoulsIISOTFS)
        {
            foreach (var res in resMaps)
            {
                var msb = MSB2.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.DarkSoulsIII)
        {
            foreach (var res in resMaps)
            {
                var msb = MSB3.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.Bloodborne)
        {
            foreach (var res in resMaps)
            {
                var msb = MSBB.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.Sekiro)
        {
            foreach (var res in resMaps)
            {
                var msb = MSBS.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.EldenRing)
        {
            foreach (var res in resMaps)
            {
                var msb = MSBE.Read(res.AssetPath);
            }
        }
        if (Locator.ActiveProject.Type is GameType.ArmoredCoreVI)
        {
            foreach (var res in resMaps)
            {
                var msb = MSB_AC6.Read(res.AssetPath);
            }
        }

        HasFinished = true;
        TaskLogs.AddLog("Finished validating MSBs");
    }
}
