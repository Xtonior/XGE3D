using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Core.SceneSystem;

namespace XGE3D.Core
{
    public abstract class Renderer
    {
        public SceneData currentScene;

        public abstract void Initialize();
        public abstract void LoadScene(SceneData scene);
        public abstract void UpdateFrame();
        public abstract void UpdateFrame(float deltaTime);
        public abstract void RenderFrame(ComponentSystem.Components.Camera camera);
        public abstract void RenderFrame(float deltaTime);
        public abstract void UpdateViewport(int x, int y, int width, int height);
        public abstract void SetSkybox(Cubemap cubemap);
        public abstract string GetRendererInfo();
        public abstract Common.Shader GetCurrentShader();
    }
}
