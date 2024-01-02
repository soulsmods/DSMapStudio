using ImGuiNET;
using StudioCore.DebugPrimitives;
using StudioCore.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using Veldrid;
using Veldrid.Utilities;

namespace StudioCore.MsbEditor;

public class ViewGrid
{
    private DbgPrimWireGrid ViewportGridPrim;
    private DebugPrimitiveRenderableProxy ViewportGridProxy;

    // TODO: this needs to ignore the camera frustrum culling during rendering

    public ViewGrid(MeshRenderables renderlist)
    {
        ViewportGridPrim = new DbgPrimWireGrid(Color.Red, Color.Red, CFG.Current.Map_ViewportGrid_TotalSize, CFG.Current.Map_ViewportGrid_IncrementSize);

        ViewportGridProxy = new DebugPrimitiveRenderableProxy(renderlist, ViewportGridPrim);
        ViewportGridProxy.BaseColor = GetViewGridColor(CFG.Current.GFX_Viewport_Grid_Color);
    }

    public Vector3 CameraPosition { get; set; }

    private Color GetViewGridColor(Vector3 color)
    {
        return Color.FromArgb((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
    }

    public void Update(Ray ray)
    {
        if (CFG.Current.Map_EnableViewportGrid)
        {
            ViewportGridProxy.BaseColor = GetViewGridColor(CFG.Current.GFX_Viewport_Grid_Color);
            ViewportGridProxy.Visible = true;
            ViewportGridProxy.World = new Transform(0, CFG.Current.Map_ViewportGrid_GridHeight, 0, 0, 0, 0).WorldMatrix;
        }
        else
        {
            ViewportGridProxy.Visible = false;
        }
    }
}
