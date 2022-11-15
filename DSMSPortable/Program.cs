using System.CodeDom;
using System.Collections;
using System.Globalization;
using StudioCore;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using StudioCore.ParamEditor;

namespace DSMSPortable
{
    class DSMSPortable
    {
        // Check this file locally for the full gamepath
        static readonly string GAMEPATH_FILE = "gamepath.txt";
        static readonly string DEFAULT_ER_GAMEPATH = "Steam\\steamapps\\common\\ELDEN RING\\Game";
        static string gamepath = null;
        static ArrayList csvFiles;
        static ArrayList masseditFiles;
        static GameType gameType = GameType.EldenRing;
        static string outputFile = null;
        static string inputFile = null;
        static void Main(string[] args)
        {
            masseditFiles = new ArrayList();
            csvFiles = new ArrayList();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try 
            { 
                ProcessArgs(args);
            }
            catch(ArgumentException e)
            {
                System.Console.Error.WriteLine(e.Message);
                return;
            }
            FindGamepath();
            if (inputFile == null)
            {
                System.Console.Error.WriteLine("ERROR: No regulation.bin file specified as input");
                return;
            }
            ProjectSettings settings = new()
            {
                PartialParams = false,
                UseLooseParams = false,
                GameType = gameType
            };
            if (gamepath != null) settings.GameRoot = gamepath;
            NewProjectOptions options = new()
            {
                settings = settings,
                loadDefaultNames = false,
                directory = new FileInfo(inputFile).Directory.FullName
            };
            AssetLocator locator = new();
            locator.SetFromProjectSettings(settings, new FileInfo(inputFile).Directory.FullName);
            ParamBank.PrimaryBank.SetAssetLocator(locator);
            ParamBank.VanillaBank.SetAssetLocator(locator);
            // This operation takes time in a separate thread, so just wait and poll it
            ParamBank.ReloadParams(settings, options);
            System.Console.Out.Write("Loading Params");
            while (ParamBank.PrimaryBank.IsLoadingParams)
            {
                System.Threading.Thread.Sleep(500);
                System.Console.Out.Write(".");
            }
            System.Console.Out.Write("\n");
            MassEditResult meresult;
            string opstring;
            // Process CSV edits first
            foreach (string csvfile in csvFiles)
            {
                opstring = File.ReadAllText(csvfile);
                meresult = MassParamEditCSV.PerformMassEdit(ParamBank.PrimaryBank, opstring, new StudioCore.Editor.ActionManager(), Path.GetFileNameWithoutExtension(csvfile), true, false, ',');
                if (meresult.Type == MassEditResultType.SUCCESS) System.Console.Out.WriteLine($@"{Path.GetFileNameWithoutExtension(csvfile)} {meresult.Type}: {meresult.Information}");
                else System.Console.Error.WriteLine($@"{Path.GetFileNameWithoutExtension(csvfile)} {meresult.Type}: {meresult.Information}");
                if (meresult.Information.Contains(" 0 rows added")) System.Console.Out.WriteLine("WARNING: Use MASSEDIT scripts for modifying existing params to avoid conflicts\n");
            }
            // Then process massedit scripts
            foreach (string mefile in masseditFiles)
            {
                opstring = File.ReadAllText(mefile).ReplaceLineEndings("\n").Trim();
                // MassEdit throws errors if there are any empty lines
                while (!opstring.Equals(opstring.Replace("\n\n", "\n")))
                    opstring = opstring.Replace("\n\n", "\n");
                (meresult, StudioCore.Editor.ActionManager tmp) = MassParamEditRegex.PerformMassEdit(ParamBank.PrimaryBank, opstring, new ParamEditorSelectionState());
                if (meresult.Type == MassEditResultType.SUCCESS) System.Console.Out.WriteLine($@"{Path.GetFileNameWithoutExtension(mefile)} {meresult.Type}: {meresult.Information}");
                else System.Console.Error.WriteLine($@"{Path.GetFileNameWithoutExtension(mefile)} {meresult.Type}: {meresult.Information}");
            }
            try
            {
                ParamBank.PrimaryBank.SaveParams(false, false);
            }
            catch (SavingFailedException e)
            {
                try
                {   // Try to stick the landing if SaveParams finds itself unable to overwrite the param file
                    if (gameType == GameType.EldenRing)
                    {
                        File.Move($@"{new FileInfo(inputFile).Directory.FullName}\regulation.bin.temp", $@"{new FileInfo(inputFile).Directory.FullName}\regulation.bin");
                    }
                    else File.Move($@"{inputFile}.temp", inputFile);
                }
                catch (Exception)
                {
                    System.Console.Error.WriteLine(e.Message);
                    System.Console.Error.WriteLine(e.StackTrace);
                }
            }
            if (outputFile != null)
            {
                try
                {   // if an output file is specified, wing it by just copying the param file, and renaming the backup
                    if (gameType == GameType.EldenRing)
                    {
                        File.Move($@"{new FileInfo(inputFile).Directory.FullName}\regulation.bin", outputFile);
                        File.Move($@"{new FileInfo(inputFile).Directory.FullName}\regulation.bin.prev", $@"{new FileInfo(inputFile).Directory.FullName}\regulation.bin");
                    }
                    else if (File.Exists($@"{inputFile}.prev"))
                    {
                        File.Move(inputFile, outputFile);
                        File.Move($@"{inputFile}.prev", inputFile);
                    }
                    else File.Copy(inputFile, outputFile);
                }
                catch (IOException ioe)
                {
                    System.Console.Error.WriteLine(ioe.Message);
                }
            }
        }
        private static void FindGamepath()
        {
            if(gamepath == null && File.Exists(GAMEPATH_FILE)) 
                gamepath = File.ReadAllText(GAMEPATH_FILE);
            if(gameType == GameType.EldenRing)
            {
                if (gamepath != null && File.Exists($@"{gamepath}\EldenRing.exe")) return;
                if (File.Exists($@"{System.Environment.GetEnvironmentVariable("ProgramFiles")}\{DEFAULT_ER_GAMEPATH}\EldenRing.exe"))
                {
                    gamepath = $@"{System.Environment.GetEnvironmentVariable("ProgramFiles")}\{DEFAULT_ER_GAMEPATH}";
                    return;
                }
                if (File.Exists($@"{System.Environment.GetEnvironmentVariable("ProgramFiles(x86)")}\{DEFAULT_ER_GAMEPATH}\EldenRing.exe"))
                {
                    gamepath = $@"{System.Environment.GetEnvironmentVariable("ProgramFiles(x86)")}\{DEFAULT_ER_GAMEPATH}";
                    return;
                }
                if (gamepath != null && File.Exists($@"{gamepath}\Game\EldenRing.exe"))
                {
                    gamepath = $@"{gamepath}\Game";
                    File.WriteAllText(GAMEPATH_FILE, gamepath);
                    return;
                }
                gamepath = null;
            }
        }
        private static void ProcessArgs(string[] args)
        {
            if (args.Length == 0)
                Help();
            ParamMode mode = ParamMode.NONE;
            foreach (string param in args)
            {
                if (IsSwitch(param))
                {
                    switch (param.ToUpper()[1])
                    {
                        case 'C':
                            mode = ParamMode.CSV;
                            break;
                        case 'M':
                            mode = ParamMode.MASSEDIT;
                            break;
                        case 'O':
                            mode = ParamMode.OUTPUT;
                            break;
                        case 'G':
                            mode = ParamMode.SETGAMETYPE;
                            break;
                        case 'P':
                            mode = ParamMode.SETGAMEPATH;
                            break;
                        case 'H':
                        case '?':
                            Help();
                            break;
                        default:
                            throw new ArgumentException("Invalid switch: " + param);
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case ParamMode.CSV:
                            if (File.Exists(param) && (param.ToLower().EndsWith("csv") || param.ToLower().EndsWith("txt")))
                                csvFiles.Add(param);
                            else System.Console.Out.WriteLine("WARNING: Invalid CSV filename given: " + param);
                            break;
                        case ParamMode.MASSEDIT:
                            if (File.Exists(param) && (param.ToLower().EndsWith("txt") || param.ToLower().EndsWith("massedit")))
                                masseditFiles.Add(param);
                            else System.Console.Out.WriteLine("WARNING: Invalid MASSEDIT filename given: " + param);
                            break;
                        case ParamMode.OUTPUT:
                            if (outputFile != null)
                                throw new ArgumentException("Multiple output paths specified at once: " + outputFile + " and " + param);
                            outputFile = param;
                            mode = ParamMode.NONE;
                            break;
                        case ParamMode.SETGAMETYPE:
                            switch (param.ToLower())
                            {
                                case "eldenring":
                                case "er":
                                    gameType = GameType.EldenRing;
                                    break;
                                case "dsiii":
                                case "darksoulsiii":
                                case "ds3":
                                case "darksouls3":
                                    gameType = GameType.DarkSoulsIII;
                                    break;
                                case "des":
                                case "demonsouls":
                                case "demonssouls":
                                    gameType = GameType.DemonsSouls;
                                    break;
                                case "ds1":
                                case "darksouls":
                                case "darksouls1":
                                    gameType = GameType.DarkSoulsPTDE;
                                    break;
                                case "ds1r":
                                case "ds1remastered":
                                    gameType = GameType.DarkSoulsRemastered;
                                    break;
                                case "bloodborn":
                                case "bloodborne":
                                case "bb":
                                    gameType = GameType.Bloodborne;
                                    break;
                                case "sekiro":
                                    gameType = GameType.Sekiro;
                                    break;
                                case "dsii":
                                case "darksoulsii":
                                case "ds2":
                                case "ds2s":
                                case "darksouls2":
                                    gameType = GameType.DarkSoulsIISOTFS;
                                    break;
                                default:
                                    gameType = GameType.Undefined;
                                    break;
                            }
                            break;
                        case ParamMode.SETGAMEPATH:
                            gamepath = param;
                            break;
                        case ParamMode.NONE:
                            if (param.ToLower().Equals("help") || param.Equals("?"))
                            {
                                Help();
                                break;
                            }
                            if (inputFile != null)
                                throw new ArgumentException("Multiple input files specified at once: " + inputFile + " and " + param);
                            if (gameType != GameType.EldenRing || Path.GetFileName(param).ToLower().Equals("regulation.bin") || File.Exists(param+"\\regulation.bin")) inputFile = param;
                            else System.Console.Error.WriteLine("WARNING: Invalid input regulation.bin given: " + param);
                            break;
                    }
                }
            }
        }

        private static void Help()
        {
            System.Console.Out.WriteLine("DSMS Portable by mountlover.");
            System.Console.Out.WriteLine("Lightweight utility for patching FromSoft param files. Free to distribute with other mods, but not for sale.");
            System.Console.Out.WriteLine("DS Map Studio Core developed and maintained by the SoulsMods team: https://github.com/soulsmods/DSMapStudio\n");
            System.Console.Out.WriteLine("Usage: DSMSPortable [paramfile] [-M masseditfile1 masseditfile2 ...] [-C csvfile1 csvfile2 ...] [-G gametype]");
            System.Console.Out.WriteLine("                                [-P gamepath] [-O outputpath]\n");
            System.Console.Out.WriteLine("  paramfile  Path to regulation.bin file (or respective param file for other FromSoft games) to modify");
            System.Console.Out.WriteLine("  -M masseditfile1 masseditfile2 ...");
            System.Console.Out.WriteLine("             List of text files (.TXT or .MASSEDIT) containing a script of DS Map Studio MASSEDIT commands.");
            System.Console.Out.WriteLine("             It is highly recommended to use massedit scripts to modify existing params to avoid conflicting edits.");
            System.Console.Out.WriteLine("             Edit scripts of the same type are processed in the order in which they are specified.");
            System.Console.Out.WriteLine("  -C csvfile1 csvfile2 ...");
            System.Console.Out.WriteLine("             List of csv files (.TXT or .CSV) containing entire rows of params to add.");
            System.Console.Out.WriteLine("             Each file's name must perfectly match the param it is modifying (i.e. SpEffectParam.csv).");
            System.Console.Out.WriteLine("             CSV input must used in order to add new rows.");
            System.Console.Out.WriteLine("             CSV edits will be always be processed before massedit scripts.");
            System.Console.Out.WriteLine("  -G gametype");
            System.Console.Out.WriteLine("             Code indicating which game is being modified. The default is Elden Ring. Options are as follows:");
            System.Console.Out.WriteLine("             DS1R  Dark Souls Remastered    DS2  Dark Souls 2    DS3     Dark Souls 3");
            System.Console.Out.WriteLine("             ER    Elden Ring               BB   Bloodborne      SEKIRO  Sekiro");
            System.Console.Out.WriteLine("             DS1   Dark Souls PTDE          DES  Demon's Souls");
            System.Console.Out.WriteLine("  -P gamepath");
            System.Console.Out.WriteLine("             Path to the vanilla install directory for the selected game.");
            System.Console.Out.WriteLine("             If this is the default install directory in Program Files, this will be autodetected.");
            System.Console.Out.WriteLine("             The gamepath can also be implicitly specified in a gamepath.txt file in the working directory.");
            System.Console.Out.WriteLine("  -O outputpath");
            System.Console.Out.WriteLine("             Path where the resulting regulation.bin (or equivalent param file) will be saved.");
            System.Console.Out.WriteLine("             If this is not specified, the input file will be overwritten, and a backup will be made if possible.");
        }
        // Indicates what the last read switch was
        private enum ParamMode
        {
            CSV,
            MASSEDIT,
            OUTPUT,
            SETGAMETYPE,
            SETGAMEPATH,
            NONE
        }
        // No reason to be anal about the exact switch character used, any of these is fine
        private static bool IsSwitch(string arg)
        {
            return (arg[0] == '\\' || arg[0] == '/' || arg[0] == '-');
        }
    }
}
