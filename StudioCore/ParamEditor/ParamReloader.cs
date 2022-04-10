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

namespace StudioCore.ParamEditor
{
    class ParamReloader
    {

        public static uint numberOfItemsToGive = 1;
        public static uint upgradeLevelItemToGive = 0;

        public static void ReloadMemoryParamsDS3()
        {
            GameOffsets offsets = GameOffsets.OffsetsDS3;
            var processArray = Process.GetProcessesByName("DarkSoulsIII");
            if (processArray.Any())
            {
                SoulsMemoryHandler memoryHandler = new SoulsMemoryHandler(processArray.First());
                List<Thread> threads = new List<Thread>();

                foreach (var (paramFileName, param) in ParamBank.Params)
                {
                    if (offsets.paramOffsets.ContainsKey(paramFileName))
                    {
                        threads.Add(new Thread(() => WriteMemoryPARAM(offsets, param, offsets.paramOffsets[paramFileName], memoryHandler)));
                    }
                }

                foreach (var thread in threads)
                {
                    thread.Start();
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
                memoryHandler.Terminate();
            }
        }
        public static void GiveItemMenu(List<PARAM.Row> rowsToGib, string param)
        {
            GameOffsets offsets = GameOffsets.OffsetsDS3;

            if (!offsets.itemGibOffsets.ContainsKey(param))
                return;
            if (ImGui.MenuItem("Spawn Selected Items In Game"))
            {
                GiveItemDS3(rowsToGib, param, param == "EquipParamGoods" ? (int)numberOfItemsToGive : 1, param == "EquipParamWeapon" ? (int)upgradeLevelItemToGive : 0);
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
        public static void GiveItemDS3(List<PARAM.Row> rowsToGib, string studioParamType, int itemQuantityReceived, int upgradeLevelItemToGive = 0)
        {
            if (rowsToGib.Any())
            {
                var processArray = Process.GetProcessesByName("DarkSoulsIII");
                if (processArray.Any())
                {
                    SoulsMemoryHandler memoryHandler = new SoulsMemoryHandler(processArray.First());

                    memoryHandler.PlayerItemGiveDS3(rowsToGib, studioParamType, itemQuantityReceived, -1,upgradeLevelItemToGive);

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
                if (displayType == SoulsFormats.PARAMDEF.DefType.u8)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(8);
                    }
                    bits.Set(bitFieldPos, Convert.ToBoolean(cell.Value));
                    bitFieldPos++;
                    if (bitFieldPos == 8)
                    {
                        byte valueRead = 0;
                        memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                        byte[] bitField = new byte[1];
                        bits.CopyTo(bitField, 0);
                        bitFieldPos = 0;
                        byte bitbuffer = bitField[0];
                        if (valueRead != bitbuffer)
                        {
                            memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                        }
                        return sizeof(byte);
                    }
                    return 0;
                }
                else if (displayType == SoulsFormats.PARAMDEF.DefType.u16)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(16);
                    }
                    bits.Set(bitFieldPos, Convert.ToBoolean(cell.Value));
                    bitFieldPos++;
                    if (bitFieldPos == 16)
                    {
                        ushort valueRead = 0;
                        memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                        ushort[] bitField = new ushort[1];
                        bits.CopyTo(bitField, 0);
                        bitFieldPos = 0;
                        ushort bitbuffer = bitField[0];
                        if (valueRead != bitbuffer)
                        {
                            memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                        }
                        return sizeof(UInt16);
                    }
                    return 0;
                }
                else if (displayType == SoulsFormats.PARAMDEF.DefType.u32)
                {
                    if (bitFieldPos == 0)
                    {
                        bits = new BitArray(32);
                    }
                    bits.Set(bitFieldPos, Convert.ToBoolean(cell.Value));
                    bitFieldPos++;
                    if (bitFieldPos == 32)
                    {
                        uint valueRead = 0;
                        memoryHandler.ReadProcessMemory(CellDataPtr, ref valueRead);

                        uint[] bitField = new uint[1];
                        bits.CopyTo(bitField, 0);
                        bitFieldPos = 0;
                        uint bitbuffer = bitField[0];
                        if (valueRead != bitbuffer)
                        {
                            memoryHandler.WriteProcessMemory(CellDataPtr, ref bitbuffer);
                        }
                        return sizeof(UInt32);
                    }
                    return 0;
                }
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
                return cell.Def.ArrayLength;
            }
            else
            {
                throw new Exception("Unexpected Field Type");
            }
        }
    }

    internal class GameOffsets
    {
        internal int paramBase;
        internal int[] paramInnerPath;
        internal int paramCountOffset;
        internal int paramDataOffset;
        internal int rowPointerOffset;
        internal int rowHeaderSize;
        internal Dictionary<string, int> paramOffsets;
        internal Dictionary<string, int> itemGibOffsets;

        internal GameOffsets(int pbase, int[] path, int paramCountOff, int paramDataOff, int rowPointerOff, int rowHeadSize, Dictionary<string, int> pOffs, Dictionary<string, int> eOffs){
            paramBase = pbase;
            paramInnerPath = path;
            paramCountOffset = paramCountOff;
            paramDataOffset = paramDataOff;
            rowPointerOffset = rowPointerOff;
            rowHeaderSize = rowHeadSize;
            paramOffsets = pOffs;
            itemGibOffsets = eOffs;
        }

        internal static GameOffsets OffsetsDS3 = new GameOffsets(
            0x4782838, //paramBase
            new int[]{0x68, 0x68}, //paramInnerPath
            0xA, //paramCountOffset
            0x40, //paramDataOffset
            0x8, //rowPointerOffset
            0x18, //rowHeaderSize
            new Dictionary<string, int>() //paramOffsets
            {
                {"ActionButtonParam", 0xAD8},
                {"AiSoundParam", 0xD60},
                {"AtkParam_Npc", 0x268},
                {"AtkParam_Pc", 0x2B0},
                {"AttackElementCorrectParam", 0x1660},
                {"BehaviorParam", 0x3D0},
                {"BehaviorParam_PC", 0x418},
                {"BonfireWarpParam", 0xF10},
                {"BudgetParam", 0xEC8},
                {"Bullet", 0x388},
                {"BulletCreateLimitParam", 0x1780},
                {"CalcCorrectGraph", 0x8E0},
                {"Ceremony", 0x1078},
                {"CharaInitParam", 0x658},
                {"CharMakeMenuListItemParam", 0x1150},
                {"CharMakeMenuTopParam", 0x1108},
                {"ClearCountCorrectParam", 0x17C8},
                {"CoolTimeParam", 0x1A98},
                {"CultSettingParam", 0x1468},
                {"DecalParam", 0xA90},
                {"DirectionCameraParam", 0x1390},
                {"EquipMtrlSetParam", 0x6A0},
                {"EquipParamAccessory", 0x100},
                {"EquipParamGoods", 0x148},
                {"EquipParamProtector", 0xB8},
                {"EquipParamWeapon", 0x70},
                {"FaceGenParam", 0x6E8},
                {"FaceParam", 0x730},
                {"FaceRangeParam", 0x778},
                {"FootSfxParam", 0x16F0},
                {"GameAreaParam", 0x850},
                {"GameProgressParam", 0x1810},
                {"GemCategoryParam", 0xC40},
                {"GemDropDopingParam", 0xC88},
                {"GemDropModifyParam", 0xCD0},
                {"GemeffectParam", 0xBF8},
                {"GemGenParam", 0xBB0},
                {"HitEffectSeParam", 0x1270},
                {"HitEffectSfxConceptParam", 0x11E0},
                {"HitEffectSfxParam", 0x1228},
                {"HPEstusFlaskRecoveryParam", 0x14F8},
                {"ItemLotParam", 0x5C8},
                {"KnockBackParam", 0xA00},
                {"KnowledgeLoadScreenItemParam", 0x18E8},
                {"LoadBalancerDrawDistScaleParam", 0x1A50},
                {"LoadBalancerParam", 0x1858},
                {"LockCamParam", 0x928},
                {"Magic", 0x460},
                {"MapMimicryEstablishmentParam", 0x15D0},
                {"MenuOffscrRendParam", 0x1930},
                {"MenuPropertyLayoutParam", 0xFA0},
                {"MenuPropertySpecParam", 0xF58},
                {"MenuValueTableParam", 0xFE8},
                {"ModelSfxParam", 0xD18},
                {"MoveParam", 0x610},
                {"MPEstusFlaskRecoveryParam", 0x1540},
                {"MultiHPEstusFlaskBonusParam", 0x1978},
                {"MultiMPEstusFlaskBonusParam", 0x19C0},
                {"MultiPlayCorrectionParam", 0x1588},
                {"NetWorkAreaParam", 0xDF0},
                {"NetworkMsgParam", 0xE80},
                {"NetworkParam", 0xE38},
                {"NewMenuColorTableParam", 0x1198},
                {"NpcAiActionParam", 0x1738},
                {"NpcParam", 0x220},
                {"NpcThinkParam", 0x2F8},
                {"ObjActParam", 0x970},
                {"ObjectMaterialSfxParam", 0x18A0},
                {"ObjectParam", 0x340},
                {"PhantomParam", 0x10C0},
                {"PlayRegionParam", 0xDA8},
                {"ProtectorGenParam", 0xB68},
                {"RagdollParam", 0x7C0},
                {"ReinforceParamProtector", 0x1D8},
                {"ReinforceParamWeapon", 0x190},
                {"RoleParam", 0x13D8},
                {"SeMaterialConvertParam", 0x1348},
                {"ShopLineupParam", 0x808},
                {"SkeletonParam", 0x898},
                {"SpEffectParam", 0x4A8},
                {"SpEffectVfxParam", 0x4F0},
                {"SwordArtsParam", 0x14B0},
                {"TalkParam", 0x538},
                {"ThrowDirectionSfxParam", 0x16A8},
                {"ToughnessParam", 0x1300},
                {"UpperArmParam", 0x1618},
                {"WeaponGenParam", 0xB20},
                {"WepAbsorpPosParam", 0x12B8},
                {"WetAspectParam", 0x1420},
                {"Wind", 0xA48},
            },
            new Dictionary<string, int>() //itemGibOffsets
            {
            {"EquipParamWeapon", 0x0},
            {"EquipParamProtector", 0x10000000},
            {"EquipParamAccessory", 0x20000000},
            {"EquipParamGoods", 0x40000000},
            {"Magic", 0x40000000}
            }
        );
        internal static GameOffsets OffsetsER = new GameOffsets(
            0x0, //paramBase TODO
            new int[]{0x80, 0x80}, //paramInnerPath
            0xA, //paramCountOffset //TODO
            0x40, //paramDataOffset //TODO
            0x8, //rowPointerOffset //TODO
            0x18, //rowHeaderSize //TODO
            new Dictionary<string, int>() //paramOffsets
            {
                {"EquipParamWeapon", 0x88},
                {"EquipParamProtector", 0xD0},
                {"EquipParamAccessory", 0x118},
                {"EquipParamGoods", 0x160},
                {"ReinforceParamWeapon", 0x1A8},
                {"ReinforceParamProtector", 0x1F0},
                {"NpcParam", 0x238},
                {"AtkParam_Npc", 0x280},
                {"AtkParam_Pc", 0x2C8},
                {"NpcThinkParam", 0x310},
                {"Bullet", 0x358},
                {"BulletCreateLimitParam", 0x3A0},
                {"BehaviorParam", 0x3E8},
                {"BehaviorParam_PC", 0x430},
                {"Magic", 0x478},
                {"SpEffectParam", 0x4C0},
                {"SpEffectVfxParam", 0x508},
                {"SpEffectSetParam", 0x550},
                {"TalkParam", 0x598},
                {"MenuColorTableParam", 0x5E0},
                {"ItemLotParam_enemy", 0x628},
                {"ItemLotParam_map", 0x670},
                {"MoveParam", 0x6B8},
                {"CharaInitParam", 0x700},
                {"EquipMtrlSetParam", 0x748},
                {"FaceParam", 0x790},
                {"FaceRangeParam", 0x7D8},
                {"ShopLineupParam", 0x820},
                {"ShopLineupParam_Recipe", 0x868},
                {"GameAreaParam", 0x8B0},
                {"CalcCorrectGraph", 0x8F8},
                {"LockCamParam", 0x940},
                {"ObjActParam", 0x988},
                {"HitMtrlParam", 0x9D0},
                {"KnockBackParam", 0xA18},
                {"DecalParam", 0xA60},
                {"ActionButtonParam", 0xAA8},
                {"AiSoundParam", 0xAF0},
                {"PlayRegionParam", 0xB38},
                {"NetworkAreaParam", 0xB80},
                {"NetworkParam", 0xBC8},
                {"NetworkMsgParam", 0xC10},
                {"BudgetParam", 0xC58},
                {"BonfireWarpParam", 0xCA0},
                {"BonfireWarpTabParam", 0xCE8},
                {"BonfireWarpSubCategoryParam", 0xD30},
                {"MenuPropertySpecParam", 0xD78},
                {"MenuPropertyLayoutParam", 0xDC0},
                {"MenuValueTableParam", 0xE08},
                {"Ceremony", 0xE50},
                {"PhantomParam", 0xE98},
                {"CharMakeMenuTopParam", 0xEE0},
                {"CharMakeMenuListItemParam", 0xF28},
                {"HitEffectSfxConceptParam", 0xF70},
                {"HitEffectSfxParam", 0xFB8},
                {"WepAbsorpPosParam", 0x1000},
                {"ToughnessParam", 0x1048},
                {"SeMaterialConvertParam", 0x1090},
                {"ThrowDirectionSfxParam", 0x10D8},
                {"DirectionCameraParam", 0x1120},
                {"RoleParam", 0x1168},
                {"WaypointParam", 0x11B0},
                {"ThrowParam", 0x11F8},
                {"GrassTypeParam", 0x1240},
                {"GrassTypeParam_Lv1", 0x1288},
                {"GrassTypeParam_Lv2", 0x12D0},
                {"GrassLodRangeParam", 0x1318},
                {"NpcAiActionParam", 0x1360},
                {"PartsDrawParam", 0x13A8},
                {"AssetEnvironmentGeometryParam", 0x13F0},
                {"AssetModelSfxParam", 0x1438},
                {"AssetMaterialSfxParam", 0x1480},
                {"AttackElementCorrectParam", 0x14C8},
                {"FootSfxParam", 0x1510},
                {"MaterialExParam", 0x1558},
                {"HPEstusFlaskRecoveryParam", 0x15A0},
                {"MPEstusFlaskRecoveryParam", 0x15E8},
                {"MultiPlayCorrectionParam", 0x1630},
                {"MenuOffscrRendParam", 0x1678},
                {"ClearCountCorrectParam", 0x16C0},
                {"MapMimicryEstablishmentParam", 0x1708},
                {"WetAspectParam", 0x1750},
                {"SwordArtsParam", 0x1798},
                {"KnowledgeLoadScreenItemParam", 0x17E0},
                {"MultiHPEstusFlaskBonusParam", 0x1828},
                {"MultiMPEstusFlaskBonusParam", 0x1870},
                {"MultiSoulBonusRateParam", 0x18B8},
                {"WorldMapPointParam", 0x1900},
                {"WorldMapPieceParam", 0x1948},
                {"WorldMapLegacyConvParam", 0x1990},
                {"WorldMapPlaceNameParam", 0x19D8},
                {"ChrModelParam", 0x1A20},
                {"LoadBalancerParam", 0x1A68},
                {"LoadBalancerDrawDistScaleParam", 0x1AB0},
                {"LoadBalancerDrawDistScaleParam_ps4", 0x1AF8},
                {"LoadBalancerDrawDistScaleParam_ps5", 0x1B40},
                {"LoadBalancerDrawDistScaleParam_xb1", 0x1B88},
                {"LoadBalancerDrawDistScaleParam_xb1x", 0x1BD0},
                {"LoadBalancerDrawDistScaleParam_xss", 0x1C18},
                {"LoadBalancerDrawDistScaleParam_xsx", 0x1C60},
                {"LoadBalancerNewDrawDistScaleParam_win64", 0x1CA8},
                {"LoadBalancerNewDrawDistScaleParam_ps4", 0x1CF0},
                {"LoadBalancerNewDrawDistScaleParam_ps5", 0x1D38},
                {"LoadBalancerNewDrawDistScaleParam_xb1", 0x1D80},
                {"LoadBalancerNewDrawDistScaleParam_xb1x", 0x1DC8},
                {"LoadBalancerNewDrawDistScaleParam_xss", 0x1E10},
                {"LoadBalancerNewDrawDistScaleParam_xsx", 0x1E58},
                {"WwiseValueToStrParam_Switch_AttackType", 0x1EA0},
                {"WwiseValueToStrParam_Switch_DamageAmount", 0x1EE8},
                {"WwiseValueToStrParam_Switch_DeffensiveMaterial", 0x1F30},
                {"WwiseValueToStrParam_Switch_HitStop", 0x1F78},
                {"WwiseValueToStrParam_Switch_OffensiveMaterial", 0x1FC0},
                {"WwiseValueToStrParam_Switch_GrassHitType", 0x2008},
                {"WwiseValueToStrParam_Switch_PlayerShoes", 0x2050},
                {"WwiseValueToStrParam_Switch_PlayerEquipmentTops", 0x2098},
                {"WwiseValueToStrParam_Switch_PlayerEquipmentBottoms", 0x20E0},
                {"WwiseValueToStrParam_Switch_PlayerVoiceType", 0x2128},
                {"WwiseValueToStrParam_Switch_AttackStrength", 0x2170},
                {"WwiseValueToStrParam_EnvPlaceType", 0x21B8},
                {"WeatherParam", 0x2200},
                {"WeatherLotParam", 0x2248},
                {"WeatherAssetCreateParam", 0x2290},
                {"WeatherAssetReplaceParam", 0x22D8},
                {"SpeedtreeParam", 0x2320},
                {"RideParam", 0x2368},
                {"SeActivationRangeParam", 0x23B0},
                {"RollingObjLotParam", 0x23F8},
                {"NpcAiBehaviorProbability", 0x2440},
                {"BuddyParam", 0x2488},
                {"GparamRefSettings", 0x24D0},
                {"RandomAppearParam", 0x2518},
                {"MapGridCreateHeightLimitInfoParam", 0x2560},
                {"EnvObjLotParam", 0x25A8},
                {"MapDefaultInfoParam", 0x25F0},
                {"BuddyStoneParam", 0x2638},
                {"LegacyDistantViewPartsReplaceParam", 0x2680},
                {"SoundCommonIngameParam", 0x26C8},
                {"SoundAutoEnvSoundGroupParam", 0x2710},
                {"SoundAutoReverbEvaluationDistParam", 0x2758},
                {"SoundAutoReverbSelectParam", 0x27A0},
                {"EnemyCommonParam", 0x26C8},
                {"GameSystemCommonParam", 0x2830},
                {"GraphicsCommonParam", 0x2878},
                {"MenuCommonParam", 0x28C0},
                {"PlayerCommonParam", 0x2908},
                {"CutsceneGparamWeatherParam", 0x2950},
                {"CutsceneGparamTimeParam", 0x2998},
                {"CutsceneTimezoneConvertParam", 0x29E0},
                {"CutsceneWeatherOverrideGparamConvertParam", 0x2A28},
                {"SoundCutsceneParam", 0x2A70},
                {"ChrActivateConditionParam", 0x2AB8},
                {"CutsceneMapIdParam", 0x2B00},
                {"CutSceneTextureLoadParam", 0x2B48},
                {"GestureParam", 0x2B90},
                {"EquipParamGem", 0x2BD8},
                {"EquipParamCustomWeapon", 0x2C20},
                {"GraphicsConfig", 0x2C68},
                {"SoundChrPhysicsSeParam", 0x2CB0},
                {"FeTextEffectParam", 0x2CF8},
                {"CoolTimeParam", 0x2D40},
                {"WhiteSignCoolTimeParam", 0x2D88},
                {"MapPieceTexParam", 0x2DD0},
                {"MapNameTexParam", 0x2E18},
                {"WeatherLotTexParam", 0x2E60},
                {"KeyAssignParam_TypeA", 0x2EA8},
                {"KeyAssignParam_TypeB", 0x2EF0},
                {"KeyAssignParam_TypeC", 0x2F38},
                {"MapGdRegionInfoParam", 0x2F80},
                {"MapGdRegionDrawParam", 0x2FC8},
                {"KeyAssignMenuItemParam", 0x3010},
                {"SoundAssetSoundObjEnableDistParam", 0x3058},
                {"SignPuddleParam", 0x30A0},
                {"AutoCreateEnvSoundParam", 0x30E8},
                {"WwiseValueToStrParam_BgmBossChrIdConv", 0x3130},
                {"ResistCorrectParam", 0x3178},
                {"PostureControlParam_WepRight", 0x31C0},
                {"PostureControlParam_WepLeft", 0x3208},
                {"PostureControlParam_Gender", 0x3250},
                {"PostureControlParam_Pro", 0x3298},
                {"RuntimeBoneControlParam", 0x32E0},
                {"TutorialParam", 0x3328},
                {"BaseChrSelectMenuParam", 0x3370},
                {"MimicryEstablishmentTexParam", 0x33B8},
                {"SfxBlockResShareParam", 0x3400},
                {"HitEffectSeParam", 0x3448},
            },
            new Dictionary<string, int>() //itemGibOffsets
            {
            }
        );
    }

    public class SoulsMemoryHandler
    {
        public IntPtr memoryHandle;
        private readonly Process gameProcess;
        public SoulsMemoryHandler(Process gameProcess)
        {
            this.gameProcess = gameProcess;
            this.memoryHandle = NativeWrapper.OpenProcess(ProcessAccessFlags.CreateThread|ProcessAccessFlags.ReadWrite, gameProcess.Id);
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

        internal short GetRowCount(GameOffsets gOffsets, int pOffset)
        {
            IntPtr ParamPtr = GetParamPtr(gOffsets, pOffset);

            Int16 buffer = 0;
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
        public void PlayerItemGiveDS3(List<PARAM.Row> rows, string paramDefParamType, int itemQuantityReceived = 1, int itemDurabilityReceived = -1, int upgradeLevelItemToGive = 0)
        {//Thanks Church Guard for providing the foundation of this.

            GameOffsets offsets = GameOffsets.OffsetsDS3;
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
