using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGE3D.Common;
using XGE3D.Core.ComponentSystem.Components;
using XGE3D.Tools.Wraps;
using static Assimp.Metadata;

namespace XGE3D.Core
{
    internal static class Visualizer
    {
        private static int bboxVBO = -1;
        private static int bboxVAO = -1;
        private static int bboxEBO = -1;

        public static void DrawBoundingBox(Mesh mesh, Shader renderShader, Camera camera)
        {
            if (mesh.GetVertices().Length == 0)
            {
                DebugLogger.Warn($"{mesh} cannot draw bounding box: there's no vertices in the mesh");
                return;
            }

            float[] bboxVerts =
            {
                -0.5f, -0.5f, -0.5f, 1.0f,
                0.5f, -0.5f, -0.5f, 1.0f,
                0.5f, 0.5f, -0.5f, 1.0f,
                -0.5f, 0.5f, -0.5f, 1.0f,
                -0.5f, -0.5f, 0.5f, 1.0f,
                0.5f, -0.5f, 0.5f, 1.0f,
                0.5f, 0.5f, 0.5f, 1.0f,
                -0.5f, 0.5f, 0.5f, 1.0f
            };

            if (bboxVBO == -1)
            {
                bboxVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, bboxVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, bboxVerts.Length * 8 * sizeof(float), bboxVerts, BufferUsageHint.StaticDraw);
            }

            int[] elements =
                {
                    0, 1, 2, 3,
                    4, 5, 6, 7,
                    0, 4, 1, 5, 2, 6, 3, 7
                };

            if (bboxEBO == -1)
            {
                bboxEBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, bboxEBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * 8 * sizeof(float), elements, BufferUsageHint.StaticDraw);
            }

            if (bboxVAO == -1)
            {
                bboxVAO = GL.GenVertexArray();
                GL.BindVertexArray(bboxVAO);
            }

            float minX, maxX;
            float minY, maxY;
            float minZ, maxZ;

            minX = maxX = mesh.GetVertices()[0].Position.X;
            minY = maxY = mesh.GetVertices()[0].Position.Y;
            minZ = maxZ = mesh.GetVertices()[0].Position.Z;

            foreach (var vert in mesh.GetVertices())
            {
                if (vert.Position.X < minX) minX = vert.Position.X;
                if (vert.Position.X > maxX) maxX = vert.Position.X;

                if (vert.Position.Y < minY) minY = vert.Position.Y;
                if (vert.Position.Y > maxY) maxY = vert.Position.Y;

                if (vert.Position.Z < minZ) minZ = vert.Position.Z;
                if (vert.Position.Z > maxZ) maxZ = vert.Position.Z;
            }

            Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            Matrix4 transform = Matrix4.CreateTranslation(center) * Matrix4.CreateScale(size);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bboxVBO);

            var vertexLocation = renderShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bboxEBO);
            GL.DrawElements(PrimitiveType.Triangles, elements.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawElements(PrimitiveType.Triangles, 4, DrawElementsType.UnsignedShort, 4 * sizeof(short));
            //GL.DrawElements(PrimitiveType.Triangles, 8, DrawElementsType.UnsignedShort, 8 * sizeof(short));

            renderShader.Use();
            renderShader.SetMatrix4("model", transform);

            renderShader.SetMatrix4("view", camera.GetViewMatrix());
            renderShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            renderShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            renderShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));

            GL.DrawArrays(PrimitiveType.Triangles, 0, bboxVerts.Length);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
