using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireCapsule : DbgPrimWire
    {
        public class DbgPrimWireCapsule_End : DbgPrimWire
        {
            private static DbgPrimGeometryData GeometryData = null;

            public const int Segments = 12;

            public DbgPrimWireCapsule_End()
            {
                //if (!(Segments >= 4))
                //    throw new ArgumentException($"Number of segments must be >= 4", nameof(Segments));

                if (GeometryData != null)
                {
                    SetBuffers(GeometryData.VertBuffer, GeometryData.IndexBuffer);
                }
                else
                {
                    var topPoint = Vector3.UnitY * 1;
                    var bottomPoint = -Vector3.UnitY * 1;
                    var points = new Vector3[Segments, Segments];

                    int verticalSegments = Segments / 2;

                    for (int i = 0; i <= verticalSegments; i++)
                    {
                        for (int j = 0; j < Segments; j++)
                        {
                            float horizontalAngle = (1.0f * j / Segments) * Utils.Pi * 2.0f;
                            float verticalAngle = ((1.0f * (i) / (verticalSegments)) * Utils.PiOver2);
                            float altitude = (float)Math.Sin(verticalAngle);
                            float horizontalDist = (float)Math.Cos(verticalAngle);
                            points[i, j] = new Vector3((float)Math.Cos(horizontalAngle) * horizontalDist, altitude, (float)Math.Sin(horizontalAngle) * horizontalDist) * 1;
                        }
                    }

                    for (int i = 0; i <= verticalSegments; i++)
                    {
                        for (int j = 0; j < Segments; j++)
                        {
                            //// On the bottom, we must connect each to the bottom point
                            //if (i == 0)
                            //{
                            //    AddLine(points[i, j], bottomPoint, Color.White);
                            //}

                            // On the top, we must connect each point to the top
                            // Note: this isn't "else if" because with 2 segments, 
                            // these are both true for the only ring
                            if (i == Segments - 1)
                            {
                                AddLine(points[i, j], topPoint, Color.White);
                            }

                            // Make vertical lines that connect from this 
                            // horizontal ring to the one above
                            // Since we are connecting 
                            // (current) -> (the one above current)
                            // we dont need to do this for the very last one.
                            if (i < Segments - 1)
                            {
                                AddLine(points[i, j], points[i + 1, j], Color.White);
                            }


                            // Make lines that connect points horizontally
                            //---- if we reach end, we must wrap around, 
                            //---- otherwise, simply make line to next one
                            if (j == Segments - 1)
                            {
                                AddLine(points[i, j], points[i, 0], Color.White);
                            }
                            else
                            {
                                AddLine(points[i, j], points[i, j + 1], Color.White);
                            }
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

        public class DbgPrimWireCapsule_Middle : DbgPrimWire
        {
            private static DbgPrimGeometryData GeometryData = null;

            public const int Segments = 12;

            public DbgPrimWireCapsule_Middle()
            {
                if (GeometryData != null)
                {
                    SetBuffers(GeometryData.VertBuffer, GeometryData.IndexBuffer);
                }
                else
                {
                    for (int i = 0; i < Segments; i++)
                    {
                        float horizontalAngle = (1.0f * i / Segments) * Utils.Pi * 2.0f;
                        Vector3 a = new Vector3((float)Math.Cos(horizontalAngle), 0, (float)Math.Sin(horizontalAngle));
                        Vector3 b = new Vector3(a.X, 1, a.Z);
                        AddLine(a, b, Color.White);
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

        public readonly DbgPrimWireCapsule_End HemisphereA;
        public readonly DbgPrimWireCapsule_Middle Midst;
        public readonly DbgPrimWireCapsule_End HemisphereB;

        public void UpdateCapsuleEndPoints(Vector3 a, Vector3 b, float radius)
        {
            float dist = (b - a).Length();

            var mtHemisphereA = Matrix4x4.Identity;
            var mtMidst = Matrix4x4.Identity;
            var mtHemisphereB = Matrix4x4.Identity;

            mtHemisphereA *= Matrix4x4.CreateScale(Vector3.One * radius);
            mtHemisphereA *= Matrix4x4.CreateRotationX(-Utils.PiOver2);

            mtMidst *= Matrix4x4.CreateScale(new Vector3(radius, dist, radius));
            mtMidst *= Matrix4x4.CreateRotationX(Utils.PiOver2);

            mtHemisphereB *= Matrix4x4.CreateScale(Vector3.One * radius);
            mtHemisphereB *= Matrix4x4.CreateRotationX(Utils.PiOver2);
            mtHemisphereB *= Matrix4x4.CreateTranslation(new Vector3(0, 0, dist));

            HemisphereA.Transform = new Transform(mtHemisphereA);
            Midst.Transform = new Transform(mtMidst);
            HemisphereB.Transform = new Transform(mtHemisphereB);

            var forward = -Vector3.Normalize(b - a);

            Matrix4x4 hitboxMatrix = Matrix4x4.CreateWorld(a, forward, Vector3.UnitY);

            if (forward.X == 0 && forward.Z == 0)
            {
                if (forward.Y >= 0)
                    hitboxMatrix = Matrix4x4.CreateRotationX(Utils.PiOver2) * Matrix4x4.CreateTranslation(a);
                else
                    hitboxMatrix = Matrix4x4.CreateRotationX(-Utils.PiOver2) * Matrix4x4.CreateTranslation(a);
            }

            //Matrix hitboxMatrix = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.Normalize(b - a), 0)) * Matrix.CreateTranslation(a);

            Transform = new Transform(hitboxMatrix);
        }

        public DbgPrimWireCapsule(Color color)
        {
            HemisphereA = new DbgPrimWireCapsule_End()
            {
                Category = DbgPrimCategory.DummyPolyHelper,
                OverrideColor = color
            };
            Midst = new DbgPrimWireCapsule_Middle()
            {
                Category = DbgPrimCategory.DummyPolyHelper,
                OverrideColor = color
            };
            HemisphereB = new DbgPrimWireCapsule_End()
            {
                Category = DbgPrimCategory.DummyPolyHelper,
                OverrideColor = color
            };

            Children.Add(HemisphereA);
            Children.Add(Midst);
            Children.Add(HemisphereB);
        }
    }
}
