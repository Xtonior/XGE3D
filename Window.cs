using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using XGE3D.Core;
using XGE3D.Scripts;
using XGE3D.BulletSharpPhysics;
using XGE3D.Core.ComponentSystem.Components;

namespace XGE3D
{
    public class Window : GameWindow
    {
        private float frameTime;
        public float deltaTime { get; private set; }
        private int fps;
        private int prevFps;
        private int minFps = int.MaxValue;
        private int maxFps;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
                : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.Off;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Title += ": OpenGL Version: " + RenderEngine.CurrentRenderer.GetRendererInfo();

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            CalculateDeltaTime(e);
        }

        private void CalculateDeltaTime(FrameEventArgs e)
        {
            deltaTime = (float)e.Time;
            frameTime += (float)e.Time;
            fps++;
            if (frameTime >= 1)
            {
                frameTime = 0;

                if (fps < minFps) minFps = fps;

                if (fps > prevFps) maxFps = fps;

                prevFps = fps;
                fps = 0;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            RenderEngine.CurrentRenderer.UpdateViewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
