using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_enemy_spawner : MonoBehaviour {
    [SerializeField] private GameObject overworldEnemyPrefab;
    [SerializeField] private SCO_enemy[] enemies;

    private static SCR_enemy_spawner instance;

    private void Start() {
        SCR_tick_system.returnTickSystem().subscribe(.1f, () => spawnEnemy());
        SCR_tick_system.returnTickSystem().subscribe(.1f, () => print("a"));
        SCR_tick_system.returnTickSystem().unsubscribe(.1f, () => spawnEnemy());
    }
    private void spawnEnemy() {
        //SCO_enemy enemy = pickEnemy();
        Vector2 spawnLoc = returnEnemySpawnLocation();

        GameObject enemyInst = Instantiate(overworldEnemyPrefab, spawnLoc, Quaternion.identity);
    }
    private SCO_enemy pickEnemy() {
        return enemies[Random.Range(0,enemies.Length)];
    }
    private Vector2 returnEnemySpawnLocation() {
        //return Random.insideUnitCircle * 5 + (Vector2)SCR_player_main.returnInstance().gameObject.transform.position;
        return (Vector2)SCR_player_main.returnInstance().gameObject.transform.position;
    }

}
