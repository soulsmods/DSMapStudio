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
    public class Entity : Scene.ISelectable
    {
        public object WrappedObject { get; set; }

        private string CachedName = null;

        [XmlIgnore]
        public ObjectContainer Container { get; set; } = null;

        [XmlIgnore]
        public Universe Universe { 
            get
            {
                return (Container != null) ? Container.Universe : null;
            }
        }
        [XmlIgnore]
        public Entity Parent { get; private set; } = null;
        public List<Entity> Children { get; set; } = new List<Entity>();

        /// <summary>
        /// A map that contains references for each property
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, Entity[]> References { get; private set; } = new Dictionary<string, Entity[]>();

        [XmlIgnore]
        public virtual bool HasTransform
        {
            get
            {
                return false;
            }
        }

        protected Scene.IDrawable _RenderSceneMesh = null;
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
        public virtual string Name
        {
            get
            {
                if (CachedName != null)
                {
                    return CachedName;
                }
                CachedName = (string)WrappedObject.GetType().GetProperty("Name").GetValue(WrappedObject, null);
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
                    WrappedObject.GetType().GetProperty("Name").SetValue(WrappedObject, value);
                    CachedName = value;
                }
            }
        }

        [XmlIgnore]
        public virtual string PrettyName
        {
            get
            {
                return Name;
            }
        }

        protected string CurrentModel = "";

        [XmlIgnore]
        public uint[] Drawgroups
        {
            get
            {
                var prop = WrappedObject.GetType().GetProperty("DrawGroups");
                if (prop != null)
                {
                    return (uint[])prop.GetValue(WrappedObject);
                }
                return null;
            }
        }

        [XmlIgnore]
        public uint[] Dispgroups
        {
            get
            {
                var prop = WrappedObject.GetType().GetProperty("DispGroups");
                if (prop != null)
                {
                    return (uint[])prop.GetValue(WrappedObject);
                }
                return null;
            }
        }

        protected bool _EditorVisible = true;
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

        internal bool UseTempTransform = false;
        internal Transform TempTransform = Transform.Default;

        public Entity()
        {

        }

        public Entity(ObjectContainer map, object msbo)
        {
            Container = map;
            WrappedObject = msbo;
        }

        ~Entity()
        {
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.UnregisterAndRelease();
                RenderSceneMesh = null;
            }
        }

        public void AddChild(Entity child)
        {
            if (child.Parent != null)
            {
                Parent.Children.Remove(child);
            }
            child.Parent = this;
            Children.Add(child);
        }

        public void AddChild(Entity child, int index)
        {
            if (child.Parent != null)
            {
                Parent.Children.Remove(child);
            }
            child.Parent = this;
            Children.Insert(index, child);
        }

        public int ChildIndex(Entity child)
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

        public int RemoveChild(Entity child)
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

        private void CloneRenderable(Entity obj)
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

        internal virtual Entity DuplicateEntity(object clone)
        {
            return new Entity(Container, clone);
        }

        public virtual Entity Clone()
        {
            var typ = WrappedObject.GetType();

            // use copy constructor if available
            var typs = new Type[1];
            typs[0] = typ;
            ConstructorInfo constructor = typ.GetConstructor(typs);
            if (constructor != null)
            {
                var clone = constructor.Invoke(new object[] { WrappedObject });
                var obj = DuplicateEntity(clone);
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
                    if (sourceProperty.PropertyType.IsArray)
                    {
                        Array arr = (Array)sourceProperty.GetValue(WrappedObject);
                        Array.Copy(arr, (Array)targetProperty.GetValue(clone), arr.Length);
                    }
                    else if (sourceProperty.CanWrite)
                    {
                        targetProperty.SetValue(clone, sourceProperty.GetValue(WrappedObject, null), null);
                    }

                    else
                    {
                        // Sanity check
                        // Console.WriteLine($"Can't copy {type.Name} {sourceProperty.Name} of type {sourceProperty.PropertyType}");
                    }
                }
                var obj = DuplicateEntity(clone);
                CloneRenderable(obj);
                return obj;
            }
        }

        public object GetPropertyValue(string prop)
        {
            if (WrappedObject == null)
            {
                return null;
            }
            if (WrappedObject is PARAM.Row row)
            {
                var pp = row.Cells.FirstOrDefault(cell => cell.Def.InternalName == prop);
                if (pp != null)
                {
                    return pp.Value;
                }
            }
            else if (WrappedObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return pp.Value;
                }
            }
            var p = WrappedObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return p.GetValue(WrappedObject, null);
            }
            return null;
        }

        public T GetPropertyValue<T>(string prop)
        {
            if (WrappedObject == null)
            {
                return default(T);
            }
            if (WrappedObject is PARAM.Row row)
            {
                var pp = row.Cells.FirstOrDefault(cell => cell.Def.InternalName == prop);
                if (pp != null)
                {
                    return (T)pp.Value;
                }
            }
            else if (WrappedObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return (T)pp.Value;
                }
            }
            var p = WrappedObject.GetType().GetProperty(prop);
            if (p != null && p.PropertyType == typeof(T))
            {
                return (T)p.GetValue(WrappedObject, null);
            }
            return default(T);
        }

        public PropertyInfo GetProperty(string prop)
        {
            if (WrappedObject == null)
            {
                return null;
            }
            if (WrappedObject is PARAM.Row row)
            {
                var pp = row[prop];
                if (pp != null)
                {
                    return pp.GetType().GetProperty("Value");
                }
            }
            else if (WrappedObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    return pp.GetType().GetProperty("Value");
                }
            }
            var p = WrappedObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return p;
            }
            return null;
        }

        public PropertiesChangedAction GetPropertyChangeAction(string prop, object newval)
        {
            if (WrappedObject == null)
            {
                return null;
            }
            if (WrappedObject is PARAM.Row row)
            {
                var pp = row[prop];
                if (pp != null)
                {
                    var pprop = pp.GetType().GetProperty("Value");
                    return new PropertiesChangedAction(pprop, pp, newval);
                }
            }
            if (WrappedObject is MergedParamRow mrow)
            {
                var pp = mrow[prop];
                if (pp != null)
                {
                    var pprop = pp.GetType().GetProperty("Value");
                    return new PropertiesChangedAction(pprop, pp, newval);
                }
            }
            var p = WrappedObject.GetType().GetProperty(prop);
            if (p != null)
            {
                return new PropertiesChangedAction(p, WrappedObject, newval);
            }
            return null;
        }

        public void BuildReferenceMap()
        {
            if (!(WrappedObject is PARAM.Row) && !(WrappedObject is MergedParamRow))
            {
                var type = WrappedObject.GetType();
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
                            var sref = (string)p.GetValue(WrappedObject);
                            if (sref != null && sref != "")
                            {
                                var obj = Container.GetObjectByName(sref);
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

        private HashSet<Entity> ReferencingObjects = null;

        public IReadOnlyCollection<Entity> GetReferencingObjects()
        {
            if (Container == null)
            {
                return new List<Entity>();
            }

            if (ReferencingObjects != null)
            {
                return ReferencingObjects;
            }

            ReferencingObjects = new HashSet<Entity>();
            foreach (var m in Container.Objects)
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

        public virtual Transform GetTransform()
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
            if (WrappedObject is PARAM.Row || WrappedObject is MergedParamRow)
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
                var act = new PropertiesChangedAction(WrappedObject);
                var prop = WrappedObject.GetType().GetProperty("Position");
                act.AddPropertyChange(prop, newt.Position);
                prop = WrappedObject.GetType().GetProperty("Rotation");
                act.AddPropertyChange(prop, newt.EulerRotation * Utils.Rad2Deg);
                act.SetPostExecutionAction((undo) =>
                {
                    UpdateRenderModel();
                });
                return act;
            }
        }

        public virtual void UpdateRenderModel()
        {

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

    public class NamedEntity : Entity
    {
        public override string Name { get; set; }

        public NamedEntity(ObjectContainer map, object msbo, string name) : base(map, msbo)
        {
            Name = name;
        }
    }

    public class TransformableNamedEntity : Entity
    {
        public override string Name { get; set; }

        public TransformableNamedEntity(ObjectContainer map, object msbo, string name) : base(map, msbo)
        {
            Name = name;
        }

        public override bool HasTransform
        {
            get
            {
                return true;
            }
        }
    }

    public class MapSerializationEntity
    {
        public string Name { get; set; } = null;
        public int Msbidx { get; set; } = -1;
        public MapEntity.MapEntityType Type { get; set; }
        public Transform Transform { get; set; }
        public List<MapSerializationEntity> Children { get; set; } = null;

        public bool ShouldSerializeChildren()
        {
            return (Children != null && Children.Count > 0);
        }
    }

    public class MapEntity : Entity
    {
        public enum MapEntityType
        {
            MapRoot,
            Editor,
            Part,
            Region,
            Event,
            DS2Generator,
            DS2GeneratorRegist,
            DS2Event,
            DS2EventLocation,
        }

        public MapEntityType Type { get; set; }

        public Map ContainingMap
        {
            get
            {
                return (Map)Container;
            }
        }

        public override string PrettyName
        {
            get
            {
                string icon = "";
                if (Type == MapEntityType.Part)
                {
                    icon = ForkAwesome.PuzzlePiece;
                }
                else if (Type == MapEntityType.Event)
                {
                    icon = ForkAwesome.Flag;
                }
                else if (Type == MapEntityType.Region)
                {
                    icon = ForkAwesome.LocationArrow;
                }
                else if (Type == MapEntityType.DS2Generator)
                {
                    icon = ForkAwesome.Male;
                }
                else if (Type == MapEntityType.DS2GeneratorRegist)
                {
                    icon = ForkAwesome.UserCircleO;
                }
                else if (Type == MapEntityType.DS2EventLocation)
                {
                    icon = ForkAwesome.FlagO;
                }
                else if (Type == MapEntityType.DS2Event)
                {
                    icon = ForkAwesome.FlagCheckered;
                }

                return $@"{icon} {Name}";
            }
        }

        public override bool HasTransform
        {
            get
            {
                return Type != MapEntityType.Event && Type != MapEntityType.DS2GeneratorRegist && Type != MapEntityType.DS2Event;
            }
        }

        [XmlIgnore]
        public string MapID
        {
            get
            {
                var parent = Parent;
                while (parent != null && parent is MapEntity e)
                {
                    if (e.Type == MapEntityType.MapRoot)
                    {
                        return parent.Name;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        public MapEntity()
        {

        }

        public MapEntity(ObjectContainer map, object msbo)
        {
            Container = map;
            WrappedObject = msbo;
        }

        public MapEntity(ObjectContainer map, object msbo, MapEntityType type)
        {
            Container = map;
            WrappedObject = msbo;
            Type = type;
            if (!(msbo is PARAM.Row) && !(msbo is MergedParamRow))
            {
                CurrentModel = GetPropertyValue<string>("ModelName");
            }
        }

        public override void UpdateRenderModel()
        {
            // If the model field changed, then update the visible model
            if (Type == MapEntityType.DS2Generator)
            {

            }
            else if (Type == MapEntityType.Region && RenderSceneMesh == null)
            {
                if (RenderSceneMesh != null)
                {
                    RenderSceneMesh.AutoRegister = false;
                    RenderSceneMesh.UnregisterAndRelease();
                }
                RenderSceneMesh = Universe.GetRegionDrawable(ContainingMap, this);
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
                    if (Universe.Selection.IsSelected(this))
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
                var prop = WrappedObject.GetType().GetProperty("DrawGroups");
                if (prop != null && RenderSceneMesh != null)
                {
                    RenderSceneMesh.DrawGroups.AlwaysVisible = false;
                    RenderSceneMesh.DrawGroups.Drawgroups = (uint[])prop.GetValue(WrappedObject);
                }
            }

            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.IsVisible = _EditorVisible;
            }
        }

        public override Transform GetTransform()
        {
            var t = base.GetTransform();
            // If this is a region scale the region primitive by its respective parameters
            if (Type == MapEntityType.Region)
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
            if (Type == MapEntityType.DS2EventLocation)
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

        internal override Entity DuplicateEntity(object clone)
        {
            return new MapEntity(Container, clone);
        }

        public override Entity Clone()
        {
            MapEntity c = (MapEntity)base.Clone();
            c.Type = Type;
            return c;
        }

        public MapSerializationEntity Serialize(Dictionary<Entity, int> idmap)
        {
            var e = new MapSerializationEntity();
            e.Name = Name;
            if (HasTransform)
            {
                e.Transform = GetTransform();
            }
            e.Type = Type;
            e.Children = new List<MapSerializationEntity>();
            if (idmap.ContainsKey(this))
            {
                e.Msbidx = idmap[this];
            }
            foreach (var c in Children)
            {
                e.Children.Add(((MapEntity)c).Serialize(idmap));
            }
            return e;
        }
    }
}
