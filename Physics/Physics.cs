using BulletSharp;
using BulletSharp.Math;
using OpenTK.Mathematics;
using XGE3D.Tools.Wraps;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace XGE3D.BulletSharpPhysics
{
    public class Physics
    {
        DefaultCollisionConfiguration _collisionConfiguration;
        CollisionDispatcher _dispatcher;
        DbvtBroadphase _dbvtBroadphase;
        SequentialImpulseConstraintSolver _solver;
        public static DiscreteDynamicsWorld World;
        private static List<CollisionShape> _collisionShapes = new List<CollisionShape>();

        BulletSharp.Math.Vector3 g = new BulletSharp.Math.Vector3(0, -9.8f, 0);

        public Physics()
        {
            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _dbvtBroadphase = new DbvtBroadphase();
            _solver = new SequentialImpulseConstraintSolver();
            World = new DiscreteDynamicsWorld(_dispatcher, _dbvtBroadphase, _solver, _collisionConfiguration);

            World.SetGravity(ref g);
        }

        public virtual void Update(float elapsedTime)
        {
            World.StepSimulation(elapsedTime);
            World.ApplyGravity();
        }

        public static void AddCollisionShape(CollisionShape collisionShape)
        {
            _collisionShapes.Add(collisionShape);
        }

        public static bool RayCast(Vector3 start, Vector3 end)
        {
            BulletSharp.Math.Vector3 rayStart = Vec3TKtoBS(start);
            BulletSharp.Math.Vector3 rayEnd = Vec3TKtoBS(end);

            ClosestRayResultCallback callback = new ClosestRayResultCallback(ref rayStart, ref rayEnd);
            World.RayTest(rayStart, rayEnd, callback);

            return callback.HasHit;
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

        /*private RigidBody CreateStaticBody(Matrix startTransform, CollisionShape shape)
        {
            Vector3 localInertia = Vector3.Zero;
            return CreateBody(0, startTransform, shape, localInertia);
        }

        private RigidBody CreateDynamicBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            Vector3 localInertia = shape.CalculateLocalInertia(mass);
            return CreateBody(mass, startTransform, shape, localInertia);
        }*/

        private RigidBody CreateBody(float mass, Matrix startTransform, CollisionShape shape, BulletSharp.Math.Vector3 localInertia)
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

            BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
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


        //Convert from BS to TK
        //
        // TODO:
        // Write seperate converter class
        //
        public static Vector3 Vec3BStoTK(BulletSharp.Math.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Quaternion QuatBStoTK(BulletSharp.Math.Quaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static BulletSharp.Math.Vector3 Vec3TKtoBS(Vector3 vector)
        {
            return new BulletSharp.Math.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static BulletSharp.Math.Quaternion QuatTKtoBS(Quaternion quaternion)
        {
            return new BulletSharp.Math.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static Matrix4 Mat4BStoTK(Matrix m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }

        public static Matrix Mat4TKtoBS(Matrix4 m)
        {
            return new Matrix(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    }
}