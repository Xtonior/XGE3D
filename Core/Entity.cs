using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Common;

namespace XGE3D.Core
{
    internal class Entity
    {
        public Model renderModel;

        public string name;

        public Matrix4 model;

        public Vector3 position { get; set; }
        public Vector3 scale = Vector3.One;
        public Vector3 rotation { get; set; }

        public Entity(string modelPath, string texturePath, string name)
        {
            renderModel = new Model(modelPath, texturePath);
            this.name = name;

            model = Matrix4.Identity
                * Matrix4.CreateTranslation(position)
                * Matrix4.CreateScale(scale)
                * Matrix4.CreateRotationX(rotation.X)
                * Matrix4.CreateRotationY(rotation.Y)
                * Matrix4.CreateRotationZ(rotation.Z);
        }

        public void Render(Shader shader, Camera camera, Vector3 pos)
        {
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            model = Matrix4.Identity
                * Matrix4.CreateTranslation(pos)
                * Matrix4.CreateScale(scale)
                * Matrix4.CreateRotationX(rotation.X)
                * Matrix4.CreateRotationY(rotation.Y)
                * Matrix4.CreateRotationZ(rotation.Z);

            shader.SetMatrix4("model", model);
            shader.SetVector3("viewPos", camera.Position);

            //_lightingShader.SetInt("material.diffuse", 0);
            //_lightingShader.SetInt("material.specular", 1);
            //_lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            shader.SetFloat("material.shininess", 32.0f);

            renderModel.Draw(shader);
        }
    }
}
