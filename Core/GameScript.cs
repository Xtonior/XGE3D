using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGE3D.Core
{
    public abstract class GameScript
    {
        public GameScript()
        {
            //Initialize();
        }

        public abstract void Start();
        public abstract void Update(FrameEventArgs args);
        public void Initialize()
        {
            GameEngine.GetWindow().RenderFrame += Update;

            Start();
        }
    }
}
