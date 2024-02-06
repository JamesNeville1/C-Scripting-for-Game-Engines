using IzzetUtils.IzzetAttributes;
using IzzetUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using System.Net;

public class SCR_master_combat : MonoBehaviour {

    #region Set Instance
    private static SCR_master_combat instance;
    public static SCR_master_combat returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    #endregion
    #region Structs & Enums
    public struct setupEncounterEnemy { 
        public SCO_enemy enemy; public Vector2Int position;

        public setupEncounterEnemy(SCO_enemy enemy, Vector2Int position) {
            this.enemy = enemy;
            this.position = position;
        }
    }
    #endregion

    [Header("Require Dev Input")]
    [SerializeField] private RuleTile tile;
    [SerializeField] private GameObject boardParent;
    [SerializeField] private Vector2 cameraOffset;
    [SerializeField] private Color hoverColour;
    [SerializeField] private Color clickColour;

    [Header("Read Only")]
    [SerializeField] [MyReadOnly] private Tilemap tilemap;
    [SerializeField] [MyReadOnly] private bool playerTurn = false;
    [SerializeField] [MyReadOnly] private Vector2Int oldHoverTilePos;
    [SerializeField] [MyReadOnly] private Vector2 midPoint;

    //
    private Dictionary<Vector2Int, SCR_combat_unit> boardData = new Dictionary<Vector2Int, SCR_combat_unit>(); //Use unit in future

    #region Unity
    private void Update() {
        takeInput();
    }
    #endregion
    #region Setup
    public void setup(Vector2Int size) {
        tilemap = GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector2Int currentPos = new Vector2Int(x, y);
                tilemap.SetTile((Vector3Int)currentPos, tile);

                Debug.Log($"Is in boardData: {currentPos}");

                bool isBound = currentPos.x == 0 || currentPos.x == size.x - 1 || currentPos.y == 0 || currentPos.y == size.y - 1;
                
                if(!isBound) {
                    boardData.Add(currentPos, null);    
                }
            }
        }

        midPoint = IzzetMain.getMidPoint(Vector2.zero, size);
        tilemap.gameObject.transform.position = new Vector3(-.5f, -.5f, 0);
    }
    public void setupEncounter(setupEncounterEnemy[] enemies) {
        Vector3 cameraPos = new Vector3(midPoint.x, midPoint.y, Camera.main.transform.position.z);
        Camera.main.transform.position = cameraPos + (Vector3)cameraOffset;

        //Create Player Unit

        //Create Enemies
        foreach (var enemyData in enemies) {
            addEnemy(enemyData);
        }
    }
    public void unload() {

    }
    #endregion
    #region Logic
    private void addEnemy(setupEncounterEnemy enemyData) {
        SCR_combat_unit newEnemy = SCR_combat_unit.createInstance("Temp", enemyData.enemy.returnAthletics(), enemyData.enemy.returnDexterity(), enemyData.enemy.returnEndurance());
        newEnemy.gameObject.transform.position = (Vector2)enemyData.position;

        addToData(enemyData.position, newEnemy);
    }
    private void addToData(Vector2Int pos, SCR_combat_unit unit) {
        boardData.Remove(pos);
        boardData.Add(pos, unit);
    }
    private void takeInput() {
        RaycastHit2D hit = Physics2D.Raycast(IzzetMain.getMousePos(Camera.main), Vector2.down);

        if (hit.collider != null) {
            Vector2Int posInt = IzzetMain.castToVector2Int(hit.point);
            if (boardData.ContainsKey(posInt)) {
                tilemap.SetColor((Vector3Int)oldHoverTilePos, Color.white);

                if(Input.GetMouseButton(0)) {
                    tilemap.SetColor((Vector3Int)posInt, clickColour);
                }
                else {
                    tilemap.SetColor((Vector3Int)posInt, hoverColour);
                }

                oldHoverTilePos = posInt;
            }
        }
    }
    #endregion
}
