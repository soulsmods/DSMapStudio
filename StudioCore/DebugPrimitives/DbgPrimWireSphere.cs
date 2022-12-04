﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireSphere : DbgPrimWire
    {
        private static DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireSphere(Transform location, float radius, Color color, int numVerticalSegments = 11, int numSidesPerSegment=12)
        {
            NameColor = color;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.GeomBuffer);
            }
            else
            {
                var topPoint = Vector3.UnitY * radius;
                var bottomPoint = -Vector3.UnitY * radius;
                var points = new Vector3[numVerticalSegments, numSidesPerSegment];

                for (int i = 0; i < numVerticalSegments; i++)
                {
                    for (int j = 0; j < numSidesPerSegment; j++)
                    {
                        float horizontalAngle = (1.0f * j / numSidesPerSegment) * Utils.Pi * 2.0f;
                        float verticalAngle = ((1.0f * (i + 1) / (numVerticalSegments + 1)) * Utils.Pi) - Utils.PiOver2;
                        float altitude = (float)Math.Sin(verticalAngle);
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
