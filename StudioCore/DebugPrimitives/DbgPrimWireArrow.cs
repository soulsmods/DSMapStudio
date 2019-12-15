using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireArrow : DbgPrimWire
    {
        public const int Segments = 6;

        public const float TipLength = 0.25f;
        public const float TipRadius = 0.10f;
        public const float StemRadius = 0.04f;

        public const float OrientingSpinesLength = 0.5f;

        private DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireArrow(string name, Transform location, Color color)
        {
            Category = DbgPrimCategory.HkxBone;

            Transform = location;
            NameColor = color;
            Name = name;

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

                AddLine(Vector3.Zero, Vector3.UnitY * OrientingSpinesLength);
                AddLine(Vector3.Zero, -Vector3.UnitX * OrientingSpinesLength * 0.5f);

                Vector3 tip = Vector3.UnitZ;

                for (int i = 0; i <= Segments; i++)
                {
                    var ptBase = GetPoint(i, StemRadius, 0);
                    var ptTipStartInner = GetPoint(i, StemRadius, 1 - TipLength);
                    var ptTipStartOuter = GetPoint(i, TipRadius, 1 - TipLength);

                    AddLine(ptBase, ptTipStartInner, color);
                    AddLine(ptTipStartInner, ptTipStartOuter, color);
                    AddLine(ptTipStartOuter, tip, color);

                    // Connect this vertical spline to the previous one with horizontal lines.
                    if (i > 0)
                    {
                        var prevPtBase = GetPoint(i - 1, StemRadius, 0);
                        var prevPtTipStartInner = GetPoint(i - 1, StemRadius, 1 - TipLength);
                        var prevPtTipStartOuter = GetPoint(i - 1, TipRadius, 1 - TipLength);

                        AddLine(prevPtBase, ptBase, color);
                        AddLine(prevPtTipStartInner, ptTipStartInner, color);
                        AddLine(prevPtTipStartOuter, ptTipStartOuter, color);
                    }
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
