using Assimp;
using BulletSharp;
using BulletSharp.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Xml.Linq;
using System.Xml.Serialization;
using XGE3D.BulletSharpPhysics;
using XGE3D.Common;
using XGE3D.Core.ComponentSystem;
using XGE3D.Core.ComponentSystem.Components;
using XGE3D.Tools.Wraps;
using Camera = XGE3D.Core.ComponentSystem.Components.Camera;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace XGE3D.Core
{
    public class Entity
    {
        [XmlIgnore]
        public Transform transform { get; set; } = new Transform();

        public string Name;
        public List<Component> Components { get; private set; } = new List<Component>();
        public float Triplanar;

        public Entity()
        {

        }

        public Entity(string name)
        {
            /*transform.position = Vector3.Zero;*/
            //transform.Rotation = Quaternion.Identity;
            Name = name;
            Components.Add(transform);

            DebugLogger.Trace(this, "Creating a new Entity");
        }

        public Entity(string name, Vector3 position)
        {
            transform.ModelMatrix = Matrix4.CreateTranslation(position);
            Name = name;
            Components.Add(transform);

            DebugLogger.Trace(this, "Creating a new Entity");
        }

        public Entity(string name, Vector3 position, Vector3 scale)
        {
            transform.ModelMatrix = Matrix4.CreateTranslation(position) * Matrix4.CreateScale(scale);
            Name = name;
            Components.Add(transform);

            DebugLogger.Trace(this, "Creating a new Entity");
        }

        public void AddComponent(Component component)
        {
            component.ParentEntity = this;
            Components.Add(component);
            component.Init();

            DebugLogger.Trace(this, $"Adding component {component} to the Entity");
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component is T)
                {
                    return (T)component;
                }
            }

            return null;
        }

        public void Update(float deltaTime)
        { 
            RenderEngine.CurrentRenderer.GetCurrentShader().SetFloat("material.triplanarStrength", Triplanar);

            foreach (Component component in Components)
            {
                component.Update(deltaTime);
            }
        }
    }
}
