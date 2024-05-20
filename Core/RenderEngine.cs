using Assimp.Unmanaged;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Core.ComponentSystem.Components;
using XGE3D.Core.SceneSystem;

namespace XGE3D.Core
{
    public class RenderEngine
    {
        public static Renderer CurrentRenderer { get; private set; }
        public static Camera MainCamera { get; private set; }
        public static Action OnRender;

        public void Run(Renderer renderer, SceneData scene)
        {
            CurrentRenderer = renderer;
            CurrentRenderer.Initialize();
            CurrentRenderer.LoadScene(scene);
        }

        public void SetRenderCamera(Camera camera)
        {
            MainCamera = camera;
        }

        public void RenderFrame(float deltaTime)
        {
            CurrentRenderer.RenderFrame(MainCamera);

            for (int i = 0; i < CurrentRenderer.currentScene.Entities.Count; i++)
            {
                CurrentRenderer.currentScene.Entities[i].Update(deltaTime);
            }

            OnRender?.Invoke();

            GameEngine.GetWindow().SwapBuffers();
        }
    }
}
