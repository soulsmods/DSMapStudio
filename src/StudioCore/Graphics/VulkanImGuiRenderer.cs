using Silk.NET.SDL;
using static Andre.Native.ImGuiBindings;
using StudioCore.Scene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Veldrid;
using Vortice.Vulkan;
using Renderer = StudioCore.Scene.Renderer;
using Texture = Veldrid.Texture;
using Andre.Native;

namespace StudioCore.Graphics;

/// <summary>
///     Can render draw lists produced by ImGui.
///     Also provides functions for updating ImGui input.
/// </summary>
public unsafe class VulkanImGuiRenderer : IImguiRenderer, IDisposable
{
    private readonly Assembly _assembly;

    private readonly Dictionary<Texture, TextureView> _autoViewsByTexture = new();

    //private Texture _fontTexture;
    private readonly TexturePool.TextureHandle _fontTexture;

    private readonly List<IDisposable> _ownedResources = new();
    private readonly Vector2 _scaleFactor = Vector2.One;

    // Image trackers
    private readonly Dictionary<TextureView, ResourceSetInfo> _setsByView = new();

    private readonly Dictionary<IntPtr, ResourceSetInfo> _viewsById = new();
    private bool _altDown;

    private ColorSpaceHandling _colorSpaceHandling;

    //private ResourceSet _fontTextureResourceSet;
    //private IntPtr _fontAtlasID = (IntPtr)1;
    private bool _controlDown;

    private int _firstFrame;
    private Shader _fragmentShader;
    private bool _frameBegun;
    private GraphicsDevice _gd;
    private DeviceBuffer _indexBuffer;
    private int _lastAssignedID = 100;
    private ResourceLayout _layout;
    private ResourceSet _mainResourceSet;
    private Pipeline _pipeline;
    private DeviceBuffer _projMatrixBuffer;
    private bool _shiftDown;
    private ResourceLayout _textureLayout;

    // Device objects
    private DeviceBuffer _vertexBuffer;
    private Shader _vertexShader;
    private int _windowHeight;

    private int _windowWidth;

    /// <summary>
    ///     Constructs a new ImGuiRenderer.
    /// </summary>
    /// <param name="gd">The GraphicsDevice used to create and update resources.</param>
    /// <param name="outputDescription">The output format.</param>
    /// <param name="width">The initial width of the rendering target. Can be resized.</param>
    /// <param name="height">The initial height of the rendering target. Can be resized.</param>
    public VulkanImGuiRenderer(GraphicsDevice gd, OutputDescription outputDescription, int width, int height)
        : this(gd, outputDescription, width, height, ColorSpaceHandling.Legacy)
    {
    }

    /// <summary>
    ///     Constructs a new ImGuiRenderer.
    /// </summary>
    /// <param name="gd">The GraphicsDevice used to create and update resources.</param>
    /// <param name="outputDescription">The output format.</param>
    /// <param name="width">The initial width of the rendering target. Can be resized.</param>
    /// <param name="height">The initial height of the rendering target. Can be resized.</param>
    /// <param name="colorSpaceHandling">Identifies how the renderer should treat vertex colors.</param>
    public VulkanImGuiRenderer(GraphicsDevice gd, OutputDescription outputDescription, int width, int height,
        ColorSpaceHandling colorSpaceHandling)
    {
        _gd = gd;
        _assembly = typeof(VulkanImGuiRenderer).GetTypeInfo().Assembly;
        _colorSpaceHandling = colorSpaceHandling;
        _windowWidth = width;
        _windowHeight = height;

        _fontTexture = Renderer.GlobalTexturePool.AllocateTextureDescriptor();

        var context = ImGui.CreateContext(null);
        ImGui.SetCurrentContext(context);

        ImGuiIO* io = ImGui.GetIO();
        io->ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        ImFontAtlasAddFontDefault(io->Fonts, null);

        CreateDeviceResources(gd, outputDescription);

        SetPerFrameImGuiData(1f / 60f);
    }

    /// <summary>
    ///     Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _projMatrixBuffer.Dispose();
        _fontTexture.Dispose();
        _vertexShader.Dispose();
        _fragmentShader.Dispose();
        _layout.Dispose();
        _textureLayout.Dispose();
        _pipeline.Dispose();
        _mainResourceSet.Dispose();

        foreach (IDisposable resource in _ownedResources)
        {
            resource.Dispose();
        }
    }

    public void OnSetupDone()
    {
        ImGui.NewFrame();
        _frameBegun = true;
    }

    /// <summary>
    ///     Recreates the device texture used to render text.
    /// </summary>
    public void RecreateFontDeviceTexture()
    {
        RecreateFontDeviceTexture(_gd);
    }

    /// <summary>
    ///     Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(float deltaSeconds, InputSnapshot snapshot, Action updateFontAction)
    {
        BeginUpdate(deltaSeconds);
        if (updateFontAction != null)
        {
            updateFontAction.Invoke();
        }

        UpdateImGuiInput(snapshot);
        EndUpdate();
    }

    public void WindowResized(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
    }

    public void DestroyDeviceObjects()
    {
        Dispose();
    }

    public void CreateDeviceResources(GraphicsDevice gd, OutputDescription outputDescription)
    {
        CreateDeviceResources(gd, outputDescription, _colorSpaceHandling);
    }

    public void CreateDeviceResources(GraphicsDevice gd, OutputDescription outputDescription,
        ColorSpaceHandling colorSpaceHandling)
    {
        _gd = gd;
        _colorSpaceHandling = colorSpaceHandling;
        ResourceFactory factory = gd.ResourceFactory;
        _vertexBuffer = factory.CreateBuffer(
            new BufferDescription(
                10000,
                VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst,
                VmaMemoryUsage.Auto,
                0));
        _vertexBuffer.Name = "ImGui.NET Vertex Buffer";
        _indexBuffer = factory.CreateBuffer(
            new BufferDescription(
                2000,
                VkBufferUsageFlags.IndexBuffer | VkBufferUsageFlags.TransferDst,
                VmaMemoryUsage.Auto,
                0));
        _indexBuffer.Name = "ImGui.NET Index Buffer";

        _projMatrixBuffer = factory.CreateBuffer(
            new BufferDescription(
                64,
                VkBufferUsageFlags.UniformBuffer | VkBufferUsageFlags.TransferDst,
                VmaMemoryUsage.Auto,
                0));
        _projMatrixBuffer.Name = "ImGui.NET Projection Buffer";

        Tuple<Shader, Shader> res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "imgui").ToTuple();
        _vertexShader = res.Item1;
        _fragmentShader = res.Item2;

        VertexLayoutDescription[] vertexLayouts =
        {
            new(
                new VertexElementDescription("in_position", VkFormat.R32G32Sfloat),
                new VertexElementDescription("in_texCoord", VkFormat.R32G32Sfloat),
                new VertexElementDescription("in_color", VkFormat.R8G8B8A8Unorm))
        };

        _layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionMatrixBuffer", VkDescriptorType.UniformBuffer,
                VkShaderStageFlags.Vertex),
            new ResourceLayoutElementDescription("MainSampler", VkDescriptorType.Sampler,
                VkShaderStageFlags.Fragment)));
        _textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", VkDescriptorType.SampledImage,
                VkShaderStageFlags.Fragment)));

        GraphicsPipelineDescription pd = new(
            BlendStateDescription.SingleAlphaBlend,
            new DepthStencilStateDescription(false, false, VkCompareOp.Always),
            new RasterizerStateDescription(VkCullModeFlags.None, VkPolygonMode.Fill, VkFrontFace.Clockwise, true,
                true),
            VkPrimitiveTopology.TriangleList,
            new ShaderSetDescription(
                vertexLayouts,
                new[] { _vertexShader, _fragmentShader },
                new[]
                {
                    new SpecializationConstant(0, gd.IsClipSpaceYInverted),
                    new SpecializationConstant(1, _colorSpaceHandling == ColorSpaceHandling.Legacy)
                }),
            new[] { _layout, Renderer.GlobalTexturePool.GetLayout() },
            outputDescription);
        _pipeline = factory.CreateGraphicsPipeline(ref pd);
        _pipeline.Name = "ImGuiPipeline";

        _mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(_layout,
            _projMatrixBuffer,
            gd.PointSampler));

        RecreateFontDeviceTexture(gd);
    }

    /// <summary>
    ///     Gets or creates a handle for a texture to be drawn with ImGui.
    ///     Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, TextureView textureView)
    {
        if (!_setsByView.TryGetValue(textureView, out ResourceSetInfo rsi))
        {
            ResourceSet resourceSet =
                factory.CreateResourceSet(new ResourceSetDescription(_textureLayout, textureView));
            rsi = new ResourceSetInfo(GetNextImGuiBindingID(), resourceSet);

            _setsByView.Add(textureView, rsi);
            _viewsById.Add(rsi.ImGuiBinding, rsi);
            _ownedResources.Add(resourceSet);
        }

        return rsi.ImGuiBinding;
    }

    public void RemoveImGuiBinding(TextureView textureView)
    {
        if (_setsByView.TryGetValue(textureView, out ResourceSetInfo rsi))
        {
            _setsByView.Remove(textureView);
            _viewsById.Remove(rsi.ImGuiBinding);
            _ownedResources.Remove(rsi.ResourceSet);
            rsi.ResourceSet.Dispose();
        }
    }

    private IntPtr GetNextImGuiBindingID()
    {
        var newID = _lastAssignedID++;
        return newID;
    }

    /// <summary>
    ///     Gets or creates a handle for a texture to be drawn with ImGui.
    ///     Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, Texture texture)
    {
        if (!_autoViewsByTexture.TryGetValue(texture, out TextureView textureView))
        {
            textureView = factory.CreateTextureView(texture);
            _autoViewsByTexture.Add(texture, textureView);
            _ownedResources.Add(textureView);
        }

        return GetOrCreateImGuiBinding(factory, textureView);
    }

    public void RemoveImGuiBinding(Texture texture)
    {
        if (_autoViewsByTexture.TryGetValue(texture, out TextureView textureView))
        {
            _autoViewsByTexture.Remove(texture);
            _ownedResources.Remove(textureView);
            textureView.Dispose();
            RemoveImGuiBinding(textureView);
        }
    }

    /// <summary>
    ///     Retrieves the shader texture binding for the given helper handle.
    /// </summary>
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
    {
        if (!_viewsById.TryGetValue(imGuiBinding, out ResourceSetInfo rsi))
        {
            throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding);
        }

        return rsi.ResourceSet;
    }

    public void ClearCachedImageResources()
    {
        foreach (IDisposable resource in _ownedResources)
        {
            resource.Dispose();
        }

        _ownedResources.Clear();
        _setsByView.Clear();
        _viewsById.Clear();
        _autoViewsByTexture.Clear();
        _lastAssignedID = 100;
    }

    private string GetEmbeddedResourceText(string resourceName)
    {
        using (StreamReader sr = new(_assembly.GetManifestResourceStream(resourceName)))
        {
            return sr.ReadToEnd();
        }
    }

    private byte[] GetEmbeddedResourceBytes(string resourceName)
    {
        using (Stream s = _assembly.GetManifestResourceStream(resourceName))
        {
            var ret = new byte[s.Length];
            s.Read(ret, 0, (int)s.Length);
            return ret;
        }
    }

    /// <summary>
    ///     Recreates the device texture used to render text.
    /// </summary>
    public void RecreateFontDeviceTexture(GraphicsDevice gd)
    {
        ImGuiIO *io = ImGui.GetIO();
        // Build
        ulong* pixels;
        int width, height, bytesPerPixel;
        ImFontAtlasGetTexDataAsRGBA32(io->Fonts, &pixels, &width, &height, &bytesPerPixel);

        // Store our identifier
        ImFontAtlasSetTexID(io->Fonts, ((IntPtr)_fontTexture.TexHandle).ToPointer());

        Texture tex = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            (uint)width,
            (uint)height,
            1,
            1,
            VkFormat.R8G8B8A8Unorm,
            VkImageUsageFlags.Sampled,
            VkImageCreateFlags.None,
            VkImageTiling.Optimal));
        tex.Name = "ImGui.NET Font Texture";
        gd.UpdateTexture(
            tex,
            (IntPtr)pixels,
            (uint)(bytesPerPixel * width * height),
            0,
            0,
            0,
            (uint)width,
            (uint)height,
            1,
            0,
            0);
        _fontTexture.FillWithGPUTexture(tex);

        ImFontAtlasClearTexData(io->Fonts);
    }

    /// <summary>
    ///     Renders the ImGui draw list data.
    /// </summary>
    public void Render(GraphicsDevice gd, CommandList cl)
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData(), gd, cl);
        }
    }

    /// <summary>
    ///     Called before we handle the input in <see cref="Update(float, InputSnapshot)" />.
    ///     This render ImGui and update the state.
    /// </summary>
    protected void BeginUpdate(float deltaSeconds)
    {
        if (_frameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(deltaSeconds);
    }

    /// <summary>
    ///     Called at the end of <see cref="Update(float, InputSnapshot)" />.
    ///     This tells ImGui that we are on the next frame.
    /// </summary>
    protected void EndUpdate()
    {
        _frameBegun = true;
        ImGui.NewFrame();
    }

    /// <summary>
    ///     Sets per-frame data based on the associated window.
    ///     This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIO *io = ImGui.GetIO();
        io->DisplaySize = new Vector2(
            _windowWidth / _scaleFactor.X,
            _windowHeight / _scaleFactor.Y);
        io->DisplayFramebufferScale = _scaleFactor;
        io->DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    private void UpdateImGuiInput(InputSnapshot snapshot)
    {
        ImGuiIO *io = ImGui.GetIO();

        // Determine if any of the mouse buttons were pressed during this snapshot period, even if they are no longer held.
        var leftPressed = false;
        var middlePressed = false;
        var rightPressed = false;
        for (var i = 0; i < snapshot.MouseEvents.Count; i++)
        {
            MouseEvent me = snapshot.MouseEvents[i];
            if (me.Down)
            {
                switch (me.MouseButton)
                {
                    case MouseButton.Left:
                        leftPressed = true;
                        break;
                    case MouseButton.Middle:
                        middlePressed = true;
                        break;
                    case MouseButton.Right:
                        rightPressed = true;
                        break;
                }
            }
        }

        io->MouseDown[0] = leftPressed || snapshot.IsMouseDown(MouseButton.Left);
        io->MouseDown[1] = rightPressed || snapshot.IsMouseDown(MouseButton.Right);
        io->MouseDown[2] = middlePressed || snapshot.IsMouseDown(MouseButton.Middle);
        io->MousePos = snapshot.MousePosition;
        io->MouseWheel = snapshot.WheelDelta;

        IReadOnlyList<char> keyCharPresses = snapshot.KeyCharPresses;
        for (var i = 0; i < keyCharPresses.Count; i++)
        {
            var c = keyCharPresses[i];
            
            //ImGui.GetIO().AddInputCharacter(c);
            ImGuiIOAddInputCharacterUTF16(io, c);
        }

        IReadOnlyList<KeyEvent> keyEvents = snapshot.KeyEvents;
        for (var i = 0; i < keyEvents.Count; i++)
        {
            KeyEvent keyEvent = keyEvents[i];
            ImGuiIOAddKeyEvent(io, SDLKeyToImGuiKey(keyEvent.Key), keyEvent.Down);
            if (keyEvent.Key == Key.ControlLeft || keyEvent.Key == Key.ControlRight)
            {
                _controlDown = keyEvent.Down;
                ImGuiIOAddKeyEvent(io, ImGuiKey.ImGuiModCtrl, keyEvent.Down);
            }

            if (keyEvent.Key == Key.ShiftLeft || keyEvent.Key == Key.ShiftRight)
            {
                _shiftDown = keyEvent.Down;
                ImGuiIOAddKeyEvent(io, ImGuiKey.ImGuiModShift, keyEvent.Down);
            }

            if (keyEvent.Key == Key.AltLeft || keyEvent.Key == Key.AltRight)
            {
                _altDown = keyEvent.Down;
                ImGuiIOAddKeyEvent(io, ImGuiKey.ImGuiModAlt, keyEvent.Down);
            }
        }

        io->KeyCtrl = _controlDown;
        io->KeyAlt = _altDown;
        io->KeyShift = _shiftDown;
    }

    private static ImGuiKey SDLKeyToImGuiKey(Key key)
    {
        switch (key)
        {
            case Key.Tab: return ImGuiKey.Tab;
            case Key.Left: return ImGuiKey.LeftArrow;
            case Key.Right: return ImGuiKey.RightArrow;
            case Key.Up: return ImGuiKey.UpArrow;
            case Key.Down: return ImGuiKey.DownArrow;
            case Key.PageUp: return ImGuiKey.PageUp;
            case Key.PageDown: return ImGuiKey.PageDown;
            case Key.Home: return ImGuiKey.Home;
            case Key.End: return ImGuiKey.End;
            case Key.Insert: return ImGuiKey.Insert;
            case Key.Delete: return ImGuiKey.Delete;
            case Key.BackSpace: return ImGuiKey.Backspace;
            case Key.Space: return ImGuiKey.Space;
            case Key.Enter: return ImGuiKey.Enter;
            case Key.Escape: return ImGuiKey.Escape;
            case Key.Quote: return ImGuiKey.Apostrophe;
            case Key.Comma: return ImGuiKey.Comma;
            case Key.Minus: return ImGuiKey.Minus;
            case Key.Period: return ImGuiKey.Period;
            case Key.Slash: return ImGuiKey.Slash;
            case Key.Semicolon: return ImGuiKey.Semicolon;
            //case Key.Equal: return ImGuiKey.Equal;
            case Key.BracketLeft: return ImGuiKey.LeftBracket;
            case Key.BackSlash: return ImGuiKey.Backslash;
            case Key.BracketRight: return ImGuiKey.RightBracket;
            case Key.Grave: return ImGuiKey.GraveAccent;
            case Key.CapsLock: return ImGuiKey.CapsLock;
            case Key.ScrollLock: return ImGuiKey.ScrollLock;
            case Key.NumLock: return ImGuiKey.NumLock;
            case Key.PrintScreen: return ImGuiKey.PrintScreen;
            case Key.Pause: return ImGuiKey.Pause;
            case Key.Keypad0: return ImGuiKey.Keypad0;
            case Key.Keypad1: return ImGuiKey.Keypad1;
            case Key.Keypad2: return ImGuiKey.Keypad2;
            case Key.Keypad3: return ImGuiKey.Keypad3;
            case Key.Keypad4: return ImGuiKey.Keypad4;
            case Key.Keypad5: return ImGuiKey.Keypad5;
            case Key.Keypad6: return ImGuiKey.Keypad6;
            case Key.Keypad7: return ImGuiKey.Keypad7;
            case Key.Keypad8: return ImGuiKey.Keypad8;
            case Key.Keypad9: return ImGuiKey.Keypad9;
            case Key.KeypadPeriod: return ImGuiKey.KeypadDecimal;
            case Key.KeypadDivide: return ImGuiKey.KeypadDivide;
            case Key.KeypadMultiply: return ImGuiKey.KeypadMultiply;
            case Key.KeypadMinus: return ImGuiKey.KeypadSubtract;
            case Key.Plus: return ImGuiKey.KeypadAdd;
            case Key.KeypadEnter: return ImGuiKey.KeypadEnter;
            //case Key.KeypadEqual: return ImGuiKey.KeypadEqual;
            case Key.ControlLeft: return ImGuiKey.LeftCtrl;
            case Key.ShiftLeft: return ImGuiKey.ImGuiModShift;
            case Key.AltLeft: return ImGuiKey.LeftAlt;
            case Key.WinLeft: return ImGuiKey.LeftSuper;
            case Key.ControlRight: return ImGuiKey.RightCtrl;
            case Key.ShiftRight: return ImGuiKey.RightShift;
            case Key.AltRight: return ImGuiKey.RightAlt;
            case Key.WinRight: return ImGuiKey.RightSuper;
            case Key.Menu: return ImGuiKey.Menu;
            case Key.Number0: return ImGuiKey._0;
            case Key.Number1: return ImGuiKey._1;
            case Key.Number2: return ImGuiKey._2;
            case Key.Number3: return ImGuiKey._3;
            case Key.Number4: return ImGuiKey._4;
            case Key.Number5: return ImGuiKey._5;
            case Key.Number6: return ImGuiKey._6;
            case Key.Number7: return ImGuiKey._7;
            case Key.Number8: return ImGuiKey._8;
            case Key.Number9: return ImGuiKey._9;
            case Key.A: return ImGuiKey.A;
            case Key.B: return ImGuiKey.B;
            case Key.C: return ImGuiKey.C;
            case Key.D: return ImGuiKey.D;
            case Key.E: return ImGuiKey.E;
            case Key.F: return ImGuiKey.F;
            case Key.G: return ImGuiKey.G;
            case Key.H: return ImGuiKey.H;
            case Key.I: return ImGuiKey.I;
            case Key.J: return ImGuiKey.J;
            case Key.K: return ImGuiKey.K;
            case Key.L: return ImGuiKey.L;
            case Key.M: return ImGuiKey.M;
            case Key.N: return ImGuiKey.N;
            case Key.O: return ImGuiKey.O;
            case Key.P: return ImGuiKey.P;
            case Key.Q: return ImGuiKey.Q;
            case Key.R: return ImGuiKey.R;
            case Key.S: return ImGuiKey.S;
            case Key.T: return ImGuiKey.T;
            case Key.U: return ImGuiKey.U;
            case Key.V: return ImGuiKey.V;
            case Key.W: return ImGuiKey.W;
            case Key.X: return ImGuiKey.X;
            case Key.Y: return ImGuiKey.Y;
            case Key.Z: return ImGuiKey.Z;
            case Key.F1: return ImGuiKey.F1;
            case Key.F2: return ImGuiKey.F2;
            case Key.F3: return ImGuiKey.F3;
            case Key.F4: return ImGuiKey.F4;
            case Key.F5: return ImGuiKey.F5;
            case Key.F6: return ImGuiKey.F6;
            case Key.F7: return ImGuiKey.F7;
            case Key.F8: return ImGuiKey.F8;
            case Key.F9: return ImGuiKey.F9;
            case Key.F10: return ImGuiKey.F10;
            case Key.F11: return ImGuiKey.F11;
            case Key.F12: return ImGuiKey.F12;
            case Key.F13: return ImGuiKey.F13;
            case Key.F14: return ImGuiKey.F14;
            case Key.F15: return ImGuiKey.F15;
            case Key.F16: return ImGuiKey.F16;
            case Key.F17: return ImGuiKey.F17;
            case Key.F18: return ImGuiKey.F18;
            case Key.F19: return ImGuiKey.F19;
            case Key.F20: return ImGuiKey.F20;
            case Key.F21: return ImGuiKey.F21;
            case Key.F22: return ImGuiKey.F22;
            case Key.F23: return ImGuiKey.F23;
            case Key.F24: return ImGuiKey.F24;
            //case Key.AC.BACK: return ImGuiKey.AppBack;
            //case Key.AC.FORWARD: return ImGuiKey.AppForward;
        }

        return ImGuiKey.None;
    }

    private void RenderImDrawData(ImDrawData *draw_data, GraphicsDevice gd, CommandList cl)
    {
        if (_firstFrame < 30)
        {
            _firstFrame++;
            return;
        }

        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (draw_data->CmdListsCount == 0)
        {
            return;
        }

        var totalVBSize = (uint)(draw_data->TotalVtxCount * sizeof(ImDrawVert));
        if (totalVBSize > _vertexBuffer.SizeInBytes)
        {
            _vertexBuffer.Dispose();
            _vertexBuffer = gd.ResourceFactory.CreateBuffer(
                new BufferDescription((uint)(totalVBSize * 1.5f),
                    VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst,
                    VmaMemoryUsage.Auto,
                    0));
        }

        var totalIBSize = (uint)(draw_data->TotalIdxCount * sizeof(ushort));
        if (totalIBSize > _indexBuffer.SizeInBytes)
        {
            _indexBuffer.Dispose();
            _indexBuffer = gd.ResourceFactory.CreateBuffer(
                new BufferDescription((uint)(totalIBSize * 1.5f),
                    VkBufferUsageFlags.IndexBuffer | VkBufferUsageFlags.TransferDst,
                    VmaMemoryUsage.Auto,
                    0));
        }

        for (var i = 0; i < draw_data->CmdListsCount; i++)
        {
            ImDrawList *cmd_list = draw_data->CmdLists.Data[i];

            cl.UpdateBuffer(
                _vertexBuffer,
                vertexOffsetInVertices * (uint)sizeof(ImDrawVert),
                new IntPtr(cmd_list->VtxBuffer.Data),
                (uint)(cmd_list->VtxBuffer.Size * sizeof(ImDrawVert)));

            cl.UpdateBuffer(
                _indexBuffer,
                indexOffsetInElements * sizeof(ushort),
                new IntPtr(cmd_list->IdxBuffer.Data),
                (uint)(cmd_list->IdxBuffer.Size * sizeof(ushort)));

            vertexOffsetInVertices += (uint)cmd_list->VtxBuffer.Size;
            indexOffsetInElements += (uint)cmd_list->IdxBuffer.Size;
        }

        if (draw_data->CmdListsCount > 0)
        {
            cl.Barrier(VkPipelineStageFlags2.Transfer,
                VkAccessFlags2.TransferWrite,
                VkPipelineStageFlags2.VertexInput,
                VkAccessFlags2.VertexAttributeRead | VkAccessFlags2.IndexRead);
        }

        // Setup orthographic projection matrix into our constant buffer
        {
            ImGuiIO *io = ImGui.GetIO();

            var mvp = Matrix4x4.CreateOrthographicOffCenter(
                0f,
                io->DisplaySize.X,
                io->DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            _gd.UpdateBuffer(_projMatrixBuffer, 0, ref mvp);
        }

        cl.SetVertexBuffer(0, _vertexBuffer);
        cl.SetIndexBuffer(_indexBuffer, VkIndexType.Uint16);
        cl.SetPipeline(_pipeline);
        cl.SetGraphicsResourceSet(0, _mainResourceSet);

        ImDrawDataScaleClipRects(draw_data, ImGui.GetIO()->DisplayFramebufferScale);

        // Render command lists
        var vtx_offset = 0;
        var idx_offset = 0;
        for (var n = 0; n < draw_data->CmdListsCount; n++)
        {
            ImDrawList *cmd_list = draw_data->CmdLists.Data[n];
            for (var cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
            {
                ImDrawCmd *pcmd = &cmd_list->CmdBuffer.Data[cmd_i];
                if (pcmd->UserCallback.Data.Pointer != null)
                {
                    throw new NotImplementedException();
                }

                //cl.SetGraphicsResourceSet(1, _fontTextureResourceSet);
                /*if (pcmd.TextureId != IntPtr.Zero)
                    {
                        if (pcmd.TextureId == _fontAtlasID)
                        {
                            cl.SetGraphicsResourceSet(1, _fontTextureResourceSet);
                        }
                        else
                        {
                            cl.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                        }
                    }*/
                Renderer.GlobalTexturePool.BindTexturePool(cl, 1);

                cl.SetScissorRect(
                    0,
                    (uint)Math.Max(pcmd->ClipRect.X, 0),
                    (uint)Math.Max(pcmd->ClipRect.Y, 0),
                    (uint)(pcmd->ClipRect.Z - pcmd->ClipRect.X),
                    (uint)(pcmd->ClipRect.W - pcmd->ClipRect.Y));

                cl.DrawIndexed(pcmd->ElemCount, 1, (uint)idx_offset + pcmd->IdxOffset, vtx_offset,
                    (uint)pcmd->TextureId.Data);
            }

            idx_offset += cmd_list->IdxBuffer.Size;
            vtx_offset += cmd_list->VtxBuffer.Size;
        }
    }

    public static float GetUIScale()
    {
        // TODO: Multiply by monitor DPI when available.
        return CFG.Current.UIScale;
    }

    private struct ResourceSetInfo
    {
        public readonly IntPtr ImGuiBinding;
        public readonly ResourceSet ResourceSet;

        public ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
        {
            ImGuiBinding = imGuiBinding;
            ResourceSet = resourceSet;
        }
    }
}
