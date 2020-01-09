using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    /// <summary>
    /// The "renderer" for a scene. This pipeline is instantiated for every real or virtual viewport, and will
    /// render a scene into an internally maintained framebuffer.
    /// </summary>
    public class SceneRenderPipeline
    {
        private RenderScene Scene;

        public DeviceBuffer ProjectionMatrixBuffer { get; private set; }
        public DeviceBuffer ViewMatrixBuffer { get; private set; }
        public DeviceBuffer EyePositionBuffer { get; private set; }

        public ResourceSet ProjViewRS { get; private set; }

        private Renderer.RenderQueue RenderQueue;

        public float CPURenderTime { get => RenderQueue.CPURenderTime; }

        public SceneRenderPipeline(RenderScene scene, GraphicsDevice device, int width, int height)
        {
            Scene = scene;

            var factory = device.ResourceFactory;
            ProjectionMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ViewMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            EyePositionBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            //Matrix4x4 proj = Matrix4x4.CreatePerspective(width, height, 0.1f, 100.0f);
            Matrix4x4 proj = Utils.CreatePerspective(device, false, 60.0f * (float)Math.PI / 180.0f, (float)width / (float)height, 0.1f, 1000.0f);
            Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0.0f, 2.0f, 0.0f), new Vector3(1.0f, 2.0f, 0.0f), Vector3.UnitY);
            Vector3 eye = new Vector3(0.0f, 2.0f, 0.0f);
            device.UpdateBuffer(ProjectionMatrixBuffer, 0, ref proj, 64);
            device.UpdateBuffer(ViewMatrixBuffer, 0, ref view, 64);
            device.UpdateBuffer(EyePositionBuffer, 0, ref view, 12);
            ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                device.ResourceFactory,
                StaticResourceCache.ProjViewLayoutDescription);
            ProjViewRS = StaticResourceCache.GetResourceSet(device.ResourceFactory, new ResourceSetDescription(projViewLayout,
                ProjectionMatrixBuffer,
                ViewMatrixBuffer,
                EyePositionBuffer));

            RenderQueue = new Renderer.RenderQueue(device, this);
            Renderer.RegisterRenderQueue(RenderQueue);
        }

        public void SetViewportSetupAction(Action<GraphicsDevice, CommandList> action)
        {
            RenderQueue.SetPredrawSetupAction(action);
        }

        public void TestUpdateView(Matrix4x4 view, Vector3 eye)
        {
            //cl.UpdateBuffer(ViewMatrixBuffer, 0, ref view, 64);
            Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                cl.UpdateBuffer(ViewMatrixBuffer, 0, ref view, 64);
                cl.UpdateBuffer(EyePositionBuffer, 0, ref eye, 12);
            });
        }

        public void RenderScene(BoundingFrustum frustum)
        {
            Scene.Render(RenderQueue, frustum, this);
        }
    }
}
