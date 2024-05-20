using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Core.ComponentSystem.Components;
using XGE3D.Core.SceneSystem;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core
{
    public class Engine
    {
        private SceneData currentScene;

        public void Run()
        {
            GameEngine.GetWindow().RenderFrame += UpdateRender;
            GameEngine.GetWindow().UpdateFrame += Update;

            Entity ent = new Entity("dir_light");
            LightSource ls;
            ls = new LightSource(new OpenTK.Mathematics.Vector3(-0.2f, -1.0f, -0.3f),
                                                            new OpenTK.Mathematics.Vector3(0.5f, 0.5f, 0.5f),
                                                            new OpenTK.Mathematics.Vector3(0.25f, 0.25f, 0.25f),
                                                            new OpenTK.Mathematics.Vector3(0.5f, 0.5f, 0.5f));
            ent.AddComponent(ls);
            currentScene.AddObject(ent);

            Renderer rend = new OpenGLRenderer();
            rend.SetSkybox(new Cubemap("Shaders/cubemap.vert", "Shaders/cubemap.frag", "Resources/skybox"));
            GameEngine.renderEngine.Run(rend, currentScene);
            GameEngine.physicsEngine.Run();
        }

        public void LoadScene(SceneData scene)
        {
            currentScene = scene;
        }

        private void UpdateRender(FrameEventArgs args)
        {
            GameEngine.renderEngine.RenderFrame((float)args.Time);
        }

        private void Update(FrameEventArgs args)
        {
            GameEngine.physicsEngine.Update((float)args.Time);
        }
    }
}
