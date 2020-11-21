
using System;
using System.Collections.Generic;
using System.Linq;

namespace MouseAI
{
    public enum DIRECTION
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

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
        public OBJECT_STATE object_state { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool isVisited { get; set; }
        public bool isScanned { get; set; }
        public bool isDeadEnd { get; set; }
        public bool isJunction { get; set; }
        public bool isPath { get; set; }
        public bool isVision { get; set; }
        public bool isCheesePath { get; set; }
        public int count { get; set; }
        public DateTime dtLastVisit { get; set; }

        public MazeObject(OBJECT_TYPE element_type, int x, int y)
        {
            object_type = element_type;
            object_state = OBJECT_STATE.NONE;
            isVisited = false;
            isScanned = false;
            isDeadEnd = false;
            isJunction = false;
            isVision = false;
            count = 0;
            this.x = x;
            this.y = y;
        }
    }

    public class MazeObjects : List<MazeObject>
    {
        public MazeObjects()
        {
        }

        public MazeObjects(List<MazeObject> mazeObjects)
        {
            AddRange(mazeObjects);
        }
    }

    public class MazeObjectSegments : List<MazeObjects>
    {
        public MazeObjectSegments()
        { }
    }
}
