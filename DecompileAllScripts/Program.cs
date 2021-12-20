using SoulsFormats;
using luadec;
using luadec.IR;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace DecompileAllScripts
{
    class Program
    {
        public static bool AreFileContentsEqual(String path1, byte[] bytes) =>
              File.ReadAllBytes(path1).SequenceEqual(bytes);

        // Lua files can't be naively compared by file for a match because of metadata that has stuff like line numbers,
        // so we will just settle for comparing the bytecode of all the functions
        static bool CompareLuaFiles(byte[] a, byte[] b, out int mismatched, out int total)
        {
            var br1 = new luadec.Utilities.BinaryReaderEx(false, a);
            var br2 = new luadec.Utilities.BinaryReaderEx(false, b);
            var lua1 = new LuaFile(br1);
            var lua2 = new LuaFile(br2);

            int lmismatched = 0;
            int ltotal = 0;

            bool compareFunction(LuaFile.Function f1, LuaFile.Function f2)
            {
                bool ret = true;
                if (f1.ChildFunctions.Length != f2.ChildFunctions.Length)
                {
                    lmismatched++;
                    ret = false;
                }
                else if (!f1.Bytecode.SequenceEqual(f2.Bytecode))
                {
                    lmismatched++;
                    ret = false;
                }
                for (int i = 0; i < f1.ChildFunctions.Length; i++)
                {
                    if (!compareFunction(f1.ChildFunctions[i], f2.ChildFunctions[i]))
                    {
                        ret = false;
                    }
                }
                ltotal++;
                return ret;
            }

            bool ret2 = compareFunction(lua1.MainFunction, lua2.MainFunction);
            mismatched = lmismatched;
            total = ltotal;
            return ret2;
        }

        static void TestLua(string name, byte[] input, string dir, string dirmiss, string dirfail, ref int fails, ref int compilefails, ref int matches, ref int mismatches)
        {
            Encoding outEncoding = Encoding.UTF8;
            Console.WriteLine($@"    Decompiling {Path.GetFileName(name)}");
            try
            {
                var br = new luadec.Utilities.BinaryReaderEx(false, input);
                var lua = new LuaFile(br);
                Function.DebugIDCounter = 0;
                Function main = new Function();

                bool hks = false;
                if (lua.Version == LuaFile.LuaVersion.Lua50)
                {
                    LuaDisassembler.GenerateIR50(main, lua.MainFunction);
                    outEncoding = Encoding.GetEncoding("shift_jis");
                }
                else if (lua.Version == LuaFile.LuaVersion.Lua51HKS)
                {
                    hks = true;
                    LuaDisassembler.GenerateIRHKS(main, lua.MainFunction);
                    outEncoding = Encoding.UTF8;
                }
                else if (lua.Version == LuaFile.LuaVersion.Lua53Smash)
                {
                    LuaDisassembler.GenerateIR53(main, lua.MainFunction, true);
                    outEncoding = Encoding.UTF8;
                }

                var outfile = Path.GetTempFileName();
                File.WriteAllText(outfile, main.ToString(), outEncoding);
                //Console.WriteLine(outfile);

                var cout = Path.GetTempFileName();
                var p = new Process();
                p.StartInfo.FileName = hks ? "hksc.exe" : "luac-5.0.2.exe";
                p.StartInfo.Arguments = $@"-s -o {cout} {outfile}";
                p.Start();
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("        Recompilation Failed!");
                    Console.ForegroundColor = ConsoleColor.White;
                    compilefails++;

                    File.WriteAllBytes($@"{dirmiss}\{Path.GetFileName(name)}", input);
                    File.Copy(outfile, $@"{dirmiss}\{Path.GetFileName(name)}.decomp");

                    return;
                }
                //Console.WriteLine($@"    Exit code: {p.ExitCode}");

                //if (AreFileContentsEqual(cout, luafile.Bytes))
                if (Path.GetFileName(name) == "014000_battle.lua")
                {
                    Console.WriteLine("hi");
                }
                int total, mismatched;
                if (CompareLuaFiles(File.ReadAllBytes(cout), input, out mismatched, out total))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("        Matched");
                    Console.ForegroundColor = ConsoleColor.White;
                    matches++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($@"        Mismatched ({total-mismatched}/{total} matched)");
                    Console.ForegroundColor = ConsoleColor.White;
                    mismatches++;

                    File.WriteAllBytes($@"{dir}\{Path.GetFileName(name)}", input);
                    File.Copy(outfile, $@"{dir}\{Path.GetFileName(name)}.decomp");
                    File.Copy(cout, $@"{dir}\{Path.GetFileName(name)}.recomp");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($@"        Failed to decompile {Path.GetFileName(name)}");
                Console.ForegroundColor = ConsoleColor.White;
                fails++;

                File.WriteAllBytes($@"{dirfail}\{Path.GetFileName(name)}", input);
            }
        }

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding outEncoding = Encoding.UTF8;
            Console.OutputEncoding = outEncoding;
            Console.ForegroundColor = ConsoleColor.White;
            var luabnds = Directory.GetFileSystemEntries(args[0], @"*.luabnd.dcx").ToList();
            var hks = Directory.GetFileSystemEntries(args[0], @"*.hks").ToList();

            int total = 0;
            int fails = 0;
            int compilefails = 0;
            int mismatches = 0;
            int matches = 0;

            if (Directory.Exists("output"))
            {
                Directory.Delete("output", true);
            }
            if (!Directory.Exists("output/mismatches"))
            {
                Directory.CreateDirectory("output/mismatches");
            }
            if (!Directory.Exists("output/miscompiles"))
            {
                Directory.CreateDirectory("output/miscompiles");
            }
            if (!Directory.Exists("output/failures"))
            {
                Directory.CreateDirectory("output/failures");
            }

            foreach (var bndpath in luabnds)
            {
                var dir = $@"output/mismatches/{Path.GetFileNameWithoutExtension(bndpath)}";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var dirmiss = $@"output/miscompiles/{Path.GetFileNameWithoutExtension(bndpath)}";
                if (!Directory.Exists(dirmiss))
                {
                    Directory.CreateDirectory(dirmiss);
                }
                var dirfail = $@"output/failures/{Path.GetFileNameWithoutExtension(bndpath)}";
                if (!Directory.Exists(dirfail))
                {
                    Directory.CreateDirectory(dirfail);
                }
                Console.WriteLine($@"Decompiling luabnd {Path.GetFileName(bndpath)}");
                var bnd = BND4.Read(bndpath);
                foreach (var luafile in bnd.Files)
                {
                    if (!luafile.Name.EndsWith(".lua"))
                    {
                        continue;
                    }
                    total++;
                    TestLua(luafile.Name, luafile.Bytes, dir, dirmiss, dirfail, ref fails, ref compilefails, ref matches, ref mismatches);
                }
            }

            foreach (var h in hks)
            {
                var dir = $@"output/mismatches/action";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var dirmiss = $@"output/miscompiles/action";
                if (!Directory.Exists(dirmiss))
                {
                    Directory.CreateDirectory(dirmiss);
                }
                var dirfail = $@"output/failures/action";
                if (!Directory.Exists(dirfail))
                {
                    Directory.CreateDirectory(dirfail);
                }

                var data = File.ReadAllBytes(h);
                total++;
                TestLua(h, data, dir, dirmiss, dirfail, ref fails, ref compilefails, ref matches, ref mismatches);
            }

            Console.WriteLine();
            Console.WriteLine("Decompilation stats:");
            Console.WriteLine($@"Total Lua Files:      {total}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($@"Decompilation Failed: {fails}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($@"Recompilation Failed: {compilefails}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($@"Mismatches:           {mismatches}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Matches:              {matches}");
            Console.ReadLine();
        }
    }
}
