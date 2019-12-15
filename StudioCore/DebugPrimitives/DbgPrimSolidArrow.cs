using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimSolidArrow : DbgPrimSolid
    {
        public const int Segments = 12;
        public const float TipLength = 0.25f;
        public const float TipRadius = 0.25f;
        public const float StemRadius = 0.1f;

        private DbgPrimGeometryData GeometryData = null;

        public DbgPrimSolidArrow()
        {
            //BackfaceCulling = false;

            Category = DbgPrimCategory.HkxBone;

            Transform = Transform.Default;
            //NameColor = color;
            //Name = name;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.VertBuffer, GeometryData.IndexBuffer);
            }
            else
            {
                Vector3 GetPoint(int segmentIndex, float radius, float depth)
                {
                    float horizontalAngle = (1.0f * segmentIndex / Segments) * Utils.Pi * 2.0f;
                    float x = (float)Math.Cos(horizontalAngle);
                    float y = (float)Math.Sin(horizontalAngle);
                    return new Vector3(x * radius, y * radius, depth);
                }

                Vector3 tip = Vector3.UnitZ;

                for (int i = 1; i <= Segments; i++)
                {
                    var ptBase = GetPoint(i, StemRadius, 0);

                    var prevPtBase = GetPoint(i - 1, StemRadius, 0);

                    //Face: Part of Bottom
                    AddTri(Vector3.Zero, prevPtBase, ptBase, Color.Green);
                }

                for (int i = 1; i <= Segments; i++)
                {
                    var ptBase = GetPoint(i, StemRadius, 0);
                    var ptTipStartInner = GetPoint(i, StemRadius, 1 - TipLength);

                    var prevPtBase = GetPoint(i - 1, StemRadius, 0);
                    var prevPtTipStartInner = GetPoint(i - 1, StemRadius, 1 - TipLength);

                    // Face: Base to Tip Inner
                    AddQuad(prevPtBase, prevPtTipStartInner, ptTipStartInner, ptBase, Color.Green);
                }

                for (int i = 1; i <= Segments; i++)
                {
                    var ptTipStartInner = GetPoint(i, StemRadius, 1 - TipLength);
                    var ptTipStartOuter = GetPoint(i, TipRadius, 1 - TipLength);

                    var prevPtTipStartInner = GetPoint(i - 1, StemRadius, 1 - TipLength);
                    var prevPtTipStartOuter = GetPoint(i - 1, TipRadius, 1 - TipLength);

                    // Face: Tip Start Inner to Tip Start Outer
                    AddQuad(prevPtTipStartInner, prevPtTipStartOuter, ptTipStartOuter, ptTipStartInner, Color.Green);
                }

                for (int i = 1; i <= Segments; i++)
                {
                    var ptTipStartOuter = GetPoint(i, TipRadius, 1 - TipLength);
                    var prevPtTipStartOuter = GetPoint(i - 1, TipRadius, 1 - TipLength);

                    // Face: Tip Start to Tip
                    AddTri(prevPtTipStartOuter, tip, ptTipStartOuter, Color.Green);
                }

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    VertBuffer = VertBuffer,
                    IndexBuffer = IndexBuffer,
                };
            }
        }
    }
}
