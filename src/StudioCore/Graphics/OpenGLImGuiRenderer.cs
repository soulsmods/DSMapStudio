using static Andre.Native.ImGuiBindings;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using Veldrid;

namespace StudioCore.Graphics;

public unsafe class OpenGLImGuiRenderer : IImguiRenderer
{
    private readonly Assembly _assembly;
    private readonly uint _indexBuffer;
    private readonly Vector2 _scaleFactor = Vector2.One;
    private readonly uint _shader;
    private readonly int _shaderFontTextureLocation;
    private readonly int _shaderProjectionMatrixLocation;
    private readonly uint _vertexBuffer;
    private readonly GL GL;
    private bool _altDown;
    private ColorSpaceHandling _colorSpaceHandling;

    private bool _controlDown;

    private int _firstFrame = 0;
    private uint _fontTexture;

    private bool _frameBegun;
    private uint _indexBufferSize;
    private bool _shiftDown;

    private uint _vertexArray;
    private uint _vertexBufferSize;
    private int _windowHeight;

    private int _windowWidth;

    public OpenGLImGuiRenderer(GL gl, int width, int height, ColorSpaceHandling colorSpaceHandling)
    {
        GL = gl;
        _assembly = typeof(VulkanImGuiRenderer).GetTypeInfo().Assembly;
        _colorSpaceHandling = colorSpaceHandling;
        _windowWidth = width;
        _windowHeight = height;

        var context = ImGui.CreateContext(null);
        ImGui.SetCurrentContext(context);

        ImGuiIO *io = ImGui.GetIO();
        io->ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io->BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        ImFontAtlasAddFontDefault(io->Fonts, null);

        //SetOpenTKKeyMappings();

        SetPerFrameImGuiData(1f / 60f);

        _vertexBufferSize = 10000;
        _indexBufferSize = 3000;

        _vertexBuffer = GL.GenBuffer();
        _indexBuffer = GL.GenBuffer();

        var vertexSource = """
                           #version 330 core
                           layout(location = 0) in vec2 in_position;
                           layout(location = 1) in vec2 in_texCoord;
                           layout(location = 2) in vec4 in_color;
                           uniform mat4 projection_matrix;
                           out vec4 color;
                           out vec2 texCoord;
                           void main()
                           {
                               gl_Position = projection_matrix * vec4(in_position, 0, 1);
                               color = in_color;
                               texCoord = in_texCoord;
                           }
                           """;

        var fragmentSource = """
                             #version 330 core
                             uniform sampler2D in_fontTexture;
                             in vec4 color;
                             in vec2 texCoord;
                             layout (location = 0) out vec4 outputColor;
                             void main()
                             {
                                 outputColor = color * texture(in_fontTexture, texCoord);
                             }
                             """;

        _shader = CreateProgram("ImGui", vertexSource, fragmentSource);
        _shaderProjectionMatrixLocation = GL.GetUniformLocation(_shader, "projection_matrix");
        _shaderFontTextureLocation = GL.GetUniformLocation(_shader, "in_fontTexture");
    }

    public void OnSetupDone()
    {
        ImGui.NewFrame();
        _frameBegun = true;
    }

    public void RecreateFontDeviceTexture()
    {
        ImGuiIO *io = ImGui.GetIO();
        // Build
        ulong* pixels;
        int width, height, bytesPerPixel;
        ImFontAtlasGetTexDataAsRGBA32(io->Fonts, &pixels, &width, &height, &bytesPerPixel);

        var mips = (uint)Math.Floor(Math.Log(Math.Max(width, height), 2));
        GL.ActiveTexture(TextureUnit.Texture0);
        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexStorage2D(TextureTarget.Texture2D, mips, SizedInternalFormat.Rgba8, (uint)width, (uint)height);
        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (uint)width, (uint)height, PixelFormat.Bgra,
            PixelType.UnsignedByte, pixels);
        GL.GenerateTextureMipmap(_fontTexture);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);

        // Store our identifier
        ImFontAtlasSetTexID(io->Fonts, ((IntPtr)_fontTexture).ToPointer());
        ImFontAtlasClearTexData(io->Fonts);
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

    private uint CreateProgram(string name, string vertexSource, string fragmentSource)
    {
        var program = GL.CreateProgram();

        var vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
        var fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSource);

        GL.AttachShader(program, vertex);
        GL.AttachShader(program, fragment);

        GL.LinkProgram(program);

        GL.GetProgram(program, GLEnum.LinkStatus, out var success);
        if (success == 0)
        {
            var info = GL.GetProgramInfoLog(program);
            Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
        }

        GL.DetachShader(program, vertex);
        GL.DetachShader(program, fragment);

        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);

        return program;
    }

    private uint CompileShader(string name, ShaderType type, string source)
    {
        var shader = GL.CreateShader(type);

        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, GLEnum.CompileStatus, out var success);
        if (success == 0)
        {
            var info = GL.GetShaderInfoLog(shader);
            Debug.WriteLine($"GL.CompileShader for shader '{name}' [{type}] had info log:\n{info}");
        }

        return shader;
    }

    public void WindowResized(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
    }

    /// <summary>
    ///     Renders the ImGui draw list data.
    /// </summary>
    public void Render()
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
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
            case Key.ShiftLeft: return ImGuiKey.LeftShift;
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

    private void RenderImDrawData(ImDrawData *draw_data)
    {
        if (draw_data->CmdListsCount == 0)
        {
            return;
        }

        var framebufferWidth = (int)(draw_data->DisplaySize.X * draw_data->FramebufferScale.X);
        var framebufferHeight = (int)(draw_data->DisplaySize.Y * draw_data->FramebufferScale.Y);
        if (framebufferWidth <= 0 || framebufferHeight <= 0)
        {
            return;
        }

        GL.Enable(GLEnum.Blend);
        GL.Enable(GLEnum.ScissorTest);
        GL.BlendEquation(GLEnum.FuncAdd);
        GL.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.StencilTest);

        // Setup orthographic projection matrix into our constant buffer
        ImGuiIO *io = ImGui.GetIO();
        GL.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);
        var L = draw_data->DisplayPos.X;
        var R = draw_data->DisplayPos.X + draw_data->DisplaySize.X;
        var T = draw_data->DisplayPos.Y;
        var B = draw_data->DisplayPos.Y + draw_data->DisplaySize.Y;
        var mvp = stackalloc float[]
        {
            2.0f / (R - L), 0.0f, 0.0f, 0.0f, 0.0f, 2.0f / (T - B), 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
            (R + L) / (L - R), (T + B) / (B - T), 0.0f, 1.0f
        };
        GL.UseProgram(_shader);
        GL.UniformMatrix4(_shaderProjectionMatrixLocation, false, new Span<float>(mvp, 16));
        GL.Uniform1(_shaderFontTextureLocation, 0);
        GL.BindSampler(0, 0);

        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);

        GL.BindBuffer(GLEnum.ArrayBuffer, _vertexBuffer);
        GL.BindBuffer(GLEnum.ElementArrayBuffer, _indexBuffer);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        var stride = (uint)sizeof(ImDrawVert);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (void*)8);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, (void*)16);

        Vector2 clipOffset = draw_data->DisplayPos;
        Vector2 clipScale = draw_data->FramebufferScale;

        // Render command lists
        for (var n = 0; n < draw_data->CmdListsCount; n++)
        {
            ImDrawList *cmd_list = draw_data->CmdLists.Data[n];

            GL.BufferData(GLEnum.ArrayBuffer, (nuint)(cmd_list->VtxBuffer.Size * sizeof(ImDrawVert)),
                (void*)cmd_list->VtxBuffer.Data, GLEnum.StreamDraw);
            GL.BufferData(GLEnum.ElementArrayBuffer, (nuint)(cmd_list->IdxBuffer.Size * sizeof(ushort)),
                (void*)cmd_list->IdxBuffer.Data, GLEnum.StreamDraw);

            for (var cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
            {
                ImDrawCmd *pcmd = &cmd_list->CmdBuffer.Data[cmd_i];
                if (pcmd->UserCallback.Data.Pointer != null)
                {
                    throw new NotImplementedException();
                }

                Vector4 clipRect;
                clipRect.X = (pcmd->ClipRect.X - clipOffset.X) * clipScale.X;
                clipRect.Y = (pcmd->ClipRect.Y - clipOffset.Y) * clipScale.Y;
                clipRect.Z = (pcmd->ClipRect.Z - clipOffset.X) * clipScale.X;
                clipRect.W = (pcmd->ClipRect.W - clipOffset.Y) * clipScale.Y;

                if (clipRect.X < framebufferWidth && clipRect.Y < framebufferHeight && clipRect.Z >= 0.0f &&
                    clipRect.W >= 0.0f)
                {
                    GL.Scissor((int)clipRect.X, framebufferHeight - (int)clipRect.W,
                        (uint)(clipRect.Z - clipRect.X), (uint)(clipRect.W - clipRect.Y));

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, (uint)pcmd->TextureId.Data);

                    if ((io->BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    {
                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, pcmd->ElemCount,
                            DrawElementsType.UnsignedShort, (void*)(pcmd->IdxOffset * sizeof(ushort)),
                            (int)pcmd->VtxOffset);
                    }
                    else
                    {
                        GL.DrawElements(PrimitiveType.Triangles, pcmd->ElemCount, DrawElementsType.UnsignedShort,
                            (void*)(pcmd->IdxOffset * sizeof(ushort)));
                    }
                }
            }
        }

        GL.DeleteVertexArray(_vertexArray);
        _vertexArray = 0;
    }
}
