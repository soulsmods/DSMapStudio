﻿using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;

namespace StudioCore.Gui;

/// <summary>
/// A null viewport that doesn't actually do anything
/// </summary>
public class NullViewport : IViewport
{
    public WorldView WorldView { get; private set; }
    
    public int X;
    public int Y;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public float NearClip { get; set; } = 0.1f;
    public float FarClip { get; set; } = CFG.Current.GFX_RenderDistance_Max;
    
    private string _vpid = "";

    public bool ViewportSelected => false;
    
    public NullViewport(string id, MsbEditor.ActionManager am, MsbEditor.Selection sel, int width, int height)
    {
        _vpid = id;
        Width = width;
        Height = height;
        WorldView = new WorldView(new Veldrid.Rectangle(0, 0, Width, Height));
    }
    
    public void OnGui()
    {
        if (ImGui.Begin($@"Viewport##{_vpid}", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoNav))
        {
            var p = ImGui.GetWindowPos();
            var s = ImGui.GetWindowSize();
            var newvp = new Rectangle((int)p.X, (int)p.Y + 3, (int)s.X, (int)s.Y - 3);
            ResizeViewport(null, newvp);
            ImGui.Text("Disabled...");
        }
        ImGui.End();

        if (ImGui.Begin($@"Profiling##{_vpid}"))
        {
            ImGui.Text($@"Disabled...");
        }
        ImGui.End();
    }
    
    public void SceneParamsGui()
    {
    }
    
    public void ResizeViewport(GraphicsDevice device, Rectangle newvp)
    {
        Width = newvp.Width;
        Height = newvp.Height;
        X = newvp.X;
        Y = newvp.Y;
        WorldView.UpdateBounds(newvp);
    }
    
    public bool Update(Sdl2Window window, float dt)
    {
        return false;
    }
    
    public void Draw(GraphicsDevice device, CommandList cl)
    {
    }
    
    public void SetEnvMap(uint index)
    {
    }
    
    public void FrameBox(BoundingBox box)
    {
    }
    
    public void FramePosition(Vector3 pos, float dist)
    {
    }
}