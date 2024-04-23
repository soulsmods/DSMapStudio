using static Andre.Native.ImGuiBindings;
using System.Collections.Generic;

namespace StudioCore.MsbEditor;

public interface AssetBrowserEventHandler
{
    public void OnInstantiateChr(string chrid);
    public void OnInstantiateObj(string objid);
    public void OnInstantiateParts(string objid);
    public void OnInstantiateMapPiece(string mapid, string modelid);
}
