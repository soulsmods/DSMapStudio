using System;
using System.Collections.Generic;
using System.Text;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// A universe is a collection of loaded maps with methods to load, serialize,
    /// and unload individual maps.
    /// </summary>
    public class Universe
    {
        public List<Map> LoadedMaps { get; private set; } = new List<Map>();
        private AssetLocator AssetLocator;
        private Resource.ResourceManager ResourceMan;
        private Scene.RenderScene RenderScene;

        public Universe(AssetLocator al, Resource.ResourceManager rm,
            Scene.RenderScene scene)
        {
            AssetLocator = al;
            ResourceMan = rm;
            RenderScene = scene;
        }

        public void LoadMap(string mapid)
        {
            var map = new Map(mapid);

            var chrsToLoad = new HashSet<AssetDescription>();
            var objsToLoad = new HashSet<AssetDescription>();
            var colsToLoad = new HashSet<AssetDescription>();
            var navsToLoad = new HashSet<AssetDescription>();

            var ad = AssetLocator.GetMapMSB(mapid);
            IMsb msb;
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                msb = MSB3.Read(ad.AssetPath);
            }
            else if (AssetLocator.Type == GameType.Sekiro)
            {
                msb = MSBS.Read(ad.AssetPath);
            }
            else
            {
                msb = MSB1.Read(ad.AssetPath);
            }
            map.LoadMSB(msb);

            // Temporary garbage
            foreach (var obj in map.MapObjects)
            {
                if (obj.MsbObject is IMsbPart mp && mp.ModelName != null && mp.ModelName != "")
                {
                    AssetDescription asset;
                    bool loadcol = false;
                    bool loadnav = false;
                    Scene.RenderFilter filt = Scene.RenderFilter.All;
                    if (mp.ModelName.StartsWith("m"))
                    {
                        asset = AssetLocator.GetMapModel(mapid, AssetLocator.MapModelNameToAssetName(mapid, mp.ModelName));
                        filt = Scene.RenderFilter.MapPiece;
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
                        asset = AssetLocator.GetMapCollisionModel(mapid, AssetLocator.MapModelNameToAssetName(mapid, mp.ModelName));
                        filt = Scene.RenderFilter.Collision;
                        colsToLoad.Add(asset);
                    }
                    else if (mp.ModelName.StartsWith("n"))
                    {
                        loadnav = true;
                        asset = AssetLocator.GetMapNVMModel(mapid, AssetLocator.MapModelNameToAssetName(mapid, mp.ModelName));
                        filt = Scene.RenderFilter.Navmesh;
                        navsToLoad.Add(asset);
                    }
                    else
                    {
                        asset = AssetLocator.GetNullAsset();
                    }

                    if (loadcol)
                    {
                        var res = ResourceMan.GetResource<Resource.HavokCollisionResource>(asset.AssetVirtualPath);
                        var mesh = new Scene.CollisionMesh(RenderScene, res, false);
                        mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = mesh;
                        mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                    else if (loadnav)
                    {
                        var res = ResourceMan.GetResource<Resource.NVMNavmeshResource>(asset.AssetVirtualPath);
                        var mesh = new Scene.NvmMesh(RenderScene, res, false);
                        mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = mesh;
                        mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                    else
                    {
                        var res = ResourceMan.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
                        var model = new NewMesh(RenderScene, res, false);
                        model.DrawFilter = filt;
                        model.WorldMatrix = obj.GetTransform().WorldMatrix;
                        obj.RenderSceneMesh = model;
                        model.Selectable = new WeakReference<Scene.ISelectable>(obj);
                    }
                }
                if (obj.MsbObject is MSB1.Region r && r.Shape is MSB1.Shape.Box b)
                {
                    var mesh = Scene.Region.GetBoxRegion(RenderScene, b.Width, b.Height, b.Depth);
                    mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                    obj.RenderSceneMesh = mesh;
                    mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                }
            }
            LoadedMaps.Add(map);

            var job = ResourceMan.CreateNewJob($@"Loading {mapid} geometry");
            foreach (var mappiece in AssetLocator.GetMapModels(mapid))
            {
                if (mappiece.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(mappiece.AssetArchiveVirtualPath, false);
                }
                else if (mappiece.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(mappiece.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
            job = ResourceMan.CreateNewJob($@"Loading {mapid} collisions");
            foreach (var col in colsToLoad)
            {
                if (col.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(col.AssetArchiveVirtualPath, false);
                }
                else if (col.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(col.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
            job = ResourceMan.CreateNewJob($@"Loading chrs");
            foreach (var chr in chrsToLoad)
            {
                if (chr.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, false);
                }
                else if (chr.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(chr.AssetVirtualPath);
                }
            }
            job.StartJobAsync();
            job = ResourceMan.CreateNewJob($@"Loading objs");
            foreach (var obj in objsToLoad)
            {
                if (obj.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(obj.AssetArchiveVirtualPath, false);
                }
                else if (obj.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(obj.AssetVirtualPath);
                }
            }
            job.StartJobAsync();

            job = ResourceMan.CreateNewJob($@"Loading Navmeshes");
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
        }
    }
}
