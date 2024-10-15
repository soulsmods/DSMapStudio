using Andre.Formats;
using SoulsFormats;
using StudioCore.Platform;
using StudioCore.Renderer.Scene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Serialization;

namespace StudioCore.MsbEditor;

/// <summary>
///     High level class that stores a single map (msb) and can serialize/
///     deserialize it. This is the logical portion of the map and does not
///     handle tasks like rendering or loading associated assets with it.
///
///     CURRENTLY ALSO USED TO HOLD A FLVER???
/// </summary>
public class ObjectContainer
{
    /// <summary>
    ///     Parent entities used to organize lights per-BTL file.
    /// </summary>
    [XmlIgnore] public List<Entity> BTLParents = new();

    [XmlIgnore] public List<Entity> Objects = new();

    public ObjectContainer()
    {
    }

    public ObjectContainer(Universe u, string name)
    {
        Name = name;
        Universe = u;
        RootObject = new Entity(this, new TransformNode());
    }

    public string Name { get; set; }
    public Entity RootObject { get; set; }

    [XmlIgnore] public Universe Universe { get; protected set; }

    public bool HasUnsavedChanges { get; set; } = false;

    public void AddObject(Entity obj)
    {
        Objects.Add(obj);
        RootObject.AddChild(obj);
    }

    public void Clear()
    {
        Objects.Clear();
    }

    public Entity GetObjectByName(string name)
    {
        foreach (Entity m in Objects)
        {
            if (m.Name == name)
            {
                return m;
            }
        }

        return null;
    }

    public IEnumerable<Entity> GetObjectsByName(string name)
    {
        foreach (Entity m in Objects)
        {
            if (m.Name == name)
            {
                yield return m;
            }
        }
    }

    public byte GetNextUnique(string prop, byte value)
    {
        HashSet<byte> usedvals = new();
        foreach (Entity obj in Objects)
        {
            if (obj.GetPropertyValue(prop) != null)
            {
                var val = obj.GetPropertyValue<byte>(prop);
                usedvals.Add(val);
            }
        }

        for (var i = 0; i < 256; i++)
        {
            if (!usedvals.Contains((byte)((value + i) % 256)))
            {
                return (byte)((value + i) % 256);
            }
        }

        return value;
    }

    public void LoadFlver(FLVER2 flver, MeshRenderableProxy proxy)
    {
        var meshesNode = new NamedEntity(this, null, "Meshes");
        Objects.Add(meshesNode);
        RootObject.AddChild(meshesNode);
        for (var i = 0; i < flver.Meshes.Count; i++)
        {
            var meshnode = new NamedEntity(this, flver.Meshes[i], $@"mesh_{i}");
            if (proxy.Submeshes.Count > 0)
            {
                meshnode.RenderSceneMesh = proxy.Submeshes[i];
                proxy.Submeshes[i].SetSelectable(meshnode);
            }

            Objects.Add(meshnode);
            meshesNode.AddChild(meshnode);
        }

        var materialsNode = new NamedEntity(this, null, "Materials");
        Objects.Add(materialsNode);
        RootObject.AddChild(materialsNode);
        for (var i = 0; i < flver.Materials.Count; i++)
        {
            var matnode = new Entity(this, flver.Materials[i]);
            Objects.Add(matnode);
            materialsNode.AddChild(matnode);
        }

        var layoutsNode = new NamedEntity(this, null, "Layouts");
        Objects.Add(layoutsNode);
        RootObject.AddChild(layoutsNode);
        for (var i = 0; i < flver.BufferLayouts.Count; i++)
        {
            var laynode = new NamedEntity(this, flver.BufferLayouts[i], $@"layout_{i}");
            Objects.Add(laynode);
            layoutsNode.AddChild(laynode);
        }

        var bonesNode = new NamedEntity(this, null, "Bones");
        Objects.Add(bonesNode);
        RootObject.AddChild(bonesNode);
        var boneEntList = new List<TransformableNamedEntity>();
        for (var i = 0; i < flver.Bones.Count; i++)
        {
            var bonenode =
                new TransformableNamedEntity(this, flver.Bones[i], flver.Bones[i].Name);
            bonenode.RenderSceneMesh = Universe.GetBoneDrawable(this, bonenode);
            Objects.Add(bonenode);
            boneEntList.Add(bonenode);
        }

        for (var i = 0; i < flver.Bones.Count; i++)
        {
            if (flver.Bones[i].ParentIndex == -1)
            {
                bonesNode.AddChild(boneEntList[i]);
            }
            else
            {
                boneEntList[flver.Bones[i].ParentIndex].AddChild(boneEntList[i]);
            }
        }

        // Add dummy polys attached to bones
        var dmysNode = new NamedEntity(this, null, "DummyPolys");
        Objects.Add(dmysNode);
        RootObject.AddChild(dmysNode);
        for (var i = 0; i < flver.Dummies.Count; i++)
        {
            var dmynode = new TransformableNamedEntity(this, flver.Dummies[i], $@"dmy_{i}");
            dmynode.RenderSceneMesh = Universe.GetDummyPolyDrawable(this, dmynode);
            Objects.Add(dmynode);
            dmysNode.AddChild(dmynode);
        }
    }
}
