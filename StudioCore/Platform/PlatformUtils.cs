using System;
using Silk.NET.SDL;
using Veldrid;

namespace StudioCore.Platform;

/// <summary>
/// Trying to match Winforms version of this
/// </summary>
public enum MessageBoxButtons
{
    OK,
    OKCancel,
    YesNoCancel,
    YesNo,
}

/// <summary>
/// Also trying to match Winforms version of this
/// </summary>
[Flags]
public enum MessageBoxIcon
{
    None,
    Error = MessageBoxFlags.Error,
    Warning = MessageBoxFlags.Warning,
    Information = MessageBoxFlags.Information,
}

public enum DialogResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 3,
    No = 4,
}

/// <summary>
/// Helper to abstract away some platform specific things to make things like a native Linux build
/// maybe more feasible in the future.
/// </summary>
public abstract unsafe class PlatformUtils
{
    public static PlatformUtils Instance { get; private set; }

    private Window* _sdlWindow;

    protected PlatformUtils(Window* window)
    {
        _sdlWindow = window;
    }

    public static void InitializeWindows(Window* window)
    {
        Instance = new WindowsPlatformUtils(window);
    }

    public DialogResult MessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        string[] buttonNames = new string[3];
        DialogResult[] buttonResults = new DialogResult[3];
        int buttonCount;
        int returnID = -1;
        int escapeID = -1;
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                buttonCount = 1;
                buttonNames[0] = "Ok";
                buttonResults[0] = DialogResult.OK;
                returnID = 0;
                break;
            case MessageBoxButtons.OKCancel:
                buttonCount = 2;
                buttonNames[0] = "Ok";
                buttonResults[0] = DialogResult.OK;
                buttonNames[1] = "Cancel";
                buttonResults[1] = DialogResult.Cancel;
                returnID = 0;
                escapeID = 1;
                break;
            case MessageBoxButtons.YesNoCancel:
                buttonCount = 3;
                buttonNames[0] = "Yes";
                buttonResults[0] = DialogResult.Yes;
                buttonNames[1] = "No";
                buttonResults[1] = DialogResult.No;
                buttonNames[2] = "Cancel";
                buttonResults[2] = DialogResult.Cancel;
                returnID = 0;
                escapeID = 2;
                break;
            case MessageBoxButtons.YesNo:
                buttonCount = 2;
                buttonNames[0] = "Yes";
                buttonResults[0] = DialogResult.Yes;
                buttonNames[1] = "No";
                buttonResults[1] = DialogResult.No;
                returnID = 0;
                escapeID = 1;
                break;
            default:
                throw new Exception("Invalid button type");
        }

        MessageBoxButtonData* buttonData = stackalloc MessageBoxButtonData[buttonCount];
        for (int i = 0; i < buttonCount; i++)
        {
            MessageBoxButtonFlags buttonflags = 0;
            buttonflags |= i == returnID ? MessageBoxButtonFlags.ReturnkeyDefault : 0;
            buttonflags |= i == escapeID ? MessageBoxButtonFlags.EscapekeyDefault : 0;
            buttonData[i] = new MessageBoxButtonData()
            {
                Flags = (uint)buttonflags,
                Buttonid = (int)buttonResults[i],
                Text = (byte*)((FixedUtf8String)buttonNames[i]).StringPtr
            };
        }

        var messageBoxData = new MessageBoxData()
        {
            Flags = (uint)icon,
            Window = _sdlWindow,
            Title = (byte*)((FixedUtf8String)caption).StringPtr,
            Message = (byte*)((FixedUtf8String)text).StringPtr,
            Numbuttons = buttonCount,
            Buttons = buttonData,
            ColorScheme = null,
        };

        int clickedButton = 0;
        int result = SdlProvider.SDL.Value.ShowMessageBox(&messageBoxData, ref clickedButton);
        if (result == 0)
        {
            return (DialogResult)clickedButton;
        }

        return DialogResult.None;
    }

    public void SetClipboardText(string text)
    {
        SdlProvider.SDL.Value.SetClipboardText((byte*)((FixedUtf8String)text).StringPtr);
    }

    public void blah()
    {
        
    }
}