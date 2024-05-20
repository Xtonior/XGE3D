using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using XGE3D.Common;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core.ComponentSystem.Components
{
    public class MeshRenderer : Component
    {
        private Model renderModel;
        private Shader renderShader;
        private Camera renderCamera;
        private Mesh renderMesh;

        public MeshRenderer(Model model, Shader shader, Camera camera)
        {
            renderModel = model;
            renderShader = shader;
            renderCamera = camera;
        }

        public MeshRenderer(Mesh mesh, Shader shader, Camera camera)
        {
            renderShader = shader;
            renderCamera = camera;
            renderMesh = mesh;
        }

        public Model RenderModel
        {
            get { return renderModel; }
            set { renderModel = value; }
        }

        public Shader RenderShader
        {
            get { return renderShader; }
            set { renderShader = value; }
        }

        public Camera RenderCamera
        {
            get { return renderCamera; }
            set { renderCamera = value; }
        }

        public override XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            string n = ParentEntity.Name;
            Render(renderCamera);
        }

        public override void WriteXml(XmlWriter writer)
        {
            
        }

        private void Render(XGE3D.Core.ComponentSystem.Components.Camera camera)
        {
            Transform transform = ParentEntity.transform;

            renderShader.Use();
            renderShader.SetMatrix4("model", transform.ModelMatrix);

            renderShader.SetMatrix4("view", camera.GetViewMatrix());
            renderShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            renderShader.SetVector3("viewPos", camera.Position);

            renderShader.SetFloat("material.shininess", 32.0f);


            //if (renderModel != null)
                renderModel.Draw(renderShader);
            /*else if (renderMesh != null)
                renderMesh.Draw(renderShader);*/
        }
    }
}
