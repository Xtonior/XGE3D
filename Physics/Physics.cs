using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Vector3 = BulletSharp.Math.Vector3;

namespace XGE3D.BulletSharpPhysics
{
    class Physics
    {
        DefaultCollisionConfiguration _collisionConfiguration;
        CollisionDispatcher _dispatcher;
        DbvtBroadphase _dbvtBroadphase;
        SequentialImpulseConstraintSolver _solver;
        DiscreteDynamicsWorld World;
        private List<CollisionShape> _collisionShapes = new List<CollisionShape>();

        public Physics()
        {
            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _dbvtBroadphase = new DbvtBroadphase();
            _solver = new SequentialImpulseConstraintSolver();
            World = new DiscreteDynamicsWorld(_dispatcher, _dbvtBroadphase, _solver, _collisionConfiguration);
        }

        public void CraeteBox()
        {
            
        }
        public void ExitPhysics()
        {
            // remove/dispose constraints
            for (int i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            // remove the rigidbodies from the dynamics world and delete them
            for (int i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            // delete collision shapes
            foreach (CollisionShape shape in _collisionShapes)
            {
                shape.Dispose();
            }
            _collisionShapes.Clear();

            World.Dispose();
            _dbvtBroadphase.Dispose();
            if (_dispatcher != null)
            {
                _dispatcher.Dispose();
            }
            _collisionConf.Dispose();
        }

        private RigidBody CreateStaticBody(Matrix startTransform, CollisionShape shape)
        {
            Vector3 localInertia = Vector3.Zero;
            return CreateBody(0, startTransform, shape, localInertia);
        }

        private RigidBody CreateDynamicBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            Vector3 localInertia = shape.CalculateLocalInertia(mass);
            return CreateBody(mass, startTransform, shape, localInertia);
        }

        private RigidBody CreateBody(float mass, Matrix startTransform, CollisionShape shape, Vector3 localInertia)
        {
            var motionState = new DefaultMotionState(startTransform);
            using (var rbInfo = new RigidBodyConstructionInfo(mass, motionState, shape, localInertia))
            {
                var body = new RigidBody(rbInfo);
                World.AddRigidBody(body);
                return body;
            }
        }
    }
}
}
