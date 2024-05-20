using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Core.ComponentSystem;
using XGE3D.Core.ComponentSystem.Components;
using System.Xml.Schema;
using System.Xml;

namespace XGE3D.Scripts
{
    internal class FPController : Component
    {
        private float speed = 6000f;
        private Vector3 moveDir;

        public override XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
        }

        public void Move(Vector2 input, Vector3 forward, Vector3 right)
        {
            moveDir = forward * input.X + right * input.Y;
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            ParentEntity.GetComponent<BodyRigid>().ApplyForce(moveDir * speed * deltaTime);
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
