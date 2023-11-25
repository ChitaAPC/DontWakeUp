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


    public const int MapSize = 32;
    private byte[,] map = new byte[MapSize, MapSize];

    private Vector2Int playerSpawnCell;
    private Vector2Int bossSpawnCell;


    private void Start()
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
        SpawnMap();
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
        int req3 = 16;
        int req2Attemp = 128;

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



    private void SpawnMap()
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
                            Debug.Log($"For current {x}, {y} had an exception at {x + xi}, {y + yi}");
                        }
                    }
                }
            }
        }
    }





}
