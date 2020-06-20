using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.DebugPrimitives
{
    public interface IDbgPrim : IDisposable
    {
        Transform Transform { get; set; }
        string Name { get; set; }
        Color NameColor { get; set; }

        object ExtraData { get; set; }

        DbgPrimCategory Category { get; set; }

        bool EnableDraw { get; set; }
        bool EnableDbgLabelDraw { get; set; }
        bool EnableNameDraw { get; set; }

        List<IDbgPrim> Children { get; set; }
        List<IDbgPrim> UnparentedChildren { get; set; }

        void Draw(Scene.Renderer.IndirectDrawEncoder encoder, Scene.SceneRenderPipeline sp, IDbgPrim parent, Matrix4x4 world);
        void LabelDraw(Matrix4x4 world);

        void LabelDraw_Billboard(Matrix4x4 world);

    }
}
