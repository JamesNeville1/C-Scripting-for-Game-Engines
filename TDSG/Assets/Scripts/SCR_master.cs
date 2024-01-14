using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master : MonoBehaviour {

    private SCR_map_generation mapGen;

    [SerializeField]
    private GameObject playerPrefab;

    private void Awake() {
        mapGen = GetComponent<SCR_map_generation>();

        SceneManager.LoadSceneAsync("SCE_inventory", LoadSceneMode.Additive);
    }

    private void OnDestroy() {
        PlayerPrefs.Save();
    }

    public void temp() {
        SceneManager.UnloadSceneAsync("SCE_overworld");
        SceneManager.LoadSceneAsync("SCE_inventory");
    }

    private void Start() {
        string randSeed = mapGen.randomSeed();
        Debug.Log("Map Seed: " + randSeed);
        mapGen.generate(randSeed);


        //Make Player (player contains inventory logic)
        SCR_player_main player = Instantiate(playerPrefab, mapGen.startPos(), Quaternion.identity).GetComponent<SCR_player_main>();
    }
}
