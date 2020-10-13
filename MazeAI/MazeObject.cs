using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeAI
{
    public enum OBJECT_TYPE
    {
        SPACE,
        BLOCK
    }

    public enum OBJECT_STATE
    {
        NONE,
        VISITED,
        MOUSE,
        CHEESE,
        SCANNED
    }

    public class MazeObject
    {
        public OBJECT_TYPE object_type { get; set; }
        public bool isVisited { get; set; }
        public OBJECT_STATE object_state { get; set; }

        public int x { get; set; }
        public int y { get; set; }
        public bool isScanned { get; set; }

        public MazeObject(OBJECT_TYPE element_type, int x, int y)
        {
            this.object_type = element_type;
            object_state = OBJECT_STATE.NONE;
            isVisited = false;
            isScanned = false;
            this.x = x;
            this.y = y;
        }
    }
}
