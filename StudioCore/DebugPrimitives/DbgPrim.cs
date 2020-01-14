using Veldrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using StudioCore.Scene;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public enum DbgPrimCategory
    {
        HkxBone,
        FlverBone,
        FlverBoneBoundingBox,
        DummyPoly,
        WeaponDummyPoly,
        DummyPolyHelper,
        Skybox,
        DummyPolySpawnArrow,
        Other,
    }

    public class DbgLabel
    {
        public Matrix4x4 World = Matrix4x4.Identity;
        public float Height = 1;
        public string Text = "?LabelText?";
        public Color Color;

        public DbgLabel(Matrix4x4 world, float height, string text, Color color)
        {
            World = world;
            Height = height;
            Text = text;
            Color = color;
        }
    }

    public abstract class DbgPrim : Scene.RenderObject, Scene.IDrawable, IDbgPrim
    {
        public Transform Transform { get; set; } = Transform.Default;

        public Matrix4x4 WorldMatrix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name { get; set; }
        public Color NameColor { get; set; } = Color.Yellow;

        public Color? OverrideColor { get; set; } = null;

        public DbgPrimCategory Category { get; set; } = DbgPrimCategory.Other;

        private List<DbgLabel> DbgLabels = new List<DbgLabel>();

        public object ExtraData { get; set; } = null;

        public bool EnableDraw { get; set; } = true;
        public bool EnableDbgLabelDraw { get; set; } = true;
        public bool EnableNameDraw { get; set; } = true;

        public List<IDbgPrim> Children { get; set; } = new List<IDbgPrim>();
        public List<IDbgPrim> UnparentedChildren { get; set; } = new List<IDbgPrim>();

        public RenderFilter DrawFilter { get; set; } = RenderFilter.Debug;

        public DrawGroup DrawGroups { get; set; } = new DrawGroup();

        protected short[] Indices = new short[0];
        protected VertexPositionColorNormal[] Vertices = new VertexPositionColorNormal[0];
        //protected VertexBuffer VertBuffer;
        //protected IndexBuffer IndexBuffer;
        protected Pipeline RenderPipeline;
        protected Shader[] Shaders;
        protected DeviceBuffer WorldBuffer;
        protected DeviceBuffer VertBuffer;
        protected DeviceBuffer IndexBuffer;
        protected ResourceSet PerObjRS;
        protected bool NeedToRecreateVertBuffer = true;
        protected bool NeedToRecreateIndexBuffer = true;

        public bool BackfaceCulling = true;
        public bool Wireframe = false;
        public bool DisableLighting = false;

        public int VertexCount => Vertices.Length;
        public int IndexCount => Indices.Length;

        protected void SetBuffers(DeviceBuffer vertBuffer, DeviceBuffer indexBuffer)
        {
            VertBuffer = vertBuffer;
            IndexBuffer = indexBuffer;
            //NeedToRecreateIndexBuffer = false;
            //NeedToRecreateVertBuffer = false;
        }

        public override void UpdatePerFrameResources(Veldrid.GraphicsDevice device, CommandList cl, Scene.SceneRenderPipeline sp)
        {
            if (NeedToRecreateVertBuffer)
            {
                VertBuffer?.Dispose();
                VertBuffer = null;
                if (Vertices.Length > 0)
                {
                    //VertBuffer = new VertexBuffer(GFX.Device,
                    //typeof(VertexPositionColorNormal), Vertices.Length, BufferUsage.WriteOnly);
                    //VertBuffer.SetData(Vertices);
                    //Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                    //{
                        VertBuffer = device.ResourceFactory.CreateBuffer(new BufferDescription(28 * (uint)Vertices.Length, Veldrid.BufferUsage.VertexBuffer));
                        cl.UpdateBuffer(VertBuffer, 0, Vertices);
                    //});
                }
                else
                {
                    throw new Exception("WTF");
                }
                NeedToRecreateVertBuffer = false;
            }

            if (NeedToRecreateIndexBuffer)
            {
                IndexBuffer?.Dispose();
                IndexBuffer = null;
                if (Indices.Length > 0)
                {
                    //IndexBuffer = new IndexBuffer(GFX.Device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.WriteOnly);
                    //IndexBuffer.SetData(Indices);
                    //Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                    //{
                        IndexBuffer = device.ResourceFactory.CreateBuffer(new BufferDescription(2 * (uint)Indices.Length, Veldrid.BufferUsage.IndexBuffer));
                        cl.UpdateBuffer(IndexBuffer, 0, Indices);
                    //});
                }
                NeedToRecreateIndexBuffer = false;
            }
            var mat = Transform.WorldMatrix;
            cl.UpdateBuffer(WorldBuffer, 0, ref mat, 64);
        }

        protected void AddVertex(Vector3 pos, Color color, Vector3? normal = null)
        {
            Array.Resize(ref Vertices, Vertices.Length + 1);
            Vertices[Vertices.Length - 1].Position = pos;
            Vertices[Vertices.Length - 1].Color = color;
            Vertices[Vertices.Length - 1].Normal = normal ?? Vector3.UnitX;

            NeedToRecreateVertBuffer = true;
        }

        protected void AddVertex(VertexPositionColorNormal vert)
        {
            Array.Resize(ref Vertices, Vertices.Length + 1);
            Vertices[Vertices.Length - 1] = vert;

            NeedToRecreateVertBuffer = true;
        }

        protected void AddIndex(short index)
        {
            Array.Resize(ref Indices, Indices.Length + 1);
            Indices[Indices.Length - 1] = index;
            NeedToRecreateIndexBuffer = true;
        }

        public void AddDbgLabel(Vector3 position, float height, string text, Color color)
        {
            DbgLabels.Add(new DbgLabel(Matrix4x4.CreateTranslation(position), height, text, color));
        }

        public void AddDbgLabel(Matrix4x4 world, float height, string text, Color color)
        {
            DbgLabels.Add(new DbgLabel(world, height, text, color));
        }


        public abstract DbgPrim Instantiate(string newName, Transform newLocation, Color? newNameColor = null);

        /// <summary>
        /// Set this to choose specific technique(s).
        /// Null to just use the current technique.
        /// </summary>
        public virtual string[] ShaderTechniquesSelection => null;

        public WeakReference<ISelectable> Selectable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Highlighted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoRegister { get; set; } = false;
        public bool IsVisible { get; set; }

        private void DrawPrimitive(Veldrid.GraphicsDevice device, CommandList cl, Scene.SceneRenderPipeline sp)
        {
            //FinalizeBuffers(device, cl);

            if (VertBuffer == null || IndexBuffer == null || VertBuffer.SizeInBytes == 0 || IndexBuffer.SizeInBytes == 0)
            {
                // This is some dummy parent thing with no geometry.
                if (Children.Count > 0 || UnparentedChildren.Count > 0)
                    return;

                //If it's NOT a parent thing, then it shouldn't have empty geometry.
                // Some mistake was made.
                throw new Exception("DbgPrim geometry is empty and it had no children. Something went wrong...");
            }

            //GFX.Device.SetVertexBuffer(VertBuffer);
            //GFX.Device.Indices = IndexBuffer;
            //GFX.BackfaceCulling = BackfaceCulling;
            //GFX.Wireframe = Wireframe;

            //if (Shader is DbgPrimSolidShader solid)
            //    solid.Effect.LightingEnabled = !DisableLighting;

            //GFX.Device.DrawIndexedPrimitives(PrimType, 0, 0, IndexBuffer.IndexCount);

            cl.SetPipeline(RenderPipeline);
            cl.SetGraphicsResourceSet(0, sp.ProjViewRS);
            uint offset = 0;
            cl.SetGraphicsResourceSet(1, PerObjRS, 1, ref offset);
            cl.SetVertexBuffer(0, VertBuffer);
            cl.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            cl.DrawIndexed(IndexBuffer.SizeInBytes / 2, 1, 0, 0, 0);
        }

        protected virtual void PreDraw()
        {

        }

        public void Draw(Veldrid.GraphicsDevice device, CommandList cl, Scene.SceneRenderPipeline sp, IDbgPrim parentPrim, Matrix4x4 world)
        {
            PreDraw();

            // Always draw unparented children :fatcat:
            foreach (var c in UnparentedChildren)
                c.Draw(device, cl, sp, this, world);

            if (!EnableDraw)
                return;

            DrawPrimitive(device, cl, sp);

            foreach (var c in Children)
                c.Draw(device, cl, sp, this, Transform.WorldMatrix * world);
        }

        public void LabelDraw(Matrix4x4 world)
        {
            // Always draw unparented children :fatcat:
            foreach (var c in UnparentedChildren)
                c.LabelDraw(world);

            if (DbgLabels.Count > 0)
            {
                foreach (var label in DbgLabels)
                {
                    //DBG.DrawTextOn3DLocation_FixedPixelSize(label.World * Transform.WorldMatrix * world, Vector3.Zero,
                    //    label.Text, label.Color, label.Height * 1.5f, startAndEndSpriteBatchForMe: false);
                }
            }

            foreach (var c in Children)
                c.LabelDraw(Transform.WorldMatrix * world);
        }

        public void LabelDraw_Billboard(Matrix4x4 world)
        {
            // Always draw unparented children :fatcat:
            foreach (var c in UnparentedChildren)
                c.LabelDraw_Billboard(world);

            if (DbgLabels.Count > 0)
            {
                //foreach (var label in DbgLabels.OrderByDescending(lbl => (GFX.World.CameraTransform.Position - Vector3.Transform(Vector3.Zero, lbl.World)).LengthSquared()))
                //{
                    //DBG.Draw3DBillboard(label.Text, label.World * Transform.WorldMatrix * world, label.Color);
                //}
            }

            foreach (var c in Children)
                c.LabelDraw_Billboard(Transform.WorldMatrix * world);
        }

        protected abstract void DisposeBuffers();

        public void Dispose()
        {
            DisposeBuffers();
        }

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            queue.Add(this, new RenderKey(0));
        }

        public virtual BoundingBox GetBounds()
        {
            return new BoundingBox();
        }

        public virtual bool RayCast(Ray ray, out float dist)
        {
            dist = float.MaxValue;
            return false;
        }

        public void RegisterWithScene(RenderScene scene)
        {
            throw new NotImplementedException();
        }

        public void UnregisterWithScene()
        {
            throw new NotImplementedException();
        }

        public void UnregisterAndRelease()
        {
            throw new NotImplementedException();
        }
    }
}
