using System.Collections.Specialized;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Andre.Native;
using bottlenoselabs.C2CS.Runtime;

public static unsafe partial class ImGuiBindings
{
    public static unsafe partial class ImGui
    {
        private const int STACKALLOCMAX = 4096;

        public static void ShowDemoWindow(ref bool open)
        {
            CBool cOpen = open;
            ShowDemoWindow(&cOpen);
            open = cOpen;
        }
        
        public static void ShowMetricsWindow(ref bool open)
        {
            CBool cOpen = open;
            ShowMetricsWindow(&cOpen);
            open = cOpen;
        }
        
        public static void ShowDebugLogWindow(ref bool open)
        {
            CBool cOpen = open;
            ShowDebugLogWindow(&cOpen);
            open = cOpen;
        }
        
        public static void ShowIDStackToolWindow(ref bool open)
        {
            CBool cOpen = open;
            ShowIDStackToolWindow(&cOpen);
            open = cOpen;
        }
        
        public static void Indent()
        {
            Indent(0.0f);
        }
        
        public static void Unindent()
        {
            Unindent(0.0f);
        }
        
        public static void PushID(string strId)
        {
            using var cId = (CString)strId;
            PushIDStr(cId);
        }
        
        public static void PushID(int id)
        {
            PushIDInt(id);
        }
        
        public static void Text(string fmt)
        {
            using var cFmt = (CString)fmt;
            Text(cFmt);
        }
        
        public static void TextColored(Vector4 color, string fmt)
        {
            using var cFmt = (CString)fmt;
            TextColored(color, cFmt);
        }
        
        public static void TextUnformatted(string text, string? textEnd=null)
        {
            using var cText = (CString)text;
            using var cTextEnd = (CString)textEnd;
            TextUnformatted(cText, cTextEnd);
        }
        
        public static void TextDisabled(string fmt)
        {
            using var cFmt = (CString)fmt;
            TextDisabled(cFmt);
        }
        
        public static void TextWrapped(string fmt)
        {
            using var cFmt = (CString)fmt;
            TextWrapped(cFmt);
        }
        
        public static void LabelText(string label, string fmt)
        {
            using var cLabel = (CString)label;
            using var cFmt = (CString)fmt;
            LabelText(cLabel, cFmt);
        }
        
        public static bool ArrowButton(string strId, ImGuiDir dir)
        {
            using var cId = (CString)strId;
            var res = ArrowButton(cId, dir);
            return res;
        }
        
        public static bool DragFloat(string label, ref float v, float speed=1.0f, float min=0.0f, float max=0.0f,
            string format = "%.3f", ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            float f = v;
            var res = DragFloat(cLabel, &f, speed, min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool DragFloat2(string label, ref Vector2 v, float speed=1.0f, float min=0.0f, float max=0.0f,
            string format = "%.3f", ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector2 f = v;
            var res = DragFloat2(cLabel, (float*)&f, speed, min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool DragFloat3(string label, ref Vector3 v, float speed=1.0f, float min=0.0f, float max=0.0f,
            string format = "%.3f", ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector3 f = v;
            var res = DragFloat3(cLabel, (float*)&f, speed, min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool DragFloat4(string label, ref Vector4 v, float speed=1.0f, float min=0.0f, float max=0.0f,
            string format = "%.3f", ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector4 f = v;
            var res = DragFloat4(cLabel, (float*)&f, speed, min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool DragInt(string label, ref int v, float speed=1.0f, int min=0, int max=0,
            string format = "%d", ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            int f = v;
            var res = DragInt(cLabel, &f, speed, min, max, cFormat, flags);
            v = f;
            return res;
        }

        public static bool SliderFloat(string label, ref float v, float min, float max, string format = "%.3f",
            ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            float f = v;
            var res = SliderFloat(cLabel, &f, min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool SliderFloat2(string label, ref Vector2 v, float min, float max, string format = "%.3f",
            ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            var f = v;
            var res = SliderFloat2(cLabel, (float*)(&f), min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool SliderFloat3(string label, ref Vector3 v, float min, float max, string format = "%.3f",
            ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            var f = v;
            var res = SliderFloat3(cLabel, (float*)(&f), min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool SliderFloat4(string label, ref Vector4 v, float min, float max, string format = "%.3f",
            ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            var f = v;
            var res = SliderFloat4(cLabel, (float*)(&f), min, max, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool SliderInt(string label, ref int v, int min, int max, string format = "%.3f",
            ImGuiSliderFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            int i = v;
            var res = SliderInt(cLabel, &i, min, max, cFormat, flags);
            v = i;
            return res;
        }
        
        public static bool InputText(string label, ref string buf, int bufSize, ImGuiInputTextFlags flags=0)
        {
            bufSize += 1;
            CString cBufString;
            if (bufSize <= STACKALLOCMAX)
            {
                var cBuf = stackalloc byte[bufSize];
                if (Encoding.UTF8.TryGetBytes(buf.AsSpan(), new Span<byte>(cBuf, bufSize), out int bw))
                    cBufString = new CString(cBuf);
                else
                    cBufString = new CString(buf);
            }
            else
            {
                cBufString = new CString(buf);
            }
            using var cLabel = (CString)label;
            var result = InputText(cLabel, cBufString, bufSize, flags, new ImGuiInputTextCallback(), null);
            buf = cBufString.ToString();
            return result;
        }
        public static bool InputTextMultiline(string label, ref string buf, int bufSize, Vector2 size=default, ImGuiInputTextFlags flags=0)
        {
            bufSize += 1;
            CString cBufString;
            if (bufSize <= STACKALLOCMAX)
            {
                var cBuf = stackalloc byte[bufSize];
                if (Encoding.UTF8.TryGetBytes(buf.AsSpan(), new Span<byte>(cBuf, bufSize), out int bw))
                    cBufString = new CString(cBuf);
                else
                    cBufString = new CString(buf);
            }
            else
            {
                cBufString = new CString(buf);
            }
            using var cLabel = (CString)label;
            var result = InputTextMultiline(cLabel, cBufString, bufSize, size, flags, new ImGuiInputTextCallback(), null);
            buf = cBufString.ToString();
            return result;
        }
        
        public static bool InputTextWithHint(string label, string hint, ref string buf, int bufSize)
        {
            bufSize += 1;
            CString cBufString;
            if (bufSize <= STACKALLOCMAX)
            {
                var cBuf = stackalloc byte[bufSize];
                if (Encoding.UTF8.TryGetBytes(buf.AsSpan(), new Span<byte>(cBuf, bufSize), out int bw))
                    cBufString = new CString(cBuf);
                else
                    cBufString = new CString(buf);
            }
            else
            {
                cBufString = new CString(buf);
            }
            using var cLabel = (CString)label;
            using var cHint = (CString)hint;
            var result = InputTextWithHint(cLabel, cHint, cBufString, bufSize, 0, new ImGuiInputTextCallback(), null);
            buf = cBufString.ToString();
            return result;
        }
        
        public static bool InputFloat(string label, ref float v, float step=0.0f, float stepFast=0.0f, string format = "%.3f",
            ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            float f = v;
            var res = InputFloat(cLabel, &f, step, stepFast, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool InputFloat2(string label, ref Vector2 v, string format = "%.3f",
            ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector2 f = v;
            var res = InputFloat2(cLabel, (float*)&f, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool InputFloat3(string label, ref Vector3 v, string format = "%.3f",
            ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector3 f = v;
            var res = InputFloat3(cLabel, (float*)&f, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool InputFloat4(string label, ref Vector4 v, string format = "%.3f",
            ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            Vector4 f = v;
            var res = InputFloat4(cLabel, (float*)&f, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool InputInt(string label, ref int v, int step=1, int stepFast=100, ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            int i = v;
            var res = InputInt(cLabel, &i, step, stepFast, flags);
            v = i;
            return res;
        }
        
        public static bool InputDouble(string label, ref double v, double step=0.0, double stepFast=0.0, string format = "%.6f",
            ImGuiInputTextFlags flags = 0)
        {
            using var cLabel = (CString)label;
            using var cFormat = (CString)format;
            double f = v;
            var res = InputDouble(cLabel, &f, step, stepFast, cFormat, flags);
            v = f;
            return res;
        }
        
        public static bool ColorEdit3(string label, ref Vector3 v, ImGuiColorEditFlags flags = 0)
        {
            using var cLabel = (CString)label;
            Vector3 f = v;
            var res = ColorEdit3(cLabel, (float*)&f, flags);
            v = f;
            return res;
        }
        
        public static bool ColorEdit4(string label, ref Vector4 v, ImGuiColorEditFlags flags = 0)
        {
            using var cLabel = (CString)label;
            Vector4 f = v;
            var res = ColorEdit4(cLabel, (float*)&f, flags);
            v = f;
            return res;
        }
        
        public static bool TreeNodeEx(string label, ImGuiTreeNodeFlags flags=0)
        {
            using var cLabel = (CString)label;
            var res = TreeNodeExStr(cLabel, flags);
            return res;
        }
        
        public static bool TreeNodeEx(string label, ImGuiTreeNodeFlags flags, string fmt)
        {
            using var cLabel = (CString)label;
            using var cFmt = (CString)fmt;
            var res = TreeNodeExStrStr(cLabel, flags, cFmt);
            return res;
        }
        
        public static void PlotLines(string label, float[] values, int valuesOffset=0, string? overlayText=null, 
            float scaleMin=float.MaxValue, float scaleMax=float.MaxValue, Vector2 graphSize=default, 
            int stride=sizeof(float))
        {
            using var cLabel = (CString)label;
            using var cOverlayText = (CString)overlayText;
            fixed (float* pValues = values)
                PlotLinesFloatPtr(cLabel, pValues, values.Length, valuesOffset, cOverlayText, scaleMin, 
                    scaleMax, graphSize, stride);
        }
        
        public static bool Begin(string name)
        {
            using var cName = (CString)name;
            var res = Begin(cName, null, 0);
            return res;
        }
        
        public static bool Begin(string name, ImGuiWindowFlags flags)
        {
            using var cName = (CString)name;
            var res = Begin(cName, null, flags);
            return res;
        }
        
        public static bool Begin(string name, ref bool open, ImGuiWindowFlags flags=0)
        {
            using var cName = (CString)name;
            CBool cOpen = open;
            var res = Begin(cName, &cOpen, flags);
            open = cOpen;
            return res;
        }

        public static Vector2 GetWindowPos()
        {
            Vector2 ret;
            GetWindowPos(&ret);
            return ret;
        }
        
        public static Vector2 GetWindowSize()
        {
            Vector2 ret;
            GetWindowSize(&ret);
            return ret;
        }
        
        public static Vector2 GetContentRegionAvail()
        {
            Vector2 ret;
            GetContentRegionAvail(&ret);
            return ret;
        }
        
        public static Vector2 GetContentRegionMax()
        {
            Vector2 ret;
            GetContentRegionMax(&ret);
            return ret;
        }
        
        public static Vector2 GetWindowContentRegionMin()
        {
            Vector2 ret;
            GetWindowContentRegionMin(&ret);
            return ret;
        }
        
        public static Vector2 GetWindowContentRegionMax()
        {
            Vector2 ret;
            GetWindowContentRegionMax(&ret);
            return ret;
        }

        public static void SameLine()
        {
            SameLine(0.0f, -1.0f);
        }
        
        public static bool CollapsingHeader(string label, ImGuiTreeNodeFlags flags=0)
        {
            using var cLabel = (CString)label;
            var result = CollapsingHeaderTreeNodeFlags(cLabel, flags);
            return result;
        }
        
        public static bool Button(string fmt, Vector2 size=default)
        {
            using var cFmt = (CString)fmt;
            var result = Button(cFmt, size);
            return result;
        }
        
        public static bool BeginCombo(string label, string previewValue, ImGuiComboFlags flags=0)
        {
            using var cLabel = (CString)label;
            using var cPreviewValue = (CString)previewValue;
            var result = BeginCombo(cLabel, cPreviewValue, flags);
            return result;
        }
        
        public static bool Combo(string label, ref int currentItem, string[] items, int length)
        {
            var result = false;
            if (BeginCombo(label, items[currentItem]))
            {
                for (int i = 0; i < length; i++)
                {
                    if (Selectable(items[i]))
                    {
                        currentItem = i;
                        result = true;
                    }
                }
                EndCombo();
            }
            return result;
        }
        
        public static ImGuiID GetID(string fmt)
        {
            using var cFmt = (CString)fmt;
            var id = GetIDStr(cFmt);
            return id;
        }
        
        public static bool BeginChild(string strId, Vector2 size=default, ImGuiChildFlags childFlags=0, ImGuiWindowFlags windowFlags=0)
        {
            using var cId = (CString)strId;
            var result = BeginChildStr(cId, size, childFlags, windowFlags);
            return result;
        }
        
        public static void SetNextWindowPos(Vector2 pos)
        {
            SetNextWindowPos(pos, 0, default);
        }
        
        public static void SetNextWindowSize(Vector2 pos)
        {
            SetNextWindowSize(pos, 0);
        }
        
        public static void SetScrollHereX()
        {
            SetScrollHereX(0.5f);
        }
        
        public static void SetScrollHereY()
        {
            SetScrollHereY(0.5f);
        }

        public static bool Selectable(string label, bool selected=false, ImGuiSelectableFlags flags=0, Vector2 size=default)
        {
            using var cLabel = (CString)label;
            var result = SelectableBool(cLabel, selected, flags, size);
            return result;
        }
        
        public static bool BeginPopupModal(string name, ref bool open, ImGuiWindowFlags flags=0)
        {
            using var cName = (CString)name;
            CBool cOpen = open;
            var res = BeginPopupModal(cName, &cOpen, flags);
            open = cOpen;
            return res;
        }

        public static void OpenPopup(string strId, ImGuiPopupFlags popupFlags=0)
        {
            using var cId = (CString)strId;
            OpenPopupStr(cId, popupFlags);
        }

        public static bool BeginPopup(string strId, ImGuiWindowFlags flags=0)
        {
            using var cId = (CString)strId;
            var result = BeginPopup(cId, flags);
            return result;
        }
        
        public static bool BeginPopupContextItem(string? strId=null, ImGuiPopupFlags flags=ImGuiPopupFlags.MouseButtonRight)
        {
            using var cId = (CString)strId;
            var result = BeginPopupContextItem(cId, flags);
            return result;
        }
        
        public static bool BeginTable(string strId, int column, ImGuiTableFlags flags=0, Vector2 outerSize=default, float innerWidth=0)
        {
            using var cId = (CString)strId;
            var result = BeginTable(cId, column, flags, outerSize, innerWidth);
            return result;
        }
        
        public static void TableSetupColumn(string label, ImGuiTableColumnFlags flags=0, float initWidthOrWeight=0, ImGuiID userId=default)
        {
            using var cLabel = (CString)label;
            TableSetupColumn(cLabel, flags, initWidthOrWeight, userId);
        }

        public static void Columns(int count=1, string? strId=null, bool border=true)
        {
            using var cId = (CString)strId;
            Columns(count, cId, border);
        }
        
        public static bool SetDragDropPayload(string type, IntPtr data, int size, ImGuiCond cond=0)
        {
            using var cType = (CString)type;
            var result = SetDragDropPayload(cType, data.ToPointer(), size, cond);
            return result;
        }
        
        public static ImGuiPayload* AcceptDragDropPayload(string type, ImGuiDragDropFlags flags=0)
        {
            using var cType = (CString)type;
            var result = AcceptDragDropPayload(cType, flags);
            return result;
        }
        
        public static void BeginDisabled()
        {
            BeginDisabled(true);
        }
        
        public static void SetKeyboardFocusHere()
        {
            SetKeyboardFocusHere(0);
        }
        
        public static bool IsItemClicked()
        {
            return IsItemClicked(0);
        }

        public static bool BeginMenu(string label, bool enabled=true)
        {
            using var cLabel = (CString)label;
            var result = BeginMenu(cLabel, enabled);
            return result;
        }

        public static bool MenuItem(string label, string? shortcut=null, bool selected=false, bool enabled=true)
        {
            using var cLabel = (CString)label;
            using var cShortcut = (CString)shortcut;
            var res = MenuItemBool(cLabel, cShortcut, selected, enabled);
            return res;
        }
        
        public static bool MenuItem(string label, bool enabled)
        {
            using var cLabel = (CString)label;
            var res = MenuItemBool(cLabel, default, false, enabled);
            return res;
        }
        
        public static bool BeginTabBar(string strId, ImGuiTabBarFlags flags=0)
        {
            using var cId = (CString)strId;
            var result = BeginTabBar(cId, flags);
            return result;
        }
        
        public static bool BeginTabItem(string name, ref bool open, ImGuiTabItemFlags flags=0)
        {
            using var cName = (CString)name;
            CBool cOpen = open;
            var res = BeginTabItem(cName, &cOpen, flags);
            open = cOpen;
            return res;
        }
        
        public static bool BeginTabItem(string name, ImGuiTabItemFlags flags=0)
        {
            using var cName = (CString)name;
            var res = BeginTabItem(cName, null, flags);
            return res;
        }
        
        public static bool InvisibleButton(string strId, Vector2 size, ImGuiButtonFlags flags=0)
        {
            using var cId = (CString)strId;
            var res = InvisibleButton(cId, size, flags);
            return res;
        }
        
        public static bool Checkbox(string label, ref bool v)
        {
            using var cLabel = (CString)label;
            CBool cValue = v;
            var res = Checkbox(cLabel, &cValue);
            v = cValue;
            return res;
        }

        public static void ProgressBar(float fraction)
        {
            ProgressBar(fraction, new Vector2(float.NegativeInfinity, 0), new CString());
        }
        
        public static void ProgressBar(float fraction, Vector2 sizeArg, string? overlay=null)
        {
            using var cOverlay = (CString)overlay;
            ProgressBar(fraction, sizeArg, cOverlay);
        }

        public static Vector2 CalcTextSize(string text, bool hideTextAfterDoubleDash = false, float wrapWidth = -1.0f)
        {
            Vector2 res;
            using var cText = (CString)text;
            CalcTextSize(&res, cText, default, hideTextAfterDoubleDash, wrapWidth);
            return res;
        }
    }
}
