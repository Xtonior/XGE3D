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

        public RigidBody myBody;
        public RigidBody myBody2;
        Vector3 g = new Vector3(0, -9.8f, 0);

        public CollisionObject ground;

        public Vector3 groundScale = new Vector3(3f, 0.5f, 6f);

        public Physics()
        {
            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _dbvtBroadphase = new DbvtBroadphase();
            _solver = new SequentialImpulseConstraintSolver();
            World = new DiscreteDynamicsWorld(_dispatcher, _dbvtBroadphase, _solver, _collisionConfiguration);

            World.SetGravity(ref g);

            // create the ground
            var groundShape = new BoxShape(groundScale);
            _collisionShapes.Add(groundShape);
            ground = LocalCreateRigidBody(0, Matrix.Translation(0, -5, 0), groundShape);
            ground.UserObject = "Ground";

            // create a dynamic rigidbodis
            AddRigidBody(ref myBody);
            AddRigidBody(ref myBody2);

            myBody2.Translate(new Vector3(0, 0, 1f));
        }

        public virtual void Update(float elapsedTime)
        {
            World.StepSimulation(elapsedTime);
            World.ApplyGravity();
        }

        public void Foo()
        {
            //myBody.WorldTransform = Matrix.RotationX(45f);
            myBody.WorldTransform = Matrix.Identity * Matrix.Translation(new Vector3(0, 10, .3f));
            myBody2.WorldTransform = Matrix.Identity;
            myBody.Activate();
        }

        private void AddRigidBody(ref RigidBody rBody)
        {
            float mass = 1.0f;

            var colShape = new BoxShape(1);
            _collisionShapes.Add(colShape);

            var rbInfo = new RigidBodyConstructionInfo(mass, null, colShape);
            rbInfo.LocalInertia = colShape.CalculateLocalInertia(mass);
            rbInfo.MotionState = new DefaultMotionState(Matrix.Identity);
            var body = new RigidBody(rbInfo);
            body.WorldTransform = Matrix.RotationX(45f);
            rBody = body;

            World.AddRigidBody(body);
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

            _collisionConfiguration.Dispose();
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

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            var myMotionState = new DefaultMotionState(startTransform);

            using (var rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia))
            {
                var body = new RigidBody(rbInfo);
                World.AddRigidBody(body);
                return body;
            }
        }

        public Matrix4 ConvertFromBulletToOpenTK(BulletSharp.Math.Matrix m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    }
}