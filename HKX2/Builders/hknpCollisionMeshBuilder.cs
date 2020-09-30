using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace HKX2.Builders
{
    public class hknpCollisionMeshBuilder
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct NativeBVHNode
        {
            public float minX;
            public float maxX;
            public float minY;
            public float maxY;
            public float minZ;
            public float maxZ;
            public uint isLeaf;
            public uint primitiveCount;
            public uint firstChildOrPrimitive;
        }

        [DllImport("NavGen.dll")]
        public static extern bool BuildBVHForMesh([In] Vector3[] verts, int vcount, [In] ushort[] indices, int icount);

        [DllImport("NavGen.dll")]
        public static extern ulong GetNodeSize();

        [DllImport("NavGen.dll")]
        public static extern ulong GetBVHSize();

        [DllImport("NavGen.dll")]
        public static extern void GetBVHNodes([In, Out] NativeBVHNode[] buffer);

        private class BVNode
        {
            public uint Primitive;
            public bool IsLeaf;
            public BVNode Left;
            public BVNode Right;
            public Vector3 Min;
            public Vector3 Max;
            public uint PrimitiveCount;

            public bool IsSectionHead;

            public uint ComputePrimitiveCounts()
            {
                if (IsLeaf)
                {
                    return PrimitiveCount;
                }
                PrimitiveCount = Left.ComputePrimitiveCounts() + Right.ComputePrimitiveCounts();
                return PrimitiveCount;
            }

            /// <summary>
            /// Marks nodes that are the head of sections - independently compressed mesh
            /// chunks with their own BVH
            /// </summary>
            public void AttemptSectionSplit()
            {
                // Very simple primitive count based splitting heuristic for now
                if (!IsLeaf && PrimitiveCount > 127)
                {
                    IsSectionHead = false;
                    Left.IsSectionHead = true;
                    Right.IsSectionHead = true;
                    Left.AttemptSectionSplit();
                    Right.AttemptSectionSplit();
                }
            }
        }

        private class CollisionSection
        {
            public BVNode SectionHead;
            public List<ushort> Indices;
            public HashSet<ushort> UsedIndices;

            public void GatherSectionIndices(List<ushort> indices)
            {
                Indices = new List<ushort>();
                UsedIndices = new HashSet<ushort>();

                Stack<BVNode> tstack = new Stack<BVNode>();
                tstack.Push(SectionHead);
                BVNode n;
                while (tstack.TryPop(out n))
                {
                    if (n.IsLeaf)
                    {
                        Indices.Add(indices[(int)n.Primitive * 3]);
                        Indices.Add(indices[(int)n.Primitive * 3 + 1]);
                        Indices.Add(indices[(int)n.Primitive * 3 + 2]);
                        UsedIndices.Add(indices[(int)n.Primitive * 3]);
                        UsedIndices.Add(indices[(int)n.Primitive * 3 + 1]);
                        UsedIndices.Add(indices[(int)n.Primitive * 3 + 2]);
                    }
                    else
                    {
                        tstack.Push(n.Left);
                        tstack.Push(n.Right);
                    }
                }
            }
        }

        /// <summary>
        /// Build a material with some default values shamelessly stolen from actual
        /// DS3 cols
        /// </summary>
        /// <returns>A material</returns>
        private hknpMaterial BuildMaterial()
        {
            var mat = new hknpMaterial();
            mat.m_disablingCollisionsBetweenCvxCvxDynamicObjectsDistance = 16544;
            mat.m_dynamicFriction = 16128;
            mat.m_flags = 0;
            mat.m_fractionOfClippedImpulseToApply = 1;
            mat.m_frictionCombinePolicy = CombinePolicy.COMBINE_MIN;
            mat.m_isExclusive = 0;
            mat.m_isShared = false;
            mat.m_massChangerCategory = MassChangerCategory.MASS_CHANGER_IGNORE;
            mat.m_massChangerHeavyObjectFactor = 16256;
            mat.m_maxContactImpulse = 3.40282E+38F;
            mat.m_name = "";
            mat.m_restitution = 16076;
            mat.m_restitutionCombinePolicy = CombinePolicy.COMBINE_MAX;
            mat.m_softContactDampFactor = 0;
            mat.m_softContactForceFactor = 0;
            mat.m_softContactSeperationVelocity = new hkUFloat8();
            mat.m_softContactSeperationVelocity.m_value = 0;
            mat.m_staticFriction = 16128;
            mat.m_surfaceVelocity = null;
            mat.m_triggerManifoldTolerance = new hkUFloat8();
            mat.m_triggerManifoldTolerance.m_value = 255;
            mat.m_triggerType = TriggerType.TRIGGER_TYPE_NONE;
            mat.m_userData = 0;
            mat.m_weldingTolerance = 15692;
            return mat;
        }

        private hknpBodyCinfo BuildBodyCInfo(string name)
        {
            var cinfo = new hknpBodyCinfo();
            cinfo.m_collisionFilterInfo = 0;
            cinfo.m_collisionLookAheadDistance = 0;
            cinfo.m_flags = 1;
            cinfo.m_localFrame = null;
            cinfo.m_materialId = 0;
            cinfo.m_motionId = 2147483647;
            cinfo.m_name = name;
            cinfo.m_orientation = Quaternion.Identity;
            cinfo.m_position = new Vector4(0, 0, 0, 1);
            cinfo.m_qualityId = 255;
            cinfo.m_reservedBodyId = 2147483647;
            cinfo.m_spuFlags = 0;
            cinfo.m_userData = 0;
            return cinfo;
        }

        public void AddMesh(List<Vector3> verts, List<ushort> indices)
        {
            // Try and build the BVH for the mesh first
            var bv = verts.ToArray();
            var bi = indices.ToArray();
            bool didbuild = BuildBVHForMesh(bv, bv.Count(), bi, bi.Count());
            if (didbuild)
            {
                var nodecount = GetBVHSize();
                var nsize = GetNodeSize();
                var nodes = new NativeBVHNode[nodecount];
                GetBVHNodes(nodes);

                // Rebuild in friendlier tree form
                List<BVNode> bnodes = new List<BVNode>((int)nodecount);
                foreach (var n in nodes)
                {
                    var bnode = new BVNode();
                    bnode.Min = new Vector3(n.minX, n.minY, n.minZ);
                    bnode.Max = new Vector3(n.maxX, n.maxY, n.maxZ);
                    bnode.IsLeaf = n.isLeaf == 1;
                    bnode.PrimitiveCount = n.primitiveCount;
                    bnode.Primitive = n.firstChildOrPrimitive;
                    bnodes.Add(bnode);
                }
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].isLeaf == 0)
                    {
                        bnodes[i].Left = bnodes[(int)nodes[i].firstChildOrPrimitive];
                        bnodes[i].Right = bnodes[(int)nodes[i].firstChildOrPrimitive + 1];
                    }
                }

                // Split the mesh into sections using the BVH and primitive counts as
                // guidence
                bnodes[0].ComputePrimitiveCounts();
                bnodes[0].IsSectionHead = true;
                bnodes[0].AttemptSectionSplit();

                // Take out the section heads and replace them with new leafs that reference
                // the new section
                List<BVNode> sectionBVHs = new List<BVNode>();
                foreach (var node in bnodes)
                {
                    if (node.IsSectionHead)
                    {
                        var secnode = new BVNode();
                        secnode.Min = node.Min;
                        secnode.Max = node.Max;
                        secnode.PrimitiveCount = node.PrimitiveCount;
                        secnode.Left = node.Left;
                        secnode.Right = node.Right;
                        node.Left = null;
                        node.Right = null;
                        node.IsLeaf = true;
                        node.Primitive = (uint)sectionBVHs.Count();
                        sectionBVHs.Add(secnode);
                    }
                }

                List<CollisionSection> sections = new List<CollisionSection>();
                foreach (var b in sectionBVHs)
                {
                    var s = new CollisionSection();
                    s.SectionHead = b;
                    s.GatherSectionIndices(indices);
                    sections.Add(s);
                }

                // Count all the indices across sections to figure out what vertices need to be shared
                byte[] indicescount = new byte[indices.Count];
                foreach (var s in sections)
                {
                    foreach (var v in s.UsedIndices)
                    {
                        indicescount[v]++;
                    }
                }
                var shared = new HashSet<ushort>();
                for (ushort i = 0; i < indices.Count(); i++)
                {
                    if (indicescount[i] > 1)
                    {
                        shared.Add(i);
                    }
                }
            }
        }

        public hkRootLevelContainer CookCollision()
        {
            // Root container
            var root = new hkRootLevelContainer();
            root.m_namedVariants = new List<hkRootLevelContainerNamedVariant>();

            // Named variant
            var variant = new hkRootLevelContainerNamedVariant();
            variant.m_className = "hknpPhysicsSceneData";
            variant.m_name = "Physics Scene Data";
            root.m_namedVariants.Add(variant);

            // Physics scene data
            var physicsSceneData = new hknpPhysicsSceneData();
            variant.m_variant = physicsSceneData;
            physicsSceneData.m_systemDatas = new List<hknpPhysicsSystemData>();

            // Physics system data
            var physicsSystemData = new hknpPhysicsSystemData();
            physicsSceneData.m_systemDatas.Add(physicsSystemData);
            physicsSystemData.m_name = "Default Physics System Data";

            return root;
        }
    }
}
