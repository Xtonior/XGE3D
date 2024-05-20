using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGE3D.Physics.Shapes
{
    public class SphereShape : PhysicsShape
    {
        public BulletSharp.CollisionShape shape { get; private set; }

        public SphereShape(float radius)
        {
            shape = new BulletSharp.SphereShape(radius);
        }
    }
}
