using StudioCore.DebugPrimitives;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace StudioCore
{
    public class NewAnimSkeleton
    {
        public bool BoneLimitExceeded => FlverSkeleton.Count > MaxBoneCount;

        public const int MaxBoneCount = 1024;

        public Matrix4x4[] ShaderMatrices = new Matrix4x4[1024];

        public List<FlverBoneInfo> FlverSkeleton = new List<FlverBoneInfo>();
        public List<HkxBoneInfo> HkxSkeleton = new List<HkxBoneInfo>();

        public List<int> RootBoneIndices = new List<int>();

        public HKX.HKASkeleton OriginalHavokSkeleton = null;

        public NewAnimSkeleton(List<FLVER.Bone> flverBones)
        {
            int[] childCounts = new int[flverBones.Count];

            FlverSkeleton = new List<FlverBoneInfo>();

            for (int i = 0; i < flverBones.Count; i++)
            {
                var newBone = new FlverBoneInfo(flverBones[i], flverBones);
                if (flverBones[i].ParentIndex >= 0)
                    childCounts[flverBones[i].ParentIndex]++;
                FlverSkeleton.Add(newBone);
            }

            for (int i = 0; i < FlverSkeleton.Count; i++)
            {
                FlverSkeleton[i].Length = Math.Max(0.1f, 
                    (flverBones[i].BoundingBoxMax.Z - flverBones[i].BoundingBoxMin.Z) * 0.8f);

                if (childCounts[i] == 1 && flverBones[i].ChildIndex >= 0)
                {
                    var parentChildDifference = Vector3.Transform(Vector3.Zero, 
                        FlverSkeleton[flverBones[i].ChildIndex].ReferenceMatrix) -
                        Vector3.Transform(Vector3.Zero, FlverSkeleton[i].ReferenceMatrix);

                    var parentChildDirection = Vector3.Normalize(parentChildDifference);

                    var parentDir = Vector3.TransformNormal(-Vector3.UnitZ,
                        Matrix4x4.CreateRotationX(flverBones[i].Rotation.X) *
                        Matrix4x4.CreateRotationZ(flverBones[i].Rotation.Z) *
                        Matrix4x4.CreateRotationY(flverBones[i].Rotation.Y));

                    var dot = Vector3.Dot(parentDir, parentChildDirection);

                    FlverSkeleton[i].Length = parentChildDifference.Length() * (float)Math.Cos(dot);
                }
                else
                {
                     FlverSkeleton[i].Length = Math.Max(0.1f, 
                    (flverBones[i].BoundingBoxMax.Z - flverBones[i].BoundingBoxMin.Z) * 0.8f);
                }
            }

            for (int i = 0; i < 1024; i++)
            {
                ShaderMatrices[i] = Matrix4x4.Identity;
            }
        }

        public void LoadHKXSkeleton(HKX.HKASkeleton skeleton)
        {
            OriginalHavokSkeleton = skeleton;
            HkxSkeleton.Clear();
            for (int i = 0; i < skeleton.Bones.Size; i++)
            {
                var newHkxBone = new HkxBoneInfo();
                newHkxBone.Name = skeleton.Bones[i].Name.GetString();
                newHkxBone.ParentIndex = skeleton.ParentIndices[i].data;
                newHkxBone.RelativeReferenceMatrix =
                    Matrix4x4.CreateScale(new Vector3(
                        skeleton.Transforms[i].Scale.Vector.X,
                        skeleton.Transforms[i].Scale.Vector.Y,
                        skeleton.Transforms[i].Scale.Vector.Z))
                    * Matrix4x4.CreateFromQuaternion(new Quaternion(
                        skeleton.Transforms[i].Rotation.Vector.X,
                        skeleton.Transforms[i].Rotation.Vector.Y,
                        skeleton.Transforms[i].Rotation.Vector.Z,
                        skeleton.Transforms[i].Rotation.Vector.W))
                    * Matrix4x4.CreateTranslation(new Vector3(
                        skeleton.Transforms[i].Position.Vector.X,
                        skeleton.Transforms[i].Position.Vector.Y,
                        skeleton.Transforms[i].Position.Vector.Z));

                for (int j = 0; j < FlverSkeleton.Count; j++)
                {
                    if (FlverSkeleton[j].Name == newHkxBone.Name)
                    {
                        FlverSkeleton[j].HkxBoneIndex = i;
                        newHkxBone.FlverBoneIndex = j;
                        break;
                    }
                }

                HkxSkeleton.Add(newHkxBone);
            }

            Matrix4x4 GetAbsoluteReferenceMatrix(int i)
            {
                Matrix4x4 result = Matrix4x4.Identity;

                do
                {
                    result *= HkxSkeleton[i].RelativeReferenceMatrix;
                    i = HkxSkeleton[i].ParentIndex;
                }
                while (i >= 0);

                return result;
            }

            for (int i = 0; i < HkxSkeleton.Count; i++)
            {
                HkxSkeleton[i].ReferenceMatrix = GetAbsoluteReferenceMatrix(i);
                for (int j = 0; j < HkxSkeleton.Count; j++)
                {
                    if (HkxSkeleton[j].ParentIndex == i)
                    {
                        HkxSkeleton[i].ChildIndices.Add(j);
                    }
                }
                if (HkxSkeleton[i].ParentIndex < 0)
                    RootBoneIndices.Add(i);
            }
        }

        public void SetHkxBoneMatrix(int hkxBoneIndex, Matrix4x4 m)
        {
            int flverBoneIndex = HkxSkeleton[hkxBoneIndex].FlverBoneIndex;
            if (flverBoneIndex >= 0)
            {
                this[flverBoneIndex] = Utils.Inverse(FlverSkeleton[flverBoneIndex].ReferenceMatrix) * m;
            }
        }

        public void DrawPrimitives()
        {
            //foreach (var f in FlverSkeleton)
            //    f.DrawPrim(MODEL.CurrentTransform.WorldMatrix);
        }

        public Matrix4x4 this[int boneIndex]
        {
            get
            {
                return ShaderMatrices[boneIndex];
            }
            set
            {
                FlverSkeleton[boneIndex].CurrentMatrix = FlverSkeleton[boneIndex].ReferenceMatrix * value;
                ShaderMatrices[boneIndex] = value;
            }
        }

        public void ApplyBakedFlverReferencePose()
        {
            for (int i = 0; i < FlverSkeleton.Count; i++)
            {
                this[i] = FlverSkeleton[i].ReferenceMatrix;
            }
        }

        public void RevertToReferencePose()
        {
            for (int i = 0; i < FlverSkeleton.Count; i++)
            {
                this[i] = Matrix4x4.Identity;
            }
        }

        public class FlverBoneInfo
        {
            public string Name;
            public Matrix4x4 ReferenceMatrix = Matrix4x4.Identity;
            public int HkxBoneIndex = -1;
            public Matrix4x4 CurrentMatrix = Matrix4x4.Identity;

            public float Length = 1.0f;
            public IDbgPrim BonePrim;
            public DbgPrimWireBox BoundingBoxPrim;

            public FlverBoneInfo(FLVER.Bone bone, List<FLVER.Bone> boneList)
            {
                Matrix4x4 GetBoneMatrix(SoulsFormats.FLVER.Bone b)
                {
                    SoulsFormats.FLVER.Bone parentBone = b;

                    var result = Matrix4x4.Identity;

                    do
                    {
                        result *= Matrix4x4.CreateScale(parentBone.Scale.X, parentBone.Scale.Y, parentBone.Scale.Z);
                        result *= Matrix4x4.CreateRotationX(parentBone.Rotation.X);
                        result *= Matrix4x4.CreateRotationZ(parentBone.Rotation.Z);
                        result *= Matrix4x4.CreateRotationY(parentBone.Rotation.Y);
                        result *= Matrix4x4.CreateTranslation(parentBone.Position.X, parentBone.Position.Y, parentBone.Position.Z);

                        if (parentBone.ParentIndex >= 0)
                            parentBone = boneList[parentBone.ParentIndex];
                        else
                            parentBone = null;
                    }
                    while (parentBone != null);

                    return result;
                }

                ReferenceMatrix = GetBoneMatrix(bone);
                Name = bone.Name;

                if (bone.Unk3C == 0)
                {
                    //BonePrim = new DbgPrimWireBone(bone.Name, new Transform(ReferenceMatrix), DBG.COLOR_FLVER_BONE)
                    //{
                    //    Category = DbgPrimCategory.FlverBone,
                    //};

                    /*BoundingBoxPrim = new DbgPrimWireBox(Transform.Default,
                        new Vector3(bone.BoundingBoxMin.X, bone.BoundingBoxMin.Y, bone.BoundingBoxMin.Z),
                        new Vector3(bone.BoundingBoxMax.X, bone.BoundingBoxMax.Y, bone.BoundingBoxMax.Z),
                        DBG.COLOR_FLVER_BONE_BBOX)
                    {
                        Category = DbgPrimCategory.FlverBoneBoundingBox,
                    };*/
                }
               
            }

            public void DrawPrim(Matrix4x4 world)
            {
                if (BonePrim != null && BoundingBoxPrim != null)
                {
                    //BonePrim.Transform = new Transform(Matrix.CreateScale(Length) * CurrentMatrix);
                    //BonePrim.Draw(null, world);
                    //BoundingBoxPrim.UpdateTransform(new Transform(CurrentMatrix));
                    //BoundingBoxPrim.Draw(null, world);
                }
            }
        }

        public class HkxBoneInfo
        {
            public string Name;
            public short ParentIndex = -1;
            public Matrix4x4 RelativeReferenceMatrix = Matrix4x4.Identity;
            public Matrix4x4 ReferenceMatrix = Matrix4x4.Identity;
            public int FlverBoneIndex = -1;
            public List<int> ChildIndices = new List<int>();
        }
    }
}
