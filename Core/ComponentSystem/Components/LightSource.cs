using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XGE3D.Core.ComponentSystem.Components
{
    public class LightSource : Component
    {
        public Vector3 Direction { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }

        public LightSource()
        {

        }

        public LightSource(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
        {
            Direction = direction;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
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
            string data =
                $"\nDirection: {Direction}\n" +
                $"Ambient: {Ambient}\n" +
                $"Diffuse: {Diffuse}\n" +
                $"Specular: {Specular}\n";

            writer.WriteString(data);
        }
    }
}
