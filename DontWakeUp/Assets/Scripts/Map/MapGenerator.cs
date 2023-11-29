using static System.IndexOutOfRangeException;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Special Units")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;
    [Range(0.2f, 0.5f)]
    [SerializeField] private float SpecialUnitStartingRoomOffsetMin;
    [Range(0.5f, 0.8f)]
    [SerializeField] private float SpecialUnitStartingRoomOffsetMax;


    [Header("Rooms")]
    [SerializeField] private int roomUnitSize;
    [SerializeField] private GameObject oneByOneRoom;
    [SerializeField] private GameObject twoByTwoRoom;
    [SerializeField] private GameObject fourByFourRoom;

    [Header("Units")]
    [SerializeField] EnemySpawnHandler enemySpawnHandler;


    [Header("Room Sprites")]
    [SerializeField] Sprite floorSprite;
    [SerializeField] Sprite horizontalWallSprite;
    [SerializeField] Sprite verticalWallSprite;
    [SerializeField] Sprite cournerWallSprite;
    [SerializeField] Sprite rockSprite;
    [SerializeField] Sprite holeSpritte;

    #region OLD SPAWN SETTINGS
    public const int MapSize = 16;//32;
    private byte[,] map = new byte[MapSize, MapSize];

    private Vector2Int playerSpawnCell;
    private Vector2Int bossSpawnCell;

    #endregion


    #region NEW SPAWN SETTINGS

    
    private const float min_room_size = 5f;
    private const float max_room_size = 20f;

    private const float max_loop_distance = 60f;

    private const float min_dist_for_boss_room = 300f;

    private const int maxDeadEndSeg = 3;

    private enum Segment
    {
        straight,
        bend,
        loop
    }

    #endregion

    private void Start()
    {
        SpawnMap();
    }

    private void SpawnMap()
    {
        Vector2 dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        if (dir == Vector2.zero)
        {
            dir = new Vector2(0f, 1f);
        }

        //temp
        dir = new Vector2(1f, 0f);

        Vector2 roomSize = SpawnStartingRoom(dir);

        Debug.Log($"Room size = {roomSize}");

        if (dir.x < 0)
        {
            //come in from left of the new room
            SpawnDeadEndSeg(new Vector2(roomSize.x / 2f, 0f), false, false, false, true);
        }
        if (dir.x > 0)
        {
            //come in from the right of the new room
            SpawnDeadEndSeg(new Vector2(-roomSize.x / 2f, 0f), false, true, false, false);
        }

        if (dir.y > 0)
        {
            //come in from the bot of the new room
            SpawnDeadEndSeg(new Vector2(0f, roomSize.y / 2f), false, false, true, false);
        }
        if (dir.y < 0)
        {
            //come in from the top of the new room
            SpawnDeadEndSeg(new Vector2(0f, -roomSize.y / 2f), true, false, false, false);
        }


    }

    private Vector2 SpawnStartingRoom(Vector2 dir)
    {
        float width = Random.Range(min_room_size, max_room_size);
        float height = Random.Range(min_room_size, max_room_size);
        GameObject room = SpawnRoom(width, height, dir.y>0, dir.x>0, dir.y<0, dir.x<0);
        room.transform.parent = transform;
        Player.transform.position = new Vector3(0f,0f,0f);
        return new Vector2(width, height);
    }

    private void SpawnDeadEndSeg(Vector2 entrance, bool top, bool right, bool bot, bool left)
    {
        int segLen = Random.Range(1, maxDeadEndSeg + 1);
        float w, h;
        GameObject room;

        Debug.Log($"Curent entrance = {entrance}");

        for (int i = 1; i < segLen; i++)
        {
            w = Random.Range(min_room_size, max_room_size);
            h = Random.Range(min_room_size, max_room_size);

            room = SpawnRoom(w, h, top || bot, right || left, top || bot, right || left);

            if (top)
            {
                room.transform.position = new Vector3(entrance.x, entrance.y - (h / 2f));
                entrance = room.transform.position - new Vector3(0f, h / 2f);
            }
            else if (bot)
            {
                room.transform.position = new Vector3(entrance.x, entrance.y + (h / 2f));
                entrance = room.transform.position + new Vector3(0f, h / 2f);
            }
            else if (right)
            {
                room.transform.position = new Vector3(entrance.x - (w / 2f), entrance.y);
                entrance = room.transform.position - new Vector3(w / 2f, 0f);
            }
            else
            {
                room.transform.position = new Vector3(entrance.x + (w / 2f), entrance.y);
                entrance = room.transform.position + new Vector3(w / 2f, 0f);
            }
            room.transform.parent = transform;
            Debug.Log($"Updated entrance {entrance}");
        }

        w = Random.Range(min_room_size, max_room_size);
        h = Random.Range(min_room_size, max_room_size);

        room = SpawnRoom(w, h, top, right, bot, left);

        if (top)
        {
            room.transform.position = new Vector3(entrance.x, entrance.y - (h / 2f));
        }
        else if (bot)
        {
            room.transform.position = new Vector3(entrance.x, entrance.y + (h / 2f));
        }
        else if (right)
        {
            room.transform.position = new Vector3(entrance.x - (w / 2f), entrance.y);
        }
        else
        {
            room.transform.position = new Vector3(entrance.x + (w / 2f), entrance.y);
        }
        room.transform.parent = transform;
    }


    #region OLD SPAWN

    private void GenerateMap_Old()
    {
        int playerQuad = Random.Range(0, 4);
        int bossQuad = Random.Range(0, 4);
        while (playerQuad == bossQuad)
        {
            bossQuad = Random.Range(0, 4);
        }

        playerSpawnCell = InitialiseSpecialUnitPlacement(playerQuad, Player);
        bossSpawnCell = InitialiseSpecialUnitPlacement(bossQuad, Boss);

        enemySpawnHandler.SetSpawnVariables(playerSpawnCell, bossSpawnCell, roomUnitSize, Player);

        PlanMap();
        SpawnMap_old();
    }

    private Vector2Int InitialiseSpecialUnitPlacement(int quad, GameObject unit)
    {
        Vector2 inRoomOffset = new Vector2
            (
            Random.Range(SpecialUnitStartingRoomOffsetMin, SpecialUnitStartingRoomOffsetMax) * roomUnitSize,
            Random.Range(SpecialUnitStartingRoomOffsetMin, SpecialUnitStartingRoomOffsetMax) * roomUnitSize
            );

        Vector2Int unitCell = new Vector2Int
            (
                Random.Range(Mathf.RoundToInt(MapSize * 0.2f), Mathf.RoundToInt(MapSize * 0.4f)),
                Random.Range(Mathf.RoundToInt(MapSize * 0.2f), Mathf.RoundToInt(MapSize * 0.4f))
            );

        if (quad == 1 || quad == 3)
            unitCell += new Vector2Int(Mathf.RoundToInt(MapSize * 0.5f), 0);
        if (quad == 2 || quad == 3)
            unitCell += new Vector2Int(0, Mathf.RoundToInt(MapSize * 0.5f));


        unit.transform.position = new Vector3
            (
            (unitCell.x * roomUnitSize) + inRoomOffset.x,
            (unitCell.y * roomUnitSize) + inRoomOffset.y,
            0f
            );

        return unitCell;
    }

    private bool IsRoomValid(int roomX, int roomY, int roomW, int roomH)
    {

        for (int r = roomX; r < roomX + roomW; r++)
        {
            for (int c = roomY; c < roomY+roomH; c++)
            {
                if (map[r, c] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void SetRoom(int roomX, int roomY, int roomW, int roomH, byte room)
    {
        for (int x = roomX; x < roomX + roomW; x++)
        {
            for (int y = roomY; y < roomY + roomH; y++)
            {
                map[x, y] = room;
            }
        }
    }

    private void PlanMap()
    {
        int req3 = MapSize / 2;
        int req2Attemp = MapSize * 4;

        while (req3 > 0)
        {
            int roomX = Random.Range(4, MapSize - 4);
            int roomY = Random.Range(4, MapSize - 4);

            int roomW = 4;
            int roomH = 2;

            if (Random.value > 0.5f)
            {
                roomW = 2;
                roomH = 4;
            }

            if (IsRoomValid(roomX, roomY, roomW, roomH))
            {
                SetRoom(roomX, roomY, roomW, roomH, 2);
                req3--;
            }
        }


        for (int i = 0; i < req2Attemp; i++)
        {
            int roomX = Random.Range(2, MapSize - 2);
            int roomY = Random.Range(2, MapSize - 2);

            int roomW = 2;
            int roomH = 2;

            if (IsRoomValid(roomX, roomY, roomW, roomH))
            {
                SetRoom(roomX, roomY, roomW, roomH, 1);
            }
        }
    }

    private void SpawnMap_old()
    {
        int x = 0;
        int y = 0;

        bool[,] mapSpawned = new bool[MapSize, MapSize];

        while (!mapSpawned[MapSize - 1, MapSize - 1])
        {
            if (mapSpawned[x, y])
            {
                x++;
                if (x >= MapSize)
                {
                    x = 0;
                    y++;
                }
                continue;
            }
            if (map[x, y] == 0)
            {
                GameObject room = Instantiate(oneByOneRoom, transform);
                room.transform.position = new Vector3(x * roomUnitSize, y * roomUnitSize);
                mapSpawned[x, y] = true;
            }

            if (map[x, y] == 1)
            {
                GameObject room = Instantiate(twoByTwoRoom, transform);
                room.transform.position = new Vector3(x * roomUnitSize, y * roomUnitSize);
                mapSpawned[x, y] = true;
                mapSpawned[x + 1, y] = true;
                mapSpawned[x, y + 1] = true;
                mapSpawned[x + 1, y + 1] = true;
            }

            if (map[x,y] == 2)
            {
                bool isHorizontal = x < MapSize - 5 && map[x + 4, y] == 2;

                GameObject room = Instantiate(fourByFourRoom, transform);
                room.transform.position = new Vector3(x * roomUnitSize, y * roomUnitSize);

                int x_m = 4;
                int y_m = 2;
                if (!isHorizontal)
                {
                    room.transform.Rotate(new Vector3(0f, 0f, 90f));
                    room.transform.position += new Vector3(20f, 0f);
                    x_m = 2;
                    y_m = 4;
                }

                for (int xi = 0; xi < x_m; xi++)
                {
                    for (int yi = 0; yi < y_m; yi++)
                    {
                        try
                        {
                            mapSpawned[x + xi, y + yi] = true;
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Debug.Log($"For current ({x}, {y}) had an exception at ({x + xi}, {y + yi})");
                        }
                    }
                }
            }
        }
    }

    #endregion


    private GameObject SpawnRoom(float horizontalSize, float verticalSize, bool topOpen, bool rightOpen, bool botOpen, bool leftOpen) 
    {
        GameObject room = new GameObject($"Room({horizontalSize} by {verticalSize})");

        CreateFloor(horizontalSize, verticalSize, room);
        
        CreateHorizontalWall(horizontalSize, verticalSize, room, true, topOpen);
        CreateHorizontalWall(horizontalSize, verticalSize, room, false, botOpen);

        CreateCournerWall(horizontalSize, verticalSize, room, true, true);
        CreateCournerWall(horizontalSize, verticalSize, room, true, false);
        CreateCournerWall(horizontalSize, verticalSize, room, false, true);
        CreateCournerWall(horizontalSize, verticalSize, room, false, false);

        CreateVerticalWall(horizontalSize, verticalSize, room, true, leftOpen);
        CreateVerticalWall(horizontalSize, verticalSize, room, false, rightOpen);

        CreateExtraSprites(horizontalSize, verticalSize, room, topOpen, rightOpen, botOpen, leftOpen);

        return room;
    }

    private void CreateFloor(float horizontalSize, float verticalSize, GameObject room)
    {
        GameObject floor = new GameObject("floor");
        SpriteRenderer sr = floor.AddComponent<SpriteRenderer>();
        sr.sprite = floorSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(horizontalSize, verticalSize);
        sr.sortingOrder = -1;

        floor.transform.parent = room.transform;
        floor.transform.localPosition = new Vector3(0, 0, 0);
    }

    private GameObject CreateHorizontalWall(float horizontalSize, float verticalSize, GameObject room, bool isTop, bool isOpen)
    {
        if (isOpen)
        {
            GameObject wall1 = CreateHorizontalWall(horizontalSize / 2, verticalSize, room, isTop, false);
            GameObject wall2 = CreateHorizontalWall(horizontalSize / 2, verticalSize, room, isTop, false);

            wall1.transform.localPosition = new Vector3(horizontalSize / 4, wall1.transform.localPosition.y, 0f);
            wall2.transform.localPosition = new Vector3(-horizontalSize / 4, wall2.transform.localPosition.y, 0f);
            return null;
        }

        GameObject wall = new GameObject("bottom wall");
        if (isTop)
            wall.name = "top wall";

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = horizontalWallSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(horizontalSize - 2, 1f);
        sr.flipY = !isTop;

        BoxCollider2D bc = wall.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(horizontalSize - 2, 0.5f);
        //if (isTop)
        bc.offset = new Vector2(0f, 0.25f);
        //else
        //    bc.offset = new Vector2(0f, -0.25f);

        wall.transform.parent = room.transform;
        
        if (isTop)
            wall.transform.localPosition = new Vector3(0, (verticalSize / 2) - 0.5f, 0f);
        else
            wall.transform.localPosition = new Vector3(0, -((verticalSize / 2) - 0.5f), 0f);

        return wall;
    }

    private void CreateCournerWall(float horizontalSize, float verticalSize, GameObject room, bool isTop, bool isLeft)
    {
        GameObject courner = new GameObject();

        if (isTop)
        {
            if (isLeft)
                courner.name = "Top left courner";
            else
                courner.name = "Top right courner";
        }
        else
        {
            if (isLeft)
                courner.name = "Bot left courner";
            else
                courner.name = "Bot right courner";
        }

        SpriteRenderer sr = courner.AddComponent<SpriteRenderer>();
        sr.sprite = cournerWallSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(1f, 1f);
        sr.flipX = !isLeft;
        sr.flipY = !isTop;
        BoxCollider2D bc = courner.AddComponent<BoxCollider2D>();

        if (isLeft)
        {
            bc.size = new Vector2(0.5f, 1f);
            bc.offset = new Vector2(-0.25f, 0f);
        }
        else
        {
            bc.size = new Vector2(0.5f, 1f);
            bc.offset = new Vector2(0.25f, 0f);
        }

        courner.transform.parent = room.transform;
        float x = (horizontalSize / 2) - 0.5f;
        float y = (verticalSize / 2) - 0.5f;
        if (isLeft)
            x = -x;
        if (!isTop)
            y = -y;

        courner.transform.localPosition = new Vector3(x, y, 0f);

    }

    private GameObject CreateVerticalWall(float horizontalSize, float verticalSize, GameObject room, bool isLeft, bool isOpen)
    {
        if (isOpen)
        {
            GameObject wall1 = CreateVerticalWall(horizontalSize, verticalSize / 2f, room, isLeft, false);
            GameObject wall2 = CreateVerticalWall(horizontalSize, verticalSize / 2f, room, isLeft, false);

            wall1.transform.localPosition = new Vector3(wall1.transform.localPosition.x, verticalSize / 4f, 0f);
            wall2.transform.localPosition = new Vector3(wall2.transform.localPosition.x, -verticalSize / 4f, 0f);

            return null;
        }

        GameObject wall = new GameObject("right wall");
        if (isLeft)
            wall.name = "left wall";

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = verticalWallSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(0.5f, verticalSize - 2f);
        sr.flipX = isLeft;

        BoxCollider2D bc = wall.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(0.5f, verticalSize - 2f);
        wall.transform.parent = room.transform;

        if (isLeft)
            wall.transform.localPosition = new Vector3((horizontalSize / 2) - 0.25f, 0f, 0f);
        else
            wall.transform.localPosition = new Vector3(-((horizontalSize/ 2) - 0.25f), 0f, 0f);

        return wall;
    }

    private void CreateExtraSprites(float horizontalSize, float verticalSize, GameObject room, bool topOpen, bool rightOpen, bool botOpen, bool leftOpen)
    {

        List<Vector3> pointsToAvoid = new List<Vector3>();

        if (topOpen)
            pointsToAvoid.Add(new Vector3(0f, verticalSize/2f));
        
        if (botOpen)
            pointsToAvoid.Add(new Vector3(0f, -verticalSize/2f));
        
        if (leftOpen)
            pointsToAvoid.Add(new Vector3(-horizontalSize / 2f, 0f));
        
        if (rightOpen)
            pointsToAvoid.Add(new Vector3(horizontalSize / 2f, 0f));

        for (float perim = 15f; perim <= 30f; perim += 5f)
        {
            if (horizontalSize + verticalSize >= perim)
            {
                float r = Random.value;
                //maybve add one extra bit
                if (r <= 0.333f)
                {
                    //spawn rock
                    pointsToAvoid.Add(SpawnExtraSpriteInRoom(horizontalSize, verticalSize, room, pointsToAvoid, rockSprite));
                }
                else if (r <= 0.666f)
                {
                    pointsToAvoid.Add(SpawnExtraSpriteInRoom(horizontalSize, verticalSize, room, pointsToAvoid, holeSpritte));
                }
            }
        }
    }

    private Vector3 SpawnExtraSpriteInRoom(float horizontalSize, float verticalSize, GameObject room, List<Vector3> positionsToAvoid, Sprite sprite)
    {
        Vector3 pos = new Vector3(Random.Range(-((horizontalSize / 2f) - 1f), (horizontalSize / 2f) - 1f), Random.Range(-((verticalSize / 2f) - 1f), (verticalSize / 2f) - 1f), 0f);

        while (positionsToAvoid.FindAll(p=> (p - pos).sqrMagnitude < 3f).Count > 0)
        {
            pos = new Vector3(Random.Range(-((horizontalSize / 2f) - 1f), (horizontalSize/2f) - 1f), Random.Range(-((verticalSize / 2f) - 1f), (verticalSize/2f) - 1f), 0f);
        }

        GameObject extra = new GameObject(sprite.name);
        SpriteRenderer sr = extra.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 1;

        CircleCollider2D cc = extra.AddComponent<CircleCollider2D>();
        cc.radius = 0.8f;

        float scale = Random.Range(0.5f, 1f);

        extra.transform.localScale = new Vector3(scale, scale, 1f);
        extra.transform.parent = room.transform;
        extra.transform.localPosition = pos;

        return pos;
    }
}
