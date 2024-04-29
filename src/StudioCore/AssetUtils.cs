using SoulsFormats;
using StudioCore.Editor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore;

/// <summary>
///     Helper functions and statics for Project/Assetlocator
/// </summary>
public class AssetUtils
{
    public static readonly string GameExecutableFilter;
    public static readonly string ProjectJsonFilter;
    public static readonly string RegulationBinFilter;
    public static readonly string Data0Filter;
    public static readonly string ParamBndDcxFilter;
    public static readonly string ParamBndFilter;
    public static readonly string EncRegulationFilter;
    public static readonly string ParamLooseFilter;
    public static readonly string CsvFilter;
    public static readonly string TxtFilter;
    public static readonly string FmgJsonFilter;

    static AssetUtils()
    {
        // These patterns are meant to be passed directly into PlatformUtils.
        // Everything about their handling should be done there.

        // Game Executable (.EXE, EBOOT.BIN)|*.EXE*;*EBOOT.BIN*
        // Windows executable (*.EXE)|*.EXE*
        // Playstation executable (*.BIN)|*.BIN*
        GameExecutableFilter = "exe,bin";
        // Project file (project.json)|PROJECT.JSON
        ProjectJsonFilter = "json";
        // Regulation file (regulation.bin)|REGULATION.BIN
        RegulationBinFilter = "bin";
        // Data file (Data0.bdt)|DATA0.BDT
        Data0Filter = "bdt";
        // ParamBndDcx (gameparam.parambnd.dcx)|GAMEPARAM.PARAMBND.DCX
        ParamBndDcxFilter = "parambnd.dcx";
        // ParamBnd (gameparam.parambnd)|GAMEPARAM.PARAMBND
        ParamBndFilter = "parambnd";
        // Enc_RegBndDcx (enc_regulation.bnd.dcx)|ENC_REGULATION.BND.DCX
        EncRegulationFilter = "bnd.dcx";
        // Loose Param file (*.Param)|*.Param
        ParamLooseFilter = "param";
        // CSV file (*.csv)|*.csv
        CsvFilter = "csv";
        // Text file (*.txt)|*.txt
        TxtFilter = "txt";
        // Exported FMGs (*.fmg.json)|*.fmg.json
        FmgJsonFilter = "fmg.json";
        // All file filter is implicitly added by NFD. Ideally this is used explicitly.
        // All files|*.*
    }
    public static GameType GetGameTypeForExePath(string exePath)
    {
        var type = GameType.Undefined;
        if (exePath.ToLower().Contains("darksouls.exe"))
        {
            type = GameType.DarkSoulsPTDE;
        }
        else if (exePath.ToLower().Contains("darksoulsremastered.exe"))
        {
            type = GameType.DarkSoulsRemastered;
        }
        else if (exePath.ToLower().Contains("darksoulsii.exe"))
        {
            type = GameType.DarkSoulsIISOTFS;
        }
        else if (exePath.ToLower().Contains("darksoulsiii.exe"))
        {
            type = GameType.DarkSoulsIII;
        }
        else if (exePath.ToLower().Contains("eboot.bin"))
        {
            var path = Path.GetDirectoryName(exePath);
            if (Directory.Exists($@"{path}\dvdroot_ps4"))
            {
                type = GameType.Bloodborne;
            }
            else
            {
                type = GameType.DemonsSouls;
            }
        }
        else if (exePath.ToLower().Contains("sekiro.exe"))
        {
            type = GameType.Sekiro;
        }
        else if (exePath.ToLower().Contains("eldenring.exe"))
        {
            type = GameType.EldenRing;
        }
        else if (exePath.ToLower().Contains("armoredcore6.exe"))
        {
            type = GameType.ArmoredCoreVI;
        }

        return type;
    }

    public static bool CheckFilesExpanded(string gamepath, GameType game)
    {
        if (game == GameType.EldenRing)
        {
            if (!Directory.Exists($@"{gamepath}\map"))
            {
                return false;
            }

            if (!Directory.Exists($@"{gamepath}\asset"))
            {
                return false;
            }
        }

        if (game is GameType.DarkSoulsPTDE or GameType.DarkSoulsIII or GameType.Sekiro)
        {
            if (!Directory.Exists($@"{gamepath}\map"))
            {
                return false;
            }

            if (!Directory.Exists($@"{gamepath}\obj"))
            {
                return false;
            }
        }

        if (game == GameType.DarkSoulsIISOTFS)
        {
            if (!Directory.Exists($@"{gamepath}\map"))
            {
                return false;
            }

            if (!Directory.Exists($@"{gamepath}\model\obj"))
            {
                return false;
            }
        }

        if (game == GameType.ArmoredCoreVI)
        {
            if (!Directory.Exists($@"{gamepath}\map"))
            {
                return false;
            }

            if (!Directory.Exists($@"{gamepath}\asset"))
            {
                return false;
            }
        }

        return true;
    }

    public static string GetGameIDForDir(GameType Type)
    {
        switch (Type)
        {
            case GameType.DemonsSouls:
                return "DES";
            case GameType.DarkSoulsPTDE:
                return "DS1";
            case GameType.DarkSoulsRemastered:
                return "DS1R";
            case GameType.DarkSoulsIISOTFS:
                return "DS2S";
            case GameType.Bloodborne:
                return "BB";
            case GameType.DarkSoulsIII:
                return "DS3";
            case GameType.Sekiro:
                return "SDT";
            case GameType.EldenRing:
                return "ER";
            case GameType.ArmoredCoreVI:
                return "AC6";
            default:
                throw new Exception("Game type not set");
        }
    }

    public static AssetDescription GetNullAsset()
    {
        AssetDescription ret = new();
        ret.AssetPath = "null";
        ret.AssetName = "null";
        ret.AssetArchiveVirtualPath = "null";
        ret.AssetVirtualPath = "null";
        return ret;
    }

    public static string GetBinderVirtualPath(string virtualPathToBinder, string binderFilePath)
    {
        var filename = Path.GetFileNameWithoutExtension($@"{binderFilePath}.blah");
        if (filename.Length > 0)
        {
            filename = $@"{virtualPathToBinder}/{filename}";
        }
        else
        {
            filename = virtualPathToBinder;
        }

        return filename;
    }
}
