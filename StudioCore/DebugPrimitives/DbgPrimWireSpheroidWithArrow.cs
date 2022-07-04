using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireSpheroidWithArrow : DbgPrimWire
    {
        private static DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireSpheroidWithArrow(Transform location, float radius, Color color, int numVerticalSegments = 11, int numSidesPerSegment = 12, bool flatBottom = false)
        {
            NameColor = color;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.GeomBuffer);
            }
            else
            {
                #region Sphere
                Vector3 topPoint;
                Vector3 bottomPoint;
                if (flatBottom)
                {
                    topPoint = Vector3.UnitY * radius;
                    bottomPoint = Vector3.Zero;
                }
                else
                {
                    topPoint = Vector3.UnitY * radius;
                    bottomPoint = -Vector3.UnitY * radius;
                }
                var points = new Vector3[numVerticalSegments, numSidesPerSegment];

                for (int i = 0; i < numVerticalSegments; i++)
                {
                    for (int j = 0; j < numSidesPerSegment; j++)
                    {
                        float horizontalAngle = (1.0f * j / numSidesPerSegment) * Utils.Pi * 2.0f;
                        float verticalAngle = ((1.0f * (i + 1) / (numVerticalSegments + 1)) * Utils.Pi) - Utils.PiOver2;
                        float altitude = (float)Math.Sin(verticalAngle);
                        if (flatBottom && altitude < 0)
                            altitude = 0f;
                        float horizontalDist = (float)Math.Cos(verticalAngle);
                        points[i, j] = new Vector3((float)Math.Cos(horizontalAngle) * horizontalDist, altitude, (float)Math.Sin(horizontalAngle) * horizontalDist) * radius;
                    }
                }

                for (int i = 0; i < numVerticalSegments; i++)
                {
                    for (int j = 0; j < numSidesPerSegment; j++)
                    {
                        // On the bottom, we must connect each to the bottom point
                        if (i == 0)
                        {
                            AddLine(points[i, j], bottomPoint, color);
                        }
                        // On the top, we must connect each point to the top
                        // Note: this isn't "else if" because with 2 segments, 
                        // these are both true for the only ring
                        if (i == numVerticalSegments - 1)
                        {
                            AddLine(points[i, j], topPoint, color);
                        }

                        // Make vertical lines that connect from this 
                        // horizontal ring to the one above
                        // Since we are connecting 
                        // (current) -> (the one above current)
                        // we dont need to do this for the very last one.
                        if (i < numVerticalSegments - 1)
                        {
                            AddLine(points[i, j], points[i + 1, j], color);
                        }


                        // Make lines that connect points horizontally
                        //---- if we reach end, we must wrap around, 
                        //---- otherwise, simply make line to next one
                        if (j == numSidesPerSegment - 1)
                        {
                            AddLine(points[i, j], points[i, 0], color);
                        }
                        else
                        {
                            AddLine(points[i, j], points[i, j + 1], color);
                        }
                    }
                }
                #endregion

                #region Arrow
                int Segments = 8;

                float TipRadius = 0.10f;

                Vector3 GetPoint(int segmentIndex, float radius, float depth)
                {
                    float horizontalAngle = (1.0f * segmentIndex / Segments) * Utils.Pi * 2.0f;
                    float x = (float)Math.Cos(horizontalAngle);
                    float y = (float)Math.Sin(horizontalAngle);
                    return new Vector3(x * radius, y * radius, depth);
                }

                //AddLine(Vector3.Zero, Vector3.UnitY * OrientingSpinesLength);
                //AddLine(Vector3.Zero, -Vector3.UnitX * OrientingSpinesLength * 0.5f);

                Vector3 tip = -Vector3.UnitZ;

                for (int i = 0; i <= Segments; i++)
                {
                    var ptTipStartOuter = GetPoint(i, TipRadius, 0);

                    AddLine(ptTipStartOuter, tip, color);

                    // Connect this vertical spline to the previous one with horizontal lines.
                    if (i > 0)
                    {
                        var prevPtTipStartOuter = GetPoint(i - 1, TipRadius, 0);

                        AddLine(prevPtTipStartOuter, ptTipStartOuter, color);
                    }
                }
                #endregion


                //FinalizeBuffers(true);

                /* //crashes
                GeometryData = new DbgPrimGeometryData()
                {
                    GeomBuffer = GeometryBuffer
                };*/
                Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                {
                    UpdatePerFrameResources(d, cl, null);
                });
            }
        }

        public bool RayCast(Ray ray, Matrix4x4 transform, out float dist)
        {
            var radius = Vector3.TransformNormal(Vector3.UnitX, transform).Length();
            var pos = Vector3.Transform(Vector3.Zero, transform);
            return Utils.RaySphereIntersection(ref ray, pos, radius, out dist);
        }

    }
}
