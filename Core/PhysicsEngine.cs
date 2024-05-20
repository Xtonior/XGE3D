using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Core.ComponentSystem.Components;

namespace XGE3D.Core
{
    public class PhysicsEngine
    {
        private BulletSharpPhysics.Physics physics;
        public void Run()
        {
            physics = new BulletSharpPhysics.Physics();
        }

        public void Update(float deltaTime)
        {
            physics.Update(deltaTime);
        }
    }
}
