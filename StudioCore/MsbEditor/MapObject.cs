using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;

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
            TypePart,
            TypeRegion,
            TypeEvent
        }

        public ObjectType Type { get; private set; }

        public object MsbObject;

        private string CachedName = null;

        public Scene.IDrawable RenderSceneMesh { set; get; } = null;

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
                MsbObject.GetType().GetProperty("Name").SetValue(MsbObject, value);
                CachedName = value;
            }
        }

        private bool UseTempTransform = false;
        private Transform TempTransform = Transform.Default;

        public MapObject(object msbo, ObjectType type)
        {
            MsbObject = msbo;
            Type = type;
        }

        ~MapObject()
        {
            if (RenderSceneMesh != null)
            {
                RenderSceneMesh.UnregisterAndRelease();
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
                var obj = new MapObject(clone, Type);
                obj.RenderSceneMesh = new NewMesh((NewMesh)RenderSceneMesh);
                return obj;
            }
            else
            {
                // Otherwise use standard constructor and abuse reflection
                constructor = typ.GetConstructor(System.Type.EmptyTypes);
                var clone = constructor.Invoke(null);
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
                var obj = new MapObject(clone, Type);
                obj.RenderSceneMesh = new NewMesh((NewMesh)RenderSceneMesh);
                return obj;
            }
        }

        public Transform GetTransform()
        {
            var t = Transform.Default;
            t.Position = (Vector3)MsbObject.GetType().GetProperty("Position").GetValue(MsbObject, null);
            var rot = (Vector3)MsbObject.GetType().GetProperty("Rotation").GetValue(MsbObject, null);
            t.EulerRotation = new Vector3(Utils.DegToRadians(rot.X), Utils.DegToRadians(rot.Y), Utils.DegToRadians(rot.Z));
            var scaleprop = MsbObject.GetType().GetProperty("Scale");
            if (scaleprop != null)
            {
                t.Scale = (Vector3)scaleprop.GetValue(MsbObject, null);
            }
            return t;
        }

        public void SetTemporaryTransform(Transform t)
        {
            TempTransform = t;
            UseTempTransform = true;
            UpdateRenderTransform();
        }

        public void ClearTemporaryTransform()
        {
            UseTempTransform = false;
            UpdateRenderTransform();
        }

        public Action GetUpdateTransformAction(Transform newt)
        {
            var act = new PropertiesChangedAction(MsbObject);
            var prop = MsbObject.GetType().GetProperty("Position");
            act.AddPropertyChange(prop, newt.Position);
            prop = MsbObject.GetType().GetProperty("Rotation");
            act.AddPropertyChange(prop, newt.EulerRotation * Utils.Rad2Deg);
            act.SetPostExecutionAction((undo) =>
            {
                UpdateRenderTransform();
            });
            return act;
        }

        public void UpdateRenderTransform()
        {
            if (RenderSceneMesh != null)
            {
                if (UseTempTransform)
                {
                    RenderSceneMesh.WorldMatrix = TempTransform.WorldMatrix;
                }
                else
                {
                    RenderSceneMesh.WorldMatrix = GetTransform().WorldMatrix;
                }
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
