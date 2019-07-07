using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
{
    abstract class GameSystem
    {
        protected World _world;
        public abstract void Update();
    }
}
