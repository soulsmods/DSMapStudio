using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;

namespace StudioCore
{
    public class WorldView
    {
        public bool DisableAllInput = false;

        public Transform CameraTransform = Transform.Default;
        public Transform CameraOrigin = Transform.Default;
        public Transform CameraPositionDefault = Transform.Default;
        public float OrbitCamDistance = 2;
        public float ModelHeight_ForOrbitCam = 1;
        public float ModelDepth_ForOrbitCam = 1;
        public Vector3 ModelCenter_ForOrbitCam = Vector3.Zero;
        public Vector3 OrbitCamCenter = new Vector3(0, 0.5f, 0);
        public bool IsOrbitCam = false;

        private Rectangle BoundingRect;

        public Matrix4x4 WorldMatrixMOD = Matrix4x4.Identity;

        //private float ViewportAspectRatio => 1.0f * GFX.LastViewport.Width / GFX.LastViewport.Height;

        public WorldView(Rectangle bounds)
        {
            BoundingRect = bounds;
        }

        public void UpdateBounds(Rectangle bounds)
        {
            BoundingRect = bounds;
        }

        public void OrbitCamReset()
        {
            var distDetermine = Math.Max(ModelHeight_ForOrbitCam, ModelDepth_ForOrbitCam);
            //if (ViewportAspectRatio < 1)
            //    OrbitCamDistance = (float)Math.Sqrt((distDetermine * 2) / (ViewportAspectRatio * 0.66f));
            //else
            //    OrbitCamDistance = (float)Math.Sqrt(distDetermine * 2);

            OrbitCamCenter = new Vector3(0, ModelCenter_ForOrbitCam.Y, 0);

            CameraTransform.EulerRotation = CameraDefaultRot;
        }

        public Vector3 LightRotation = Vector3.Zero;
        public Vector3 LightDirectionVector => 
            Vector3.Transform(Vector3.UnitX,
            Matrix4x4.CreateRotationY(LightRotation.Y)
            * Matrix4x4.CreateRotationZ(LightRotation.Z)
            * Matrix4x4.CreateRotationX(LightRotation.X)
            );

        public Matrix4x4 MatrixWorld;
        public Matrix4x4 MatrixProjection;

        public float CameraTurnSpeedGamepad = 1.5f * 0.1f;
        public float CameraTurnSpeedMouse = 1.5f * 0.25f;

        public float CameraMoveSpeed_Slow = CFG.Current.GFX_Camera_MoveSpeed_Slow;
        public float CameraMoveSpeed_Normal = CFG.Current.GFX_Camera_MoveSpeed_Normal;
        public float CameraMoveSpeed_Fast = CFG.Current.GFX_Camera_MoveSpeed_Fast;

        public static readonly Vector3 CameraDefaultPos = new Vector3(0, 0.25f, -5);
        public static readonly Vector3 CameraDefaultRot = new Vector3(0, 0, 0);

        public void ResetCameraLocation()
        {
            CameraTransform.Position = CameraDefaultPos;
            CameraTransform.EulerRotation = CameraDefaultRot;
        }

        public void LookAtTransform(Transform t)
        {
            var newLookDir = Vector3.Normalize(t.Position - (CameraTransform.Position));
            var eu = CameraTransform.EulerRotation;
            eu.Y = (float)Math.Atan2(-newLookDir.X, newLookDir.Z);
            eu.X = (float)Math.Asin(newLookDir.Y);
            eu.Z = 0;
            CameraTransform.EulerRotation = eu;
        }

        public void GoToTransformAndLookAtIt(Transform t, float distance)
        {
            var positionOffset = Vector3.Transform(Vector3.UnitX, t.RotationMatrix) * distance;
            CameraTransform.Position = t.Position + positionOffset;
            LookAtTransform(t);
        }

        public float GetDistanceSquaredFromCamera(Transform t)
        {
            return (t.Position - GetCameraPhysicalLocation().Position).LengthSquared();
        }

        public Vector3 ROUGH_GetPointOnFloor(Vector3 pos, Vector3 dir, float stepDist)
        {
            Vector3 result = pos;
            Vector3 nDir = Vector3.Normalize(dir);
            while (result.Y > 0)
            {
                if (result.Y >= 1)
                    result += nDir * 1;
                else
                    result += nDir * stepDist;
            }
            result.Y = 0;
            return result;
        }

        public Transform GetSpawnPointFromScreenPos(Vector2 screenPos, float distance, bool faceBackwards, bool lockPitch, bool alignToFloor)
        {
            var result = Transform.Default;
            return result;
        }

        public Transform GetCameraPhysicalLocation()
        {
            var result = Transform.Default;

            return result;
        }

        public void SetCameraLocation(Vector3 pos, Vector3 rot)
        {
            CameraTransform.Position = pos;
            CameraTransform.EulerRotation = rot;
        }

        public void UpdateMatrices()
        {
            MatrixWorld = Matrix4x4.CreateRotationY(Utils.Pi)
                * Matrix4x4.CreateTranslation(0, 0, 0)
                * Matrix4x4.CreateScale(-1, 1, 1)
                // * Matrix.Invert(CameraOrigin.ViewMatrix)
                ;

        }

        public void MoveCamera(float x, float y, float z, float speed)
        {
            CameraTransform.Position += Vector3.Transform(new Vector3(x, y, z),
                CameraTransform.Rotation
                ) * speed;
        }

        public void RotateCameraOrbit(float h, float v, float speed)
        {
            var eu = CameraTransform.EulerRotation;
            eu.Y -= h * speed;
            eu.X += v * speed;
            eu.Z = 0;
            CameraTransform.EulerRotation = eu;
        }

        

        public void MoveCamera_OrbitCenterPoint_MouseDelta(Vector2 curMouse, Vector2 oldMouse)
        {
        }

        public void MoveCamera_OrbitCenterPoint(float x, float y, float z, float speed)
        {
            OrbitCamCenter += (Vector3.Transform(new Vector3(x, y, z),
                Matrix4x4.CreateRotationX(-CameraTransform.EulerRotation.X)
                * Matrix4x4.CreateRotationY(-CameraTransform.EulerRotation.Y)
                * Matrix4x4.CreateRotationZ(-CameraTransform.EulerRotation.Z)
                ) * speed) * (OrbitCamDistance * OrbitCamDistance) * 0.5f;
        }

        public void PointCameraToLocation(Vector3 location)
        {
            var newLookDir = Vector3.Normalize(location - (CameraTransform.Position));
            var eu = CameraTransform.EulerRotation;
            eu.Y = (float)Math.Atan2(newLookDir.X, newLookDir.Z);
            eu.X = (float)Math.Asin(newLookDir.Y);
            eu.Z = 0;
            CameraTransform.EulerRotation = eu;
        }


        private Vector2 mousePos = Vector2.Zero;
        private Vector2 oldMouse = Vector2.Zero;
        private int oldWheel = 0;
        private bool currentMouseClickL = false;
        private bool currentMouseClickR = false;
        private bool currentMouseClickM = false;
        private bool currentMouseClickStartedInWindow = false;
        private bool oldMouseClickL = false;
        private bool oldMouseClickR = false;
        private bool oldMouseClickM = false;
        private MouseClickType currentClickType = MouseClickType.None;
        private MouseClickType oldClickType = MouseClickType.None;
        //軌道カムトグルキー押下
        bool oldOrbitCamToggleKeyPressed = false;
        //非常に悪いカメラピッチ制限    ファトキャット
        const float SHITTY_CAM_PITCH_LIMIT_FATCAT = 0.999f;
        //非常に悪いカメラピッチ制限リミッタ    ファトキャット
        const float SHITTY_CAM_PITCH_LIMIT_FATCAT_CLAMP = 0.999f;
        const float SHITTY_CAM_ZOOM_MIN_DIST = 0.2f;

        private bool oldResetKeyPressed = false;

        private float GetGamepadTriggerDeadzone(float t, float d)
        {
            if (t < d)
                return 0;
            else if (t >= 1)
                return 0;

            return (t - d) * (1.0f / (1.0f - d));
        }

        public enum MouseClickType
        {
            None,
            Left,
            Right,
            Middle,
            Extra1,
            Extra2,
        }

        private bool MousePressed = false;
        private Vector2 MousePressedPos = new Vector2();

        public bool UpdateInput(Sdl2Window window, float dt)
        {
            if (DisableAllInput)
            {
                //oldWheel = Mouse.GetState(game.Window).ScrollWheelValue;
                return false;
            }

            float clampedLerpF = Utils.Clamp(30 * dt, 0, 1);

            mousePos = new Vector2(Utils.Lerp(oldMouse.X, InputTracker.MousePosition.X, clampedLerpF),
                Utils.Lerp(oldMouse.Y, InputTracker.MousePosition.Y, clampedLerpF));



            //KeyboardState keyboard = DBG.EnableKeyboardInput ? Keyboard.GetState() : DBG.DisabledKeyboardState;
            //int currentWheel = mouse.ScrollWheelValue;

            //bool mouseInWindow = MapStudio.Active && mousePos.X >= game.ClientBounds.Left && mousePos.X < game.ClientBounds.Right && mousePos.Y > game.ClientBounds.Top && mousePos.Y < game.ClientBounds.Bottom;

            currentClickType = MouseClickType.None;

            if (InputTracker.GetMouseButton(Veldrid.MouseButton.Left))
                currentClickType = MouseClickType.Left;
            else if (InputTracker.GetMouseButton(Veldrid.MouseButton.Right))
                currentClickType = MouseClickType.Right;
            else if (InputTracker.GetMouseButton(Veldrid.MouseButton.Middle))
                currentClickType = MouseClickType.Middle;
            else if (InputTracker.GetMouseButton(Veldrid.MouseButton.Button1))
                currentClickType = MouseClickType.Extra1;
            else if (InputTracker.GetMouseButton(Veldrid.MouseButton.Button2))
                currentClickType = MouseClickType.Extra2;
            else
                currentClickType = MouseClickType.None;

            currentMouseClickL = currentClickType == MouseClickType.Left;
            currentMouseClickR = currentClickType == MouseClickType.Right;
            currentMouseClickM = currentClickType == MouseClickType.Middle;

            if (currentClickType != MouseClickType.None && oldClickType == MouseClickType.None)
                currentMouseClickStartedInWindow = true;

            if (currentClickType == MouseClickType.None)
            {
                // If nothing is pressed, just dont bother lerping
                //mousePos = new Vector2(mouse.X, mouse.Y);
                if (MousePressed)
                {
                    mousePos = InputTracker.MousePosition;
                    Sdl2Native.SDL_WarpMouseInWindow(window.SdlWindowHandle, (int)MousePressedPos.X, (int)MousePressedPos.Y);
                    Sdl2Native.SDL_SetWindowGrab(window.SdlWindowHandle, false);
                    Sdl2Native.SDL_ShowCursor(1);
                    MousePressed = false;
                }
                return false;
            }

            bool isSpeedupKeyPressed = InputTracker.GetKey(Veldrid.Key.LShift) || InputTracker.GetKey(Veldrid.Key.RShift);
            bool isSlowdownKeyPressed = InputTracker.GetKey(Veldrid.Key.LControl) || InputTracker.GetKey(Veldrid.Key.RControl);
            bool isResetKeyPressed = InputTracker.GetKeyDown(KeyBindings.Current.Viewport_Cam_Reset);
            bool isMoveLightKeyPressed = InputTracker.GetKey(Veldrid.Key.Space);
            bool isOrbitCamToggleKeyPressed = false;// keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F);
            bool isPointCamAtObjectKeyPressed = false;// keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.T);


            if (!currentMouseClickStartedInWindow)
            {
                oldMouse = mousePos;

                if (IsOrbitCam)
                {
                    OrbitCamDistance = Math.Max(OrbitCamDistance, SHITTY_CAM_ZOOM_MIN_DIST);

                    var distanceVectorAfterMove = -Vector3.Transform(Vector3.UnitX, CameraTransform.RotationMatrixXYZ * Matrix4x4.CreateRotationY(Utils.Pi)) * new Vector3(-1, 1, 1);
                    CameraTransform.Position = (OrbitCamCenter + (distanceVectorAfterMove * (OrbitCamDistance * OrbitCamDistance)));


                }
                else
                {
                    var eu = CameraTransform.EulerRotation;
                    eu.X = Utils.Clamp(CameraTransform.EulerRotation.X, -Utils.PiOver2, Utils.PiOver2);
                    CameraTransform.EulerRotation = eu;
                }

                LightRotation.X = Utils.Clamp(LightRotation.X, -Utils.PiOver2, Utils.PiOver2);

                oldClickType = currentClickType;

                oldMouseClickL = currentMouseClickL;
                oldMouseClickR = currentMouseClickR;
                oldMouseClickM = currentMouseClickM;

                oldOrbitCamToggleKeyPressed = isOrbitCamToggleKeyPressed;

                return true;
            }

            if (currentMouseClickM && !oldMouseClickM && IsOrbitCam)
            {
                OrbitCamReset();
            }


            if (isResetKeyPressed && !oldResetKeyPressed)
            {
                ResetCameraLocation();
            }

            oldResetKeyPressed = isResetKeyPressed;

            if (isOrbitCamToggleKeyPressed && !oldOrbitCamToggleKeyPressed)
            {
                if (!IsOrbitCam)
                {
                    CameraOrigin.Position.Y = CameraPositionDefault.Position.Y;
                    OrbitCamDistance = (CameraOrigin.Position - (CameraTransform.Position)).Length();
                }
                IsOrbitCam = !IsOrbitCam;
            }

            if (isPointCamAtObjectKeyPressed)
            {
                PointCameraToLocation(CameraPositionDefault.Position);
            }

            // Change camera speed via mousewheel
            float moveMult = 1;
            float mouseWheelSpeedStep = 1.15f;
            if (InputTracker.GetMouseWheelDelta() > 0)
                moveMult *= mouseWheelSpeedStep;
            if (InputTracker.GetMouseWheelDelta() < 0)
                moveMult *= 1/mouseWheelSpeedStep;
            if (isSpeedupKeyPressed)
            {
                CameraMoveSpeed_Fast *= moveMult;
                moveMult = dt * CameraMoveSpeed_Fast;
            }
            else if (isSlowdownKeyPressed)
            {
                CameraMoveSpeed_Slow *= moveMult;
                moveMult = dt * CameraMoveSpeed_Slow;
            }
            else
            {
                CameraMoveSpeed_Normal *= moveMult;
                moveMult = dt * CameraMoveSpeed_Normal;
            }

            var cameraDist = CameraOrigin.Position - CameraTransform.Position;

            if (IsOrbitCam)
            {
                if (currentMouseClickL)
                {
                    float x = 0;
                    float z = 0;
                    float y = 0;

                    if (InputTracker.GetKeyDown(Veldrid.Key.W) && Math.Abs(cameraDist.Length()) > 0.1f)
                        z += 1;
                    if (InputTracker.GetKeyDown(Veldrid.Key.S))
                        z -= 1;
                    if (InputTracker.GetKeyDown(Veldrid.Key.E))
                        y += 1;
                    if (InputTracker.GetKeyDown(Veldrid.Key.Q))
                        y -= 1;
                    if (InputTracker.GetKeyDown(Veldrid.Key.A))
                        x -= 1;
                    if (InputTracker.GetKeyDown(Veldrid.Key.D))
                        x += 1;


                    if (Math.Abs(cameraDist.Length()) <= SHITTY_CAM_ZOOM_MIN_DIST)
                    {
                        z = Math.Min(z, 0);
                    }

                }
                else if (currentMouseClickR)
                {
                    MoveCamera_OrbitCenterPoint_MouseDelta(mousePos, oldMouse);
                    //Vector2 mouseDelta = mousePos - oldMouse;
                    //MoveCamera_OrbitCenterPoint(-mouseDelta.X, mouseDelta.Y, 0, moveMult);
                    }


                    //if (GFX.LastViewport.Bounds.Contains(mouse.Position))
                    //    OrbitCamDistance -= ((currentWheel - oldWheel) / 150f) * 0.25f;

            }
            else
            {
                float x = 0;
                float y = 0;
                float z = 0;

                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Right))
                    x += 1;
                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Left))
                    x -= 1;
                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Up))
                    y += 1;
                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Down))
                    y -= 1;
                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Forward))
                    z += 1;
                if (InputTracker.GetKey_IgnoreModifier(KeyBindings.Current.Viewport_Cam_Back))
                    z -= 1;

                MoveCamera(x, y, z, moveMult);
            }

            if (currentMouseClickR)
            {

                if (!MousePressed)
                {
                    var x = InputTracker.MousePosition.X;
                    var y = InputTracker.MousePosition.Y;
                    if (x >= BoundingRect.Left && x < BoundingRect.Right && y >= BoundingRect.Top && y < BoundingRect.Bottom)
                    {
                        MousePressed = true;
                        MousePressedPos = InputTracker.MousePosition;
                        Sdl2Native.SDL_ShowCursor(0);
                        Sdl2Native.SDL_SetWindowGrab(window.SdlWindowHandle, true);
                    }
                }
                else
                {
                    Vector2 mouseDelta = MousePressedPos - InputTracker.MousePosition;
                    Sdl2Native.SDL_WarpMouseInWindow(window.SdlWindowHandle, (int)MousePressedPos.X, (int)MousePressedPos.Y);

                    if (mouseDelta.LengthSquared() == 0)
                    {
                        // Prevents a meme
                        //oldWheel = currentWheel;
                        return true;
                    }

                    //Mouse.SetPosition(game.ClientBounds.X + game.ClientBounds.Width / 2, game.ClientBounds.Y + game.ClientBounds.Height / 2);



                    float camH = mouseDelta.X * 1 * CameraTurnSpeedMouse * 0.0160f;
                    float camV = mouseDelta.Y * -1 * CameraTurnSpeedMouse * 0.0160f;

                    if (IsOrbitCam && !isMoveLightKeyPressed)
                    {
                        if (CameraTransform.EulerRotation.X >= Utils.PiOver2 * SHITTY_CAM_PITCH_LIMIT_FATCAT)
                        {
                            camV = Math.Min(camV, 0);
                        }
                        if (CameraTransform.EulerRotation.X <= -Utils.PiOver2 * SHITTY_CAM_PITCH_LIMIT_FATCAT)
                        {
                            camV = Math.Max(camV, 0);
                        }

                        RotateCameraOrbit(camH, camV, Utils.PiOver2);
                        //PointCameraToModel();
                    }
                    else if (isMoveLightKeyPressed)
                    {
                        LightRotation.Y += camH;
                        LightRotation.X -= camV;
                    }
                    else
                    {
                        var eu = CameraTransform.EulerRotation;
                        eu.Y -= camH;
                        eu.X += camV;
                        CameraTransform.EulerRotation = eu;
                    }
                }


                //CameraTransform.Rotation.Z -= (float)Math.Cos(MathHelper.PiOver2 - CameraTransform.Rotation.Y) * camV;

                //RotateCamera(mouseDelta.Y * -0.01f * (float)moveMult, 0, 0, moveMult);
                //RotateCamera(0, mouseDelta.X * 0.01f * (float)moveMult, 0, moveMult);
            }
            else
            {
                if (MousePressed)
                {
                    Sdl2Native.SDL_WarpMouseInWindow(window.SdlWindowHandle, (int)MousePressedPos.X, (int)MousePressedPos.Y);
                    Sdl2Native.SDL_SetWindowGrab(window.SdlWindowHandle, false);
                    Sdl2Native.SDL_ShowCursor(1);
                    MousePressed = false;
                }
                if (IsOrbitCam)
                {
                    RotateCameraOrbit(0, 0, Utils.PiOver2);
                }

                if (oldMouseClickL)
                {
                    //Mouse.SetPosition((int)oldMouse.X, (int)oldMouse.Y);
                }
                //game.IsMouseVisible = true;
            }


            if (IsOrbitCam)
            {
                OrbitCamDistance = Math.Max(OrbitCamDistance, SHITTY_CAM_ZOOM_MIN_DIST);

                var distanceVectorAfterMove = -Vector3.Transform(Vector3.UnitX, CameraTransform.RotationMatrixXYZ * Matrix4x4.CreateRotationY(Utils.Pi)) * new Vector3(-1, 1, 1);
                CameraTransform.Position = (OrbitCamCenter + (distanceVectorAfterMove * (OrbitCamDistance * OrbitCamDistance)));
            }
            else
            {
                var eu = CameraTransform.EulerRotation;
                eu.X = Utils.Clamp(CameraTransform.EulerRotation.X, -Utils.PiOver2, Utils.PiOver2);
                CameraTransform.EulerRotation = eu;
            }


            LightRotation.X = Utils.Clamp(LightRotation.X, -Utils.PiOver2, Utils.PiOver2);

            oldClickType = currentClickType;

            oldMouseClickL = currentMouseClickL;
            oldMouseClickR = currentMouseClickR;
            oldMouseClickM = currentMouseClickM;

            oldOrbitCamToggleKeyPressed = isOrbitCamToggleKeyPressed;

            oldMouse = mousePos;
            return true;
        }
    }
}
