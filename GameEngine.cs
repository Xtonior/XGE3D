using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using XGE3D.Core;
using XGE3D.Core.SceneSystem;

namespace XGE3D
{
    public static class GameEngine
    {
        public static Engine engine { get; private set; } = new Engine();
        public static RenderEngine renderEngine { get; private set; } = new RenderEngine(); 
        public static PhysicsEngine physicsEngine { get; private set; } = new PhysicsEngine();
        private static Window gameWindow; 

        public static Window GetWindow()
        {
            return gameWindow;
        }

        public static void Start(GameScript mainScript)
        {
            Main();

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "XGE",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                gameWindow = window;
                engine.Run();
                mainScript.Initialize();
                window.Run();
            }
        }

        public static void SetCurrentScene(SceneData scene)
        {
            engine.LoadScene(scene);
        }

        private static void Main()
        {

        }
    }
}