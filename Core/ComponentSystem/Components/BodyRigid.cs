using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using OpenTK.Mathematics;
using System.Xml;
using System.Xml.Schema;
using XGE3D.BulletSharpPhysics;
using XGE3D.Tools.Wraps;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace XGE3D.Core.ComponentSystem.Components
{
    public class BodyRigid : Component
    {
        public Vector3 AngularInertia = new Vector3(1f, 1f, 1f);
        public Vector3 LinearVelocity 
        { 
            get => BulletSharpPhysics.Physics.Vec3BStoTK(rigidBody.LinearVelocity);
            private set { } 
        }
        public Vector3 AngularVelocity { get; private set; }

        public Vector3 Position { get; private set; }

        public float Friction = 0.5f;

        private RigidBody rigidBody;
        private CollisionShape collisionShape;
        private float bodyMass = 1f;
        private bool isStatic = false;

        public BodyRigid(CollisionShape shape, float mass, bool immovable)
        {
            collisionShape = shape;
            bodyMass = mass;
            isStatic = immovable;
        }

        public override void Init()
        {   
            AddRigidBody(ref rigidBody);
        }

        public void SetPosition(Vector3 position)
        {
            rigidBody.WorldTransform = BulletSharpPhysics.Physics.Mat4TKtoBS(Matrix4.CreateTranslation(position));
            rigidBody.Activate();
        }

        public void SetRotation(Quaternion rotation)
        {
            rigidBody.WorldTransform *= BulletSharpPhysics.Physics.Mat4TKtoBS(Matrix4.CreateFromQuaternion(rotation));
            rigidBody.Activate();
        }

        public void AddLinearVelocity(Vector3 velocity)
        {
            rigidBody.LinearVelocity += BulletSharpPhysics.Physics.Vec3TKtoBS(velocity);
        }

        public void AddAngularVelocity(Vector3 velocity)
        {
            rigidBody.AngularVelocity += BulletSharpPhysics.Physics.Vec3TKtoBS(velocity);
        }

        public void SetLinearVelocity(Vector3 velocity)
        {
            rigidBody.LinearVelocity = BulletSharpPhysics.Physics.Vec3TKtoBS(velocity);
        }

        public void SetLinearVelocity(float x, float y, float z)
        {
            rigidBody.LinearVelocity = new BulletSharp.Math.Vector3(x, y, z);
        }

        public void SetAngularVelocity(Vector3 velocity)
        {
            rigidBody.AngularVelocity = BulletSharpPhysics.Physics.Vec3TKtoBS(velocity);
        }

        public void ApplyForce(Vector3 force)
        {
            rigidBody.Activate();
            rigidBody.ApplyCentralForce(BulletSharpPhysics.Physics.Vec3TKtoBS(force));
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            rigidBody.Activate();
            rigidBody.ApplyCentralImpulse(BulletSharpPhysics.Physics.Vec3TKtoBS(impulse));
        }

        public void StepSimulation(float deltaTime)
        {
            if (isStatic) return;

            // Обновление матрицы модели
            BulletSharp.Math.Vector3 position;
            BulletSharp.Math.Quaternion orientation;

            rigidBody.WorldTransform.Decompose(out _, out orientation, out position);

            Position = BulletSharpPhysics.Physics.Vec3BStoTK(position);
            AngularVelocity = BulletSharpPhysics.Physics.Vec3BStoTK(rigidBody.AngularVelocity);

            Matrix4 finalMatrix = Matrix4.CreateFromQuaternion(BulletSharpPhysics.Physics.QuatBStoTK(orientation)) * Matrix4.CreateTranslation(BulletSharpPhysics.Physics.Vec3BStoTK(position));
            ParentEntity.transform.ModelMatrix = finalMatrix * Matrix4.CreateScale(ParentEntity.transform.Scale);
        }

        private void AddRigidBody(ref RigidBody rBody)
        {
            BulletSharpPhysics.Physics.AddCollisionShape(collisionShape);

            var rbInfo = new RigidBodyConstructionInfo(bodyMass, null, collisionShape);

            if (isStatic) 
                rbInfo.LocalInertia = BulletSharp.Math.Vector3.Zero;
            else 
                rbInfo.LocalInertia = collisionShape.CalculateLocalInertia(bodyMass);

            Matrix scaleless = BulletSharpPhysics.Physics.Mat4TKtoBS(ParentEntity.transform.ModelMatrix.ClearScale());
            rbInfo.MotionState = new DefaultMotionState(scaleless);
            var body = new RigidBody(rbInfo);
            body.WorldTransform = scaleless;
            body.AngularFactor = BulletSharpPhysics.Physics.Vec3TKtoBS(AngularInertia);
            rBody = body;
            body.SetSleepingThresholds(0f, 0f);
            body.Friction = Friction;
            body.SetDamping(0.2f, 0.2f);
            BulletSharpPhysics.Physics.World.AddRigidBody(body);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            StepSimulation(deltaTime);
        }

        public void Dispose()
        {
            rigidBody.Dispose();
        }

        public override XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
           
        }
    }
}
