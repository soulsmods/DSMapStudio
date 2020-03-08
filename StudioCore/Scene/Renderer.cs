using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Veldrid;

namespace StudioCore.Scene
{
    public class Renderer
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IndirectDrawIndexedArgumentsPacked
        {
            public uint IndexCount;
            public uint InstanceCount;
            public uint FirstIndex;
            public int VertexOffset;
            public uint FirstInstance;
        }

        /// <summary>
        /// A class used to hold, encode, and dispatch indirect draw calls
        /// </summary>
        public class IndirectDrawEncoder
        {
            /// <summary>
            /// If set to true, a fallback that uses direct draw calls instead of
            /// indirect will be used
            /// </summary>
            public static bool UseDirect = false;

            private DeviceBuffer _indirectBuffer = null;

            private IndirectDrawIndexedArgumentsPacked[] _indirectStagingBuffer = null;
            private IndirectDrawIndexedArgumentsPacked[] _directBuffer = null;

            private int _stagingSet = 0;
            private int _renderSet = -1;

            private uint[] _indirectDrawCount = null;
            private uint[] _batchCount = null;

            private const int MAX_BATCH = 100;

            /// <summary>
            /// All the unique parameters for a batched indirect draw call
            /// </summary>
            private struct BatchInfo
            {
                public Pipeline _pipeline;
                public ResourceSet _objectRS;
                public IndexFormat _indexFormat;
                public uint _batchStart;
                public int _bufferIndex;
            }

            private BatchInfo[] _batches = null;

            unsafe public IndirectDrawEncoder(uint initialCallCount)
            {
                BufferDescription desc = new BufferDescription(
                    initialCallCount * 20, BufferUsage.IndirectBuffer);
                _indirectBuffer = Factory.CreateBuffer(desc);
                _indirectStagingBuffer = new IndirectDrawIndexedArgumentsPacked[initialCallCount];
                _directBuffer = new IndirectDrawIndexedArgumentsPacked[initialCallCount];
                _batches = new BatchInfo[2 * MAX_BATCH];

                _indirectDrawCount = new uint[2];
                _batchCount = new uint[2];
            }

            /// <summary>
            /// Resets the buffer to prepare for a new frame
            /// </summary>
            public void Reset()
            {
                _stagingSet++;
                if (_stagingSet > 1)
                {
                    _stagingSet = 0;
                }
                _renderSet++;
                if (_renderSet > 1)
                {
                    _renderSet = 0;
                }
                _indirectDrawCount[_stagingSet] = 0;
                _batchCount[_stagingSet] = 0;
            }

            /// <summary>
            /// Encodes an instanced draw with all the information needed to batch the calls. For best results,
            /// draws should be presorted into batches before submission.
            /// </summary>
            /// <param name="args">Indexed draw parameters</param>
            /// <param name="p">The pipeline to use with rendering</param>
            /// <param name="instanceData">Per instance data resource set</param>
            /// <param name="indexf">Format of the indices (16 or 32-bit)</param>
            public void AddDraw(ref IndirectDrawIndexedArgumentsPacked args, int buffer, Pipeline p, ResourceSet instanceData, IndexFormat indexf)
            {
                // Encode the draw
                if (_indirectDrawCount[_stagingSet] >= _indirectStagingBuffer.Length)
                {
                    throw new Exception("Indirect buffer not large enough for draw");
                }
                if (p == null)
                {
                    throw new Exception("Pipeline is null");
                }
                _indirectStagingBuffer[_indirectDrawCount[_stagingSet]] = args;
                _indirectDrawCount[_stagingSet]++;

                // Determine if we need a new batch
                if (_batchCount[_stagingSet] == 0 ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._pipeline != p ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._objectRS != instanceData ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._indexFormat != indexf ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._bufferIndex != buffer)
                {
                    if (_batchCount[_stagingSet] >= MAX_BATCH)
                    {
                        throw new Exception("Batch count is not large enough");
                    }
                    // Add a new batch
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._bufferIndex = buffer;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._pipeline = p;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._objectRS = instanceData;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._indexFormat = indexf;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._batchStart = _indirectDrawCount[_stagingSet] - 1;
                    _batchCount[_stagingSet]++;
                }
            }

            public void UpdateBuffer(CommandList cl)
            {
                if (UseDirect)
                {
                    Array.Copy(_indirectStagingBuffer, 0, _directBuffer, 0, _directBuffer.Length);
                }
                else
                {
                    // Copy the indirect buffer to the gpu
                    cl.UpdateBuffer(_indirectBuffer, 0, _indirectStagingBuffer);
                }
            }

            /// <summary>
            /// Submit the encoded batches as indirect draw calls
            /// </summary>
            /// <param name="cl"></param>
            public unsafe void SubmitBatches(CommandList cl, SceneRenderPipeline pipeline)
            {
                // If renderset is -1, no work has actually been uploaded to the gpu yet
                if (_renderSet == -1)
                {
                    return;
                }

                // Dispatch indirect calls for each batch
                uint c = _batchCount[_renderSet] > 0 ? _batchCount[_renderSet] - 1 : 0;
                for (int i = 0; i < _batchCount[_renderSet]; i++)
                {
                    cl.SetPipeline(_batches[MAX_BATCH * _renderSet + i]._pipeline);
                    pipeline.BindResources(cl);
                    cl.SetGraphicsResourceSet(1, _batches[MAX_BATCH * _renderSet + i]._objectRS);
                    if (!GeometryBufferAllocator.BindAsVertexBuffer(cl, _batches[MAX_BATCH * _renderSet + i]._bufferIndex))
                    {
                        continue;
                    }
                    if (!GeometryBufferAllocator.BindAsIndexBuffer(cl, _batches[MAX_BATCH * _renderSet + i]._bufferIndex, _batches[MAX_BATCH * _renderSet + i]._indexFormat))
                    {
                        continue;
                    }
                    uint count = _indirectDrawCount[_renderSet] - _batches[MAX_BATCH * _renderSet + i]._batchStart;
                    if (i < _batchCount[_renderSet] - 1)
                    {
                        count = _batches[MAX_BATCH * _renderSet + i + 1]._batchStart - _batches[MAX_BATCH * _renderSet + i]._batchStart;
                    }

                    if (UseDirect)
                    {
                        uint start = _batches[MAX_BATCH * _renderSet + i]._batchStart;
                        for (uint d = start; d < start + count; d++)
                        {
                            cl.DrawIndexed(_directBuffer[d].IndexCount, _directBuffer[d].InstanceCount, _directBuffer[d].FirstIndex,
                                _directBuffer[d].VertexOffset, _directBuffer[d].FirstInstance);
                        }
                    }
                    else
                    {
                        cl.DrawIndexedIndirect(_indirectBuffer, _batches[MAX_BATCH * _renderSet + i]._batchStart * 20, count, 20);
                    }
                }
            }
        }

        public class RenderQueue
        {
            private struct KeyIndex : IComparable<KeyIndex>, IComparable
            {
                public RenderKey Key { get; }
                public int ItemIndex { get; }

                public KeyIndex(RenderKey key, int itemIndex)
                {
                    Key = key;
                    ItemIndex = itemIndex;
                }

                public int CompareTo(object obj)
                {
                    return ((IComparable)Key).CompareTo(obj);
                }

                public int CompareTo(KeyIndex other)
                {
                    return Key.CompareTo(other.Key);
                }

                public override string ToString()
                {
                    return string.Format("Index:{0}, Key:{1}", ItemIndex, Key);
                }
            }

            public SceneRenderPipeline Pipeline { get; private set; }
            private GraphicsDevice Device;
            private CommandList ResourceUpdateCommandList;
            //private CommandList DrawCommandList;
            private Fence ResourcesUpdatedFence;
            private Fence DrawFence;

            private IndirectDrawEncoder DrawEncoder;

            private readonly List<KeyIndex> Indices = new List<KeyIndex>(1000);
            private readonly List<RenderObject> Renderables = new List<RenderObject>(1000);

            private Action<GraphicsDevice, CommandList> PreDrawSetup = null;

            private Pipeline ActivePipeline = null;

            public int Count => Renderables.Count;

            public float CPURenderTime { get; private set; } = 0.0f;

            private string Name;

            public RenderQueue(string name, GraphicsDevice device, SceneRenderPipeline pipeline)
            {
                Device = device;
                Pipeline = pipeline;
                ResourceUpdateCommandList = device.ResourceFactory.CreateCommandList();
                //DrawCommandList = device.ResourceFactory.CreateCommandList();
                ResourcesUpdatedFence = device.ResourceFactory.CreateFence(false);
                DrawFence = device.ResourceFactory.CreateFence(true);
                DrawEncoder = new IndirectDrawEncoder(40000);
                Name = name;
            }

            public void SetPredrawSetupAction(Action<GraphicsDevice, CommandList> setup)
            {
                PreDrawSetup = setup;
            }

            public void Clear()
            {
                Indices.Clear();
                Renderables.Clear();
                DrawEncoder.Reset();
                ResourcesUpdatedFence.Reset();
            }

            public void Add(RenderObject item, RenderKey key)
            {
                int index = Renderables.Count;
                Indices.Add(new KeyIndex(key, index));
                Renderables.Add(item);
            }

            private void Sort()
            {
                Indices.Sort();
            }

            public void Execute(CommandList drawCommandList)
            {
                var watch = Stopwatch.StartNew();
                Sort();
                ActivePipeline = null;
                //DrawCommandList.Begin();
                ResourceUpdateCommandList.Begin();
                ResourceUpdateCommandList.PushDebugGroup($@"{Name}: Update resources");
                PreDrawSetup.Invoke(Device, drawCommandList);
                //DrawCommandList.ClearDepthStencil(0.0f);
                foreach (var obj in Indices)
                {
                    var o = Renderables[obj.ItemIndex];
                    o.UpdatePerFrameResources(Device, ResourceUpdateCommandList, Pipeline);
                }
                ResourceUpdateCommandList.PopDebugGroup();
                ResourceUpdateCommandList.InsertDebugMarker($@"{Name}: Indirect buffer update");
                DrawEncoder.UpdateBuffer(ResourceUpdateCommandList);
                ResourceUpdateCommandList.PopDebugGroup();
                ResourceUpdateCommandList.End();
                //Device.WaitForFence(DrawFence);
                DrawFence.Reset();
                Device.SubmitCommands(ResourceUpdateCommandList, ResourcesUpdatedFence);
                drawCommandList.InsertDebugMarker($@"{Name}: Draw");
                foreach (var obj in Indices)
                {
                    var o = Renderables[obj.ItemIndex];
                    /*var p = o.GetPipeline();
                    if (p != ActivePipeline)
                    {
                        DrawCommandList.SetPipeline(p);
                        Renderer.VertexBufferAllocator.BindAsVertexBuffer(DrawCommandList);
                        ActivePipeline = p;
                    }
                    o.Render(Device, DrawCommandList, Pipeline);*/
                    o.Render(DrawEncoder, Pipeline);
                }
                DrawEncoder.SubmitBatches(drawCommandList, Pipeline);
                drawCommandList.PopDebugGroup();
                //DrawCommandList.End();
                Device.WaitForFence(ResourcesUpdatedFence);
                //Device.SubmitCommands(DrawCommandList, DrawFence);
                watch.Stop();
                CPURenderTime = (float)(((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);
                //Device.WaitForIdle();
            }
        }

        private static GraphicsDevice Device;
        private static CommandList MainCommandList;

        private static Queue<Action<GraphicsDevice, CommandList>> RenderWorkQueue;
        private static List<RenderQueue> RenderQueues;
        private static Queue<Action<GraphicsDevice, CommandList>> BackgroundUploadQueue;

        //public static GPUBufferAllocator VertexBufferAllocator { get; private set; }
        //public static GPUBufferAllocator IndexBufferAllocator { get; private set; }
        public static VertexIndexBufferAllocator GeometryBufferAllocator { get; private set; }
        public static GPUBufferAllocator UniformBufferAllocator { get; private set; }

        public static ResourceFactory Factory
        {
            get
            {
                return Device.ResourceFactory;
            }
        }

        public static void Initialize(GraphicsDevice device)
        {
            Device = device;
            MainCommandList = device.ResourceFactory.CreateCommandList();
            RenderWorkQueue = new Queue<Action<GraphicsDevice, CommandList>>();
            BackgroundUploadQueue = new Queue<Action<GraphicsDevice, CommandList>>();
            RenderQueues = new List<RenderQueue>();

            //VertexBufferAllocator = new GPUBufferAllocator(1 * 1024 * 1024 * 1024u, BufferUsage.VertexBuffer);
            //VertexBufferAllocator = new GPUBufferAllocator(256 * 1024 * 1024u, BufferUsage.VertexBuffer);
            //IndexBufferAllocator = new GPUBufferAllocator(512 * 1024 * 1024, BufferUsage.IndexBuffer);
            //IndexBufferAllocator = new GPUBufferAllocator(128 * 1024 * 1024, BufferUsage.IndexBuffer);
            GeometryBufferAllocator = new VertexIndexBufferAllocator(256 * 1024 * 1024, 128 * 1024 * 1024);
            UniformBufferAllocator = new GPUBufferAllocator(5 * 1024 * 1024, BufferUsage.StructuredBufferReadWrite, 64);
        }

        public static void RegisterRenderQueue(RenderQueue queue)
        {
            RenderQueues.Add(queue);
        }

        public static void AddBackgroundUploadTask(Action<GraphicsDevice, CommandList> action)
        {
            lock (BackgroundUploadQueue)
            {
                BackgroundUploadQueue.Enqueue(action);
            }
        }

        public static void Frame(CommandList drawCommandList)
        {
            MainCommandList.Begin();

            Queue<Action<GraphicsDevice, CommandList>> work;
            lock (BackgroundUploadQueue)
            {
                work = new Queue<Action<GraphicsDevice, CommandList>>(BackgroundUploadQueue);
                BackgroundUploadQueue.Clear();
            }
            while (work.Count() > 0)
            {
                work.Dequeue().Invoke(Device, MainCommandList);
            }

            MainCommandList.End();
            Device.SubmitCommands(MainCommandList);

            foreach (var rq in RenderQueues)
            {
                rq.Execute(drawCommandList);
                rq.Clear();
            }
        }
    }
}
