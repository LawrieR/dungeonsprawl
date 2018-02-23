using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    GameTree parentTree;
    bool isMain = false;
    bool isObjective = false;
    float roomDifficulty = 0f; //percetage difficulty weighting
    int size = 16;
    int type = 0;
    int depth = 0; //how far along the tree is the room
    public int x;
    public int y;

    public Room parentRoom;
    public List<Room> AdjacentRooms = new List<Room>();

    public Room(GameTree gt)
    {
        parentTree = gt;
    }

    public Room(GameTree gt, bool _isMain, bool _isObjective)
    {
        parentTree = gt;
        isMain = _isMain;
        isObjective = _isObjective;
    }

    public Room(Room parent, GameTree gt, float _difficulty, bool _isObjective, int _depth, Room[,] grid, int initX, int initY)
    {
        parentRoom = parent;
        parentTree = gt;
        roomDifficulty = _difficulty;
        isObjective = _isObjective;
        depth = _depth;
        type = depth;
        x = initX;
        y = initY;
        grid[initX, initY] = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public struct Decision
    {
        public bool success;
        public int x;
        public int y;
    }

    public struct Slot
    {
        public int x;
        public int y;
    }

    public void setSize(int _size)
    {
        size = _size;
    }
    public void setType(int _type)
    {
        type = _type;
    }
    public int getType()
    {
        return type;
    }
    public void setObjective(bool _isObjective)
    {
        isObjective = _isObjective;
    }
    public bool getObjective()
    {
        return isObjective;
    }

    private Decision decideLocation(Room[,] grid, int parentX, int parentY, float chanceOfCutShort)
    {
        Decision ret = new Decision();

        List<Slot> slots = new List<Slot>();

        Slot up = new Slot();
        up.x = parentX;
        up.y = parentY + 1; //Y positive - North
        slots.Add(up);

        Slot down = new Slot();
        down.x = parentX;
        down.y = parentY - 1; //Y negative - South
        slots.Add(down);

        Slot left = new Slot();
        left.x = parentX - 1; //X negative - West
        left.y = parentY; 
        slots.Add(left);

        Slot right = new Slot();
        right.x = parentX + 1; //X positive - East
        right.y = parentY;
        slots.Add(right);

        if(Random.Range(0f, 1f) < chanceOfCutShort)
        {
            slots.Clear();
        }

        for(int x=slots.Count-1; x >= 0;  x--)
        {
            if(slots[x].x < 0 || slots[x].y < 0 || slots[x].x >= grid.GetLength(0) || slots[x].y >= grid.GetLength(1) || grid[slots[x].x,slots[x].y] != null)
            {
                slots.RemoveAt(x);
            }
        }

        if (slots.Count == 0)
        {
            ret.success = false;
        }
        else
        {
            //Should be left with options
            int rnd = Random.Range(0, slots.Count); //pick a random slot
            ret.x = slots[rnd].x;
            ret.y = slots[rnd].y;

            ret.success = true;
        }
        return ret;
    }

    public Decision up(Room[,] grid)
    {
        Decision ret = new Decision();

        ret.x = x;
        ret.y = y + 1;
        ret.success = true;

        if (ret.y >= grid.GetLength(1))
        {
            ret.success = false;
        }

        return ret;
    }

    public Decision down(Room[,] grid)
    {
        Decision ret = new Decision();

        ret.x = x;
        ret.y = y - 1;
        ret.success = true;
        if (ret.y < 0)
        {
            ret.success = false;
        }

        return ret;
    }

    public Decision left(Room[,] grid)
    {
        Decision ret = new Decision();

        ret.x = x-1;
        ret.y = y;
        ret.success = true;
        if (ret.x < 0)
        {
            ret.success = false;
        }

        return ret;
    }

    public Decision right(Room[,] grid)
    {
        Decision ret = new Decision();

        ret.x = x + 1;
        ret.y = y;
        ret.success = true;
        if (ret.x < 0)
        {
            ret.success = false;
        }

        return ret;
    }

    public void addInitialRooms(int _initialCount, float _difficulty, float chanceOfTwoRooms, float chanceOfThreeRooms, int _maxDepth, AnimationCurve _difficultyCurve, int initX, int initY, Room[,] grid, float chanceOfCutShort)
    {
        x = initX;
        y = initY;
        grid[initX,initY] = this;

        for(int x = 0; x<_initialCount; x++)
        {
            Decision newLocation = decideLocation(grid, initX, initY, chanceOfCutShort);
            if (newLocation.success)
            {
                Room newRoom = new Room(this,parentTree, roomScale(depth, _maxDepth, _difficultyCurve), false, depth + 1, grid, newLocation.x, newLocation.y);
                AdjacentRooms.Add(newRoom);
                newRoom.addRooms(roomScale(depth + 1, _maxDepth, _difficultyCurve), chanceOfTwoRooms, chanceOfThreeRooms, _maxDepth, _difficultyCurve, newLocation.x, newLocation.y, grid, chanceOfCutShort);
            }
        }
    }

    public void addRooms(float _difficulty, float chanceOfTwoRooms, float chanceOfThreeRooms, int _maxDepth, AnimationCurve _difficultyCurve, int initX, int initY, Room[,] grid, float chanceOfCutShort)
    {
        if (depth < _maxDepth)
        {
            float chance = Random.Range(0f, 1f);
            int count = 0;

            if(depth+1 == _maxDepth)
            {//At the last difficulty there is 100% chance of only 1 room
                count = 1;
            }
            if (chance <= chanceOfThreeRooms) 
            {
                count = 3;
            }
            else if (chance >= 1f - chanceOfTwoRooms)
            {
                count = 2;
            }
            else
            {
                count = 1;
            }

            for (int x = 0; x < count; x++)
            {
                Decision newLocation = decideLocation(grid, initX, initY, chanceOfCutShort);
                if (newLocation.success)
                {
                    Room newRoom = new Room(this,parentTree, roomScale(depth, _maxDepth, _difficultyCurve), false, depth + 1, grid, newLocation.x, newLocation.y);
                    AdjacentRooms.Add(newRoom);
                    newRoom.addRooms(roomScale(depth + 1, _maxDepth, _difficultyCurve), chanceOfTwoRooms, chanceOfThreeRooms, _maxDepth, _difficultyCurve, newLocation.x, newLocation.y, grid, chanceOfCutShort);
                }
            }

        }
    }

    public static float roomScale(int _roomDepth, int _maxDepth, AnimationCurve _difficultyCurve)
    {
        //Calculate difficulty curve based on depth
        float diff = _roomDepth / _maxDepth;
        return _difficultyCurve.Evaluate(diff);
    }
}
