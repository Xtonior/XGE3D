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
using System.Runtime.CompilerServices;
using XGE3D.Core.ComponentSystem.Components;
using Assimp;
using Camera = XGE3D.Core.ComponentSystem.Components.Camera;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using XGE3D.Tools.Wraps;
using Quaternion = OpenTK.Mathematics.Quaternion;
using System.Text.RegularExpressions;
using XGE3D.BulletSharpPhysics;
using XGE3D.Scripts;

namespace XGE3D
{
    // In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
    // with several point lights
    public class WindowCopy : GameWindow
    {
        private ImGuiController _controller;

        private bool _isFocused = true;

        private Camera cameraComponent;
        private FPController playerController;
        private Entity player;

        private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
            -1f, -1f, -1f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             1f, -1f, -1f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             1f,  1f, -1f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             1f,  1f, -1f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -1f,  1f, -1f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -1f, -1f, -1f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -1f, -1f,  1f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             1f, -1f,  1f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             1f,  1f,  1f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             1f,  1f,  1f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -1f,  1f,  1f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -1f, -1f,  1f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -1f,  1f,  1f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -1f,  1f, -1f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -1f, -1f, -1f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -1f, -1f, -1f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -1f, -1f,  1f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -1f,  1f,  1f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             1f,  1f,  1f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             1f,  1f, -1f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             1f, -1f, -1f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             1f, -1f, -1f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             1f, -1f,  1f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             1f,  1f,  1f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -1f, -1f, -1f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             1f, -1f, -1f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             1f, -1f,  1f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             1f, -1f,  1f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -1f, -1f,  1f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -1f, -1f, -1f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -1f,  1f, -1f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             1f,  1f, -1f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             1f,  1f,  1f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             1f,  1f,  1f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -1f,  1f,  1f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -1f,  1f, -1f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
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
        private Entity cube1;
        private Entity cube2;
        private Entity cube3;
        private Entity ground;
        //private Entity _character;
        private Entity _currentEntity;
        private List<Entity> _entities = new List<Entity>();

        private BulletSharpPhysics.Physics _physics;
        private float _frameTime;
        private float _deltaTime;
        private int _fps;
        private int _prevFps;
        private int _minFps = int.MaxValue;
        private int _maxFps;

        private Cubemap _cubemap;

        public WindowCopy(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.Off;
            _physics = new BulletSharpPhysics.Physics();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Enable(EnableCap.DepthTest);

            cameraComponent = new Camera(Vector3.UnitZ * 3, ClientSize.X / (float)ClientSize.Y);

            SetupCubemap();
            Setup();

            CursorState = CursorState.Grabbed;
        }

        private void SetupCubemap()
        {
            _cubemap = new Cubemap("Shaders/cubemap.vert", "Shaders/cubemap.frag", "Resources/skybox");
            _cubemap.Load();
        }

        Model _bp;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            CalculateDeltaTime(e);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Visualizer.DrawBoundingBox(_bp.GetMeshes()[0], _lampShader, cameraComponent);

            RenderLighting();
            RenderUI((float)e.Time);

            SwapBuffers();
        }

        private void CalculateDeltaTime(FrameEventArgs e)
        {
            _deltaTime = (float)e.Time;
            _frameTime += (float)e.Time;
            _fps++;
            if (_frameTime >= 1)
            {
                _frameTime = 0;

                if (_fps < _minFps) _minFps = _fps;

                if (_fps > _prevFps) _maxFps = _fps;

                _prevFps = _fps;
                _fps = 0;
            }
        }

        private void Setup()
        {
            /*for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                colors.Add(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
            }

            player = new Entity(new Vector3(0f, 2f, 0f));
            _entities.Add(player);

            //_character = new Entity(@"Resources\character\character.fbx", @"Resources\character\diffuse.png", "char");
            _bp = new Model(@"Resources\survival-guitar-backpack\bp.fbx", @"Resources\survival-guitar-backpack\diffuse.jpg");
            _backPack = new Entity();
            _entities.Add(_backPack);
            //_entities.Add(_character);

            Model cubeModel = new Model(@"C:\Users\Xtonior\Desktop\cube.fbx", @"Resources\survival-guitar-backpack\diffuse.jpg");
            cube1 = new Entity();
            _entities.Add(cube1);
            cube2 = new Entity();
            _entities.Add(cube2);
            cube3 = new Entity();
            _entities.Add(cube3);

            ground = new Entity(new Vector3(0f, -5f, 0f), new Vector3(10f, 1f, 10f));
            _entities.Add(ground);
            _currentEntity = _entities.FirstOrDefault();

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            playerController = new FPController();
            BodyRigid playerCol = new BodyRigid(new BoxShape(0.5f, 1f, 0.5f));
            MeshRenderer playerRenderer = new MeshRenderer(cubeModel, _lightingShader, cameraComponent);
            player.AddComponent(cameraComponent);
            player.AddComponent(playerController);
            player.AddComponent(playerRenderer);
            player.AddComponent(playerCol);
            player.transform.ModelMatrix *= Matrix4.CreateScale(1f, 2f, 1f);

            BodyRigid bodyRigid =  new BodyRigid(new BoxShape(0.5f, 0.5f, 0.5f));
            MeshRenderer meshRenderer = new MeshRenderer(_bp, _lightingShader, cameraComponent);
            _backPack.AddComponent(bodyRigid);
            _backPack.AddComponent(meshRenderer);

            BodyRigid c1rb =  new BodyRigid(new BoxShape(0.5f, 0.5f, 0.5f));
            MeshRenderer c1mr = new MeshRenderer(cubeModel, _lightingShader, cameraComponent);
            cube1.AddComponent(c1rb);
            cube1.AddComponent(c1mr);
            cube1.transform.position = new Vector3(3f, 1f, 0f);

            BodyRigid c2rb = new BodyRigid(new BoxShape(0.5f, 0.5f, 0.5f));
            MeshRenderer c2mr = new MeshRenderer(cubeModel, _lightingShader, cameraComponent);
            cube1.AddComponent(c2rb);
            cube1.AddComponent(c2mr);
            cube1.transform.position = new Vector3(1f, 1f, 0f);

            BodyRigid c3rb = new BodyRigid(new BoxShape(0.5f, 0.5f, 0.5f));
            MeshRenderer c3mr = new MeshRenderer(cubeModel, _lightingShader, cameraComponent);
            cube1.AddComponent(c3rb);
            cube1.AddComponent(c3mr);
            cube1.transform.position = new Vector3(2f, 3f, 0f);

            Collider gr = new Collider(new BoxShape(5f, 0.5f, 5f));
            MeshRenderer gmr = new MeshRenderer(cubeModel, _lightingShader, cameraComponent);

            ground.AddComponent(gmr);
            ground.AddComponent(gr);

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }*/
        }

        private void RenderLighting()
        {
            _cubemap.Render(cameraComponent, cameraComponent.GetProjectionMatrix());

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
                _lightingShader.SetVector3("spotLight.position", cameraComponent.Position);
                _lightingShader.SetVector3("spotLight.direction", cameraComponent.Front);
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
                _lightingShader.SetVector3("spotLight.position", cameraComponent.Position);
                _lightingShader.SetVector3("spotLight.direction", cameraComponent.Front);
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

            //_character.Render(_lightingShader, _camera);
            

            GL.BindVertexArray(_vaoLamp);

            /*DrawCube(_physics.ConvertFromBulletToOpenTK(_physics.myBody.WorldTransform), new Vector3(0.4f, 0.2f, 0.1f), Vector3.One);
            DrawCube(_physics.ConvertFromBulletToOpenTK(_physics.myBody2.WorldTransform), new Vector3(1.0f, 0.5f, 0.31f), Vector3.One);
            DrawCube(_physics.ConvertFromBulletToOpenTK(_physics.ground.WorldTransform) * Matrix4.CreateTranslation(0f, -5f, 0f), new Vector3(0.3f, 0.7f, 0.2f), new Vector3(_physics.groundScale.X, _physics.groundScale.Y, _physics.groundScale.Z));
            */
            _lampShader.Use();

            _lampShader.SetMatrix4("view", cameraComponent.GetViewMatrix());
            _lampShader.SetMatrix4("projection", cameraComponent.GetProjectionMatrix());

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update(_deltaTime);
            }

            /*ground.transform.ModelMatrix = Matrix4.CreateTranslation(new Vector3(0f, -15f, 0f))
                * Matrix4.CreateScale(new Vector3(1f, 1f, 1f));*/
        }

        private void RenderUI(float deltaTime)
        {
            _controller.Update(this, deltaTime, !_isFocused);

            var p = new System.Numerics.Vector3(_currentEntity.transform.Position.X, _currentEntity.transform.Position.Y, _currentEntity.transform.Position.Z);
            var s = new System.Numerics.Vector3(_currentEntity.transform.Scale.X, _currentEntity.transform.Scale.Y, _currentEntity.transform.Scale.Z);
            var r = new System.Numerics.Vector3(_currentEntity.transform.Rotation.X, _currentEntity.transform.Rotation.Y, _currentEntity.transform.Rotation.Z);

            ImGui.BeginDisabled(_isFocused);

            ImGui.Text("m");

            ImGui.Text("Render:");
            ImGui.Text("Current FPS: " + _prevFps.ToString());
            ImGui.Text("Average FPS: " + Math.Floor((_maxFps + _minFps) * 0.5f).ToString());
            ImGui.Text("Min FPS: " + _minFps.ToString());
            ImGui.Text("Max FPS: " + _maxFps.ToString());

            ImGui.Text("Properties");

            ImGui.DragFloat3("Postiton", ref p, 0.01f);
            _currentEntity.transform.Position = new Vector3(p.X, p.Y, p.Z);

            ImGui.DragFloat3("Scale", ref s, 0.01f);
            _currentEntity.transform.Scale = new Vector3(s.X, s.Y, s.Z);

            /*ImGui.SliderFloat3("Rotation", ref r, 0f, 360f);
            _currentEntity.transform.rotation = new Vector3(r.X, r.Y, r.Z);*/

            ImGui.EndDisabled();

            ImGuiController.CheckGLError("End of frame");

            _controller.Render();
        }

        private int d = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _physics.Update((float)e.Time);

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

            if (input.IsKeyPressed(Keys.Q))
            {
                //_physics.Foo();
            }

            const float cameraSpeed = 2.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                cameraComponent.Position += cameraComponent.Front * cameraSpeed * (float)e.Time; // Forward
                //playerController.Move(new Vector2(0f, 1f), cameraComponent.Front, cameraComponent.Right);
            }
            if (input.IsKeyDown(Keys.S))
            {
                cameraComponent.Position -= cameraComponent.Front * cameraSpeed * (float)e.Time; // Backwards
                //playerController.Move(new Vector2(0f, -1f), cameraComponent.Front, cameraComponent.Right);
            }
            if (input.IsKeyDown(Keys.A))
            {
                cameraComponent.Position -= cameraComponent.Right * cameraSpeed * (float)e.Time; // Left
                //playerController.Move(new Vector2(-1f, 0f), cameraComponent.Front, cameraComponent.Right);
            }
            if (input.IsKeyDown(Keys.D))
            {
                cameraComponent.Position += cameraComponent.Right * cameraSpeed * (float)e.Time; // Right
                //playerController.Move(new Vector2(1f, 0f), cameraComponent.Front, cameraComponent.Right);
            }
            if (input.IsKeyDown(Keys.Space))
            {
                cameraComponent.Position += Vector3.UnitY * cameraSpeed * (float)e.Time; // Up

            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                cameraComponent.Position -= Vector3.UnitY * cameraSpeed * (float)e.Time; // Down
            }
            if (input.IsKeyDown(Keys.P))
            {
                cameraComponent.Position = _currentEntity.transform.Position;
            }
            if (input.IsKeyDown(Keys.Q))
            {
                //_backPack.GetComponent<BodyRigid>().SetPosition(new Vector3(0f, 2f, 0f));
                _backPack.GetComponent<BodyRigid>().ApplyImpulse(new Vector3(50f, 0f, 0f) * (float)e.Time);
                //_backPack.GetComponent<BodyRigid>().SetRotation(Quaternion.FromAxisAngle(Vector3.UnitX, 45f));
            }

            if (input.IsKeyPressed(Keys.F))
            {
                flashlight = !flashlight;
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

                cameraComponent.Yaw += deltaX * sensitivity;
                cameraComponent.Pitch -= deltaY * sensitivity;
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

            cameraComponent.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
            cameraComponent.AspectRatio = ClientSize.X / (float)ClientSize.Y;
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            if (!_isFocused)
            {
                //_controller.PressChar((char)e.Unicode);
            }
        }

        protected override void OnUnload()
        {
            _physics.ExitPhysics();
            base.OnUnload();
        }
    }
}
