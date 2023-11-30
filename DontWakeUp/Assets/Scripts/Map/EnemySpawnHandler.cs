using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{

    private Vector2Int playerSpawnCell;
    private Vector2Int bossSpawnCell;
    private int roomUnitSize;
    private GameObject Player;

    [SerializeField] private int MinimumActiveUnits;
    [SerializeField] private List<GameObject> UnitsPrefabs;

    private List<AbstractEntityController> enemies = new List<AbstractEntityController>();
    private const float activeRange = 30f;
    private bool readyToSpawn = false;


    private const float minimumRoomAreaToSpawn = 50f;
    private const float midEnemyArea = 100f;
    private const float highEnemyArea = 225f;

    private const int maxEnemyCountPerSpawnRun = 4;

    public void SetSpawnVariables(Vector2Int playerSpawnCell, Vector2Int bossSpawnCell, int roomUnitSize, GameObject Player)
    {
        this.playerSpawnCell = playerSpawnCell;
        this.bossSpawnCell = bossSpawnCell;
        this.roomUnitSize = roomUnitSize;
        this.Player = Player;
        StartCoroutine(EnemyCheckCo());
        readyToSpawn = true;
    }


    private void Update_old()
    {
        if (!readyToSpawn)
            return;
        if (GetActiveEnemiesCount() < MinimumActiveUnits)
        {
            int curPlayerCellX = Mathf.FloorToInt(Player.transform.position.x / roomUnitSize);
            int curPlayerCellY = Mathf.FloorToInt(Player.transform.position.y / roomUnitSize);
            int cellX, cellY;
            float spawnState = Random.value;
            int cellOffset = 1;
            if (spawnState <= 0.125f)
            {
                if (curPlayerCellX + cellOffset < MapGenerator.MapSize)
                {
                    cellX = curPlayerCellX + cellOffset;
                    cellY = curPlayerCellY;
                }
                else
                {
                    cellX = curPlayerCellX - cellOffset;
                    cellY = curPlayerCellY;
                }
            }
            else if (spawnState <= 0.25f)
            {
                if (curPlayerCellX + cellOffset < MapGenerator.MapSize)
                    cellX = curPlayerCellX + cellOffset;
                else
                    cellX = curPlayerCellX - cellOffset;

                if (curPlayerCellY + cellOffset < MapGenerator.MapSize)
                    cellY = curPlayerCellY + cellOffset;
                else
                    cellY = curPlayerCellY - cellOffset;
            }
            else if (spawnState <= 0.375f)
            {
                if (curPlayerCellY + cellOffset < MapGenerator.MapSize)
                {
                    cellX = curPlayerCellX;
                    cellY = curPlayerCellY + cellOffset;
                }
                else
                {
                    cellX = curPlayerCellX;
                    cellY = curPlayerCellY - cellOffset;
                }
            }
            else if (spawnState <= 0.5f)
            {
                if (curPlayerCellX > 0)
                    cellX = curPlayerCellX - cellOffset;
                else
                    cellX = curPlayerCellX + cellOffset;
                if (curPlayerCellY + cellOffset < MapGenerator.MapSize)
                    cellY = curPlayerCellY + cellOffset;
                else
                    cellY = curPlayerCellY - cellOffset;
            }
            else if (spawnState <= 0.625f)
            {
                if (curPlayerCellX > 0)
                    cellX = curPlayerCellX - cellOffset;
                else
                    cellX = curPlayerCellX + cellOffset;
                cellY = curPlayerCellY;
            }
            else if (spawnState <= 0.75f)
            {
                if (curPlayerCellX > 0)
                    cellX = curPlayerCellX - cellOffset;
                else
                    cellX = curPlayerCellX + cellOffset;
                if (curPlayerCellY > 0)
                    cellY = curPlayerCellY - cellOffset;
                else
                    cellY = curPlayerCellY + cellOffset;
            }
            else if (spawnState <= 0.875f)
            {
                cellX = curPlayerCellX;
                if (curPlayerCellY > 0)
                    cellY = curPlayerCellY - cellOffset;
                else
                    cellY = curPlayerCellY + cellOffset;
            }
            else
            {
                if (curPlayerCellX + cellOffset < MapGenerator.MapSize)
                    cellX = curPlayerCellX + cellOffset;
                else
                    cellX = curPlayerCellX - cellOffset;
                if (curPlayerCellY > 0)
                    cellY = curPlayerCellY - cellOffset;
                else
                    cellY = curPlayerCellY + cellOffset;
            }
            SpawnNewEnemy(cellX, cellY);
        }
    }



    public void SetPlayer(GameObject Player)
    {
        this.Player = Player;
    }

    public void BegineEnemyCheck()
    {
        StartCoroutine(EnemyCheckCo_new());
    }

    public void SpawnEnemyInRoom(Vector2 roomPos, float w, float h)
    {
        float area = w * h;
        if (area > minimumRoomAreaToSpawn)
            SingleRoundEnemySpawn(roomPos, w, h);
        if (area > midEnemyArea)
            SingleRoundEnemySpawn(roomPos, w, h);
        if (area > highEnemyArea)
            SingleRoundEnemySpawn(roomPos, w, h);
    }

    private void SingleRoundEnemySpawn(Vector3 roomPos, float w, float h)
    {
        int maxCount = Random.Range(0, maxEnemyCountPerSpawnRun+1);
        for (int count = 0; count < maxCount; count++)
        {
            int i = Random.Range(0, UnitsPrefabs.Count);

            GameObject unit = Instantiate(UnitsPrefabs[i], transform);
            unit.name = unit.name.Replace("(Clone)", "");
            float x_off = Random.Range(0f, (w - 2f) / 2f);
            float y_off = Random.Range(0f, (h - 2f) / 2f);

            if (Random.value < 0.5f)
                x_off = -x_off;
            if (Random.value < 0.5f)
                y_off = -y_off;
            unit.transform.position = roomPos + new Vector3(x_off, y_off);
            AbstractEntityController enemy = unit.GetComponent<AbstractEntityController>();
            enemy.SetPlayerTarget(Player);
            unit.SetActive(false);
            lock (enemies)
                enemies.Add(enemy);
            
        }
    }

    private IEnumerator EnemyCheckCo_new()
    {
        int unitIndex = 0;
        yield return new WaitForFixedUpdate();  //wait until the first fixed update to allow everything to spawn correctly
        while (true)
        {
            for (int i = 0; i < 100; i++)
            {
                if (enemies.Count > unitIndex)
                {
                    lock(enemies)
                        enemies[unitIndex].gameObject.SetActive(Vector2.Distance(enemies[unitIndex].transform.position, Player.transform.position) < activeRange);
                    unitIndex++;
                }
                else
                {
                    unitIndex = 0;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RemoveEnemy(AbstractEntityController enemyToRemove)
    {
        enemies.Remove(enemyToRemove);
        Destroy(enemyToRemove.gameObject);
    }

    private int GetActiveEnemiesCount()
    {
        return enemies.FindAll(e => e.gameObject.activeInHierarchy).Count;
    }

    private void SpawnNewEnemy(int cell_x, int cell_y)
    {
        if ((playerSpawnCell.x == cell_x && playerSpawnCell.y == cell_y) || (bossSpawnCell.x == cell_x && bossSpawnCell.y == cell_y))
        {
            return;
        }
        Vector2 pos = new Vector2
            (
            Random.Range(0f, roomUnitSize) + (cell_x * roomUnitSize),
            Random.Range(0f, roomUnitSize) + (cell_y * roomUnitSize)
            );
        int i = Random.Range(0, UnitsPrefabs.Count);
        GameObject unit = Instantiate(UnitsPrefabs[i], transform);
        unit.name = unit.name.Replace("(Clone)", "");
        unit.transform.position = pos;
        AbstractEntityController enemy = unit.GetComponent<AbstractEntityController>();
        enemy.SetPlayerTarget(Player);
        enemies.Add(enemy);
    }

    private IEnumerator EnemyCheckCo()
    {
        while (true)
        {
            lock (enemies)
            {
                foreach (AbstractEntityController enemy in enemies)
                {
                    enemy.gameObject.SetActive(Vector2.Distance(enemy.transform.position, Player.transform.position) < activeRange);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

}
