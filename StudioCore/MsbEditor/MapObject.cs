using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;
using System.Xml.Serialization;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// A logical map object that can be either a part, a region, or an event. Uses
    /// reflection to access and update properties
    /// </summary>
    public class MapObject : Scene.ISelectable
    {
        public enum ObjectType
        {
            TypeMapRoot,
            TypeEditor,
            TypePart,
            TypeRegion,
            TypeEvent,
            TypeDS2Generator,
            TypeDS2GeneratorRegist,
            TypeDS2Event,
            TypeDS2EventLocation,
        }

        public ObjectType Type { get; set; }

        public object MsbObject { get; set; }

        private string CachedName = null;

        [XmlIgnore]
        public Map ContainingMap { get; set; } = null;

        [XmlIgnore]
        public Universe Universe { 
            get
            {
                return (ContainingMap != null) ? ContainingMap.Universe : null;
            }
        }
        [XmlIgnore]
        public MapObject Parent { get; private set; } = null;
        public List<MapObject> Children { get; set; } = new List<MapObject>();

        /// <summary>
        /// A map that contains references for each property
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, MapObject[]> References { get; private set; } = new Dictionary<string, MapObject[]>();

        [XmlIgnore]
        public bool HasTransform
        { 
            get
            {
                return Type != ObjectType.TypeEvent && Type != ObjectType.TypeDS2GeneratorRegist && Type != ObjectType.TypeDS2Event;
            }
        }

        private Scene.IDrawable _RenderSceneMesh = null;
        [XmlIgnore]
        public Scene.IDrawable RenderSceneMesh
        {
            set
            {
                _RenderSceneMesh = value;
                UpdateRenderModel();
            }
            get
            {
                return _RenderSceneMesh;
            }
        }
        [XmlIgnore]
        public bool UseDrawGroups { set; get; } = false;

        [XmlIgnore]
        public string Name
        {
            get
            {
                if (CachedName != null)
                {
                    return CachedName;
                }
                CachedName = (string)MsbObject.GetType().GetProperty("Name").GetValue(MsbObject, null);
                return CachedName;
            }
            set
            {
                if (value == null)
                {
                    CachedName = null;
                }
                else
                {
                    MsbObject.GetType().GetProperty("Name").SetValue(MsbObject, value);
                    CachedName = value;
                }
            }
        }

        [XmlIgnore]
        public string PrettyName
        {
            get
            {
                string icon = "";
                if (Type == ObjectType.TypePart)
                {
                    icon = ForkAwesome.PuzzlePiece;
                }
                else if (Type == ObjectType.TypeEvent)
                {
                    icon = ForkAwesome.Flag;
                }
                else if (Type == ObjectType.TypeRegion)
                {
                    icon = ForkAwesome.LocationArrow;
                }
                else if (Type == ObjectType.TypeDS2Generator)
                {
                    icon = ForkAwesome.Male;
                }
                else if (Type == ObjectType.TypeDS2GeneratorRegist)
                {
                    icon = ForkAwesome.UserCircleO;
                }
                else if (Type == ObjectType.TypeDS2EventLocation)
                {
                    icon = ForkAwesome.FlagO;
                }
                else if (Type == ObjectType.TypeDS2Event)
                {
                    icon = ForkAwesome.FlagCheckered;
                }

                return $@"{icon} {Name}";
            }
        }

        private string CurrentModel = "";

        [XmlIgnore]
        public uint[] Drawgroups
        {
            get
            {
                var prop = MsbObject.GetType().GetProperty("DrawGroups");
                if (prop != null)
                {
                    return (uint[])prop.GetValue(MsbObject);
                }
                return null;
            }
        }

        [XmlIgnore]
        public uint[] Dispgroups
        {
            get
            {
                var prop = MsbObject.GetType().GetProperty("DispGroups");
                if (prop != null)
                {
                    return (uint[])prop.GetValue(MsbObject);
                }
                return null;
            }
        }

        private bool _EditorVisible = true;
        [XmlIgnore]
        public bool EditorVisible
        {
            get
            {
                return _EditorVisible;
            }
            set
            {
                _EditorVisible = value;
                if (RenderSceneMesh != null)
                {
                    RenderSceneMesh.IsVisible = _EditorVisible;
                }
            }
        }
        [XmlIgnore]
        public string MapID
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent.Type == ObjectType.TypeMapRoot)
                    {
                        return parent.Name;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        private bool UseTempTransform = false;
        private Transform TempTransform = Transform.Default;

        public MapObject()
        {

        }

        public MapObject(Map map, object msbo, ObjectType type)
        {
            ContainingMap = map;
            MsbObject = msbo;
            Type = type;
            if (!(msbo is PARAM.Row) && !(msbo is MergedParamRow))
            {
                CurrentModel = GetPropertyValue<string>("ModelName");
            }
        }

        ~MapObject()
        {
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.UnregisterAndRelease();
                RenderSceneMesh = null;
            }
        }

        public void AddChild(MapObject child)
        {
            if (child.Parent != null)
            {
                Parent.Children.Remove(child);
            }
            child.Parent = this;
            Children.Add(child);
        }

        public void AddChild(MapObject child, int index)
        {
            if (child.Parent != null)
            {
                Parent.Children.Remove(child);
            }
            child.Parent = this;
            Children.Insert(index, child);
        }

        public int ChildIndex(MapObject child)
        {
            for (int i = 0; i < Children.Count(); i++)
            {
                if (Children[i] == child)
                {

                    return i;
                }
            }
            return -1;
        }

        public int RemoveChild(MapObject child)
        {
            for (int i = 0; i < Children.Count(); i++)
            {
                if (Children[i] == child)
                {
                    Children[i].Parent = null;
                    Children.RemoveAt(i);
                    return i;
                }
            }
            return -1;
        }

        private void CloneRenderable(MapObject obj)
        {
            if (RenderSceneMesh != null)
            {
                if (RenderSceneMesh is NewMesh m)
                {
                    obj.RenderSceneMesh = new NewMesh(m);
                    obj.RenderSceneMesh.Selectable = new WeakReference<Scene.ISelectable>(this);
                }
                else if (RenderSceneMesh is Scene.CollisionMesh c)
                {
                    obj.RenderSceneMesh = new Scene.CollisionMesh(c);
                    obj.RenderSceneMesh.Selectable = new WeakReference<Scene.ISelectable>(this);
                }
                else if (RenderSceneMesh is Scene.NvmMesh n)
                {
                    obj.RenderSceneMesh = new Scene.NvmMesh(n);
                    obj.RenderSceneMesh.Selectable = new WeakReference<Scene.ISelectable>(this);
                }
                else if (RenderSceneMesh is Scene.Region r)
                {
                    //obj.RenderSceneMesh = new Scene.Region(r);
                    //obj.RenderSceneMesh.Selectable = new WeakReference<Scene.ISelectable>(this);
                }
            }
        }

        public MapObject Clone()
        {
            var typ = MsbObject.GetType();

            // use copy constructor if available
            var typs = new Type[1];
            typs[0] = typ;
            ConstructorInfo constructor = typ.GetConstructor(typs);
            if (constructor != null)
            {
                var clone = constructor.Invoke(new object[] { MsbObject });
                var obj = new MapObject(ContainingMap, clone, Type);
                CloneRenderable(obj);
                obj.UseDrawGroups = UseDrawGroups;
                return obj;
            }
            else
            {
                // Try either default constructor or name constructor
                typs[0] = typeof(string);
                constructor = typ.GetConstructor(typs);
                object clone;
                if (constructor != null)
                {
                    clone = constructor.Invoke(new object[] { "" });
                }
                else
                {
                    // Otherwise use standard constructor and abuse reflection
                    constructor = typ.GetConstructor(System.Type.EmptyTypes);
                    clone = constructor.Invoke(null);
                }
                foreach (PropertyInfo sourceProperty in typ.GetProperties())
                {
                    PropertyInfo targetProperty = typ.GetProperty(sourceProperty.Name);
                    if (sourceProperty.CanWrite)
                    {
                        targetProperty.SetValue(clone, sourceProperty.GetValue(MsbObject, null), null);
                    }
                    else if (sourceProperty.PropertyType.IsArray)
                    {
                        Array arr = (Array)sourceProperty.GetValue(MsbObject);
                        Array.Copy(arr, (Array)targetProperty.GetValue(clone), arr.Length);
                    }
                    else
                    {
                        // Sanity check
                        // Console.WriteLine($"Can't copy {type.Name} {sourceProperty.Name} of type {sourceProperty.PropertyType}");
                    }
                }
                var obj = new MapObject(ContainingMap, clone, Type);
                CloneRenderable(obj);
                return obj;
            }
        }

        public object GetPropertyValue(string prop)
        {
            if (MsbObject == null)
            {
                return null;
            }
            if (MsbObject is PARAM.Row row)
            {
                var pp = row.Cells.FirstOrDefault(cell => cell.Def.InternalName == prop);
                if (pp != null)
                {
                    return pp.Value;
                }
            }
            else if (MsbObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return pp.Value;
                }
            }
            var p = MsbObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return p.GetValue(MsbObject, null);
            }
            return null;
        }

        public T GetPropertyValue<T>(string prop)
        {
            if (MsbObject == null)
            {
                return default(T);
            }
            if (MsbObject is PARAM.Row row)
            {
                var pp = row.Cells.FirstOrDefault(cell => cell.Def.InternalName == prop);
                if (pp != null)
                {
                    return (T)pp.Value;
                }
            }
            else if (MsbObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return (T)pp.Value;
                }
            }
            var p = MsbObject.GetType().GetProperty(prop);
            if (p != null && p.PropertyType == typeof(T))
            {
                return (T)p.GetValue(MsbObject, null);
            }
            return default(T);
        }

        public PropertyInfo GetProperty(string prop)
        {
            if (MsbObject == null)
            {
                return null;
            }
            if (MsbObject is PARAM.Row row)
            {
                var pp = row[prop];
                if (pp != null)
                {
                    return pp.GetType().GetProperty("Value");
                }
            }
            else if (MsbObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return pp.GetType().GetProperty("Value");
                }
            }
            var p = MsbObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return p;
            }
            return null;
        }

        public PropertiesChangedAction GetPropertyChangeAction(string prop, object newval)
        {
            if (MsbObject == null)
            {
                return null;
            }
            if (MsbObject is PARAM.Row row)
            {
                var pp = row[prop];
                if (pp != null)
                {
                    var pprop = pp.GetType().GetProperty("Value");
                    return new PropertiesChangedAction(pprop, pp, newval);
                }
            }
            if (MsbObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    var pprop = pp.GetType().GetProperty("Value");
                    return new PropertiesChangedAction(pprop, pp, newval);
                }
            }
            var p = MsbObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return new PropertiesChangedAction(p, MsbObject, newval);
            }
            return null;
        }

        public void BuildReferenceMap()
        {
            if (!(MsbObject is PARAM.Row) && !(MsbObject is MergedParamRow))
            {
                var type = MsbObject.GetType();
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var p in props)
                {
                    var att = p.GetCustomAttribute<MSBReference>();
                    if (att != null)
                    {
                        if (p.PropertyType.IsArray)
                        {

                        }
                        else
                        {
                            var sref = (string)p.GetValue(MsbObject);
                            if (sref != null && sref != "")
                            {
                                var obj = ContainingMap.GetObjectByName(sref);
                                if (obj != null)
                                {
                                    References.Add(p.Name, new[] { obj });
                                }
                            }
                        }
                    }
                }
            }
        }

        private HashSet<MapObject> ReferencingObjects = null;

        public IReadOnlyCollection<MapObject> GetReferencingObjects()
        {
            if (ContainingMap == null)
            {
                return new List<MapObject>();
            }

            if (ReferencingObjects != null)
            {
                return ReferencingObjects;
            }

            ReferencingObjects = new HashSet<MapObject>();
            foreach (var m in ContainingMap.MapObjects)
            {
                if (m.References != null)
                {
                    foreach (var n in m.References)
                    {
                        foreach (var o in n.Value)
                        {
                            if (o == this)
                            {
                                ReferencingObjects.Add(m);
                            }
                        }
                    }
                }
            }

            return ReferencingObjects;
        }

        public void InvalidateReferencingObjectsCache()
        {
            ReferencingObjects = null;
        }

        public Transform GetTransform()
        {
            var t = Transform.Default;
            var pos = GetPropertyValue("Position");
            if (pos != null)
            {
                t.Position = (Vector3)pos;
            }
            else
            {
                var px = GetPropertyValue("PositionX");
                var py = GetPropertyValue("PositionY");
                var pz = GetPropertyValue("PositionZ");
                if (px != null)
                {
                    t.Position.X = (float)px;
                }
                if (py != null)
                {
                    t.Position.Y = (float)py;
                }
                if (pz != null)
                {
                    t.Position.Z = (float)pz;
                }
            }

            var rot = GetPropertyValue("Rotation");
            if (rot != null)
            {
                var r = (Vector3)rot;
                t.EulerRotation = new Vector3(Utils.DegToRadians(r.X), Utils.DegToRadians(r.Y), Utils.DegToRadians(r.Z));
            }
            else
            {
                var rx = GetPropertyValue("RotationX");
                var ry = GetPropertyValue("RotationY");
                var rz = GetPropertyValue("RotationZ");
                Vector3 r = Vector3.Zero;
                if (rx != null)
                {
                    r.X = (float)rx;
                }
                if (ry != null)
                {
                    r.Y = (float)ry + 180.0f; // According to Vawser, DS2 enemies are flipped 180 relative to map rotations
                }
                if (rz != null)
                {
                    r.Z = (float)rz;
                }
                t.EulerRotation = new Vector3(Utils.DegToRadians(r.X), Utils.DegToRadians(r.Y), Utils.DegToRadians(r.Z));
            }

            var scale = GetPropertyValue("Scale");
            if (scale != null)
            {
                t.Scale = (Vector3)scale;
            }

            // If this is a region scale the region primitive by its respective parameters
            if (Type == ObjectType.TypeRegion)
            {
                var shape = GetPropertyValue("Shape");
                if (shape != null && shape is MSB.Shape.Box b2)
                {
                    t.Scale = new Vector3(b2.Width, b2.Height, b2.Depth);
                }
                else if (shape != null && shape is MSB.Shape.Sphere s)
                {
                    t.Scale = new Vector3(s.Radius);
                }
                else if (shape != null && shape is MSB.Shape.Cylinder c)
                {
                    t.Scale = new Vector3(c.Radius, c.Height, c.Radius);
                }
            }

            // DS2 event regions
            if (Type == ObjectType.TypeDS2EventLocation)
            {
                var sx = GetPropertyValue("ScaleX");
                var sy = GetPropertyValue("ScaleY");
                var sz = GetPropertyValue("ScaleZ");
                if (sx != null)
                {
                    t.Scale.X = (float)sx;
                }
                if (sy != null)
                {
                    t.Scale.Y = (float)sy;
                }
                if (sz != null)
                {
                    t.Scale.Z = (float)sz;
                }
            }

            return t;
        }

        public void SetTemporaryTransform(Transform t)
        {
            TempTransform = t;
            UseTempTransform = true;
            UpdateRenderModel();
        }

        public void ClearTemporaryTransform(bool updaterender=true)
        {
            UseTempTransform = false;
            if (updaterender)
            {
                UpdateRenderModel();
            }
        }

        public Action GetUpdateTransformAction(Transform newt)
        {
            if (MsbObject is PARAM.Row || MsbObject is MergedParamRow)
            {
                var actions = new List<Action>();
                float roty = newt.EulerRotation.Y * Utils.Rad2Deg - 180.0f;
                actions.Add(GetPropertyChangeAction("PositionX", newt.Position.X));
                actions.Add(GetPropertyChangeAction("PositionY", newt.Position.Y));
                actions.Add(GetPropertyChangeAction("PositionZ", newt.Position.Z));
                actions.Add(GetPropertyChangeAction("RotationX", newt.EulerRotation.X * Utils.Rad2Deg));
                actions.Add(GetPropertyChangeAction("RotationY", roty));
                actions.Add(GetPropertyChangeAction("RotationZ", newt.EulerRotation.Z * Utils.Rad2Deg));
                var act = new CompoundAction(actions);
                act.SetPostExecutionAction((undo) =>
                {
                    UpdateRenderModel();
                });
                return act;
            }
            else
            {
                var act = new PropertiesChangedAction(MsbObject);
                var prop = MsbObject.GetType().GetProperty("Position");
                act.AddPropertyChange(prop, newt.Position);
                prop = MsbObject.GetType().GetProperty("Rotation");
                act.AddPropertyChange(prop, newt.EulerRotation * Utils.Rad2Deg);
                act.SetPostExecutionAction((undo) =>
                {
                    UpdateRenderModel();
                });
                return act;
            }
        }

        public void UpdateRenderModel()
        {
            // If the model field changed, then update the visible model
            if (Type == ObjectType.TypeDS2Generator)
            {

            }
            else
            {
                var model = GetPropertyValue<string>("ModelName");
                if (model != null && model != CurrentModel)
                {
                    RenderSceneMesh.AutoRegister = false;
                    RenderSceneMesh.UnregisterAndRelease();
                    CurrentModel = model;
                    RenderSceneMesh = Universe.GetModelDrawable(ContainingMap, this, model);
                    if (Selection.IsSelected(this))
                    {
                        OnSelected();
                    }
                }
            }

            if (!HasTransform)
            {
                return;
            }
            Matrix4x4 t = UseTempTransform ? TempTransform.WorldMatrix : GetTransform().WorldMatrix;
            var p = Parent;
            while (p != null)
            {
                t = t * (p.UseTempTransform ? p.TempTransform.WorldMatrix : p.GetTransform().WorldMatrix);
                p = p.Parent;
            }
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.WorldMatrix = t;
            }
            foreach (var c in Children)
            {
                if (c.HasTransform)
                {
                    c.UpdateRenderModel();
                }
            }

            if (UseDrawGroups)
            {
                var prop = MsbObject.GetType().GetProperty("DrawGroups");
                if (prop != null && RenderSceneMesh != null)
                {
                    RenderSceneMesh.DrawGroups.AlwaysVisible = false;
                    RenderSceneMesh.DrawGroups.Drawgroups = (uint[])prop.GetValue(MsbObject);
                }
            }

            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.IsVisible = _EditorVisible;
            }
        }

        public void OnSelected()
        {
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.Highlighted = true;
            }
        }

        public void OnDeselected()
        {
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.Highlighted = false;
            }
        }
    }
}
