using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameTree : MonoBehaviour {

    Room mainRoom; //Initial room

    Room[,] gridPositions; //Array to store all rooms based on location
    float locationToSpriteScale = 1.4f; //used to scale position ints to world floats

    //Offset of grid
    float offsetX;
    float offsetY;

    //Variables to be utilised properly
    float baseInitialAdjacentDifficulty = 0.01f;
    float baseInitialAdjacencyVariance = 0.02f;
    float gameDifficulty = 0.01f; //percentage difficulty to apply to all rooms
    float objectiveDifficultyRestriction = 0.5f; //room difficulty must be above this level to become an objective randomly

    [Header("Level Config")]
    public int gridWidth = 16;
    public int gridHeight = 14;


    [Header("Gen Config")]
    public int initialRouteCount = 2; //two different paths to start (max 4 initially, 3 there after)
    public int objectives = 1;
    public int maxDepth = 5;
    public int seed = 123456; //random gen seed
    public float chanceOfThreeRooms = 0.01f;
    public float chanceOfTwoRooms = 0.10f;
    public float chanceOfCutShort = 0.01f;
    int count = 0;

    [Header("Game Config")]
    public AnimationCurve difficultyCurve;
        
    [Header("Prefabs")]
    [Header("Floor")]
    public GameObject floorPrefab;
    [Header("Room")]
    public GameObject[] rooms;
    public GameObject corridorH;
    public GameObject corridorV;
    public GameObject wallBlockH;
    public GameObject wallBlockV;
    public GameObject[] objectivesG;

    // Use this for initialization
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialise()
    {
        offsetX = -(gridWidth * locationToSpriteScale / 2f);
        offsetY = -(gridHeight * locationToSpriteScale / 2f);

        gridPositions = new Room[gridWidth, gridHeight];
        mainRoom = new Room(this, true, false);

        Random.InitState(seed); //Use the seed to produce random numbers to allow testing of level ideas etc
        genRooms(Random.Range((gridWidth / 2) - (gridWidth / 6), (gridWidth / 2) + (gridWidth / 6)),
                 Random.Range((gridHeight / 2) - (gridHeight / 6), (gridHeight / 2) + (gridHeight / 6)),
                 gridPositions); //Set the initial position of the grid to the middle third of the grid
        genFloor();
        setObjectives();
        setCorridors(mainRoom, gridPositions);
    }

    public void genFloor()
    {
        for (int x = 0; x<gridWidth; x++)
        {
            for(int y = 0; y<gridHeight; y++)
            {
                
                if(gridPositions[x,y] != null)
                {
                    
                    GameObject room1G = Instantiate(rooms[gridPositions[x, y].getType()]);
                    room1G.transform.position = new Vector3((x * locationToSpriteScale) + offsetX, (y * locationToSpriteScale) + offsetY, 0.6f);
                    
                }
                else
                {
                    GameObject g = Instantiate(floorPrefab);
                    g.transform.position = new Vector3((x * locationToSpriteScale) + offsetX, (y * locationToSpriteScale) + offsetY, 0.5f);

                }
            }
        }
    }

    private void setObjectives()
    {
        int countObjectives = 0;
        int times = 0;
        while (countObjectives != objectives)
        {//Keep trying to add objectives randomly until the amount is reached or if more than 40 loops have occured.
            times++;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Room r = gridPositions[x, y];
                    if (r != null)
                    {
                        if ((r.AdjacentRooms.Count == 0 || (times >20 && r.AdjacentRooms.Count == 1)) && !r.getObjective())
                        {//Doesn't have further rooms, must be end room. If 21 or more loops have occured, check for single rooms 

                            if (Random.Range(0, 2) == 1 && countObjectives < objectives)
                            {
                                GameObject objective = Instantiate(objectivesG[0]);
                                objective.transform.position = new Vector3((x * locationToSpriteScale) + offsetX, (y * locationToSpriteScale) + offsetY, 0.49f);
                                r.setObjective(true);
                                countObjectives++;
                            }
                        }
                    }
                }
            }
            if(times > 40)
            {
                break;
            }
        }


    }

    private void setCorridors(Room r, Room[,] grid)
    {
        List<Room.Slot> s = new List<Room.Slot>();
        
        //Add all directions
        for (int x = 0; x < 4; x++)
        {
            Room.Decision direction = new Room.Decision();
            switch (x)
            {
                case 0:
                    direction = r.up(grid);
                    break;
                case 1:
                    direction = r.down(grid);
                    break;
                case 2:
                    direction = r.left(grid);
                    break;
                case 3:
                    direction = r.right(grid);
                    break;
            }
            Room.Slot slot;
            slot.x = direction.x;
            slot.y = direction.y;
            s.Add(slot);

        }

        
        //Check parent if present remove from s list
        for (int x2 = s.Count-1; x2 >= 0; x2--)
        {
            if (r.parentRoom != null)
            {
                if (s[x2].x == r.parentRoom.x && s[x2].y == r.parentRoom.y)
                {
                    s.RemoveAt(x2);
                }
            }
        }

        for (int x = 0; x < r.AdjacentRooms.Count; x++)
        {
            
            GameObject cor;
            //Check adjacent rooms, if present remove from s list
            for (int x2 = s.Count - 1; x2 >= 0; x2--)
            {
                if (s[x2].x == r.AdjacentRooms[x].x && s[x2].y == r.AdjacentRooms[x].y)
                {
                    s.RemoveAt(x2);
                }
                
            }           

            //If Horizontal/Vert
            if (r.x == r.AdjacentRooms[x].x)
            {//Horizontal
                cor = Instantiate(corridorH);
                float off = (r.y > r.AdjacentRooms[x].y) ? -0.5f : 0.5f;
                
                    cor.transform.position = new Vector3((r.x * locationToSpriteScale) + offsetX,
                        (r.y * locationToSpriteScale + (locationToSpriteScale * off)) + offsetY,
                        0.49f);
                
                
            }
            else
            {//Vertical
                cor = Instantiate(corridorV);
                float off = (r.x > r.AdjacentRooms[x].x) ? -0.5f : 0.5f;
                
                    cor.transform.position = new Vector3((r.x * locationToSpriteScale + (locationToSpriteScale * off)) + offsetX,
                    (r.y * locationToSpriteScale) + offsetY,
                    0.49f);
                
                
            }

            setCorridors(r.AdjacentRooms[x], grid);

        }

        for (int x2 = s.Count - 1; x2 >= 0; x2--)
        {
            //If any blockers need to be made...
            //check position against current room
            GameObject block;
            if (r.x != s[x2].x)
            {//Vertical  
                block = Instantiate(wallBlockV);
                float off = (r.x > s[x2].x) ? -0.5f : 0.5f;
                block.transform.position = new Vector3((r.x * locationToSpriteScale + (locationToSpriteScale * off)) + offsetX,
                    (r.y * locationToSpriteScale) + offsetY,
                    0.49f);
            }
            else if (r.y != s[x2].y)
            {//Horizontal   
                block = Instantiate(wallBlockH);
                float off = (r.y > s[x2].y) ? -0.5f : 0.5f;
                block.transform.position = new Vector3((r.x * locationToSpriteScale) + offsetX,
                (r.y * locationToSpriteScale + (locationToSpriteScale * off)) + offsetY,
                0.49f);                
            }
        }

    }

    

    private void genRooms(int initialPositionX, int initialPositionY, Room[,] grid)
    {
        //TODO - Could possibly create a class to store the level configuration to lower parameter passes and enable further variables to be passed with less code to update
        mainRoom.addInitialRooms(initialRouteCount,
            baseInitialAdjacentDifficulty * Random.Range(1f, 1f + baseInitialAdjacencyVariance) * Room.roomScale(1, maxDepth, difficultyCurve),
            chanceOfTwoRooms,
            chanceOfThreeRooms, 
            maxDepth, 
            difficultyCurve, 
            initialPositionX,
            initialPositionY,
            grid,
            chanceOfCutShort,
            locationToSpriteScale,
            offsetX, 
            offsetY);
    }

	
}
