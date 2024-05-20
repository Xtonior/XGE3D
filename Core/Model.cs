using Assimp;
using OpenTK.Mathematics;
using static XGE3D.Core.Mesh;
using XGE3D.Common;
using Texture = XGE3D.Core.Mesh.Texture;
using System.IO;
using System.Linq;
using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using XGE3D.Tools.Wraps;

namespace XGE3D.Core
{
    public class Model
    {
        // model data
        private List<Mesh> meshes = new List<Mesh>();

        private List<Texture> _texturesLoaded = new List<Texture>();

        public List<Mesh> GetMeshes() => meshes;

        // constructor, expects a filepath to a 3D model.
        public Model(string path, string texturesFolder)
        {
            LoadModel(path, texturesFolder);
        }

        public void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }

        private void LoadModel(string path, string texturesFolder)
        {
            //Create a new importer
            AssimpContext importer = new AssimpContext();

            //This is how we add a logging callback
            LogStream logstream = new LogStream(delegate (String msg, String userData)
            {
                DebugLogger.Trace(this, msg);
            });
            logstream.Attach();

            //Import the model. All configs are set. The model
            //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate);

            // check for errors
            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                DebugLogger.Trace(this, "Unable to load model from: " + path);
                return;
            }

            //Reset the meshes and textures
            meshes = new List<Mesh>();

            //Set the scale of the model
            float scale = 1 / 200.0f;
            Matrix4x4 scalingMatrix = new Matrix4x4(scale, 0, 0, 0, 0, scale, 0, 0, 0, 0, scale, 0, 0, 0, 0, 1);

            // process ASSIMP's root node recursively. We pass in the scaling matrix as the first transform
            ProcessNode(scene.RootNode, scene, scalingMatrix, texturesFolder);

            importer.Dispose();
        }

        private void ProcessNode(Node node, Scene scene, Matrix4x4 parentTransform, string texturesFolder)
        {
            //Multiply the transform of each node by the node of the parent, this will place the meshes in the correct relative location
            Matrix4x4 transform = node.Transform * parentTransform;

            // process each mesh located at the current node
            for (int i = 0; i < node.MeshCount; i++)
            {
                // the node object only contains indices to index the actual objects in the scene.
                // the scene contains all the data, node is just to keep stuff organized (like relations between nodes).
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, transform, scene, texturesFolder));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, transform, texturesFolder);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Matrix4x4 transform, Scene scene, string texturesFolder)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();
                // process vertex positions, normals and texture coordinates
                var transformedVertex = transform * mesh.Vertices[i];
                Vector3 vector;
                vector.X = transformedVertex.X;
                vector.Y = transformedVertex.Y;
                vector.Z = transformedVertex.Z;
                vertex.Position = vector;

                var transformedNormal = transform * mesh.Normals[i];
                vector.X = transformedNormal.X;
                vector.Y = transformedNormal.Y;
                vector.Z = transformedNormal.Z;
                vertex.Normal = vector;

                if (mesh.HasTextureCoords(0)) // does the mesh contain texture coordinates?
                {
                    Vector2 vec;
                    vec.X = mesh.TextureCoordinateChannels[0][i].X;
                    vec.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vec;
                }
                else vertex.TexCoords = new Vector2(0.0f, 0.0f);

                vertices.Add(vertex);
            }

            // process indices
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add(face.Indices[j]);
            }

            // process material
            if (mesh.MaterialIndex >= 0)
            {
                Material material = scene.Materials[mesh.MaterialIndex];

                List<Texture> diffuseMaps = loadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse", texturesFolder);

                for (int i = 0; i < diffuseMaps.Count; i++)
                {
                    textures.Add(diffuseMaps[i]);
                }

                List<Texture> specularMaps = loadMaterialTextures(material, TextureType.Specular, "texture_specular", texturesFolder);

                for (int i = 0; i < specularMaps.Count; i++)
                {
                    textures.Add(specularMaps[i]);
                }
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), textures.ToArray());
        }

        private List<Texture> loadMaterialTextures(Material mat, TextureType type, string typeName, string path)
        {
            List<Texture> textures = new List<Texture>();

            //No textuers
            if (mat.GetMaterialTextureCount(type) == 0)
            {
                TextureSlot textureSlot = new TextureSlot();
                string str = path;

                Texture texture;

                texture.id = Common.Texture.LoadFromFile(str);
                texture.type = typeName;
                texture.path = str;

                textures.Add(texture);

                // занесем текстуру в список уже загруженных
                _texturesLoaded.Add(texture);
            }

            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot textureSlot;

                mat.GetMaterialTexture(type, i, out textureSlot);

                string str = path;

                bool skip = false;

                for (int j = 0; j < _texturesLoaded.Count; j++)
                {
                    if (_texturesLoaded[j].path == str)
                    {
                        textures.Add(_texturesLoaded[j]);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {   // если текстура не была загружена – сделаем это
                    Texture texture;

                    texture.id = Common.Texture.LoadFromFile(str);
                    texture.type = typeName;
                    texture.path = str;

                    textures.Add(texture);

                    // занесем текстуру в список уже загруженных
                    _texturesLoaded.Add(texture);
                }
            }

            return textures;
        }
    }
}
