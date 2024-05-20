using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XGE3D.Core.ComponentSystem;
using XGE3D.Tools;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core.ComponentSystem.Components
{
    public class Transform : Component
    {
        public Transform parentTransform;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }

        private Vector3 forward;
        public Vector3 Forward 
        { 
            get => forward;
            private set { }
        } 
        public Vector3 Right { get; private set; } = Vector3.UnitX;

        private Quaternion rotation;
        public Quaternion Rotation 
        {
            get => rotation;
            private set
            {

            }
        }

        private Matrix4 modelMatrix;

        public Matrix4 ModelMatrix
        {
            get { return modelMatrix; }
            set 
            { 
                modelMatrix = value;
                //modelMatrix = Matrix4.CreateTranslation(position) * Matrix4.CreateScale(scale);

                Position = modelMatrix.ExtractTranslation();
                Rotation = modelMatrix.ExtractRotation();
                Scale = modelMatrix.ExtractScale();
            }
        }

        public Action<Matrix4> OnTransformChange;

        public Transform()
        {
            if (parentTransform != null)
                parentTransform.OnTransformChange += UpdateFromParent;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Vector3 t = forward;

            t = modelMatrix.Column2.Xyz.Normalized();

            forward.X = t.Y;
            forward.Y = t.X;
            forward.Z = -t.Z;

            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }

        public void Rotate(Quaternion quaternion)
        {
            rotation = quaternion;
            UpdateMatrix();
        }

        public override XmlSchema? GetSchema()
        {
            return null;
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteString($"\nModel matrix: \n{modelMatrix}\n");
        }

        private void UpdateMatrix()
        {
            modelMatrix = Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(Position) * Matrix4.CreateScale(Scale);
            OnTransformChange?.Invoke(modelMatrix);
        }

        private void UpdateFromParent(Matrix4 matrix)
        {
            modelMatrix *= matrix;
        }
    }
}
