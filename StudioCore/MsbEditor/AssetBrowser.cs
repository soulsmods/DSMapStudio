using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public interface AssetBrowserEventHandler
    {
        public void OnInstantiateChr(string chrid);
        public void OnInstantiateObj(string objid);
    }

    public class AssetBrowser
    {
        private string _id;

        private List<string> _chrCache = new List<string>();
        private List<string> _objCache = new List<string>();

        private AssetLocator _locator;

        private AssetBrowserEventHandler _handler;

        private string _selected = null;

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
            _handler = handler;
        }

        public void RebuildCaches()
        {
            _chrCache = _locator.GetChrModels();
            _objCache = _locator.GetObjModels();
        }


        public void OnGui()
        {
            if (ImGui.Begin($@"Asset Browser##{_id}"))
            {
                ImGui.Columns(2);
                ImGui.BeginChild("AssetTypeList");
                if (ImGui.Selectable("Chr", _selected == "Chr"))
                {
                    _selected = "Chr";
                }
                if (ImGui.Selectable("Obj", _selected == "Obj"))
                {
                    _selected = "Obj";
                }
                ImGui.EndChild();
                ImGui.NextColumn();
                ImGui.BeginChild("AssetList");
                if (_selected == "Chr")
                {
                    foreach (var chr in _chrCache)
                    {
                        if (ImGui.Selectable(chr))
                        {
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _handler.OnInstantiateChr(chr);
                        }
                    }
                }
                else if (_selected == "Obj")
                {
                    foreach (var obj in _objCache)
                    {
                        if (ImGui.Selectable(obj))
                        {
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _handler.OnInstantiateObj(obj);
                        }
                    }
                }
                ImGui.EndChild();
            }
        }
    }
}
