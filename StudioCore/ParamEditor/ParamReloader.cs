using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
using SoulsFormats;
using ImGuiNET;
using System.Text;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    class ParamReloader
    {

        public static uint numberOfItemsToGive = 1;
        public static uint upgradeLevelItemToGive = 0;

        public static void ReloadMemoryParams(AssetLocator loc, string[] paramNames)
        {
            TaskManager.Run("PB:LiveParams", true, true, true, ()=>{
                GameOffsets offsets = GetGameOffsets(loc);
                var processArray = Process.GetProcessesByName(offsets.exeName);
                if (!processArray.Any())
                    processArray = Process.GetProcessesByName(offsets.exeName.Replace(".exe", ""));
                if (processArray.Any())
                {
                    SoulsMemoryHandler memoryHandler = new SoulsMemoryHandler(processArray.First());
                    ReloadMemoryParamsThreads(offsets, paramNames, memoryHandler);
                    memoryHandler.Terminate();
                } else {
                    throw new Exception("Unable to find running game");
                }
            });
        }
        private static void ReloadMemoryParamsThreads(GameOffsets offsets, string[] paramNames, SoulsMemoryHandler handler)
        {
            List<Thread> threads = new List<Thread>();
            foreach (string param in paramNames)
            {
                if (param != null && offsets.paramOffsets.ContainsKey(param))
                {
                    threads.Add(new Thread(() => WriteMemoryPARAM(offsets, ParamBank.Params[param], offsets.paramOffsets[param], handler)));
                }
            }
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
        }
        public static void GiveItemMenu(AssetLocator loc, List<PARAM.Row> rowsToGib, string param)
        {
            GameOffsets offsets = GetGameOffsets(loc);

            if (!offsets.itemGibOffsets.ContainsKey(param))
                return;
            if (ImGui.MenuItem("Spawn Selected Items In Game"))
            {
                GiveItem(offsets, rowsToGib, param, param == "EquipParamGoods" ? (int)numberOfItemsToGive : 1, param == "EquipParamWeapon" ? (int)upgradeLevelItemToGive : 0);
            }
            if (param == "EquipParamGoods")
            {
                string itemsNum = numberOfItemsToGive.ToString();
                ImGui.Indent();
                ImGui.Text("Number of Spawned Items");
                ImGui.SameLine();
                if (ImGui.InputText("##Number of Spawned Items", ref itemsNum, (uint)2))
                {
                    if (uint.TryParse(itemsNum, out uint result) && result != 0)
                    {
                        numberOfItemsToGive = result;
                    }
                }
            }
            else if (param == "EquipParamWeapon")
            {
                ImGui.Text("Spawned Weapon Level");
                ImGui.SameLine();
                string weaponLevel = upgradeLevelItemToGive.ToString();
                if (ImGui.InputText("##Spawned Weapon Level", ref weaponLevel, (uint)2))
                {
                    if (uint.TryParse(weaponLevel, out uint result) && result < 11)
                    {
                        upgradeLevelItemToGive = result;
                    }
                }
            }
            ImGui.Unindent();
        }
        private static void GiveItem(GameOffsets offsets, List<PARAM.Row> rowsToGib, string studioParamType, int itemQuantityReceived, int upgradeLevelItemToGive = 0)
        {
            if (rowsToGib.Any())
            {
                var processArray = Process.GetProcessesByName("DarkSoulsIII");
                if (processArray.Any())
                {
                    SoulsMemoryHandler memoryHandler = new SoulsMemoryHandler(processArray.First());

                    memoryHandler.PlayerItemGive(offsets, rowsToGib, studioParamType, itemQuantityReceived, -1,upgradeLevelItemToGive);

                    memoryHandler.Terminate();
                }
            }
        }
        private static void WriteMemoryPARAM(GameOffsets offsets, PARAM param, int paramOffset, SoulsMemoryHandler memoryHandler)
        {
            var BasePtr = memoryHandler.GetParamPtr(offsets, paramOffset);
            var BaseDataPtr = memoryHandler.GetToRowPtr(offsets, paramOffset);
            var RowCount = memoryHandler.GetRowCount(offsets, paramOffset);

            IntPtr DataSectionPtr;

            int RowId = 0;
            int rowPtr = 0;

            for (int i = 0; i < RowCount; i++)
            {
                memoryHandler.ReadProcessMemory(BaseDataPtr, ref RowId);
                memoryHandler.ReadProcessMemory(BaseDataPtr + offsets.rowPointerOffset, ref rowPtr);
                if (RowId < 0 || rowPtr < 0){BaseDataPtr += offsets.rowHeaderSize; continue;}

                DataSectionPtr = IntPtr.Add(BasePtr, rowPtr);

                BaseDataPtr += offsets.rowHeaderSize;

                PARAM.Row row = param[RowId];
                if (row != null)
                {
                    WriteMemoryRow(row, DataSectionPtr, memoryHandler);
                }
            }
        }
        private static void WriteMemoryRow(PARAM.Row row, IntPtr RowDataSectionPtr, SoulsMemoryHandler memoryHandler)
        {
            int offset = 0;
            int bitFieldPos = 0;
            BitArray bits = null;

            foreach (var cell in row.Cells)
            {
                offset += WriteMemoryCell(cell, RowDataSectionPtr + offset, ref bitFieldPos, ref bits, memoryHandler);
            }
        }
        private static int WriteMemoryCell(PARAM.Cell cell, IntPtr CellDataPtr, ref int bitFieldPos, ref BitArray bits, SoulsMemoryHandler memoryHandler)
        {
            PARAMDEF.DefType displayType = cell.Def.DisplayType;
            // If this can be simplified, that would be ideal. Currently we have to reconcile DefType, a numerical size in bits, and the Type used for the bitField array
            if (cell.Def.BitSize != -1)
            {
                if (displayType == SoulsFormats.PARAMDEF.DefType.u8 || displayType == SoulsFormats.PARAMDEF.DefType.dummy8)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(8);
                    }
                    return WriteBitArray(cell, CellDataPtr, ref bitFieldPos, ref bits, memoryHandler, false);
                }
                else if (displayType == SoulsFormats.PARAMDEF.DefType.u16)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(16);
                    }
                    return WriteBitArray(cell, CellDataPtr, ref bitFieldPos, ref bits, memoryHandler, false);
                }
                else if (displayType == SoulsFormats.PARAMDEF.DefType.u32)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(32);
                    }
                    return WriteBitArray(cell, CellDataPtr, ref bitFieldPos, ref bits, memoryHandler, false);
                }
            }
            else if (bits != null && bitFieldPos != 0)
            {
                int offset = WriteBitArray(null, CellDataPtr, ref bitFieldPos, ref bits, memoryHandler, true);
                return offset + WriteMemoryCell(cell, CellDataPtr + offset, ref bitFieldPos, ref bits, memoryHandler); //should recomplete current cell
            }
            if (displayType == SoulsFormats.PARAMDEF.DefType.f32)
            {
                float valueRead = 0f;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                float value = Convert.ToSingle(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(float);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.s32)
            {
                int valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                int value = Convert.ToInt32(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(Int32);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.s16)
            {
                short valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                short value = Convert.ToInt16(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(Int16);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.s8)
            {
                sbyte valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                sbyte value = Convert.ToSByte(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(sbyte);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.u32)
            {
                uint valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                uint value = Convert.ToUInt32(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(UInt32);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.u16)
            {
                ushort valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                ushort value = Convert.ToUInt16(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(UInt16);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.u8)
            {
                byte valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                byte value = Convert.ToByte(cell.Value);
                if (valueRead != value)
                {
                    memoryHandler.WriteProcessMemory(CellDataPtr, ref value);
                }
                return sizeof(byte);
            }
            else if (displayType == SoulsFormats.PARAMDEF.DefType.dummy8 || displayType == SoulsFormats.PARAMDEF.DefType.fixstr || displayType == SoulsFormats.PARAMDEF.DefType.fixstrW)
            {
                return cell.Def.ArrayLength * (displayType == SoulsFormats.PARAMDEF.DefType.fixstrW ? 2 : 1);
            }
            else
            {
                throw new Exception("Unexpected Field Type");
            }
        }
        private static int WriteBitArray(PARAM.Cell cell, IntPtr CellDataPtr, ref int bitFieldPos, ref BitArray bits, SoulsMemoryHandler memoryHandler, bool flushBits)
        {
            if (!flushBits)
            {
                BitArray cellValueBitArray = null;
                if (bits.Count == 8)
                {
                    cellValueBitArray = new BitArray(BitConverter.GetBytes((byte)cell.Value << bitFieldPos));
                }
                else if (bits.Count == 16)
                {
                    cellValueBitArray = new BitArray(BitConverter.GetBytes((ushort)cell.Value << bitFieldPos));
                }
                else if (bits.Count == 32)
                {
                    cellValueBitArray = new BitArray(BitConverter.GetBytes((uint)cell.Value << bitFieldPos));
                }
                else
                {
                    throw new Exception("Unknown bitfield length");
                }

                for (int i = 0; i < cell.Def.BitSize; i++)
                {
                    bits.Set(bitFieldPos, cellValueBitArray[bitFieldPos]);
                    bitFieldPos++;
                }
            }
            if (bitFieldPos == bits.Count || flushBits)
            {
                byte valueRead = 0;
                memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);
                byte[] bitField = new byte[bits.Count/8];
                bits.CopyTo(bitField, 0);
                if (bits.Count == 8)
                {
                    byte bitbuffer = bitField[0];
                    if (valueRead != bitbuffer)
                    {
                        memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                    }
                }
                else if (bits.Count == 16)
                {
                    ushort bitbuffer = BitConverter.ToUInt16(bitField, 0);
                    if (valueRead != bitbuffer)
                    {
                        memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                    }
                }
                else if (bits.Count == 32)
                {
                    uint bitbuffer = BitConverter.ToUInt32(bitField, 0);
                    if (valueRead != bitbuffer)
                    {
                        memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                    }
                }
                else
                {
                    throw new Exception("Unknown bitfield length");
                }
                int advance = bits.Count/8;
                bitFieldPos = 0;
                bits = null;
                return advance;
            }
            return 0;
        }

        private static GameOffsets GetGameOffsets(AssetLocator loc)
        {
            GameType game = loc.Type;
            if (!GameOffsets.offsetBank.ContainsKey(game))
            {
                try
                {
                    GameOffsets.offsetBank.Add(game, new GameOffsets(game, loc));
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return GameOffsets.offsetBank[game];
        }

        public static string[] GetReloadableParams(AssetLocator loc)
        {
            GameOffsets offs = GetGameOffsets(loc);
            if (offs == null)
                return new string[0];
            return offs.paramOffsets.Keys.ToArray();
        }
    }

    internal class GameOffsets
    {
        internal static Dictionary<GameType, GameOffsets> offsetBank = new Dictionary<GameType, GameOffsets>();
        internal string exeName;
        internal int paramBase;
        internal int[] paramInnerPath;
        internal int paramCountOffset;
        internal int paramDataOffset;
        internal int rowPointerOffset;
        internal int rowHeaderSize;
        internal Dictionary<string, int> paramOffsets;
        internal Dictionary<string, int> itemGibOffsets;

        internal GameOffsets(GameType type, AssetLocator loc){
            string dir = loc.GetGameOffsetsAssetsDir();
            Dictionary<string, string> basicData = getOffsetFile(dir+"/CoreOffsets.txt");
            exeName = basicData["exeName"];
            paramBase = int.Parse(basicData["paramBase"].Substring(2), System.Globalization.NumberStyles.HexNumber);
            string[] innerpath = basicData["paramInnerPath"].Split("/");
            paramInnerPath = new int[innerpath.Length];
            for (int i=0; i<innerpath.Length; i++)
            {
                paramInnerPath[i] = int.Parse(innerpath[i].Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            paramCountOffset = int.Parse(basicData["paramCountOffset"].Substring(2), System.Globalization.NumberStyles.HexNumber);
            paramDataOffset = int.Parse(basicData["paramDataOffset"].Substring(2), System.Globalization.NumberStyles.HexNumber);
            rowPointerOffset = int.Parse(basicData["rowPointerOffset"].Substring(2), System.Globalization.NumberStyles.HexNumber);
            rowHeaderSize = int.Parse(basicData["rowHeaderSize"].Substring(2), System.Globalization.NumberStyles.HexNumber);
            paramOffsets = getOffsetsIntFile(dir+"/ParamOffsets.txt");
            itemGibOffsets = getOffsetsIntFile(dir+"/ItemGibOffsets.txt");
            
        }

        private static Dictionary<string, int> getOffsetsIntFile(string dir)
        {
            Dictionary<string, string> paramData = getOffsetFile(dir);
            Dictionary<string, int> offsets = new Dictionary<string, int>();
            foreach (var entry in paramData)
            {
                offsets.Add(entry.Key, int.Parse(entry.Value.Substring(2), System.Globalization.NumberStyles.HexNumber));
            }
            return offsets;
        }

        private static Dictionary<string, string> getOffsetFile(string dir)
        {
            string[] data = File.ReadAllLines(dir);
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (string line in data)
            {
                string[] split = line.Split(":");
                values.Add(split[0], split[1]);
            }
            return values;
        }

        internal GameOffsets(string exe, int pbase, int[] path, int paramCountOff, int paramDataOff, int rowPointerOff, int rowHeadSize, Dictionary<string, int> pOffs, Dictionary<string, int> eOffs){
            exeName = exe;
            paramBase = pbase;
            paramInnerPath = path;
            paramCountOffset = paramCountOff;
            paramDataOffset = paramDataOff;
            rowPointerOffset = rowPointerOff;
            rowHeaderSize = rowHeadSize;
            paramOffsets = pOffs;
            itemGibOffsets = eOffs;
        }
    }

    public class SoulsMemoryHandler
    {
        public IntPtr memoryHandle;
        private readonly Process gameProcess;
        public SoulsMemoryHandler(Process gameProcess)
        {
            this.gameProcess = gameProcess;
            this.memoryHandle = NativeWrapper.OpenProcess(ProcessAccessFlags.CreateThread|ProcessAccessFlags.ReadWrite|ProcessAccessFlags.Execute|ProcessAccessFlags.VirtualMemoryOperation, gameProcess.Id);
        }
        public void Terminate()
        {
            NativeWrapper.CloseHandle(memoryHandle);
            memoryHandle = (IntPtr)0;
        }
        public bool ReadProcessMemory<T>(IntPtr baseAddress, ref T buffer) where T : unmanaged
        {
            return NativeWrapper.ReadProcessMemory(memoryHandle, baseAddress, ref buffer);
        }

        public bool WriteProcessMemory<T>(IntPtr baseAddress, ref T buffer) where T : unmanaged
        {
            return NativeWrapper.WriteProcessMemory(memoryHandle, baseAddress, ref buffer);
        }

        public bool WriteProcessMemoryArray<T>(IntPtr baseAddress, T[] buffer) where T : unmanaged
        {
            return NativeWrapper.WriteProcessMemoryArray(memoryHandle, baseAddress, buffer);
        }

        internal IntPtr GetParamPtr(GameOffsets offsets, int pOffset)
        {
            IntPtr ParamPtr = IntPtr.Add(gameProcess.MainModule.BaseAddress, offsets.paramBase);
            NativeWrapper.ReadProcessMemory(memoryHandle, ParamPtr, ref ParamPtr);
            ParamPtr = IntPtr.Add(ParamPtr, pOffset);
            NativeWrapper.ReadProcessMemory(memoryHandle, ParamPtr, ref ParamPtr);
            foreach (int innerPathPart in offsets.paramInnerPath)
            {
                ParamPtr = IntPtr.Add(ParamPtr, innerPathPart);
                NativeWrapper.ReadProcessMemory(memoryHandle, ParamPtr, ref ParamPtr);
            }

            return ParamPtr;
        }

        internal int GetRowCount(GameOffsets gOffsets, int pOffset)
        {
            IntPtr ParamPtr = GetParamPtr(gOffsets, pOffset);

            Int32 buffer = 0;
            NativeWrapper.ReadProcessMemory(memoryHandle, ParamPtr + gOffsets.paramCountOffset, ref buffer);

            return buffer;
        }

        internal IntPtr GetToRowPtr(GameOffsets gOffsets, int pOffset)
        {
            var ParamPtr = GetParamPtr(gOffsets, pOffset);
            ParamPtr = IntPtr.Add(ParamPtr, gOffsets.paramDataOffset);

            return ParamPtr;
        }


        public void ExecuteFunction(byte[] array)
        {
            IntPtr buffer = (IntPtr)0x100;

            var address = NativeWrapper.VirtualAllocEx(memoryHandle, IntPtr.Zero, buffer, AllocationType.Commit | AllocationType.Reserve, MemoryProtectionFlags.ExecuteReadWrite);

            if (address != IntPtr.Zero)
            {
                if (WriteProcessMemoryArray(address, array))
                {
                    var threadHandle = NativeWrapper.CreateRemoteThread(memoryHandle, IntPtr.Zero, (IntPtr)0, address, IntPtr.Zero, ThreadCreationFlags.Immediately, out var threadId);
                    if (threadHandle != IntPtr.Zero)
                    {
                        Kernel32.WaitForSingleObject(threadHandle, 30000);
                    }
                }
                NativeWrapper.VirtualFreeEx(memoryHandle, address, buffer, FreeType.PreservePlaceholder);
            }
        }
        public void ExecuteBufferFunction(byte[] array, byte[] argument)
        {
            var Size1 = 0x100;
            var Size2 = 0x100;

            var address = NativeWrapper.VirtualAllocEx(memoryHandle, IntPtr.Zero, (IntPtr)Size1, AllocationType.Commit | AllocationType.Reserve, MemoryProtectionFlags.ExecuteReadWrite);
            var bufferAddress = NativeWrapper.VirtualAllocEx(memoryHandle, IntPtr.Zero, (IntPtr)Size2, AllocationType.Commit | AllocationType.Reserve, MemoryProtectionFlags.ExecuteReadWrite);

            var bytjmp = 0x2;
            var bytjmpAr = new byte[7];

            WriteProcessMemoryArray(bufferAddress, argument);

            bytjmpAr = BitConverter.GetBytes((long)bufferAddress);
            Array.Copy(bytjmpAr, 0, array, bytjmp, bytjmpAr.Length);

            if (address != IntPtr.Zero)
            {
                if (WriteProcessMemoryArray(address, array))
                {

                    var threadHandle = NativeWrapper.CreateRemoteThread(memoryHandle, IntPtr.Zero, (IntPtr)0, address, IntPtr.Zero, ThreadCreationFlags.Immediately, out var threadId);
                    if (threadHandle != IntPtr.Zero)
                    {
                        Kernel32.WaitForSingleObject(threadHandle, 30000);
                    }

                }
                NativeWrapper.VirtualFreeEx(memoryHandle, address, (IntPtr)Size1, FreeType.PreservePlaceholder);
                NativeWrapper.VirtualFreeEx(memoryHandle, address, (IntPtr)Size2, FreeType.PreservePlaceholder);
            }
        }

        public void RequestReloadChr(string chrName)
        {
            byte[] chrNameBytes = Encoding.Unicode.GetBytes(chrName);

            bool memoryWriteBuffer = true;
            WriteProcessMemory(gameProcess.MainModule.BaseAddress + 0x4768F7F, ref memoryWriteBuffer);

            var buffer = new byte[]
            {
                0x48, 0xBA, 0, 0, 0, 0, 0, 0, 0, 0, //mov rdx,Alloc
                0x48, 0xA1, 0x78, 0x8E, 0x76, 0x44, 0x01, 0x00, 0x00, 0x00, //mov rax,[144768E78]
                0x48, 0x8B, 0xC8, //mov rcx,rax
                0x49, 0xBE, 0x10, 0x1E, 0x8D, 0x40, 0x01, 0x00, 0x00, 0x00, //mov r14,00000001408D1E10
                0x48, 0x83, 0xEC, 0x28, //sub rsp,28
                0x41, 0xFF, 0xD6, //call r14
                0x48, 0x83, 0xC4, 0x28, //add rsp,28
                0xC3 //ret
            };

            ExecuteBufferFunction(buffer, chrNameBytes);
        }
        internal void PlayerItemGive(GameOffsets offsets, List<PARAM.Row> rows, string paramDefParamType, int itemQuantityReceived = 1, int itemDurabilityReceived = -1, int upgradeLevelItemToGive = 0)
        {//Thanks Church Guard for providing the foundation of this.
        //Only supports ds3 as of now
            if (offsets.itemGibOffsets.ContainsKey(paramDefParamType) && rows.Any())
            {
                int paramOffset = offsets.itemGibOffsets[paramDefParamType];

                List<int> intListProcessing = new List<int>();

                //Padding? Supposedly?
                intListProcessing.Add(0);
                intListProcessing.Add(0);
                intListProcessing.Add(0);
                intListProcessing.Add(0);
                //Items to give amount
                intListProcessing.Add(rows.Count());

                foreach (var row in rows)
                {
                    intListProcessing.Add((int)row.ID + paramOffset + upgradeLevelItemToGive);
                    intListProcessing.Add(itemQuantityReceived);
                    intListProcessing.Add(itemDurabilityReceived);
                }

                //ItemGib ASM in byte format
                var itemGibByteFunctionDS3 = new byte[] { 0x48, 0x83, 0xEC, 0x48, 0x4C, 0x8D, 0x01, 0x48, 0x8D, 0x51, 0x10, 0x48, 0xA1, 0x00, 0x23, 0x75, 0x44, 0x01, 0x00, 0x00, 0x00, 0x48, 0x8B, 0xC8, 0xFF, 0x15, 0x02, 0x00, 0x00, 0x00, 0xEB, 0x08, 0x70, 0xBA, 0x7B, 0x40, 0x01, 0x00, 0x00, 0x00, 0x48, 0x83, 0xC4, 0x48, 0xC3 };

                //ItemGib Arguments Int Array
                int[] itemGibArgumentsIntArray = new int[intListProcessing.Count()];
                intListProcessing.CopyTo(itemGibArgumentsIntArray);

                //Copy itemGibArgumentsIntArray's Bytes into a byte array
                byte[] itemGibArgumentsByteArray = new byte[Buffer.ByteLength(itemGibArgumentsIntArray)];
                Buffer.BlockCopy(itemGibArgumentsIntArray, 0, itemGibArgumentsByteArray, 0, itemGibArgumentsByteArray.Length);

                //Allocate Memory for ItemGib and Arguments
                IntPtr itemGibByteFunctionPtr = NativeWrapper.VirtualAllocEx(memoryHandle, (IntPtr)0, (IntPtr)Buffer.ByteLength(itemGibByteFunctionDS3), AllocationType.Commit | AllocationType.Reserve, MemoryProtectionFlags.ExecuteReadWrite);
                IntPtr itemGibArgumentsPtr = NativeWrapper.VirtualAllocEx(memoryHandle, (IntPtr)0, (IntPtr)Buffer.ByteLength(itemGibArgumentsIntArray), AllocationType.Commit | AllocationType.Reserve, MemoryProtectionFlags.ExecuteReadWrite);

                //Write ItemGib Function and Arguments into the previously allocated memory
                NativeWrapper.WriteProcessMemoryArray(memoryHandle, itemGibByteFunctionPtr, itemGibByteFunctionDS3);
                NativeWrapper.WriteProcessMemoryArray(memoryHandle, itemGibArgumentsPtr, itemGibArgumentsByteArray);

                //Create a new thread at the copied ItemGib function in memory

                NativeWrapper.WaitForSingleObject(NativeWrapper.CreateRemoteThread(memoryHandle, itemGibByteFunctionPtr, itemGibArgumentsPtr), 30000);


                //Frees memory used by the ItemGib function and it's arguments
                NativeWrapper.VirtualFreeEx(memoryHandle, itemGibByteFunctionPtr, (IntPtr)Buffer.ByteLength(itemGibByteFunctionDS3), FreeType.PreservePlaceholder);
                NativeWrapper.VirtualFreeEx(memoryHandle, itemGibArgumentsPtr, (IntPtr)Buffer.ByteLength(itemGibArgumentsIntArray), FreeType.PreservePlaceholder);
            }
        }
    }
}
