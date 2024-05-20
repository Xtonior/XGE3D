using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Common;
using XGE3D.Core.ComponentSystem.Components;
using XGE3D.Core.SceneSystem;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core
{
    internal class OpenGLRenderer : Renderer
    {
        private Shader lightingShader;

        private List<LightSource> lightSources = new List<LightSource>();
        private Cubemap skybox = new Cubemap("Shaders/cubemap.vert", "Shaders/cubemap.frag", "Resources/skybox");

        public override void Initialize()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Enable(EnableCap.DepthTest);

            lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
        }

        public override void LoadScene(SceneData scene)
        {
            currentScene = scene;
            string lol = scene.SceneName;

            for (int i = 0; i < scene.Entities.Count; i++)
            {
                var light = scene.Entities[i].Components.Find(x => x.GetType() == typeof(LightSource));

                if (light?.GetType() == typeof(LightSource))
                {
                    DebugLogger.Trace(this, $"Found light source {light}");
                    lightSources.Add(light as LightSource);
                }
            }
        }

        public override void UpdateFrame()
        {
            
        }

        public override void UpdateFrame(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public override string GetRendererInfo()
        {
            return GL.GetString(StringName.Version);   
        }

        public override void UpdateViewport(int x, int y, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        public override void RenderFrame(Camera camera)
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderLighting(camera);
        }

        public override void RenderFrame(float deltaTime)
        {
            throw new NotImplementedException();
        }

        private void RenderLighting(Camera camera)
        {
            if (camera == null) return;

            skybox.Render(camera, camera.GetProjectionMatrix());

            lightingShader.Use();
            lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            lightingShader.SetVector3("viewPos", camera.Position); 

            // Directional light
            lightingShader.SetVector3("dirLight.direction", lightSources[0].Direction);
            lightingShader.SetVector3("dirLight.ambient", lightSources[0].Ambient);
            lightingShader.SetVector3("dirLight.diffuse", lightSources[0].Diffuse);
            lightingShader.SetVector3("dirLight.specular", lightSources[0].Specular);

            // Point lights
            //for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                lightingShader.SetVector3($"pointLights[0].position", new Vector3(5f, 2f, 0.05f));
                lightingShader.SetVector3($"pointLights[0].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                lightingShader.SetVector3($"pointLights[0].diffuse", new Vector3(0.05f, 0.05f, 0.05f));
                lightingShader.SetVector3($"pointLights[0].specular", new Vector3(1.0f, 1.0f, 1.0f));
                lightingShader.SetFloat($"pointLights[0].constant", 1.0f);
                lightingShader.SetFloat($"pointLights[0].linear", 0.09f);
                lightingShader.SetFloat($"pointLights[0].quadratic", 0.032f);
            }

            //if (flashlight)
            {
                // Spot light
                lightingShader.SetVector3("spotLight.position", camera.Position);
                lightingShader.SetVector3("spotLight.direction", camera.Front);
                lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
                lightingShader.SetVector3("spotLight.diffuse", new Vector3(0.3f, 0.3f, 0.3f));
                lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
                lightingShader.SetFloat("spotLight.constant", .3f);
                lightingShader.SetFloat("spotLight.linear", 0.59f);
                lightingShader.SetFloat("spotLight.quadratic", 0.032f);
                lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(27.5f)));
            }
            /*else
            {
                // Spot light
                lightingShader.SetVector3("spotLight.position", camera.Position);
                lightingShader.SetVector3("spotLight.direction", camera.Front);
                lightingShader.SetVector3("spotLight.ambient", Vector3.Zero);
                lightingShader.SetVector3("spotLight.diffuse", Vector3.Zero);
                lightingShader.SetVector3("spotLight.specular", Vector3.Zero);
                lightingShader.SetFloat("spotLight.constant", .3f);
                lightingShader.SetFloat("spotLight.linear", 0.59f);
                lightingShader.SetFloat("spotLight.quadratic", 0.032f);
                lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(27.5f)));
            }*/
        }

        public override void SetSkybox(Cubemap cubemap)
        {
            skybox = cubemap;
            skybox.Load();
        }

        public override Shader GetCurrentShader()
        {
            return lightingShader;
        }
    }
}
