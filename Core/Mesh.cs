using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Linq;
using XGE3D.Common;
using static XGE3D.Core.Mesh;

namespace XGE3D.Core
{
    public class Mesh
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexCoords;
        };

        public struct Texture
        {
            public Common.Texture id;
            public string type;
            public string path;
        };

        public Vertex[] GetVertices() => _vertices;

        private Vertex[] _vertices;
        private int[] _indices;
        private Texture[] _textures;

        private int _VAO, _VBO, _EBO;

        public Mesh(Vertex[] vertices, int[] indices, Texture[] textures)
        {
            _vertices = vertices;
            _indices = indices;
            _textures = textures;

            SetupMesh();
        }
        
        public void Draw(Shader shader)
        {
            int diffuseNr = 1;
            int specularNr = 1;

            for (int i = 0; i < _textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i); //activate texture

                string s = "";

                string number;
                string name = _textures[i].type;

                if (name == "texture_diffuse")
                    s = diffuseNr++.ToString();
                else if (name == "texture_specular")
                    s = specularNr++.ToString();

                number = s;

                shader.SetInt(("material." + name + number), i);

                GL.BindTexture(TextureTarget.Texture2D, _textures[i].id.Handle);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            GL.ActiveTexture(TextureUnit.Texture0);

            // draw mesh
            GL.BindVertexArray(_VAO);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private void SetupMesh()
        {
            _VAO = GL.GenVertexArray();
            _VBO = GL.GenBuffer();
            _EBO = GL.GenBuffer();

            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * 8 * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // vertex positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            // vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            // vertex texture coords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            GL.BindVertexArray(0);
        }
    }
}
