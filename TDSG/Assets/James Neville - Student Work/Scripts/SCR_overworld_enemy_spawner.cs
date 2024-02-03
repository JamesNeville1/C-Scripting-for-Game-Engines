using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_overworld_enemy_spawner : MonoBehaviour {
    [SerializeField] private GameObject overworldEnemyPrefab;
    [SerializeField] private SCO_enemy[] enemies;

    [SerializeField][MyReadOnly] SCR_unit_animation entAnimation;

    public static SCR_overworld_enemy_spawner returnInstance() {
        return instance;
    }

    private static SCR_overworld_enemy_spawner instance;

    private void Awake() {
        instance = this;
    }
    public void spawnEnemy() {
        SCO_enemy data = pickEnemy();
        Vector2 spawnLoc = returnEnemySpawnLocation();

        GameObject enemyInst = Instantiate(overworldEnemyPrefab, spawnLoc, Quaternion.identity, gameObject.transform);
        SCR_overworld_enemy script = enemyInst.GetComponent< SCR_overworld_enemy>();

        script.setup(data);

        entAnimation = enemyInst.GetComponent<SCR_unit_animation>();
        entAnimation.setPrefix(data.returnAnimatioPrefix());
        entAnimation.setAnimationController(data.returnAnimator());

        //SCR_tick_system.returnTickSystem().unsubscribe(4f, () => spawnEnemy());
    }
    private SCO_enemy pickEnemy() {
        return enemies[Random.Range(0,enemies.Length)];
    }
    private Vector2 returnEnemySpawnLocation() {
        //return Random.insideUnitCircle * 5 + (Vector2)SCR_player_main.returnInstance().gameObject.transform.position;
        return (Vector2)SCR_player_main.returnInstance().gameObject.transform.position;
    }

}
