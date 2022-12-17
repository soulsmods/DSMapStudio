using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Numerics;
using FSParam;
using StudioCore.Resource;
using SoulsFormats;
using Newtonsoft.Json;
using StudioCore.Scene;
using StudioCore.Editor;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// A universe is a collection of loaded maps with methods to load, serialize,
    /// and unload individual maps.
    /// </summary>
    public class Universe
    {

        public Exception LoadMapExceptions = null;
        public Dictionary<string, ObjectContainer> LoadedObjectContainers { get; private set; } = new Dictionary<string, ObjectContainer>();
        private AssetLocator _assetLocator;
        private Scene.RenderScene _renderScene;
        public Selection Selection { get; private set; }

        public List<string> EnvMapTextures { get; private set; } = new List<string>();

        public Universe(AssetLocator al, Scene.RenderScene scene, Selection sel)
        {
            _assetLocator = al;
            _renderScene = scene;
            Selection = sel;
        }

        public Map GetLoadedMap(string id)
        {
            if (id != null)
            {
                if (LoadedObjectContainers.ContainsKey(id) && LoadedObjectContainers[id] is Map m)
                {
                    return m;
                }
            }
            return null;
        }

        public GameType GameType => _assetLocator.Type;

        public bool postLoad = false;
        public int _dispGroupCount = 8;

        private static RenderFilter GetRenderFilter(string type)
        {
            RenderFilter filter;

            switch (type)
            {
                case "Enemy":
                case "DummyEnemy":
                    filter = RenderFilter.Character;
                    break;
                case "Asset":
                case "Object":
                case "DummyObject":
                    filter = RenderFilter.Object;
                    break;
                case "Player":
                    filter = RenderFilter.Region;
                    break;
                case "MapPiece":
                    filter = RenderFilter.MapPiece;
                    break;
                case "Collision":
                    filter = RenderFilter.Collision;
                    break;
                case "Navmesh":
                    filter = RenderFilter.Navmesh;
                    break;
                case "Region":
                    filter = RenderFilter.Region;
                    break;
                case "Light":
                    filter = RenderFilter.Light;
                    break;
                default:
                    filter = RenderFilter.All;
                    break;
            }
            return filter;
        }

        private static ModelMarkerType GetModelMarkerType(string type)
        {
            ModelMarkerType modelMarker;

            switch (type)
            {
                case "Enemy":
                case "DummyEnemy":
                    modelMarker = ModelMarkerType.Enemy;
                    break;
                case "Asset":
                case "Object":
                case "DummyObject":
                    modelMarker = ModelMarkerType.Object;
                    break;
                case "Player":
                    modelMarker = ModelMarkerType.Player;
                    break;
                case "MapPiece":
                case "Collision":
                case "Navmesh":
                case "Region":
                    modelMarker = ModelMarkerType.Other;
                    break;
                default:
                    modelMarker = ModelMarkerType.None;
                    break;
            }
            return modelMarker;
        }

        public RenderableProxy GetRegionDrawable(Map map, Entity obj)
        {
            if (obj.WrappedObject is IMsbRegion r && r.Shape is MSB.Shape.Box b)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetBoxRegionProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Region;
                return mesh;
            }
            else if (obj.WrappedObject is IMsbRegion r2 && r2.Shape is MSB.Shape.Sphere s)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetSphereRegionProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Region;
                return mesh;
            }
            else if (obj.WrappedObject is IMsbRegion r3 && r3.Shape is MSB.Shape.Point p)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetPointRegionProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Region;
                return mesh;
            }
            else if (obj.WrappedObject is IMsbRegion r4 && r4.Shape is MSB.Shape.Cylinder c)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetCylinderRegionProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Region;
                return mesh;
            }
            return null;
        }

        public RenderableProxy GetLightDrawable(Map map, Entity obj)
        {
            var light = (BTL.Light)obj.WrappedObject;
            if (light.Type is BTL.LightType.Directional)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetDirectionalLightProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Light;
                return mesh;
            }
            else if (light.Type is BTL.LightType.Point)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetPointLightProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Light;
                return mesh;
            }
            else if (light.Type is BTL.LightType.Spot)
            {
                var mesh = DebugPrimitiveRenderableProxy.GetSpotLightProxy(_renderScene);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Light;
                return mesh;
            }
            else
            {
                throw new Exception($"Unexpected BTL LightType: {light.Type}");
            }
        }

        public RenderableProxy GetDS2EventLocationDrawable(Map map, Entity obj)
        {
            var mesh = DebugPrimitiveRenderableProxy.GetBoxRegionProxy(_renderScene);
            mesh.World = obj.GetWorldMatrix();
            obj.RenderSceneMesh = mesh;
            mesh.SetSelectable(obj);
            return mesh;
        }

        public RenderableProxy GetDummyPolyDrawable(ObjectContainer map, Entity obj)
        {
            var mesh = DebugPrimitiveRenderableProxy.GetDummyPolyRegionProxy(_renderScene);
            mesh.World = obj.GetWorldMatrix();
            obj.RenderSceneMesh = mesh;
            mesh.SetSelectable(obj);
            return mesh;
        }

        public RenderableProxy GetBoneDrawable(ObjectContainer map, Entity obj)
        {
            var mesh = new SkeletonBoneRenderableProxy(_renderScene);
            mesh.World = obj.GetWorldMatrix();
            obj.RenderSceneMesh = mesh;
            mesh.SetSelectable(obj);
            return mesh;
        }

        /// <summary>
        /// Creates a drawable for a model and registers it with the scene. Will load
        /// the required assets in the background if they aren't already loaded.
        /// </summary>
        /// <param name="modelname"></param>
        /// <returns></returns>
        public RenderableProxy GetModelDrawable(Map map, Entity obj, string modelname, bool load)
        {
            AssetDescription asset;
            bool loadcol = false;
            bool loadnav = false;
            bool loadflver = false;
            Scene.RenderFilter filt = Scene.RenderFilter.All;
            var amapid = map.Name.Substring(0, 6) + "_00_00";
            if (_assetLocator.Type == GameType.EldenRing)
            {
                amapid = map.Name;
            }
            // Special case for chalice dungeon assets
            if (map.Name.StartsWith("m29"))
            {
                amapid = "m29_00_00_00";
            }
            var job = ResourceManager.CreateNewJob($@"Loading mesh");
            if (modelname.ToLower().StartsWith("m"))
            {
                loadflver = true;
                asset = _assetLocator.GetMapModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, modelname));
                filt = Scene.RenderFilter.MapPiece;
            }
            else if (modelname.ToLower().StartsWith("c"))
            {
                loadflver = true;
                asset = _assetLocator.GetChrModel(modelname);
                filt = Scene.RenderFilter.Character;
            }
            else if (modelname.ToLower().StartsWith("o") || modelname.StartsWith("AEG"))
            {
                loadflver = true;
                asset = _assetLocator.GetObjModel(modelname);
                filt = Scene.RenderFilter.Object;
            }
            else if (modelname.ToLower().StartsWith("h"))
            {
                loadcol = true;
                asset = _assetLocator.GetMapCollisionModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, modelname), false);
                filt = Scene.RenderFilter.Collision;
            }
            else if (modelname.ToLower().StartsWith("n"))
            {
                loadnav = true;
                asset = _assetLocator.GetMapNVMModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, modelname));
                filt = Scene.RenderFilter.Navmesh;
            }
            else
            {
                asset = _assetLocator.GetNullAsset();
            }

            var modelMarkerType = GetModelMarkerType(obj.WrappedObject.GetType().ToString().Split("+").Last());
            if (loadcol)
            {
                var mesh = MeshRenderableProxy.MeshRenderableFromCollisionResource(
                    _renderScene, asset.AssetVirtualPath, modelMarkerType);
                mesh.World = obj.GetWorldMatrix();
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Collision;
                obj.RenderSceneMesh = mesh;
                if (load && !ResourceManager.IsResourceLoadedOrInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly))
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                    ResourceManager.MarkResourceInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    var task = job.Complete();
                    if (obj.Universe.postLoad)
                    {
                        task.Wait();
                    }
                }
                return mesh;
            }
            else if (loadnav && _assetLocator.Type != GameType.DarkSoulsIISOTFS)
            {
                var mesh = MeshRenderableProxy.MeshRenderableFromNVMResource(
                    _renderScene, asset.AssetVirtualPath, modelMarkerType);
                mesh.World = obj.GetWorldMatrix();
                obj.RenderSceneMesh = mesh;
                mesh.SetSelectable(obj);
                mesh.DrawFilter = RenderFilter.Navmesh;
                if (load && !ResourceManager.IsResourceLoadedOrInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly))
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                    ResourceManager.MarkResourceInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    var task = job.Complete();
                    if (obj.Universe.postLoad)
                    {
                        task.Wait();
                    }
                }
                return mesh;
            }
            else if (loadnav && _assetLocator.Type == GameType.DarkSoulsIISOTFS)
            {

            }
            else if (loadflver)
            {
                var model = MeshRenderableProxy.MeshRenderableFromFlverResource(
                    _renderScene, asset.AssetVirtualPath, modelMarkerType);
                model.DrawFilter = filt;
                model.World = obj.GetWorldMatrix();
                obj.RenderSceneMesh = model;
                model.SetSelectable(obj);
                if (load && !ResourceManager.IsResourceLoadedOrInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly))
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Flver);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                    ResourceManager.MarkResourceInFlight(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    var task = job.Complete();
                    if (obj.Universe.postLoad)
                    {
                        task.Wait();
                    }
                }
                return model;
            }
            return null;
        }

        public void LoadDS2Generators(string mapid, Map map)
        {
            Dictionary<long, Param.Row> registParams = new Dictionary<long, Param.Row>();
            Dictionary<long, MergedParamRow> generatorParams = new Dictionary<long, MergedParamRow>();
            Dictionary<long, Entity> generatorObjs = new Dictionary<long, Entity>();
            Dictionary<long, Param.Row> eventParams = new Dictionary<long, Param.Row>();
            Dictionary<long, Param.Row> eventLocationParams = new Dictionary<long, Param.Row>();
            Dictionary<long, Param.Row> objectInstanceParams = new Dictionary<long, Param.Row>();

            var regparam = ParamEditor.ParamBank.PrimaryBank.Params[$"generatorregistparam_{mapid}"];
            foreach (var row in regparam.Rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    row.Name = "regist_" + row.ID;
                }
                registParams.Add(row.ID, row);

                var obj = new MapEntity(map, row, MapEntity.MapEntityType.DS2GeneratorRegist);
                map.AddObject(obj);
            }

            var locparam = ParamEditor.ParamBank.PrimaryBank.Params[$"generatorlocation_{mapid}"];
            foreach (var row in locparam.Rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    row.Name = "generator_" + row.ID.ToString();
                }

                // Offset the generators by the map offset
                row.GetCellHandleOrThrow("PositionX").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionX").Value + map.MapOffset.Position.X);
                row.GetCellHandleOrThrow("PositionY").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionY").Value + map.MapOffset.Position.Y);
                row.GetCellHandleOrThrow("PositionZ").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionZ").Value + map.MapOffset.Position.Z);
                
                var mergedRow = new MergedParamRow();
                mergedRow.AddRow("generator-loc", row);
                generatorParams.Add(row.ID, mergedRow);

                var obj = new MapEntity(map, mergedRow, MapEntity.MapEntityType.DS2Generator);
                generatorObjs.Add(row.ID, obj);
                map.AddObject(obj);
            }

            var chrsToLoad = new HashSet<AssetDescription>();
            var genparam = ParamEditor.ParamBank.PrimaryBank.Params[$"generatorparam_{mapid}"];
            foreach (var row in genparam.Rows)
            {
                if (row.Name == null || row.Name == "")
                {
                    row.Name = "generator_" + row.ID.ToString();
                }

                if (generatorParams.ContainsKey(row.ID))
                {
                    generatorParams[row.ID].AddRow("generator", row);
                }
                else
                {
                    var mergedRow = new MergedParamRow();
                    mergedRow.AddRow("generator", row);
                    generatorParams.Add(row.ID, mergedRow);
                    var obj = new MapEntity(map, mergedRow, MapEntity.MapEntityType.DS2Generator);
                    generatorObjs.Add(row.ID, obj);
                    map.AddObject(obj);
                }

                uint registid = (uint)row.GetCellHandleOrThrow("GeneratorRegistParam").Value;
                if (registParams.ContainsKey(registid))
                {
                    var regist = registParams[registid];
                    var chrid = ParamEditor.ParamBank.PrimaryBank.GetChrIDForEnemy(
                        (int)regist.GetCellHandleOrThrow("EnemyParamID").Value);
                    if (chrid != null)
                    {
                        var asset = _assetLocator.GetChrModel($@"c{chrid}");
                        var model = MeshRenderableProxy.MeshRenderableFromFlverResource(
                            _renderScene, asset.AssetVirtualPath, ModelMarkerType.Enemy);
                        model.DrawFilter = RenderFilter.Character;
                        generatorObjs[row.ID].RenderSceneMesh = model;
                        model.SetSelectable(generatorObjs[row.ID]);
                        chrsToLoad.Add(asset);
                    }
                }
            }

            var evtparam = ParamEditor.ParamBank.PrimaryBank.Params[$"eventparam_{mapid}"];
            foreach (var row in evtparam.Rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    row.Name = "event_" + row.ID.ToString();
                }
                eventParams.Add(row.ID, row);

                var obj = new MapEntity(map, row, MapEntity.MapEntityType.DS2Event);
                map.AddObject(obj);
            }

            var evtlparam = ParamEditor.ParamBank.PrimaryBank.Params[$"eventlocation_{mapid}"];
            foreach (var row in evtlparam.Rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    row.Name = "eventloc_" + row.ID.ToString();
                }
                eventLocationParams.Add(row.ID, row);

                // Offset the generators by the map offset
                row.GetCellHandleOrThrow("PositionX").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionX").Value + map.MapOffset.Position.X);
                row.GetCellHandleOrThrow("PositionY").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionY").Value + map.MapOffset.Position.Y);
                row.GetCellHandleOrThrow("PositionZ").SetValue(
                    (float)row.GetCellHandleOrThrow("PositionZ").Value + map.MapOffset.Position.Z);

                var obj = new MapEntity(map, row, MapEntity.MapEntityType.DS2EventLocation);
                map.AddObject(obj);

                // Try rendering as a box for now
                var mesh = DebugPrimitiveRenderableProxy.GetBoxRegionProxy(_renderScene);
                mesh.World = obj.GetLocalTransform().WorldMatrix;
                obj.RenderSceneMesh = mesh;
                mesh.SetSelectable(obj);
            }

            var objparam = ParamEditor.ParamBank.PrimaryBank.Params[$"mapobjectinstanceparam_{mapid}"];
            foreach (var row in objparam.Rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    row.Name = "objinstance_" + row.ID.ToString();
                }
                objectInstanceParams.Add(row.ID, row);

                var obj = new MapEntity(map, row, MapEntity.MapEntityType.DS2ObjectInstance);
                map.AddObject(obj);
            }

            var job = ResourceManager.CreateNewJob($@"Loading chrs");
            foreach (var chr in chrsToLoad)
            {
                if (chr.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (chr.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(chr.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                }
            }
            job.Complete();
        }

        public void PopulateMapList()
        {
            LoadedObjectContainers.Clear();
            foreach (var m in _assetLocator.GetFullMapList())
            {
                LoadedObjectContainers.Add(m, null);
            }
        }

        public bool LoadMap(string mapid, bool selectOnLoad = false)
        {
            if (_assetLocator.Type == GameType.DarkSoulsIISOTFS
                && ParamEditor.ParamBank.VanillaBank.Params == null)
            {
                // ParamBank must be loaded for DS2 maps
                TaskManager.warningList.TryAdd("ds2-mapload-noparams", "DS2 maps cannot be loaded until params are loaded.");
                return false;
            }

            var ad = _assetLocator.GetMapMSB(mapid);
            if (ad.AssetPath == null)
            {
                return false;
            }
            LoadMapAsync(mapid, selectOnLoad);
            return true;

        }

        public BTL ReturnBTL(AssetDescription ad)
        {
            try
            {
                BTL btl;

                if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    var bdt = BXF4.Read(ad.AssetPath, ad.AssetPath[..^3] + "bdt");
                    var file = bdt.Files.Find(f => f.Name.EndsWith("light.btl.dcx"));
                    if (file == null)
                    {
                        return null;
                    }
                    btl = BTL.Read(file.Bytes);
                }
                else
                {
                    btl = BTL.Read(ad.AssetPath);
                }

                return btl;
            }
            catch (InvalidDataException)
            {
                TaskManager.warningList.TryAdd($"{ad.AssetName} load", $"Failed to load {ad.AssetName}");
                return null;
            }
        }

        public async void LoadMapAsync(string mapid, bool selectOnLoad = false)
        {
            try
            {
                postLoad = false;
                var map = new Map(this, mapid);

                List<Task> tasks = new();
                Task task;

                var mappiecesToLoad = new HashSet<AssetDescription>();
                var chrsToLoad = new HashSet<AssetDescription>();
                var objsToLoad = new HashSet<AssetDescription>();
                var colsToLoad = new HashSet<AssetDescription>();
                var navsToLoad = new HashSet<AssetDescription>();

                //drawgroup count
                switch (_assetLocator.Type)
                {
                    // imgui checkbox click seems to break at some point after 8 (8*32) checkboxes, so let's just hope that never happens, yeah?
                    case GameType.DemonsSouls:
                    case GameType.DarkSoulsPTDE:
                    case GameType.DarkSoulsRemastered:
                    case GameType.DarkSoulsIISOTFS:
                        _dispGroupCount = 4;
                        break;
                    case GameType.Bloodborne:
                    case GameType.DarkSoulsIII:
                        _dispGroupCount = 8;
                        break;
                    case GameType.Sekiro:
                    case GameType.EldenRing:
                        _dispGroupCount = 8; //?
                        break;
                    default:
                        throw new Exception($"Error: Did not expect Gametype {_assetLocator.Type}");
                        //break;
                }

                var ad = _assetLocator.GetMapMSB(mapid);
                if (ad.AssetPath == null)
                {
                    return;
                }
                IMsb msb;
                if (_assetLocator.Type == GameType.DarkSoulsIII)
                {
                    msb = MSB3.Read(ad.AssetPath);
                }
                else if (_assetLocator.Type == GameType.Sekiro)
                {
                    msb = MSBS.Read(ad.AssetPath);
                }
                else if (_assetLocator.Type == GameType.EldenRing)
                {
                    msb = MSBE.Read(ad.AssetPath);
                }
                else if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    msb = MSB2.Read(ad.AssetPath);
                }
                else if (_assetLocator.Type == GameType.Bloodborne)
                {
                    msb = MSBB.Read(ad.AssetPath);
                }
                else if (_assetLocator.Type == GameType.DemonsSouls)
                {
                    msb = MSBD.Read(ad.AssetPath);
                }
                else
                {
                    msb = MSB1.Read(ad.AssetPath);
                }

                map.LoadMSB(msb);

                var amapid = _assetLocator.GetAssetMapID(mapid);
                foreach (var model in msb.Models.GetEntries())
                {
                    AssetDescription asset;
                    if (model.Name.StartsWith("m"))
                    {
                        asset = _assetLocator.GetMapModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, model.Name));
                        mappiecesToLoad.Add(asset);
                    }
                    else if (model.Name.StartsWith("c"))
                    {
                        asset = _assetLocator.GetChrModel(model.Name);
                        chrsToLoad.Add(asset);
                        var tasset = _assetLocator.GetChrTextures(model.Name);
                        if (tasset.AssetVirtualPath != null || tasset.AssetArchiveVirtualPath != null)
                        {
                            chrsToLoad.Add(tasset);
                        }
                    }
                    else if (model.Name.StartsWith("o") || model.Name.StartsWith("AEG"))
                    {
                        asset = _assetLocator.GetObjModel(model.Name);
                        objsToLoad.Add(asset);
                    }
                    else if (model.Name.StartsWith("h"))
                    {
                        asset = _assetLocator.GetMapCollisionModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, model.Name), false);
                        colsToLoad.Add(asset);
                    }
                    else if (model.Name.StartsWith("n") && _assetLocator.Type != GameType.DarkSoulsIISOTFS && _assetLocator.Type != GameType.Bloodborne)
                    {
                        asset = _assetLocator.GetMapNVMModel(amapid, _assetLocator.MapModelNameToAssetName(amapid, model.Name));
                        navsToLoad.Add(asset);
                    }
                }

                foreach (var obj in map.Objects)
                {
                    if (obj.WrappedObject is IMsbPart mp && mp.ModelName != null && mp.ModelName != "" && obj.RenderSceneMesh == null)
                    {
                        GetModelDrawable(map, obj, mp.ModelName, false);
                    }
                }

                // Load BTLs (must be done after MapOffset is set)
                var BTLs = _assetLocator.GetMapBTLs(mapid);
                foreach (var btl_ad in BTLs)
                {
                    var btl = ReturnBTL(btl_ad);
                    if (btl != null)
                    {
                        map.LoadBTL(btl_ad, btl);
                    }
                }

                if (_assetLocator.Type == GameType.EldenRing && CFG.Current.EnableEldenRingAutoMapOffset)
                {
                    if (SpecialMapConnections.GetEldenMapTransform(mapid, LoadedObjectContainers) is Transform loadTransform)
                    {
                        map.RootObject.GetUpdateTransformAction(loadTransform).Execute();
                    }
                }

                if (!LoadedObjectContainers.ContainsKey(mapid))
                {
                    LoadedObjectContainers.Add(mapid, map);
                }
                else
                {
                    LoadedObjectContainers[mapid] = map;
                }
                // Intervene in the UI to change selection if requested.
                // We want to do this as soon as the RootObject is available, rather than at the end of all jobs.
                if (selectOnLoad)
                {
                    Selection.ClearSelection();
                    Selection.AddSelection(map.RootObject);
                }

                if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    LoadDS2Generators(amapid, map);
                }

                // Temporary DS3 navmesh loading
                if (FeatureFlags.LoadDS3Navmeshes && _assetLocator.Type == GameType.DarkSoulsIII)
                {
                    var nvaasset = _assetLocator.GetMapNVA(amapid);
                    if (nvaasset.AssetPath != null)
                    {
                        NVA nva = NVA.Read(nvaasset.AssetPath);
                        foreach (var nav in nva.Navmeshes)
                        {
                            // TODO2: set parent to MapOffset
                            var n = new MapEntity(map, nav, MapEntity.MapEntityType.Editor);
                            map.AddObject(n);
                            var navid = $@"n{nav.ModelID:D6}";
                            var navname = "n" + _assetLocator.MapModelNameToAssetName(amapid, navid).Substring(1);
                            var nasset = _assetLocator.GetHavokNavmeshModel(amapid, navname);

                            var mesh = MeshRenderableProxy.MeshRenderableFromHavokNavmeshResource(
                                _renderScene, nasset.AssetVirtualPath, ModelMarkerType.Other);
                            mesh.World = n.GetWorldMatrix();
                            mesh.SetSelectable(n);
                            mesh.DrawFilter = RenderFilter.Navmesh;
                            n.RenderSceneMesh = mesh;
                        }
                    }
                }

                var job = ResourceManager.CreateNewJob($@"Loading {amapid} geometry");
                foreach (var mappiece in mappiecesToLoad)
                {
                    if (mappiece.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(mappiece.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Flver);
                    }
                    else if (mappiece.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(mappiece.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                }
                task = job.Complete();
                tasks.Add(task);

                if (CFG.Current.EnableTexturing)
                {
                    job = ResourceManager.CreateNewJob($@"Loading {amapid} textures");
                    foreach (var asset in _assetLocator.GetMapTextures(amapid))
                    {
                        if (asset.AssetArchiveVirtualPath != null)
                        {
                            job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false);
                        }
                        else if (asset.AssetVirtualPath != null)
                        {
                            job.AddLoadFileTask(asset.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                        }
                    }
                    task = job.Complete();
                    tasks.Add(task);
                }

                job = ResourceManager.CreateNewJob($@"Loading {amapid} collisions");
                string archive = null;
                HashSet<string> colassets = new HashSet<string>();
                foreach (var col in colsToLoad)
                {
                    if (col.AssetArchiveVirtualPath != null)
                    {
                        //job.AddLoadArchiveTask(col.AssetArchiveVirtualPath, false);
                        archive = col.AssetArchiveVirtualPath;
                        colassets.Add(col.AssetVirtualPath);
                    }
                    else if (col.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(col.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                }
                if (archive != null)
                {
                    job.AddLoadArchiveTask(archive, AccessLevel.AccessGPUOptimizedOnly, false, colassets);
                }
                task = job.Complete();
                tasks.Add(task);

                job = ResourceManager.CreateNewJob($@"Loading chrs");
                foreach (var chr in chrsToLoad)
                {
                    if (chr.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Flver);
                    }
                    else if (chr.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(chr.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                }
                task = job.Complete();
                tasks.Add(task);

                job = ResourceManager.CreateNewJob($@"Loading objs");
                foreach (var obj in objsToLoad)
                {
                    if (obj.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(obj.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Flver);
                    }
                    else if (obj.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(obj.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                    }
                }
                task = job.Complete();
                tasks.Add(task);

                if (FeatureFlags.LoadNavmeshes)
                {
                    job = ResourceManager.CreateNewJob($@"Loading Navmeshes");
                    if (_assetLocator.Type == GameType.DarkSoulsIII && FeatureFlags.LoadDS3Navmeshes)
                    {
                        var nav = _assetLocator.GetHavokNavmeshes(amapid);
                        job.AddLoadArchiveTask(nav.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, ResourceManager.ResourceType.NavmeshHKX);
                    }
                    else
                    {
                        foreach (var nav in navsToLoad)
                        {
                            if (nav.AssetArchiveVirtualPath != null)
                            {
                                job.AddLoadArchiveTask(nav.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false);
                            }
                            else if (nav.AssetVirtualPath != null)
                            {
                                job.AddLoadFileTask(nav.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                            }
                        }
                    }
                    task = job.Complete();
                    tasks.Add(task);
                }

                // Real bad hack
                EnvMapTextures = _assetLocator.GetEnvMapTextureNames(amapid);

                if (_assetLocator.Type == GameType.DarkSoulsPTDE)
                {
                    ResourceManager.ScheduleUDSMFRefresh();
                }
                ResourceManager.ScheduleUnloadedTexturesRefresh();

                // After everything loads, do some additional checks:
                await Task.WhenAll(tasks);
                postLoad = true;

                // Update models (For checking meshes for Model Markers. & updates `CollisionName` field reference info)
                foreach (var obj in map.Objects)
                {
                    obj.UpdateRenderModel();
                }
                // Check for duplicate EntityIDs
                CheckDupeEntityIDs(map);

                return;
            }
            catch(Exception e)
            {
                // Store async exception so it can be caught by crash handler.
#if DEBUG
                throw;
#else
                LoadMapExceptions = e;
                return;
#endif
            }
        }

        public static void CheckDupeEntityIDs(Map map)
        {
            /* Notes about dupe Entity ID behavior in-game: 
             * Entity ID dupes exist in vanilla (including dupe regions)
             * Duplicate Entity IDs for regions causes all regions later in the map object list to not function properly (only confirmed for DS1).
             * Currently unknown if dupes cause issues outside of regions.
             * Unique behavior can be seen when using dupe IDs with objects, and all objects with the same ID can be affected by single commands.
             * * This behavior is probably unintentional and may secretly cause issues. Unknown.
             * 
             * At the moment, only region ID checking is necessary.
             */
            Dictionary<int, string> entityIDList = new();
            foreach (var obj in map.Objects)
            {
                var objType = obj.WrappedObject.GetType().ToString();
                if (objType.Contains("Region"))
                {
                    var entityIDProp = obj.GetProperty("EntityID");
                    if (entityIDProp != null)
                    {
                        object idObj = entityIDProp.GetValue(obj.WrappedObject);
                        if (idObj is not int entityID)
                        {
                            // EntityID is uint in Elden Ring. Only <2^31 is used in practice.
                            // If really desired, a separate routine could be created.
                            if (idObj is uint uID)
                            {
                                entityID = unchecked((int)uID);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (entityID > 0)
                        {
                            var entryExists = entityIDList.TryGetValue(entityID, out string name);
                            if (entryExists)
                            {
                                var key = $"{obj.Name} Dupe EntityID";
                                var value = $"Duplicate EntityID: `{entityID}` is being used by multiple regions; `{obj.PrettyName}` and `{name}`";
                                TaskManager.warningList.TryAdd(key, value);
                            }
                            else
                            {
                                entityIDList.Add(entityID, obj.PrettyName);
                            }
                        }
                    }
                }
            }
        }

        public void LoadFlver(FLVER2 flver, MeshRenderableProxy proxy, string name)
        {
            var container = new ObjectContainer(this, name);

            container.LoadFlver(flver, proxy);

            if (!LoadedObjectContainers.ContainsKey(name))
            {
                LoadedObjectContainers.Add(name, container);
            }
            else
            {
                LoadedObjectContainers[name] = container;
            }
        }

        private void SaveDS2Generators(Map map)
        {
            // Load all the params
            var regparamad = _assetLocator.GetDS2GeneratorRegistParam(map.Name);
            var regparamadw = _assetLocator.GetDS2GeneratorRegistParam(map.Name, true);
            var regparam = Param.Read(regparamad.AssetPath);
            var reglayout = _assetLocator.GetParamdefForParam(regparam.ParamType);
            regparam.ApplyParamdef(reglayout);

            var locparamad = _assetLocator.GetDS2GeneratorLocationParam(map.Name);
            var locparamadw = _assetLocator.GetDS2GeneratorLocationParam(map.Name, true);
            var locparam = Param.Read(locparamad.AssetPath);
            var loclayout = _assetLocator.GetParamdefForParam(locparam.ParamType);
            locparam.ApplyParamdef(loclayout);

            var genparamad = _assetLocator.GetDS2GeneratorParam(map.Name);
            var genparamadw = _assetLocator.GetDS2GeneratorParam(map.Name, true);
            var genparam = Param.Read(genparamad.AssetPath);
            var genlayout = _assetLocator.GetParamdefForParam(genparam.ParamType);
            genparam.ApplyParamdef(genlayout);

            var evtparamad = _assetLocator.GetDS2EventParam(map.Name);
            var evtparamadw = _assetLocator.GetDS2EventParam(map.Name, true);
            var evtparam = Param.Read(evtparamad.AssetPath);
            var evtlayout = _assetLocator.GetParamdefForParam(evtparam.ParamType);
            evtparam.ApplyParamdef(evtlayout);

            var evtlparamad = _assetLocator.GetDS2EventLocationParam(map.Name);
            var evtlparamadw = _assetLocator.GetDS2EventLocationParam(map.Name, true);
            var evtlparam = Param.Read(evtlparamad.AssetPath);
            var evtllayout = _assetLocator.GetParamdefForParam(evtlparam.ParamType);
            evtlparam.ApplyParamdef(evtllayout);

            var objparamad = _assetLocator.GetDS2ObjInstanceParam(map.Name);
            var objparamadw = _assetLocator.GetDS2ObjInstanceParam(map.Name, true);
            var objparam = Param.Read(objparamad.AssetPath);
            var objlayout = _assetLocator.GetParamdefForParam(objparam.ParamType);
            objparam.ApplyParamdef(objlayout);

            // Clear them out
            regparam.ClearRows();
            locparam.ClearRows();
            genparam.ClearRows();
            evtparam.ClearRows();
            evtlparam.ClearRows();
            objparam.ClearRows();

            // Serialize objects
            if (!map.SerializeDS2Generators(locparam, genparam))
            {
                return;
            }
            if (!map.SerializeDS2Regist(regparam))
            {
                return;
            }
            if (!map.SerializeDS2Events(evtparam))
            {
                return;
            }
            if (!map.SerializeDS2EventLocations(evtlparam))
            {
                return;
            }
            if (!map.SerializeDS2ObjInstances(objparam))
            {
                return;
            }

            // Create a param directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(regparamadw.AssetPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(regparamadw.AssetPath));
            }

            // Save all the params
            if (File.Exists(regparamadw.AssetPath + ".temp"))
            {
                File.Delete(regparamadw.AssetPath + ".temp");
            }
            regparam.Write(regparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(regparamadw.AssetPath))
            {
                if (!File.Exists(regparamadw.AssetPath + ".bak"))
                {
                    File.Copy(regparamadw.AssetPath, regparamadw.AssetPath + ".bak", true);
                }
                File.Copy(regparamadw.AssetPath, regparamadw.AssetPath + ".prev", true);
                File.Delete(regparamadw.AssetPath);
            }
            File.Move(regparamadw.AssetPath + ".temp", regparamadw.AssetPath);

            if (File.Exists(locparamadw.AssetPath + ".temp"))
            {
                File.Delete(locparamadw.AssetPath + ".temp");
            }
            locparam.Write(locparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(locparamadw.AssetPath))
            {
                if (!File.Exists(locparamadw.AssetPath + ".bak"))
                {
                    File.Copy(locparamadw.AssetPath, locparamadw.AssetPath + ".bak", true);
                }
                File.Copy(locparamadw.AssetPath, locparamadw.AssetPath + ".prev", true);
                File.Delete(locparamadw.AssetPath);
            }
            File.Move(locparamadw.AssetPath + ".temp", locparamadw.AssetPath);

            if (File.Exists(genparamadw.AssetPath + ".temp"))
            {
                File.Delete(genparamadw.AssetPath + ".temp");
            }
            genparam.Write(genparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(genparamadw.AssetPath))
            {
                if (!File.Exists(genparamadw.AssetPath + ".bak"))
                {
                    File.Copy(genparamadw.AssetPath, genparamadw.AssetPath + ".bak", true);
                }
                File.Copy(genparamadw.AssetPath, genparamadw.AssetPath + ".prev", true);
                File.Delete(genparamadw.AssetPath);
            }
            File.Move(genparamadw.AssetPath + ".temp", genparamadw.AssetPath);

            // Events
            if (File.Exists(evtparamadw.AssetPath + ".temp"))
            {
                File.Delete(evtparamadw.AssetPath + ".temp");
            }
            evtparam.Write(evtparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(evtparamadw.AssetPath))
            {
                if (!File.Exists(evtparamadw.AssetPath + ".bak"))
                {
                    File.Copy(evtparamadw.AssetPath, evtparamadw.AssetPath + ".bak", true);
                }
                File.Copy(evtparamadw.AssetPath, evtparamadw.AssetPath + ".prev", true);
                File.Delete(evtparamadw.AssetPath);
            }
            File.Move(evtparamadw.AssetPath + ".temp", evtparamadw.AssetPath);

            // Event regions
            if (File.Exists(evtlparamadw.AssetPath + ".temp"))
            {
                File.Delete(evtlparamadw.AssetPath + ".temp");
            }
            evtlparam.Write(evtlparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(evtlparamadw.AssetPath))
            {
                if (!File.Exists(evtlparamadw.AssetPath + ".bak"))
                {
                    File.Copy(evtlparamadw.AssetPath, evtlparamadw.AssetPath + ".bak", true);
                }
                File.Copy(evtlparamadw.AssetPath, evtlparamadw.AssetPath + ".prev", true);
                File.Delete(evtlparamadw.AssetPath);
            }
            File.Move(evtlparamadw.AssetPath + ".temp", evtlparamadw.AssetPath);

            // Object instances
            if (File.Exists(objparamadw.AssetPath + ".temp"))
            {
                File.Delete(objparamadw.AssetPath + ".temp");
            }
            objparam.Write(objparamadw.AssetPath + ".temp", SoulsFormats.DCX.Type.None);
            if (File.Exists(objparamadw.AssetPath))
            {
                if (!File.Exists(objparamadw.AssetPath + ".bak"))
                {
                    File.Copy(objparamadw.AssetPath, objparamadw.AssetPath + ".bak", true);
                }
                File.Copy(objparamadw.AssetPath, objparamadw.AssetPath + ".prev", true);
                File.Delete(objparamadw.AssetPath);
            }
            File.Move(objparamadw.AssetPath + ".temp", objparamadw.AssetPath);
        }

        private DCX.Type GetCompressionType()
        {
            if (_assetLocator.Type == GameType.DarkSoulsIII)
            {
                return DCX.Type.DCX_DFLT_10000_44_9;
            }
            else if (_assetLocator.Type == GameType.EldenRing)
            {
                return DCX.Type.DCX_DFLT_10000_44_9;
            }
            else if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                return DCX.Type.None;
            }
            else if (_assetLocator.Type == GameType.Sekiro)
            {
                return DCX.Type.DCX_DFLT_10000_44_9;
            }
            else if (_assetLocator.Type == GameType.Bloodborne)
            {
                return DCX.Type.DCX_DFLT_10000_44_9;
            }
            else if (_assetLocator.Type == GameType.DemonsSouls)
            {
                return DCX.Type.None;
            }

            return DCX.Type.None;
        }

        /// <summary>
        /// Save BTL light data
        /// </summary>
        public void SaveBTL(Map map)
        {
            var BTLs = _assetLocator.GetMapBTLs(map.Name);
            var BTLs_w = _assetLocator.GetMapBTLs(map.Name, true);
            DCX.Type compressionType = GetCompressionType();
            if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                for (var i = 0; i < BTLs.Count; i++)
                {
                    var bdt = BXF4.Read(BTLs[i].AssetPath, BTLs[i].AssetPath[..^3] + "bdt");
                    var file = bdt.Files.Find(f => f.Name.EndsWith("light.btl.dcx"));
                    var btl = BTL.Read(file.Bytes);
                    if (btl != null)
                    {
                        var newLights = map.SerializeBtlLights(BTLs_w[i].AssetName);

                        // Only save BTL if it has been modified
                        if (JsonConvert.SerializeObject(btl.Lights) != JsonConvert.SerializeObject(newLights))
                        {
                            btl.Lights = newLights;
                            file.Bytes = btl.Write(DCX.Type.DCX_DFLT_10000_24_9);
                            try
                            {
                                var bdtPath = BTLs_w[i].AssetPath[..^3] + "bdt";

                                if (!File.Exists(BTLs_w[i].AssetPath + ".bak") && File.Exists(BTLs_w[i].AssetPath))
                                    File.Copy(BTLs_w[i].AssetPath, BTLs_w[i].AssetPath + ".bak", true);
                                if (!File.Exists(bdtPath + ".bak") && File.Exists(bdtPath))
                                    File.Copy(bdtPath, bdtPath + ".bak", true);

                                bdt.Write(BTLs_w[i].AssetPath, bdtPath);
                            }
                            catch (Exception e)
                            {
                                throw new SavingFailedException(Path.GetFileName(map.Name), e);
                            }
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < BTLs.Count; i++)
                {
                    var btl = ReturnBTL(BTLs[i]);
                    if (btl != null)
                    {
                        var newLights = map.SerializeBtlLights(BTLs_w[i].AssetName);

                        // Only save BTL if it has been modified
                        if (JsonConvert.SerializeObject(btl.Lights) != JsonConvert.SerializeObject(newLights))
                        {
                            btl.Lights = newLights;
                            try
                            {
                                if (!File.Exists(BTLs_w[i].AssetPath + ".bak") && File.Exists(BTLs_w[i].AssetPath))
                                    File.Copy(BTLs_w[i].AssetPath, BTLs_w[i].AssetPath + ".bak", true);
                                btl.Write(BTLs_w[i].AssetPath, compressionType);
                            }
                            catch (Exception e)
                            {
                                throw new SavingFailedException(Path.GetFileName(map.Name), e);
                            }
                        }
                    }
                }
            }
        }

        public void SaveMap(Map map)
        {
            SaveBTL(map);
            try
            {
                var ad = _assetLocator.GetMapMSB(map.Name);
                var adw = _assetLocator.GetMapMSB(map.Name, true);
                IMsb msb;
                DCX.Type compressionType = GetCompressionType();
                if (_assetLocator.Type == GameType.DarkSoulsIII)
                {
                    MSB3 prev = MSB3.Read(ad.AssetPath);
                    MSB3 n = new MSB3();
                    n.PartsPoses = prev.PartsPoses;
                    n.Layers = prev.Layers;
                    n.Routes = prev.Routes;
                    msb = n;
                }
                else if (_assetLocator.Type == GameType.EldenRing)
                {
                    MSBE prev = MSBE.Read(ad.AssetPath);
                    MSBE n = new MSBE();
                    n.Layers = prev.Layers;
                    n.Routes = prev.Routes;
                    msb = n;
                }
                else if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    MSB2 prev = MSB2.Read(ad.AssetPath);
                    MSB2 n = new MSB2();
                    n.PartPoses = prev.PartPoses;
                    msb = n;
                }
                else if (_assetLocator.Type == GameType.Sekiro)
                {
                    MSBS prev = MSBS.Read(ad.AssetPath);
                    MSBS n = new MSBS();
                    n.PartsPoses = prev.PartsPoses;
                    n.Layers = prev.Layers;
                    n.Routes = prev.Routes;
                    msb = n;
                }
                else if (_assetLocator.Type == GameType.Bloodborne)
                {
                    msb = new MSBB();
                }
                else if (_assetLocator.Type == GameType.DemonsSouls)
                {
                    MSBD prev = MSBD.Read(ad.AssetPath);
                    MSBD n = new MSBD();
                    n.Trees = prev.Trees;
                    msb = n;
                }
                else
                {
                    msb = new MSB1();
                    //var t = MSB1.Read(ad.AssetPath);
                    //((MSB1)msb).Models = t.Models;
                }

                map.SerializeToMSB(msb, _assetLocator.Type);

                // Create the map directory if it doesn't exist
                if (!Directory.Exists(Path.GetDirectoryName(adw.AssetPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(adw.AssetPath));
                }

                // Write as a temporary file to make sure there are no errors before overwriting current file 
                string mapPath = adw.AssetPath;
                //if (GetModProjectPathForFile(mapPath) != null)
                //{
                //    mapPath = GetModProjectPathForFile(mapPath);
                //}

                // If a backup file doesn't exist of the original file create it
                if (!File.Exists(mapPath + ".bak") && File.Exists(mapPath))
                {
                    File.Copy(mapPath, mapPath + ".bak", true);
                }

                if (File.Exists(mapPath + ".temp"))
                {
                    File.Delete(mapPath + ".temp");
                }
                
                msb.Write(mapPath + ".temp", compressionType);

                // Make a copy of the previous map
                if (File.Exists(mapPath))
                {
                    File.Copy(mapPath, mapPath + ".prev", true);
                }

                // Move temp file as new map file
                if (File.Exists(mapPath))
                {
                    File.Delete(mapPath);
                }

                File.Move(mapPath + ".temp", mapPath);

                if (_assetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    SaveDS2Generators(map);
                }

                CheckDupeEntityIDs(map);
                map.HasUnsavedChanges = false;
            }
            catch (Exception e)
            {
                throw new SavingFailedException(Path.GetFileName(map.Name), e);
            }
            //var json = JsonConvert.SerializeObject(map.SerializeHierarchy());
            //Utils.WriteStringWithBackup(_assetLocator.GameRootDirectory, _assetLocator.GameModDirectory,
            //    $@"{Path.GetFileNameWithoutExtension(mapPath)}.json", json);
        }

        public void SaveAllMaps()
        {
            foreach (var m in LoadedObjectContainers)
            {
                if (m.Value != null)
                {
                    if (m.Value is Map ma)
                    {
                        SaveMap(ma);
                    }
                }
            }
        }

        public void UnloadMap(Map map)
        {
            if (LoadedObjectContainers.ContainsKey(map.Name))
            {
                foreach (var obj in map.Objects)
                {
                    if (obj != null)
                    {
                        obj.Dispose();
                    }
                }
                map.Clear();
                LoadedObjectContainers[map.Name] = null;
            }
        }

        public void UnloadAllMaps()
        {
            List<ObjectContainer> toUnload = new List<ObjectContainer>();
            foreach (var key in LoadedObjectContainers.Keys)
            {
                if (LoadedObjectContainers[key] != null)
                {
                    toUnload.Add(LoadedObjectContainers[key]);
                }
            }
            foreach (var un in toUnload)
            {
                if (un is Map ma)
                {
                    UnloadMap(ma);
                }
            }
        }

        public void LoadFlver(string name, FLVER2 flver)
        {
            ObjectContainer c = new ObjectContainer(this, name);
        }

        public Type GetPropertyType(string name)
        {
            // TODO: needs to scan within structs too 
            foreach (var m in LoadedObjectContainers)
            {
                if (m.Value == null)
                {
                    continue;
                }
                foreach (var o in m.Value.Objects)
                {
                    var p = o.GetProperty(name);
                    if (p != null)
                    {
                        return p.PropertyType;
                    }
                }
            }
            return null;
        }
    }
}
