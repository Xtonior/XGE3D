using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XGE3D.Core.ComponentSystem.Components;

namespace XGE3D.Core.ComponentSystem
{
    /*[XmlInclude(typeof(Transform))]
    [XmlInclude(typeof(LightSource))]*/
    public abstract class Component : IXmlSerializable
    {
        public Entity ParentEntity;
        public Component() { }
        public abstract XmlSchema? GetSchema();

        public virtual void Init() { }

        public abstract void ReadXml(XmlReader reader);

        public virtual void Update(float deltaTime) { }

        public abstract void WriteXml(XmlWriter writer);
    }
}
