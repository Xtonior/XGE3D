using BulletSharp;
using BulletSharp.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using XGE3D.BulletSharpPhysics;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core.ComponentSystem.Components
{
    internal class Collider : Component
    {
        private CollisionShape collisionShape;
        private RigidBody rb;

        public Collider(CollisionShape shape)
        {
            collisionShape = shape;

            BulletSharpPhysics.Physics.AddCollisionShape(collisionShape);
        }

        public override void Init()
        {
            rb = LocalCreateRigidBody(ParentEntity.transform.ModelMatrix);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            StepSimulation(deltaTime);
        }

        public void StepSimulation(float deltaTime)
        {
            // Обновление физики
            //DynamicsWorld world = Physics.World;
            //world.StepSimulation(deltaTime);

            // Обновление матрицы модели
            /*Matrix4 scaleless = new Matrix4();
            scaleless = Matrix4.CreateTranslation(entity.transform.ModelMatrix.ExtractTranslation());
            
            //DebugLogger.Message($"{collisionShape.LocalScaling}");
            rb.WorldTransform = Physics.Mat4TKtoBS(scaleless);*/
            //collisionShape.LocalScaling = Physics.Vec3TKtoBS(entity.transform.ModelMatrix.ExtractScale());

            /*entity.transform.ModelMatrix = 
                Matrix4.CreateFromQuaternion(Physics.QuatBStoTK(orientation)) * 
                Matrix4.CreateTranslation(Physics.Vec3BStoTK(position));*/
        }

        public void Dispose()
        { 
            collisionShape.Dispose();
        }

        private RigidBody LocalCreateRigidBody(Matrix4 startTransform)
        {
            BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;

            Matrix4 scaleless = new Matrix4();
            scaleless = Matrix4.CreateTranslation(startTransform.ExtractTranslation());

            var motionState = new DefaultMotionState(BulletSharpPhysics.Physics.Mat4TKtoBS(scaleless));

            using (var rbInfo = new RigidBodyConstructionInfo(0f, motionState, collisionShape, localInertia))
            {
                var body = new RigidBody(rbInfo);
                BulletSharpPhysics.Physics.World.AddRigidBody(body);
                return body;
            }
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
            throw new NotImplementedException();
        }
    }
}
