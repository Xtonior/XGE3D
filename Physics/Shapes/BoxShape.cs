using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGE3D.Physics.Shapes
{
    public class BoxShape : PhysicsShape
    {
        public BulletSharp.CollisionShape shape { get; private set; }

        public BoxShape(float halfExtent)
        {
            shape = new BulletSharp.BoxShape(halfExtent);
        }

        public BoxShape(float halfExtentX, float halfExtentY, float halfExtentZ)
        {
            shape = new BulletSharp.BoxShape(halfExtentX, halfExtentY, halfExtentZ);
        }
    }
}
