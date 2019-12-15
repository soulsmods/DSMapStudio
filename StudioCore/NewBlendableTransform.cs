using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace StudioCore
{
    public struct NewBlendableTransform
    {
        public Vector3 Translation;
        public Vector3 Scale;
        public Quaternion Rotation;

        public static NewBlendableTransform Identity => new NewBlendableTransform()
        {
            Translation = Vector3.Zero,
            Rotation = Quaternion.Identity,
            Scale = Vector3.One,
        };

        public static NewBlendableTransform Lerp(float lerp, NewBlendableTransform a, NewBlendableTransform b)
        {
            float posX = Utils.Lerp(a.Translation.X, b.Translation.X, lerp);
            float posY = Utils.Lerp(a.Translation.Y, b.Translation.Y, lerp);
            float posZ = Utils.Lerp(a.Translation.Z, b.Translation.Z, lerp);

            float scaleX = Utils.Lerp(a.Scale.X, b.Scale.X, lerp);
            float scaleY = Utils.Lerp(a.Scale.Y, b.Scale.Y, lerp);
            float scaleZ = Utils.Lerp(a.Scale.Z, b.Scale.Z, lerp);

            float rotationX = Utils.Lerp(a.Rotation.X, b.Rotation.X, lerp);
            float rotationY = Utils.Lerp(a.Rotation.Y, b.Rotation.Y, lerp);
            float rotationZ = Utils.Lerp(a.Rotation.Z, b.Rotation.Z, lerp);
            float rotationW = Utils.Lerp(a.Rotation.W, b.Rotation.W, lerp);

            return new NewBlendableTransform()
            {
                Translation = new Vector3(posX, posY, posZ),
                Scale = new Vector3(scaleX, scaleY, scaleZ),
                Rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW),
            };
        }

        public Matrix4x4 GetMatrixScale()
        {
            return Matrix4x4.CreateScale(Scale);
        }

        public Matrix4x4 GetMatrix()
        {
            return
                //Matrix.CreateScale(Scale) *
                Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(Rotation)) *
                //Matrix.CreateFromQuaternion(Rotation) *
                Matrix4x4.CreateTranslation(Translation);
        }
    }
}
