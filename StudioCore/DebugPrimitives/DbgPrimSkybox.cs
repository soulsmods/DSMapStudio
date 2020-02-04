using StudioCore.Scene;
using System;
using System.Numerics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimSkybox : DbgPrim
    {
        //protected override PrimitiveType PrimType => PrimitiveType.TriangleList;

        static float Radius = 100;

        public DbgPrimSkybox()
        {
            BackfaceCulling = false;

            Category = DbgPrimCategory.Skybox;

            Vector3 min = -Vector3.One * Radius;
            Vector3 max = Vector3.One * Radius;

            // 3 Letters of below names: 
            // [T]op/[B]ottom, [F]ront/[B]ack, [L]eft/[R]ight
            var tfl = new Vector3(min.X, max.Y, max.Z);
            var tfr = new Vector3(max.X, max.Y, max.Z);
            var bfr = new Vector3(max.X, min.Y, max.Z);
            var bfl = new Vector3(min.X, min.Y, max.Z);
            var tbl = new Vector3(min.X, max.Y, min.Z);
            var tbr = new Vector3(max.X, max.Y, min.Z);
            var bbr = new Vector3(max.X, min.Y, min.Z);
            var bbl = new Vector3(min.X, min.Y, min.Z);

            //front face
            AddTri(bfl, tfl, tfr);
            AddTri(bfl, tfr, bfr);

            // top face
            AddTri(tfl, tbl, tbr);
            AddTri(tfl, tbr, tfr);

            // back face
            AddTri(bbl, tbl, tbr);
            AddTri(bbl, tbr, bbr);

            // bottom face
            AddTri(bfl, bbl, bbr);
            AddTri(bfl, bbr, bfr);

            // left face
            AddTri(bbl, tbl, tfl);
            AddTri(bbl, tfl, bfl);

            // right face
            AddTri(bbr, tbr, tfr);
            AddTri(bbr, tfr, bfr);
        }

        public void AddTri(Vector3 a, Vector3 b, Vector3 c)
        {
            //var dir = Vector3.Cross(b - a, c - a);
            //var norm = Vector3.Normalize(dir);

            var vertA = new VertexPositionColorNormal(a, Color.White, Vector3.Zero);
            var vertB = new VertexPositionColorNormal(b, Color.White, Vector3.Zero);
            var vertC = new VertexPositionColorNormal(c, Color.White, Vector3.Zero);

            int vertIndexA = Array.IndexOf(Vertices, vertA);
            int vertIndexB = Array.IndexOf(Vertices, vertB);
            int vertIndexC = Array.IndexOf(Vertices, vertC);

            //If vertex A can't be recycled from an old one, make a new one.
            if (vertIndexA == -1)
            {
                AddVertex(vertA);
                vertIndexA = Vertices.Length - 1;
            }

            //If vertex B can't be recycled from an old one, make a new one.
            if (vertIndexB == -1)
            {
                AddVertex(vertB);
                vertIndexB = Vertices.Length - 1;
            }

            //If vertex C can't be recycled from an old one, make a new one.
            if (vertIndexC == -1)
            {
                AddVertex(vertC);
                vertIndexC = Vertices.Length - 1;
            }

            AddIndex((short)vertIndexA);
            AddIndex((short)vertIndexB);
            AddIndex((short)vertIndexC);

            //if (NeedToRecreateVertBuffer)
            //{
            //    VertBuffer = new VertexBuffer(GFX.Device, 
            //        typeof(VertexPositionColor), Vertices.Length, BufferUsage.WriteOnly);
            //    VertBuffer.SetData(Vertices);
            //    NeedToRecreateVertBuffer = false;
            //} 

            //if (NeedToRecreateIndexBuffer)
            //{
            //    IndexBuffer = new IndexBuffer(GFX.Device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.WriteOnly);
            //    IndexBuffer.SetData(Indices);
            //    NeedToRecreateIndexBuffer = false;
            //}
        }

        protected override void DisposeBuffers()
        {
            //VertBuffer?.Dispose();
            //IndexBuffer?.Dispose();
        }

        public override DbgPrim Instantiate(string newName, Transform newLocation, Color? newNameColor = null)
        {
            var newPrim = new DbgPrimSkybox();
            newPrim.Indices = Indices;
            newPrim.VertBuffer = VertBuffer;
            newPrim.IndexBuffer = IndexBuffer;
            newPrim.Vertices = Vertices;
            newPrim.NeedToRecreateVertBuffer = NeedToRecreateVertBuffer;
            newPrim.NeedToRecreateIndexBuffer = NeedToRecreateIndexBuffer;

            newPrim.Transform = newLocation;

            newPrim.Name = newName;

            newPrim.NameColor = newNameColor ?? NameColor;

            return newPrim;
        }

        public override void Render(Renderer.IndirectDrawEncoder encoder, SceneRenderPipeline sp)
        {
            throw new NotImplementedException();
        }

        public override void CreateDeviceObjects(Veldrid.GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            throw new NotImplementedException();
        }

        public override void DestroyDeviceObjects()
        {
            throw new NotImplementedException();
        }
    }
}
