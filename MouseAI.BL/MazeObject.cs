// MazeObject and MazeObjects Class:
// Space and block object properties and states that comprise of a maze
// MazeObject lists which are associated to a given maze instance 

using System;
using System.Collections.Generic;

namespace MouseAI
{ 
    public class MazeObject
    {
        public MazeObjects.OBJECT_TYPE object_type { get; set; }
        public MazeObjects.OBJECT_STATE object_state { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool isVisited { get; set; }
        public bool isScanned { get; set; }
        public bool isDeadEnd { get; set; }
        public bool isJunction { get; set; }
        public bool isPath { get; set; }
        public bool isVision { get; set; }
        public bool isCheesePath { get; set; }
        public int smell_level { get; set; }
        public int count { get; set; }
        public DateTime dtLastVisit { get; set; }

        public MazeObject(MazeObjects.OBJECT_TYPE element_type, int x, int y)
        {
            object_type = element_type;
            object_state = MazeObjects.OBJECT_STATE.NONE;
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
            CHEESE
        }

        public MazeObjects()
        { }

        public MazeObjects(IEnumerable<MazeObject> mazeObjects)
        {
            AddRange(mazeObjects);
        }
    }

    public class MazeObjectSegments : List<MazeObjects>
    {
    }
}
