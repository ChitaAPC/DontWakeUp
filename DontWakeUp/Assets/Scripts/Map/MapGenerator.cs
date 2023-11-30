using static System.IndexOutOfRangeException;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Special Units")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;
    
    [Header("Units")]
    [SerializeField] EnemySpawnHandler enemySpawnHandler;


    [Header("Room Sprites")]
    [SerializeField] Sprite floorSprite;
    [SerializeField] Sprite horizontalWallSprite;
    [SerializeField] Sprite verticalWallSprite;
    [SerializeField] Sprite cournerWallSprite;
    [SerializeField] Sprite rockSprite;
    [SerializeField] Sprite holeSpritte;

    private const float min_room_size = 5f;
    private const float max_room_size = 20f;
    private const int segmentRoomLenMin = 3;
    private const int segmentRoomLenMax = 6;


    private const int min_dist_for_boss_room = 25;

    private const int maxDeadEndSeg = 3;



    private void Start()
    {
        enemySpawnHandler.SetPlayer(Player);
        SpawnMap();
        enemySpawnHandler.BegineEnemyCheck();
    }

    private void SpawnMap()
    {
        Vector2 dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        if (dir == Vector2.zero)
        {
            dir = new Vector2(0f, 1f);
        }

        Vector2 roomSize = SpawnStartingRoom(dir);

        if (dir.x != 0 && dir.y != 0)
        {
            if (Random.value >= 0.5)
            {
                //x dir for deadend
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
                dir = new Vector2(0, dir.y);
            }
            else
            {
                //y dir for deadend
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
                dir = new Vector2(dir.x, 0f);
            }
        }
        int length = Random.Range(2, 5);
        Vector2 entrance;
        if (dir.x < 0)
            entrance = new Vector2(roomSize.x / 2f, 0f);
        else if (dir.x > 0)
            entrance = new Vector2(-roomSize.x / 2f, 0f);
        else if (dir.y > 0)
            entrance = new Vector2(0f, roomSize.y / 2f);
        else
            entrance = new Vector2(0f, -roomSize.y / 2f);

        int traversed = 0;
        bool canStop = false;
        Vector2 prevDir = Vector2.zero;
        while (!canStop)
        {
            entrance = SpawnNextMainSegment(dir, entrance, length, dir == prevDir);
            traversed += length;

            if (traversed >= min_dist_for_boss_room)
            {
                canStop = Random.value >= 0.5f;
            }
            length = Random.Range(segmentRoomLenMin, segmentRoomLenMax);

            if (prevDir != dir)
                prevDir = dir;
            else
            {

                int dirMultiplier = (Random.Range(0, 2) * 2) - 1;       //either -1 or 1 exactly
                if (dir.x == 0)
                {
                    RaycastHit2D hit = Physics2D.Raycast(entrance, new Vector2(dirMultiplier * (segmentRoomLenMax + 1) * max_room_size, 0));
                    if (hit.rigidbody == null)
                    {
                        dir = new Vector2(dirMultiplier, 0f);
                    }
                    else
                    {
                        hit = Physics2D.Raycast(entrance, new Vector2(-dirMultiplier * (segmentRoomLenMax + 1) * max_room_size, 0));
                        if (hit.rigidbody == null)
                        {
                            dir = new Vector2(-dirMultiplier, 0f);
                        }
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(entrance, new Vector2(0f, dirMultiplier * (segmentRoomLenMax + 1) * max_room_size));
                    if (hit.rigidbody == null)
                    {
                        dir = new Vector2(0f, dirMultiplier);
                    }
                    else
                    {
                        hit = Physics2D.Raycast(entrance, new Vector2(0f, -dirMultiplier * (segmentRoomLenMax+1) * max_room_size));
                        if (hit.rigidbody == null)
                        {
                            dir = new Vector2(0f, -dirMultiplier);
                        }
                    }
                }

            }
            entrance = SpawnEndOfSegmentRoom(prevDir, dir, entrance);
        }
        SpawnBossRoom(entrance, dir);
    }

    private Vector2 SpawnEndOfSegmentRoom(Vector2 dir, Vector2 nextDir, Vector2 entrance)
    {
        float w = Random.Range(min_room_size, max_room_size);
        float h = Random.Range(min_room_size, max_room_size);

        GameObject room = SpawnRoom(w, h, dir.y < 0 || nextDir.y > 0, dir.x < 0 || nextDir.x > 0, dir.y > 0 || nextDir.y < 0, dir.x > 0 || nextDir.x < 0);

        room.transform.position = new Vector3(
            entrance.x - (dir.x * (w / 2f)),
            entrance.y + (dir.y * (h / 2f))
            );

        entrance = new Vector2(
            room.transform.position.x - (nextDir.x * (w/2f)),
            room.transform.position.y + (nextDir.y * (h /2f))
            );

        room.transform.parent = transform;

        room.name = $"{room.name} - courner";
        enemySpawnHandler.SpawnEnemyInRoom(room.transform.position, w, h);

        return entrance;
    }


    private Vector2 SpawnNextMainSegment(Vector2 dir, Vector2 entrance, int length, bool allowDeadEnd)
    {
        return SpawnCorridor(entrance, length, dir.y < 0, dir.x > 0, dir.y > 0, dir.x < 0, allowDeadEnd);
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

    private Vector2 SpawnCorridor(Vector2 entrance, int rooms, bool top, bool right, bool bot, bool left, bool allowDeadEnd, float maxW = max_room_size, float maxH = max_room_size)
    {
        float w = 0;
        float h = 0;
        GameObject room = null;

        for (int i = 0; i < rooms; i++)
        {
            w = Random.Range(min_room_size, maxW);
            h = Random.Range(min_room_size, maxH);

            bool deadEnd = Random.value < 0.6f && allowDeadEnd && i > 0 && i < rooms-1;

            if (deadEnd)
            {
                if (top || bot)
                {
                    float r = Random.value;
                    if (r <= 0.333f)
                    {
                        //only R
                        room = SpawnRoom(w, h, true, true, true, false);
                        if (top)
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y - (h / 2f)), false, true, false, false, max_room_size, h);
                        else
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y + (h / 2f)), false, true, false, false, max_room_size, h);
                    }
                    else if (r <= 0.666f)
                    {
                        //only L
                        room = SpawnRoom(w, h, true, false, true, true);
                        if (top)
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y - (h / 2f)), false, false, false, true, max_room_size, h);
                        else
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y + (h / 2f)), false, false, false, true, max_room_size, h);
                    }
                    else
                    {
                        //both L and R
                        room = SpawnRoom(w, h, true, true, true, true);
                        if (top)
                        {
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y - (h / 2f)), false, true, false, false, max_room_size, h);
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y - (h / 2f)), false, false, false, true, max_room_size, h);
                        }
                        else
                        {
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y + (h / 2f)), false, true, false, false, max_room_size, h);
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y + (h / 2f)), false, false, false, true, max_room_size, h);
                        }
                    }
                }
                else
                {
                    float r = Random.value;
                    if (r <= 0.333f)
                    {
                        //only top
                        room = SpawnRoom(w, h, true, true, false, true);
                        if (right)
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y + (h / 2f)), false, false, true, false, w, max_room_size);
                        else
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y + (h / 2f)), false, false, true, false, w, max_room_size);
                    }
                    else if (r <= 0.666f)
                    {
                        //only bot
                        room = SpawnRoom(w, h, false, true, true, true);
                        if (right)
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y - (h / 2f)), true, false, false, false, w, max_room_size);
                        else
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y - (h / 2f)), true, false, false, false, w, max_room_size);

                    }
                    else
                    {
                        //both top and bot
                        room = SpawnRoom(w, h, true, true, true, true);
                        if (right)
                        {
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y + (h / 2f)), false, false, true, false, w, max_room_size);
                            SpawnDeadEndSeg(new Vector3(entrance.x - (w / 2f), entrance.y - (h / 2f)), true, false, false, false, w, max_room_size);
                        }
                        else
                        {
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y + (h / 2f)), false, false, true, false, w, max_room_size);
                            SpawnDeadEndSeg(new Vector3(entrance.x + (w / 2f), entrance.y - (h / 2f)), true, false, false, false, w, max_room_size);
                        }
                    }
                }
            }
            else
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

            //spawn enemies
            enemySpawnHandler.SpawnEnemyInRoom(room.transform.position, w, h);
        }

        if (top)
            return room.transform.position - new Vector3(0f, h / 2f);
        if (bot)
            return room.transform.position + new Vector3(0f, h / 2f);
        if (right)
            return room.transform.position - new Vector3(w / 2f, 0f);
        if (left)
            return room.transform.position + new Vector3(w / 2f, 0f);

        return new Vector2();
    }

    private void SpawnBossRoom(Vector2 entrance, Vector2 dir)
    {
        float w = Random.Range(min_room_size, max_room_size);
        float h = Random.Range(min_room_size, max_room_size);

        GameObject room = SpawnRoom(w, h, dir.y < 0, dir.x < 0, dir.y > 0, dir.x > 0);
        room.transform.position = entrance + new Vector2(-dir.x * (w / 2f), dir.y * (h / 2f));
        Boss.transform.position = room.transform.position;
        room.transform.parent = transform;
    }

    private void SpawnDeadEndSeg(Vector2 entrance, bool top, bool right, bool bot, bool left, float maxW = max_room_size, float maxH = max_room_size)
    {
        int segLen = Random.Range(0, maxDeadEndSeg);

        if (segLen > 0)
            entrance = SpawnCorridor(entrance, segLen, top, right, bot, left, false, maxW, maxH);

        float w = Random.Range(min_room_size, maxW);
        float h = Random.Range(min_room_size, maxH);

        GameObject room = SpawnRoom(w, h, top, left, bot, right);

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
        enemySpawnHandler.SpawnEnemyInRoom(room.transform.position, w, h);
    }

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
