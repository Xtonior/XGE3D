using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using XGE3D.Common;

namespace XGE3D.Core
{
    public class Cubemap
    {
        private List<float> skyboxVertices = new List<float>
        {
	        // positions          
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f
        };

        private Shader _shader;

        private int _VAO, _VBO;

        private List<string> faces = new List<string>()
        {
            "right.jpg",
            "left.jpg",
            "top.jpg",
            "bottom.jpg",
            "front.jpg",
            "back.jpg"
        };

        private string _textureFolderPath;

        private int _texture;

        public Cubemap(string vertexShaderPath, string fragmentShaderPath, string textureFolderPath)
        {
            _shader = new Shader(vertexShaderPath, fragmentShaderPath);
            _textureFolderPath = textureFolderPath;
        }

        public void Load()
        {
            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.ToArray().Length * sizeof(float), skyboxVertices.ToArray(), BufferUsageHint.StaticDraw);

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            _texture = LoadCubemap(faces);

            _shader.Use();
            _shader.SetInt("skybox", 0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private int LoadCubemap(List<string> path)
        {
            int handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            TextureTarget[] targets =
            {
                TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapNegativeY, TextureTarget.TextureCubeMapPositiveY, 
                TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
            };

            for (int i = 0; i < faces.Count; i++)
            {
                using (Stream stream = File.OpenRead(_textureFolderPath + "/" + path[i]))
                {
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                    GL.TexImage2D(targets[i],
                    0,
                    PixelInternalFormat.Rgb,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Rgb,
                    PixelType.UnsignedByte,
                    image.Data);
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return handle;
        }

        public void Render(XGE3D.Core.ComponentSystem.Components.Camera camera, Matrix4 projection)
        {
            GL.DepthMask(false);
            //GL.DepthFunc(DepthFunction.Lequal);
            _shader.Use();

            Matrix4 view = new Matrix4(new Matrix3(camera.GetViewMatrix()));

            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_VAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _texture);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            //GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
        }
    }
}
