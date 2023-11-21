using System;
using XGE3D.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using XGE3D.Core;
using System.Resources;
using XGE3D.GUI;
using ImGuiNET;
using BulletSharp;
using System.Drawing;
using System.Linq;
using BulletSharp.SoftBody;

namespace XGE3D
{
    // In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
    // with several point lights
    public class Window : GameWindow
    {
        private ImGuiController _controller;

        private bool _isFocused = true;

        private Common.Camera _camera;

        private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -2.3f, -2.0f),
            new Vector3(-2.0f, 2.0f, -2.0f),
            new Vector3(0.0f, 0.0f, -2.0f)
        };

        private int _vertexBufferObject;

        private int _vaoModel;

        private int _vaoLamp;

        private Shader _lampShader;

        private Shader _lightingShader;
 
        //private Shader _skyBoxShader;

        //private Cubemap _skybox;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        Random rnd = new Random();
        List<Vector3> colors = new List<Vector3>();
        private bool flashlight = false;

        private Entity _backPack;
        private Entity _character;
        private Entity _currentEntity;
        private List<Entity> _entities = new List<Entity>();

        private BulletSharpPhysics.Physics _physics;
        private float _frameTime;
        private int _fps;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.Off;
            _physics = new BulletSharpPhysics.Physics();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            //Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.3f);

            GL.Enable(EnableCap.DepthTest);

            for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                colors.Add(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
            }

            _backPack = new Entity(@"Resources\survival-guitar-backpack\bp.fbx", @"Resources\survival-guitar-backpack\diffuse.jpg", "bp");
            _character = new Entity(@"Resources\character\character.fbx", @"Resources\character\diffuse.png", "char");

            _entities.Add(_backPack);
            _entities.Add(_character);

            _currentEntity = _entities.FirstOrDefault();

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            //_skyBoxShader = new Shader("Shaders/cubemap.vert", "Shaders/cubemap.frag");

            //_skybox = new Cubemap();

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            _camera = new Camera(Vector3.UnitZ * 3, ClientSize.X / (float)ClientSize.Y);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _frameTime += (float)e.Time;
            _fps++;
            if (_frameTime >= 1)
            {
                _frameTime = 0;
                Title = $"XGE | FPS = {_fps}";
                _fps = 0;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _lightingShader.Use();

            /*
               Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
               the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
               by defining light types as classes and set their values in there, or by using a more efficient uniform approach
               by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
            */
            // Directional light
            _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.25f, 0.25f, 0.25f));
            _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            // Point lights
            for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _lightingShader.SetVector3($"pointLights[{i}].diffuse", colors[i]);
                _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            if (flashlight)
            {
                // Spot light
                _lightingShader.SetVector3("spotLight.position", _camera.Position);
                _lightingShader.SetVector3("spotLight.direction", _camera.Front);
                _lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
                _lightingShader.SetVector3("spotLight.diffuse", new Vector3(1.3f, 1.3f, 1.3f));
                _lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
                _lightingShader.SetFloat("spotLight.constant", .3f);
                _lightingShader.SetFloat("spotLight.linear", 0.59f);
                _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
                _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(27.5f)));
            }
            else
            {
                // Spot light
                _lightingShader.SetVector3("spotLight.position", _camera.Position);
                _lightingShader.SetVector3("spotLight.direction", _camera.Front);
                _lightingShader.SetVector3("spotLight.ambient", Vector3.Zero);
                _lightingShader.SetVector3("spotLight.diffuse", Vector3.Zero);
                _lightingShader.SetVector3("spotLight.specular", Vector3.Zero);
                _lightingShader.SetFloat("spotLight.constant", .3f);
                _lightingShader.SetFloat("spotLight.linear", 0.59f);
                _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
                _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(27.5f)));
            }

            /*foreach (RigidBody body in _physics.World.CollisionObjectArray)
            {
                if ("Ground".Equals(body.UserObject))
                {
                    _character.Render(_lightingShader, _camera, new Vector3(body.WorldTransform.Origin.X, body.WorldTransform.Origin.Y, body.WorldTransform.Origin.Z));
                    Console.WriteLine(body.WorldTransform.Origin);
                    continue;
                }
            }*/

            //_backPack.Render(_lightingShader, _camera, Matrix4.Identity);

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            // We use a loop to draw all the lights at the proper position
            for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
                lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLightPositions[i]);

                _lampShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            _controller.Update(this, (float)e.Time, !_isFocused);

            var p = new System.Numerics.Vector3(_currentEntity.position.X, _currentEntity.position.Y, _currentEntity.position.Z);
            var s = new System.Numerics.Vector3(_currentEntity.scale.X, _currentEntity.scale.Y, _currentEntity.scale.Z);
            var r = new System.Numerics.Vector3(_currentEntity.rotation.X, _currentEntity.rotation.Y, _currentEntity.rotation.Z);

            ImGui.BeginDisabled(_isFocused);

            ImGui.Text(_currentEntity.name);

            ImGui.Text("Properties");

            ImGui.DragFloat3("Postiton", ref p, 0.01f);
            _currentEntity.position = new Vector3(p.X, p.Y, p.Z);

            ImGui.DragFloat3("Scale", ref s, 0.01f);
            _currentEntity.scale = new Vector3(s.X, s.Y, s.Z);

            ImGui.SliderFloat3("Rotation", ref r, 0f, 360f);
            _currentEntity.rotation = new Vector3(r.X, r.Y, r.Z);

            ImGui.EndDisabled();

            ImGuiController.CheckGLError("End of frame");

            _controller.Render();

            SwapBuffers();
        }

        private int d = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //_physics.Update((float)e.Time);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;
            var mouse = MouseState;

            if (input.IsKeyPressed(Keys.Escape))
            {
                _isFocused = !_isFocused;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                //MousePosition = new Vector2i((int)MathHelper.Round(ClientSize.X * 0.5), (int)MathHelper.Round(ClientSize.Y * 0.5));
            }

            if (input.IsKeyPressed(Keys.E))
            {
                if (d >= _entities.Count - 1)
                {
                    d = 0;
                }
                else
                {
                    d++;
                }

                _currentEntity = _entities[d];
            }

            if (_isFocused)
            {
                CursorState = CursorState.Grabbed;
            }
            else
            {
                CursorState = CursorState.Normal;
                return;
            }

            const float cameraSpeed = 2.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += Vector3.UnitY * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= Vector3.UnitY * cameraSpeed * (float)e.Time; // Down
            }
            if (input.IsKeyDown(Keys.P))
            {
                _camera.Position = _currentEntity.position;
            }

            if (input.IsKeyPressed(Keys.F))
            {
                flashlight = !flashlight;
            }

            if (input.IsKeyPressed(Keys.Q))
            {
                //_physics.Foo();
            }

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!_isFocused)
            {
                _controller.MouseScroll(e.Offset);
                return;
            }

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
            _camera.AspectRatio = ClientSize.X / (float)ClientSize.Y;
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            if (!_isFocused)
            {
                _controller.PressChar((char)e.Unicode);
            }
        }

        protected override void OnUnload()
        {
            _physics.ExitPhysics();
            base.OnUnload();
        }
    }
}
