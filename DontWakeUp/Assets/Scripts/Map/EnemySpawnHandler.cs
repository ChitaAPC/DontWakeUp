using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{

    private GameObject Player;

    //[SerializeField] private int MinimumActiveUnits;
    [SerializeField] private List<GameObject> UnitsPrefabs;

    private List<AbstractEntityController> enemies = new List<AbstractEntityController>();
    private const float activeRange = 30f;

    private const float minimumRoomAreaToSpawn = 50f;
    private const float midEnemyArea = 100f;
    private const float highEnemyArea = 225f;

    private const int maxEnemyCountPerSpawnRun = 4;


    public void SetPlayer(GameObject Player)
    {
        this.Player = Player;
    }

    public void BegineEnemyCheck()
    {
        StartCoroutine(EnemyCheckCo());
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

    private IEnumerator EnemyCheckCo()
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

}
