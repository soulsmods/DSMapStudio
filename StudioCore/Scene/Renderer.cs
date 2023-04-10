using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.Sdl2;
using System.Security.Policy;
using System.Security.Cryptography;

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
                    throw new Exception("Indirect buffer not large enough for draw\n\nTry increasing indirect draw buffer in settings.\n");
                }
                if (p == null)
                {
                    throw new Exception("Pipeline is null");
                }
                if (buffer == -1)
                {
                    throw new Exception("Invalid buffer index");
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
                        //throw new Exception("Batch count is not large enough");
                        return; // Drop the batch for now
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddDraw(ref MeshDrawParametersComponent drawparams, Pipeline pipeline)
            {
                // Encode the draw
                if (_indirectDrawCount[_stagingSet] >= _indirectStagingBuffer.Length)
                {
                    throw new Exception("Indirect buffer not large enough for draw");
                }
                if (pipeline == null)
                {
                    throw new Exception("Pipeline is null");
                }
                if (drawparams._bufferIndex == -1)
                {
                    throw new Exception("Invalid buffer index");
                }
                _indirectStagingBuffer[_indirectDrawCount[_stagingSet]] = drawparams._indirectArgs;
                _indirectDrawCount[_stagingSet]++;

                // Determine if we need a new batch
                if (_batchCount[_stagingSet] == 0 ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._pipeline != pipeline ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._objectRS != drawparams._objectResourceSet ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._indexFormat != drawparams._indexFormat ||
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet] - 1]._bufferIndex != drawparams._bufferIndex)
                {
                    if (_batchCount[_stagingSet] >= MAX_BATCH)
                    {
                        //throw new Exception("Batch count is not large enough");
                        return; // Drop the batch for now
                    }
                    // Add a new batch
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._bufferIndex = drawparams._bufferIndex;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._pipeline = pipeline;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._objectRS = drawparams._objectResourceSet;
                    _batches[MAX_BATCH * _stagingSet + _batchCount[_stagingSet]]._indexFormat = drawparams._indexFormat;
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
                    GlobalTexturePool.BindTexturePool(cl, 2);
                    GlobalCubeTexturePool.BindTexturePool(cl, 3);
                    MaterialBufferAllocator.BindAsResourceSet(cl, 4);
                    BoneBufferAllocator.BindAsResourceSet(cl, 7);
                    cl.SetGraphicsResourceSet(5, SamplerSet.SamplersSet);
                    
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

        /// <summary>
        /// Simple interface for various objects that may need to be updated by the renderer.
        /// Note that the objects will need to be submitted to the renderer every time they
        /// need to be constructed or updated.
        /// </summary>
        public interface IRendererUpdatable
        {
            /// <summary>
            /// Called when this object is scheduled on the renderer to create GPU resources and renderables
            /// </summary>
            public void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);

            /// <summary>
            /// Called when the object is scheduled to have renderables updated, but no new renderables
            /// are added or deleted
            /// </summary>
            public void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);

            /// <summary>
            /// Called when the object is scheduled to destroy renderables under its control
            /// </summary>
            public void DestroyRenderables();
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

            // Number of frames in flight planned for this queue
            private int _bufferCount = 3;
            private int _currentBuffer = 0;
            private int _nextBuffer { get => (_currentBuffer + 1) % _bufferCount; }
            private int _prevBuffer { get => (_currentBuffer - 1 + BUFFER_COUNT) % _bufferCount; }

            public SceneRenderPipeline Pipeline { get; private set; }
            private GraphicsDevice Device;
            private CommandList ResourceUpdateCommandList;
            //private CommandList DrawCommandList;
            private List<Fence> _resourcesUpdatedFence = new List<Fence>();
            private List<Fence> _drawFence = new List<Fence>();

            //private IndirectDrawEncoder DrawEncoder;
            private List<IndirectDrawEncoder> _drawEncoders = new List<IndirectDrawEncoder>();

            private readonly List<KeyIndex> Indices = new List<KeyIndex>(1000);
            private readonly List<int> Renderables = new List<int>(1000);

            private MeshDrawParametersComponent[] _drawParameters = null;
            private Pipeline[] _pipelines = null;

            private Action<GraphicsDevice, CommandList> PreDrawSetup = null;

            public int Count => Renderables.Count;

            public float CPURenderTime { get; private set; } = 0.0f;

            private string Name;

            public RenderQueue(string name, GraphicsDevice device, SceneRenderPipeline pipeline)
            {
                Device = device;
                Pipeline = pipeline;
                ResourceUpdateCommandList = device.ResourceFactory.CreateCommandList();
                _bufferCount = BUFFER_COUNT;
                //DrawCommandList = device.ResourceFactory.CreateCommandList();
                // Create per frame in flight resources
                for (int i = 0; i < _bufferCount; i++)
                {
                    _drawEncoders.Add(new IndirectDrawEncoder(CFG.Current.GFX_Limit_Buffer_Indirect_Draw));
                    _resourcesUpdatedFence.Add(device.ResourceFactory.CreateFence(i != 0));
                }
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
                _drawEncoders[_nextBuffer].Reset();
                _resourcesUpdatedFence[_nextBuffer].Reset();

                _currentBuffer = _nextBuffer;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(int item, RenderKey key)
            {
                int index = Renderables.Count;
                Indices.Add(new KeyIndex(key, index));
                Renderables.Add(item);
            }

            public void SetDrawParameters(MeshDrawParametersComponent[] parameters, Pipeline[] pipelines)
            {
                _drawParameters = parameters;
                _pipelines = pipelines;
            }

            private void Sort()
            {
                Indices.Sort();
            }

            public void Execute(CommandList drawCommandList, Fence lastOutstandingDrawFence)
            {
                if (_drawParameters == null)
                {
                    return;
                }
                var watch = Stopwatch.StartNew();

                // Build draws for current frame and kick off a buffer update
                var ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute Sort");
                Sort();
                Tracy.TracyCZoneEnd(ctx);

                ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute pre-draw");
                ResourceUpdateCommandList.Begin();
                ResourceUpdateCommandList.PushDebugGroup($@"{Name}: Update resources");
                PreDrawSetup.Invoke(Device, drawCommandList);
                ResourceUpdateCommandList.PopDebugGroup();
                Tracy.TracyCZoneEnd(ctx);

                ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute encode draws");
                foreach (var obj in Indices)
                {
                    var o = Renderables[obj.ItemIndex];
                    _drawEncoders[_nextBuffer].AddDraw(ref _drawParameters[o], _pipelines[o]);
                }
                Tracy.TracyCZoneEnd(ctx);

                ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute waiting for fence (stall)");
                Device.WaitForFence(lastOutstandingDrawFence);
                Tracy.TracyCZoneEnd(ctx);

                ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute update indirect buffer");
                ResourceUpdateCommandList.InsertDebugMarker($@"{Name}: Indirect buffer update");
                _drawEncoders[_currentBuffer].UpdateBuffer(ResourceUpdateCommandList);
                ResourceUpdateCommandList.PopDebugGroup();
                ResourceUpdateCommandList.End();
                Device.SubmitCommands(ResourceUpdateCommandList, _resourcesUpdatedFence[_currentBuffer]);
                Tracy.TracyCZoneEnd(ctx);

                // Wait on the last outstanding frame in flight and submit the draws
                //Device.WaitForFence(_resourcesUpdatedFence[_nextBuffer], ulong.MaxValue - 1);
                ctx = Tracy.TracyCZoneN(1, "RenderQueue::Execute submit draw");
                drawCommandList.InsertDebugMarker($@"{Name}: Draw");
                _drawEncoders[_currentBuffer].SubmitBatches(drawCommandList, Pipeline);
                drawCommandList.PopDebugGroup();
                Tracy.TracyCZoneEnd(ctx);
                watch.Stop();
                CPURenderTime = (float)(((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);
            }
        }

        private static GraphicsDevice Device;
        private static CommandList MainCommandList;

        private static List<RenderQueue> RenderQueues;
        private static Queue<Action<GraphicsDevice, CommandList>> BackgroundUploadQueue;
        private static Queue<Action<GraphicsDevice, CommandList>> LowPriorityBackgroundUploadQueue;
        private static Queue<Action<GraphicsDevice, CommandList>> LowPriorityBackgroundUploadQueueBackfill;

        private static Fence _readbackFence;
        private static CommandList _readbackCommandList;
        private static Queue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)> _readbackQueue;
        private static Queue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)> _readbackPendingQueue;

        private static CommandList TransferCommandList;
        private static ConcurrentQueue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)> _asyncTransfersPendingQueue;
        private static List<(Fence, Action<GraphicsDevice>)> _asyncTransfers;
        private static Queue<Fence> _freeTransferFences;

        private static bool _readyForReadback = false;
        private static int _readbackPendingFence = -1;

        public static VertexIndexBufferAllocator GeometryBufferAllocator { get; private set; }
        public static GPUBufferAllocator UniformBufferAllocator { get; private set; }
        public static GPUBufferAllocator MaterialBufferAllocator { get; private set; }
        public static GPUBufferAllocator BoneBufferAllocator { get; private set; }
        public static TexturePool GlobalTexturePool { get; private set; }
        public static TexturePool GlobalCubeTexturePool { get; private set; }

        private static int BUFFER_COUNT = 3;
        private static List<Fence> _drawFences = new List<Fence>();
        private static int _currentBuffer = 0;
        private static int _nextBuffer { get => (_currentBuffer + 1) % BUFFER_COUNT; }
        private static int _prevBuffer { get => (_currentBuffer - 1 + BUFFER_COUNT) % BUFFER_COUNT; }

        private static List<(CommandList, Fence)> _postDrawCommandLists = new List<(CommandList, Fence)>(2);

        public static ResourceFactory Factory
        {
            get
            {
                return Device.ResourceFactory;
            }
        }

        public enum DefaultTexture
        {
            Gray = 0,
            Normal = 1,
            Black = 2,
            EnvMap = 3,
        }

        public unsafe static void Initialize(GraphicsDevice device)
        {
            Device = device;
            MainCommandList = device.ResourceFactory.CreateCommandList();
            BackgroundUploadQueue = new Queue<Action<GraphicsDevice, CommandList>>();
            LowPriorityBackgroundUploadQueue = new Queue<Action<GraphicsDevice, CommandList>>(100000);
            LowPriorityBackgroundUploadQueueBackfill = new Queue<Action<GraphicsDevice, CommandList>>(100000);
            _readbackCommandList = device.ResourceFactory.CreateCommandList();
            _readbackFence = device.ResourceFactory.CreateFence(false);
            _readbackQueue = new Queue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)>();
            _readbackPendingQueue = new Queue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)>();
            RenderQueues = new List<RenderQueue>();

            TransferCommandList = device.ResourceFactory.CreateCommandList(new CommandListDescription(true));
            _asyncTransfers = new List<(Fence, Action<GraphicsDevice>)>();
            _asyncTransfersPendingQueue = new ConcurrentQueue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)>();
            _freeTransferFences = new Queue<Fence>();
            for (int i = 0; i < 3; i++)
            {
                _freeTransferFences.Enqueue(device.ResourceFactory.CreateFence(false));
            }

            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                _drawFences.Add(device.ResourceFactory.CreateFence(true));
            }

            SamplerSet.Initialize(device);

            GeometryBufferAllocator = new VertexIndexBufferAllocator(256 * 1024 * 1024, 128 * 1024 * 1024);
            UniformBufferAllocator = new GPUBufferAllocator(5 * 1024 * 1024, BufferUsage.StructuredBufferReadWrite, (uint)sizeof(InstanceData));

            MaterialBufferAllocator = new GPUBufferAllocator("materials", 5 * 1024 * 1024, BufferUsage.StructuredBufferReadWrite, (uint)sizeof(Material), ShaderStages.Fragment);
            BoneBufferAllocator = new GPUBufferAllocator("bones", CFG.Current.GFX_Limit_Buffer_Flver_Bone * 64, BufferUsage.StructuredBufferReadWrite, 64, ShaderStages.Vertex);
            GlobalTexturePool = new TexturePool(device, "globalTextures", 6000);
            GlobalCubeTexturePool = new TexturePool(device, "globalCubeTextures", 500);

            // Initialize default 2D texture at 0
            var handle = GlobalTexturePool.AllocateTextureDescriptor();
            handle.FillWithColor(device, System.Drawing.Color.Gray, "Gray");

            // Default normal at 1
            handle = GlobalTexturePool.AllocateTextureDescriptor();
            handle.FillWithColor(device, System.Drawing.Color.FromArgb(255, 0, 127, 127), "Normal");

            // Default spec at 2
            handle = GlobalTexturePool.AllocateTextureDescriptor();
            handle.FillWithColor(device, System.Drawing.Color.Black, "Black");

            // Default GI envmap texture at 0
            handle = GlobalCubeTexturePool.AllocateTextureDescriptor();
            handle.FillWithColorCube(device, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            
            // Initialize global debug primitives
            // TODO: Make vertex buffer allocation smarter so that we don't need to do this
            DebugPrimitiveRenderableProxy.InitializeDebugMeshes();
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

        public static void AddLowPriorityBackgroundUploadTask(Action<GraphicsDevice, CommandList> action)
        {
            lock (LowPriorityBackgroundUploadQueueBackfill)
            {
                LowPriorityBackgroundUploadQueueBackfill.Enqueue(action);
            }
        }

        public static void AddAsyncReadback(DeviceBuffer dest, DeviceBuffer source, Action<GraphicsDevice> onFinished)
        {
            lock(_readbackQueue)
            {
                _readbackQueue.Enqueue((dest, source, onFinished));
            }
        }

        public static void AddAsyncTransfer(DeviceBuffer dest, DeviceBuffer source, Action<GraphicsDevice> onFinished)
        {
            _asyncTransfersPendingQueue.Enqueue((dest, source, onFinished));
        }

        public static Fence Frame(CommandList drawCommandList, bool backgroundOnly)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var ctx = Tracy.TracyCZoneN(1, "RenderQueue::Frame");
            MainCommandList.Begin();

            var ctx2 = Tracy.TracyCZoneN(1, "RenderQueue::Frame Background work");
            Queue<Action<GraphicsDevice, CommandList>> work;
            bool cleanTexPool = false;
            bool cleanCubeTexPool = false;
            lock (BackgroundUploadQueue)
            {
                var ctx3 = Tracy.TracyCZoneN(1, "Regenerate descriptor tables");
                if (GlobalTexturePool.DescriptorTableDirty)
                {
                    GlobalTexturePool.RegenerateDescriptorTables();
                    cleanTexPool = true;
                }

                if (GlobalCubeTexturePool.DescriptorTableDirty)
                {
                    GlobalCubeTexturePool.RegenerateDescriptorTables();
                    cleanCubeTexPool = true;
                }
                Tracy.TracyCZoneEnd(ctx3);

                work = new Queue<Action<GraphicsDevice, CommandList>>(BackgroundUploadQueue);
                BackgroundUploadQueue.Clear();
            }
            int workitems = work.Count();
            var ctx4 = Tracy.TracyCZoneN(1, $@"Perform {workitems} background work items");
            while (work.Count() > 0)
            {
                work.Dequeue().Invoke(Device, MainCommandList);
                //work.Dequeue().Invoke(Device, drawCommandList);
            }
            Tracy.TracyCZoneEnd(ctx4);

            // If there's no work swap to the backfill queue to try and find work
            if (LowPriorityBackgroundUploadQueue.Count == 0 || LowPriorityBackgroundUploadQueueBackfill.Count > LowPriorityBackgroundUploadQueue.Count)
            {
                lock (LowPriorityBackgroundUploadQueueBackfill)
                {
                    var temp = LowPriorityBackgroundUploadQueue;
                    LowPriorityBackgroundUploadQueue = LowPriorityBackgroundUploadQueueBackfill;
                    LowPriorityBackgroundUploadQueueBackfill = temp;
                }
            }
            ctx4 = Tracy.TracyCZoneN(1, $@"Perform {Math.Min(1000, LowPriorityBackgroundUploadQueue.Count)} low priority background work items");
            int workdone = 0;
            // We will aim to complete the background work in 12 miliseconds to try and maintain at least 30-60 FPS when loading,
            // but we will process a minimum of 500 items per frame to ensure forward progress when loading.
            while ((LowPriorityBackgroundUploadQueue.Count() > 0 && (workdone < 500 || sw.ElapsedMilliseconds <= 12)))
            {
                LowPriorityBackgroundUploadQueue.Dequeue()?.Invoke(Device, MainCommandList);
                workdone++;
            }
            sw.Stop();
            Tracy.TracyCZoneEnd(ctx4);

            ctx4 = Tracy.TracyCZoneN(1, $@"Submit background work items");
            MainCommandList.End();
            Device.SubmitCommands(MainCommandList);
            Tracy.TracyCZoneEnd(ctx4);
            Tracy.TracyCZoneEnd(ctx2);

            ctx2 = Tracy.TracyCZoneN(1, "RenderQueue::Frame Transfer work");
            // Notify finished transfers
            if (_asyncTransfers.Count > 0)
            {
                HashSet<Fence> done = new HashSet<Fence>();
                for (int i = 0; i < _asyncTransfers.Count; i++)
                {
                    if (_asyncTransfers[i].Item1.Signaled)
                    {
                        done.Add(_asyncTransfers[i].Item1);
                        _asyncTransfers[i].Item2.Invoke(Device);
                        _asyncTransfers.RemoveAt(i);
                        i--;
                    }
                }
                foreach (var f in done)
                {
                    f.Reset();
                    _freeTransferFences.Enqueue(f);
                }
            }

            // Initiate async transfers
            if (!_asyncTransfersPendingQueue.IsEmpty)
            {
                // Get a fence
                Fence fence;
                if (_freeTransferFences.Count > 0)
                {
                    fence = _freeTransferFences.Dequeue();
                }
                else
                {
                    fence = Device.ResourceFactory.CreateFence(false);
                }

                TransferCommandList.Begin();
                (DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>) t;
                while (_asyncTransfersPendingQueue.TryDequeue(out t))
                {
                    TransferCommandList.CopyBuffer(t.Item2, 0, t.Item1, 0, t.Item1.SizeInBytes);
                    _asyncTransfers.Add((fence, t.Item3));
                }
                TransferCommandList.End();
                Device.SubmitCommands(TransferCommandList, fence);
            }
            Tracy.TracyCZoneEnd(ctx2);


            if (cleanTexPool)
            {
                GlobalTexturePool.CleanTexturePool();
            }

            if (cleanCubeTexPool)
            {
                GlobalCubeTexturePool.CleanTexturePool();
            }

            if (backgroundOnly)
            {
                return null;
            }

            foreach (var rq in RenderQueues)
            {
                rq.Execute(drawCommandList, _drawFences[_nextBuffer]);
                rq.Clear();
            }

            // Handle readbacks
            ctx2 = Tracy.TracyCZoneN(1, "RenderQueue::Frame Readback work");
            if (_readbackFence.Signaled)
            {
                foreach (var entry in _readbackPendingQueue)
                {
                    entry.Item3.Invoke(Device);
                }
                _readbackPendingQueue.Clear();
                _readbackFence.Reset();
            }
            if (_readbackPendingQueue.Count == 0 && !_readyForReadback)
            {
                lock (_readbackQueue)
                {
                    if (_readbackQueue.Count > 0)
                    {
                        _readbackPendingQueue = new Queue<(DeviceBuffer, DeviceBuffer, Action<GraphicsDevice>)>(_readbackQueue);
                        _readbackQueue.Clear();
                        if (_readbackPendingQueue.Count > 0)
                        {
                            _readbackPendingFence = _currentBuffer;
                        }
                        _readyForReadback = true;
                    }
                }
            }
            else if (_readbackPendingQueue.Count > 0 && _readyForReadback && _readbackPendingFence == _currentBuffer)
            {
                _readbackCommandList.Begin();
                foreach (var entry in _readbackPendingQueue)
                {
                    _readbackCommandList.CopyBuffer(entry.Item2, 0, entry.Item1, 0, entry.Item2.SizeInBytes);
                }
                _readbackCommandList.End();
                //Device.SubmitCommands(_readbackCommandList, _readbackFence);
                _postDrawCommandLists.Add((_readbackCommandList, _readbackFence));
                _readyForReadback = false;
                _readbackPendingFence = -1;
            }
            Tracy.TracyCZoneEnd(ctx2);

            ctx2 = Tracy.TracyCZoneN(1, "RenderQueue::Frame Stall waiting for fence");
            _currentBuffer = _nextBuffer;
            Device.WaitForFence(_drawFences[_prevBuffer]);
            _drawFences[_prevBuffer].Reset();
            Tracy.TracyCZoneEnd(ctx2);
            Tracy.TracyCZoneEnd(ctx);
            return _drawFences[_prevBuffer];
        }

        public static void SubmitPostDrawCommandLists()
        {
            foreach (var cl in _postDrawCommandLists)
            {
                if (cl.Item2 != null)
                {
                    Device.SubmitCommands(cl.Item1, cl.Item2);
                }
                else
                {
                    Device.SubmitCommands(cl.Item1);
                }
            }
            _postDrawCommandLists.Clear();
        }
    }
}
