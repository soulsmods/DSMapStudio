using StudioCore.Platform;
using StudioCore.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudioCore;

public partial class CFG
{
    public bool Viewport_Enable_Selection_Outline = false;

    public bool MapEditor_MapObjectList_ShowMapNames = true;
    public bool MapEditor_MapObjectList_ShowCharacterNames = true;
    public bool MapEditor_MapObjectList_ShowAssetNames = true;
    public bool MapEditor_MapObjectList_ShowMapPieceNames = true;
    public bool MapEditor_MapObjectList_ShowPlayerCharacterNames = true;
    public bool MapEditor_MapObjectList_ShowSystemCharacterNames = true;
    public bool MapEditor_MapObjectList_ShowTreasureNames = true;

    public bool EnableFrustrumCulling = false;
    public bool Map_AlwaysListLoadedMaps = true;
    public bool EnableEldenRingAutoMapOffset = true;

    public bool Map_EnableViewportGrid = false;
    public int Map_ViewportGridType = 0;
    public Vector3 GFX_Viewport_Grid_Color = Utils.GetDecimalColor(Color.Red);
    public int Map_ViewportGrid_TotalSize = 1000;
    public int Map_ViewportGrid_IncrementSize = 10;

    public float Map_ViewportGrid_Offset = 0;

    public float Map_ViewportGrid_ShortcutIncrement = 1;

    public float Map_MoveSelectionToCamera_Radius = 3.0f;
    public float GFX_Camera_FOV { get; set; } = 60.0f;
    public float GFX_Camera_MoveSpeed_Slow { get; set; } = 1.0f;
    public float GFX_Camera_MoveSpeed_Normal { get; set; } = 20.0f;
    public float GFX_Camera_MoveSpeed_Fast { get; set; } = 200.0f;
    public float GFX_Camera_Sensitivity { get; set; } = 0.0160f;
    public float GFX_RenderDistance_Max { get; set; } = 50000.0f;
    public float Map_ArbitraryRotation_X_Shift { get; set; } = 90.0f;
    public float Map_ArbitraryRotation_Y_Shift { get; set; } = 90.0f;

    public float GFX_Framerate_Limit_Unfocused = 20.0f;
    public float GFX_Framerate_Limit = 60.0f;
    public uint GFX_Limit_Buffer_Flver_Bone = 65536;
    public uint GFX_Limit_Buffer_Indirect_Draw = 50000;
    public int GFX_Limit_Renderables = 50000;

    public float GFX_Wireframe_Color_Variance = 0.11f;

    public Vector3 GFX_Renderable_Box_BaseColor = Utils.GetDecimalColor(Color.Blue);
    public Vector3 GFX_Renderable_Box_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_Cylinder_BaseColor = Utils.GetDecimalColor(Color.Blue);
    public Vector3 GFX_Renderable_Cylinder_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_Sphere_BaseColor = Utils.GetDecimalColor(Color.Blue);
    public Vector3 GFX_Renderable_Sphere_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_Point_BaseColor = Utils.GetDecimalColor(Color.Yellow);
    public Vector3 GFX_Renderable_Point_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_DummyPoly_BaseColor = Utils.GetDecimalColor(Color.Yellow);
    public Vector3 GFX_Renderable_DummyPoly_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_BonePoint_BaseColor = Utils.GetDecimalColor(Color.Blue);
    public Vector3 GFX_Renderable_BonePoint_HighlightColor = Utils.GetDecimalColor(Color.DarkViolet);

    public Vector3 GFX_Renderable_ModelMarker_Chr_BaseColor = Utils.GetDecimalColor(Color.Firebrick);
    public Vector3 GFX_Renderable_ModelMarker_Chr_HighlightColor = Utils.GetDecimalColor(Color.Tomato);

    public Vector3 GFX_Renderable_ModelMarker_Object_BaseColor = Utils.GetDecimalColor(Color.MediumVioletRed);
    public Vector3 GFX_Renderable_ModelMarker_Object_HighlightColor = Utils.GetDecimalColor(Color.DeepPink);

    public Vector3 GFX_Renderable_ModelMarker_Player_BaseColor = Utils.GetDecimalColor(Color.DarkOliveGreen);
    public Vector3 GFX_Renderable_ModelMarker_Player_HighlightColor = Utils.GetDecimalColor(Color.OliveDrab);

    public Vector3 GFX_Renderable_ModelMarker_Other_BaseColor = Utils.GetDecimalColor(Color.Wheat);
    public Vector3 GFX_Renderable_ModelMarker_Other_HighlightColor = Utils.GetDecimalColor(Color.AntiqueWhite);

    public Vector3 GFX_Renderable_PointLight_BaseColor = Utils.GetDecimalColor(Color.YellowGreen);
    public Vector3 GFX_Renderable_PointLight_HighlightColor = Utils.GetDecimalColor(Color.Yellow);

    public Vector3 GFX_Renderable_SpotLight_BaseColor = Utils.GetDecimalColor(Color.Goldenrod);
    public Vector3 GFX_Renderable_SpotLight_HighlightColor = Utils.GetDecimalColor(Color.Violet);

    public Vector3 GFX_Renderable_DirectionalLight_BaseColor = Utils.GetDecimalColor(Color.Cyan);
    public Vector3 GFX_Renderable_DirectionalLight_HighlightColor = Utils.GetDecimalColor(Color.AliceBlue);

    public Vector3 GFX_Gizmo_X_BaseColor = new(0.952f, 0.211f, 0.325f);
    public Vector3 GFX_Gizmo_X_HighlightColor = new(1.0f, 0.4f, 0.513f);

    public Vector3 GFX_Gizmo_Y_BaseColor = new(0.525f, 0.784f, 0.082f);
    public Vector3 GFX_Gizmo_Y_HighlightColor = new(0.713f, 0.972f, 0.270f);

    public Vector3 GFX_Gizmo_Z_BaseColor = new(0.219f, 0.564f, 0.929f);
    public Vector3 GFX_Gizmo_Z_HighlightColor = new(0.407f, 0.690f, 1.0f);
    public RenderFilter LastSceneFilter { get; set; } = RenderFilter.All ^ RenderFilter.Light;

    public RenderFilterPreset SceneFilter_Preset_01 { get; set; } = new("Map",
        RenderFilter.MapPiece | RenderFilter.Object | RenderFilter.Character | RenderFilter.Region);

    public RenderFilterPreset SceneFilter_Preset_02 { get; set; } = new("Collision",
        RenderFilter.Collision | RenderFilter.Object | RenderFilter.Character | RenderFilter.Region);

    public RenderFilterPreset SceneFilter_Preset_03 { get; set; } = new("Collision & Navmesh",
        RenderFilter.Collision | RenderFilter.Navmesh | RenderFilter.Object | RenderFilter.Character |
        RenderFilter.Region);

    public RenderFilterPreset SceneFilter_Preset_04 { get; set; } = new("Lighting (Map)",
        RenderFilter.MapPiece | RenderFilter.Object | RenderFilter.Character | RenderFilter.Light);

    public RenderFilterPreset SceneFilter_Preset_05 { get; set; } = new("Lighting (Collision)",
        RenderFilter.Collision | RenderFilter.Object | RenderFilter.Character | RenderFilter.Light);

    public RenderFilterPreset SceneFilter_Preset_06 { get; set; } = new("All", RenderFilter.All);

    public class RenderFilterPreset
    {
        [JsonConstructor]
        public RenderFilterPreset()
        {
        }

        public RenderFilterPreset(string name, RenderFilter filters)
        {
            Name = name;
            Filters = filters;
        }

        public string Name { get; set; }
        public RenderFilter Filters { get; set; }
    }
}
