using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using StudioCore.Resource;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// A universe is a collection of loaded maps with methods to load, serialize,
    /// and unload individual maps.
    /// </summary>
    public class Universe
    {
        public Dictionary<string, Map> LoadedMaps { get; private set; } = new Dictionary<string, Map>();
        private AssetLocator AssetLocator;
        private Scene.RenderScene RenderScene;

        public Universe(AssetLocator al, Scene.RenderScene scene)
        {
            AssetLocator = al;
            RenderScene = scene;
        }

        public Map GetLoadedMap(string id)
        {
            if (id != null)
            {
                if (LoadedMaps.ContainsKey(id))
                {
                    return LoadedMaps[id];
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a drawable for a model and registers it with the scene. Will load
        /// the required assets in the background if they aren't already loaded.
        /// </summary>
        /// <param name="modelname"></param>
        /// <returns></returns>
        public Scene.IDrawable GetModelDrawable(Map map, MapObject obj, string modelname)
        {
            AssetDescription asset;
            bool loadcol = false;
            bool loadnav = false;
            Scene.RenderFilter filt = Scene.RenderFilter.All;
            var job = ResourceManager.CreateNewJob($@"Loading mesh");
            if (modelname.StartsWith("m"))
            {
                asset = AssetLocator.GetMapModel(map.MapId, AssetLocator.MapModelNameToAssetName(map.MapId, modelname));
                filt = Scene.RenderFilter.MapPiece;
            }
            else if (modelname.StartsWith("c"))
            {
                asset = AssetLocator.GetChrModel(modelname);
                filt = Scene.RenderFilter.Character;
            }
            else if (modelname.StartsWith("o"))
            {
                asset = AssetLocator.GetObjModel(modelname);
                filt = Scene.RenderFilter.Object;
            }
            else if (modelname.StartsWith("h"))
            {
                loadcol = true;
                asset = AssetLocator.GetMapCollisionModel(map.MapId, AssetLocator.MapModelNameToAssetName(map.MapId, modelname));
                filt = Scene.RenderFilter.Collision;
            }
            else if (modelname.StartsWith("n"))
            {
                loadnav = true;
                asset = AssetLocator.GetMapNVMModel(map.MapId, AssetLocator.MapModelNameToAssetName(map.MapId, modelname));
                filt = Scene.RenderFilter.Navmesh;
            }
            else
            {
                asset = AssetLocator.GetNullAsset();
            }

            if (loadcol)
            {
                var res = ResourceManager.GetResource<Resource.HavokCollisionResource>(asset.AssetVirtualPath);
                var mesh = new Scene.CollisionMesh(RenderScene, res, AssetLocator.Type == GameType.DarkSoulsIISOTFS);
                mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                obj.RenderSceneMesh = mesh;
                mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                if (!res.IsLoaded)
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, false);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath);
                    }
                    job.StartJobAsync();
                }
                return mesh;
            }
            else if (loadnav && AssetLocator.Type != GameType.DarkSoulsIISOTFS)
            {
                var res = ResourceManager.GetResource<Resource.NVMNavmeshResource>(asset.AssetVirtualPath);
                var mesh = new Scene.NvmMesh(RenderScene, res, false);
                mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                obj.RenderSceneMesh = mesh;
                mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                if (!res.IsLoaded)
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, false);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath);
                    }
                    job.StartJobAsync();
                }
                return mesh;
            }
            else if (loadnav && AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {

            }
            else
            {
                var res = ResourceManager.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
                var model = new NewMesh(RenderScene, res, false);
                model.DrawFilter = filt;
                model.WorldMatrix = obj.GetTransform().WorldMatrix;
                obj.RenderSceneMesh = model;
                model.Selectable = new WeakReference<Scene.ISelectable>(obj);
                if (!res.IsLoaded)
                {
                    if (asset.AssetArchiveVirtualPath != null)
                    {
                        job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, false, Resource.ResourceManager.ResourceType.Flver);
                    }
                    else if (asset.AssetVirtualPath != null)
                    {
                        job.AddLoadFileTask(asset.AssetVirtualPath);
                    }
                    job.StartJobAsync();
                }
                return model;
            }
            return null;
        }

        public void LoadDS2Generators(string mapid, Map map)
        {
            Dictionary<long, PARAM.Row> registParams = new Dictionary<long, PARAM.Row>();
            Dictionary<long, MergedParamRow> generatorParams = new Dictionary<long, MergedParamRow>();
            Dictionary<long, MapObject> generatorObjs = new Dictionary<long, MapObject>();
            Dictionary<long, PARAM.Row> eventParams = new Dictionary<long, PARAM.Row>();
            Dictionary<long, PARAM.Row> eventLocationParams = new Dictionary<long, PARAM.Row>();

            var regparamad = AssetLocator.GetDS2GeneratorRegistParam(mapid);
            var regparam = PARAM.Read(regparamad.AssetPath);
            var reglayout = AssetLocator.GetParamdefForParam(regparam.ParamType);
            regparam.ApplyParamdef(reglayout);
            foreach (var row in regparam.Rows)
            {
                if (row.Name == null || row.Name == "")
                {
                    row.Name = "regist_" + row.ID.ToString();
                }
                registParams.Add(row.ID, row);

                var obj = new MapObject(map, row, MapObject.ObjectType.DS2GeneratorRegist);
                map.AddObject(obj);
            }

            var locparamad = AssetLocator.GetDS2GeneratorLocationParam(mapid);
            var locparam = PARAM.Read(locparamad.AssetPath);
            var loclayout = AssetLocator.GetParamdefForParam(locparam.ParamType);
            locparam.ApplyParamdef(loclayout);
            foreach (var row in locparam.Rows)
            {
                if (row.Name == null || row.Name == "")
                {
                    row.Name = "generator_" + row.ID.ToString();
                }

                // Offset the generators by the map offset
                row["PositionX"].Value = (float)row["PositionX"].Value + map.MapOffset.Position.X;
                row["PositionY"].Value = (float)row["PositionY"].Value + map.MapOffset.Position.Y;
                row["PositionZ"].Value = (float)row["PositionZ"].Value + map.MapOffset.Position.Z;

                var mergedRow = new MergedParamRow();
                mergedRow.AddRow("generator-loc", row);
                generatorParams.Add(row.ID, mergedRow);

                var obj = new MapObject(map, mergedRow, MapObject.ObjectType.DS2Generator);
                generatorObjs.Add(row.ID, obj);
                map.AddObject(obj);
            }

            var chrsToLoad = new HashSet<AssetDescription>();
            var genparamad = AssetLocator.GetDS2GeneratorParam(mapid);
            var genparam = PARAM.Read(genparamad.AssetPath);
            var genlayout = AssetLocator.GetParamdefForParam(genparam.ParamType);
            genparam.ApplyParamdef(genlayout);
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
                    var obj = new MapObject(map, mergedRow, MapObject.ObjectType.DS2Generator);
                    generatorObjs.Add(row.ID, obj);
                    map.AddObject(obj);
                }

                uint registid = (uint)row["GeneratorRegistParam"].Value;
                if (registParams.ContainsKey(registid))
                {
                    var regist = registParams[registid];
                    var chrid = ParamBank.GetChrIDForEnemy((uint)regist["EnemyParamID"].Value);
                    if (chrid != null)
                    {
                        var asset = AssetLocator.GetChrModel($@"c{chrid}");
                        var res = ResourceManager.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
                        var model = new NewMesh(RenderScene, res, false);
                        model.DrawFilter = Scene.RenderFilter.Character;
                        generatorObjs[row.ID].RenderSceneMesh = model;
                        model.Selectable = new WeakReference<Scene.ISelectable>(generatorObjs[row.ID]);
                        chrsToLoad.Add(asset);
                    }
                }
            }

            var evtparamad = AssetLocator.GetDS2EventParam(mapid);
            var evtparam = PARAM.Read(evtparamad.AssetPath);
            var evtlayout = AssetLocator.GetParamdefForParam(evtparam.ParamType);
            evtparam.ApplyParamdef(evtlayout);
            foreach (var row in evtparam.Rows)
            {
                if (row.Name == null || row.Name == "")
                {
                    row.Name = "event_" + row.ID.ToString();
                }
                eventParams.Add(row.ID, row);

                var obj = new MapObject(map, row, MapObject.ObjectType.DS2Event);
                map.AddObject(obj);
            }

            var evtlparamad = AssetLocator.GetDS2EventLocationParam(mapid);
            var evtlparam = PARAM.Read(evtlparamad.AssetPath);
            var evtllayout = AssetLocator.GetParamdefForParam(evtlparam.ParamType);
            evtlparam.ApplyParamdef(evtllayout);
            foreach (var row in evtlparam.Rows)
            {
                if (row.Name == null || row.Name == "")
                {
                    row.Name = "eventloc_" + row.ID.ToString();
                }
                eventLocationParams.Add(row.ID, row);

                // Offset the generators by the map offset
                row["PositionX"].Value = (float)row["PositionX"].Value + map.MapOffset.Position.X;
                row["PositionY"].Value = (float)row["PositionY"].Value + map.MapOffset.Position.Y;
                row["PositionZ"].Value = (float)row["PositionZ"].Value + map.MapOffset.Position.Z;

                var obj = new MapObject(map, row, MapObject.ObjectType.DS2EventLocation);
                map.AddObject(obj);

                // Try rendering as a box for now
                var mesh = Scene.Region.GetBoxRegion(RenderScene);
                mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                obj.RenderSceneMesh = mesh;
                mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
            }

            var job = ResourceManager.CreateNewJob($@"Loading chrs");
            foreach (var chr in chrsToLoad)
            {
                if (chr.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (chr.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(chr.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
        }

        public void PopulateMapList()
        {
            LoadedMaps.Clear();
            foreach (var m in AssetLocator.GetFullMapList())
            {
                LoadedMaps.Add(m, null);
            }
        }

        public bool LoadMap(string mapid)
        {
            var map = new Map(this, mapid);

            var mappiecesToLoad = new HashSet<AssetDescription>();
            var chrsToLoad = new HashSet<AssetDescription>();
            var objsToLoad = new HashSet<AssetDescription>();
            var colsToLoad = new HashSet<AssetDescription>();
            var navsToLoad = new HashSet<AssetDescription>();

            var ad = AssetLocator.GetMapMSB(mapid);
            if (ad.AssetPath == null)
            {
                return false;
            }
            IMsb msb;
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                msb = MSB3.Read(ad.AssetPath);
            }
            else if (AssetLocator.Type == GameType.Sekiro)
            {
                msb = MSBS.Read(ad.AssetPath);
            }
            else if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                msb = MSB2.Read(ad.AssetPath);
            }
            else if (AssetLocator.Type == GameType.Bloodborne)
            {
                msb = MSBB.Read(ad.AssetPath);
            }
            else
            {
                msb = MSB1.Read(ad.AssetPath);
            }
            map.LoadMSB(msb);

            var amapid = mapid.Substring(0, 6) + "_00_00";
            // Special case for chalice dungeon assets
            if (mapid.StartsWith("m29"))
            {
                amapid = "m29_00_00_00";
            }

            // Temporary garbage
            foreach (var obj in map.MapObjects)
            {
                if (obj.MsbObject is IMsbPart mp && mp.ModelName != null && mp.ModelName != "")
                {
                    AssetDescription asset;
                    bool loadcol = false;
                    bool loadnav = false;
                    bool usedrawgroups = false;
                    Scene.RenderFilter filt = Scene.RenderFilter.All;
                    if (mp.ModelName.StartsWith("m"))
                    {
                        asset = AssetLocator.GetMapModel(amapid, AssetLocator.MapModelNameToAssetName(amapid, mp.ModelName));
                        filt = Scene.RenderFilter.MapPiece;
                        obj.UseDrawGroups = true;
                        mappiecesToLoad.Add(asset);
                    }
                    else if (mp.ModelName.StartsWith("c"))
                    {
                        asset = AssetLocator.GetChrModel(mp.ModelName);
                        filt = Scene.RenderFilter.Character;
                        chrsToLoad.Add(asset);
                    }
                    else if (mp.ModelName.StartsWith("o"))
                    {
                        asset = AssetLocator.GetObjModel(mp.ModelName);
                        filt = Scene.RenderFilter.Object;
                        objsToLoad.Add(asset);
                    }
                    else if (mp.ModelName.StartsWith("h"))
                    {
                        loadcol = true;
                        asset = AssetLocator.GetMapCollisionModel(amapid, AssetLocator.MapModelNameToAssetName(amapid, mp.ModelName), false);
                        filt = Scene.RenderFilter.Collision;
                        colsToLoad.Add(asset);
                    }
                    else if (mp.ModelName.StartsWith("n"))
                    {
                        loadnav = true;
                        asset = AssetLocator.GetMapNVMModel(amapid, AssetLocator.MapModelNameToAssetName(amapid, mp.ModelName));
                        filt = Scene.RenderFilter.Navmesh;
                        navsToLoad.Add(asset);
                    }
                    else
                    {
                        asset = AssetLocator.GetNullAsset();
                    }

                    if (loadcol)
                    {
                        var res = ResourceManager.GetResource<Resource.HavokCollisionResource>(asset.AssetVirtualPath);
                        var mesh = new Scene.CollisionMesh(RenderScene, res, AssetLocator.Type == GameType.DarkSoulsIISOTFS
                                                                || AssetLocator.Type == GameType.DarkSoulsIII
                                                                || AssetLocator.Type == GameType.Bloodborne);
                        mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = mesh;
                        mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                    else if (loadnav && AssetLocator.Type != GameType.DarkSoulsIISOTFS && AssetLocator.Type != GameType.Bloodborne)
                    {
                        var res = ResourceManager.GetResource<Resource.NVMNavmeshResource>(asset.AssetVirtualPath);
                        var mesh = new Scene.NvmMesh(RenderScene, res, false);
                        mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = mesh;
                        mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                    else if (loadnav && (AssetLocator.Type == GameType.DarkSoulsIISOTFS || AssetLocator.Type == GameType.Bloodborne))
                    {

                    }
                    else
                    {
                        var res = ResourceManager.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
                        var model = new NewMesh(RenderScene, res, false);
                        model.DrawFilter = filt;
                        model.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = model;
                        model.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                }
                if (obj.MsbObject is IMsbRegion r && r.Shape is MSB.Shape.Box b)
                {
                    var mesh = Scene.Region.GetBoxRegion(RenderScene);
                    mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                    obj.RenderSceneMesh = mesh;
                    mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                }
                else if (obj.MsbObject is IMsbRegion r2 && r2.Shape is MSB.Shape.Sphere s)
                {
                    var mesh = Scene.Region.GetSphereRegion(RenderScene);
                    mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                    obj.RenderSceneMesh = mesh;
                    mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                }
                else if (obj.MsbObject is IMsbRegion r3 && r3.Shape is MSB.Shape.Point p)
                {
                    var mesh = Scene.Region.GetPointRegion(RenderScene);
                    mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                    obj.RenderSceneMesh = mesh;
                    mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                }
                else if (obj.MsbObject is IMsbRegion r4 && r4.Shape is MSB.Shape.Cylinder c)
                {
                    var mesh = Scene.Region.GetCylinderRegion(RenderScene);
                    mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                    obj.RenderSceneMesh = mesh;
                    mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                }

                // Try to find the map offset
                if (obj.MsbObject is MSB2.Event.MapOffset mo)
                {
                    var t = Transform.Default;
                    t.Position = mo.Translation;
                    map.MapOffset = t;
                }
            }
            if (!LoadedMaps.ContainsKey(mapid))
            {
                LoadedMaps.Add(mapid, map);
            }
            else
            {
                LoadedMaps[mapid] = map;
            }

            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                LoadDS2Generators(amapid, map);
            }

            var job = ResourceManager.CreateNewJob($@"Loading {amapid} geometry");
            foreach (var mappiece in mappiecesToLoad)
            {
                if (mappiece.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(mappiece.AssetArchiveVirtualPath, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (mappiece.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(mappiece.AssetVirtualPath);
                }
            } 
            job.StartJobAsync();

            job = ResourceManager.CreateNewJob($@"Loading {amapid} textures");
            foreach (var asset in AssetLocator.GetMapTextures(amapid))
            {
                if (asset.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, false);
                }
                else if (asset.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(asset.AssetVirtualPath);
                }
            }
            job.StartJobAsync();

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
                    job.AddLoadFileTask(col.AssetVirtualPath);
                }
            }
            if (archive != null)
            {
                job.AddLoadArchiveTask(archive, false, colassets);
            }
            job.StartJobAsync();
            job = ResourceManager.CreateNewJob($@"Loading chrs");
            foreach (var chr in chrsToLoad)
            {
                if (chr.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (chr.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(chr.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
            job = ResourceManager.CreateNewJob($@"Loading objs");
            foreach (var obj in objsToLoad)
            {
                if (obj.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(obj.AssetArchiveVirtualPath, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (obj.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(obj.AssetVirtualPath);
                }
            }
            job.StartJobAsync();

            job = ResourceManager.CreateNewJob($@"Loading Navmeshes");
            foreach (var nav in navsToLoad)
            {
                if (nav.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(nav.AssetArchiveVirtualPath, false);
                }
                else if (nav.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(nav.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
            return true;
        }

        private void SaveDS2Generators(Map map)
        {
            // Load all the params
            var regparamad = AssetLocator.GetDS2GeneratorRegistParam(map.MapId);
            var regparamadw = AssetLocator.GetDS2GeneratorRegistParam(map.MapId, true);
            var regparam = PARAM.Read(regparamad.AssetPath);
            var reglayout = AssetLocator.GetParamdefForParam(regparam.ParamType);
            regparam.ApplyParamdef(reglayout);

            var locparamad = AssetLocator.GetDS2GeneratorLocationParam(map.MapId);
            var locparamadw = AssetLocator.GetDS2GeneratorLocationParam(map.MapId, true);
            var locparam = PARAM.Read(locparamad.AssetPath);
            var loclayout = AssetLocator.GetParamdefForParam(locparam.ParamType);
            locparam.ApplyParamdef(loclayout);

            var genparamad = AssetLocator.GetDS2GeneratorParam(map.MapId);
            var genparamadw = AssetLocator.GetDS2GeneratorParam(map.MapId, true);
            var genparam = PARAM.Read(genparamad.AssetPath);
            var genlayout = AssetLocator.GetParamdefForParam(genparam.ParamType);
            genparam.ApplyParamdef(genlayout);

            var evtparamad = AssetLocator.GetDS2EventParam(map.MapId);
            var evtparamadw = AssetLocator.GetDS2EventParam(map.MapId, true);
            var evtparam = PARAM.Read(evtparamad.AssetPath);
            var evtlayout = AssetLocator.GetParamdefForParam(evtparam.ParamType);
            evtparam.ApplyParamdef(evtlayout);

            var evtlparamad = AssetLocator.GetDS2EventLocationParam(map.MapId);
            var evtlparamadw = AssetLocator.GetDS2EventLocationParam(map.MapId, true);
            var evtlparam = PARAM.Read(evtlparamad.AssetPath);
            var evtllayout = AssetLocator.GetParamdefForParam(evtlparam.ParamType);
            evtlparam.ApplyParamdef(evtllayout);

            // Clear them out
            regparam.Rows.Clear();
            locparam.Rows.Clear();
            genparam.Rows.Clear();
            evtparam.Rows.Clear();
            evtlparam.Rows.Clear();

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
        }

        public void SaveMap(Map map)
        {
            var ad = AssetLocator.GetMapMSB(map.MapId);
            var adw = AssetLocator.GetMapMSB(map.MapId, AssetLocator.Type == GameType.DarkSoulsPTDE ? false : true);
            IMsb msb;
            DCX.Type compressionType = DCX.Type.None;
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                MSB3 prev = MSB3.Read(ad.AssetPath);
                MSB3 n = new MSB3();
                n.PartsPoses = prev.PartsPoses;
                n.BoneNames = prev.BoneNames;
                n.Layers = prev.Layers;
                n.Routes = prev.Routes;
                msb = n;
                compressionType = DCX.Type.DCX_DFLT_10000_44_9;
            }
            else if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                MSB2 prev = MSB2.Read(ad.AssetPath);
                MSB2 n = new MSB2();
                n.PartPoses = prev.PartPoses;
                msb = n;
            }
            else if (AssetLocator.Type == GameType.Sekiro)
            {
                msb = new MSBS();
            }
            else if (AssetLocator.Type == GameType.Bloodborne)
            {
                msb = new MSBB();
                compressionType = DCX.Type.DCX_DFLT_10000_44_9;
            }
            else
            {
                msb = new MSB1();
                //var t = MSB1.Read(ad.AssetPath);
                //((MSB1)msb).Models = t.Models;
            }

            map.SerializeToMSB(msb, AssetLocator.Type);

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

            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                SaveDS2Generators(map);
            }

            //var xserializer = new XmlSerializer(typeof(Map));
            //TextWriter writer = new StreamWriter(mapPath + ".xml");
            //map.SerializeToXML(xserializer, writer, AssetLocator.Type);
        }

        public void SaveAllMaps()
        {
            foreach (var m in LoadedMaps)
            {
                if (m.Value != null)
                {
                    SaveMap(m.Value);
                }
            }
        }

        public void UnloadMap(Map map)
        {
            if (LoadedMaps.ContainsKey(map.MapId))
            {
                map.Clear();
                LoadedMaps[map.MapId] = null;
            }
        }

        public void UnloadAllMaps()
        {
            List<Map> toUnload = new List<Map>();
            foreach (var key in LoadedMaps.Keys)
            {
                if (LoadedMaps[key] != null)
                {
                    toUnload.Add(LoadedMaps[key]);
                }
            }
            foreach (var un in toUnload)
            {
                UnloadMap(un);
            }
        }

        public Type GetPropertyType(string name)
        {
            foreach (var m in LoadedMaps)
            {
                if (m.Value == null)
                {
                    continue;
                }
                foreach (var o in m.Value.MapObjects)
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
