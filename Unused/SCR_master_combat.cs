using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] private Color selectedColour;

    [Header("Read Only")]
    [SerializeField][MyReadOnly] private Tilemap tilemap;
    [SerializeField][MyReadOnly] private Vector2 midPoint;
    [SerializeField][MyReadOnly] private Vector2Int oldClickPos;
    [SerializeField][MyReadOnly] private Vector2Int? selected = null;

    //
    private Dictionary<Vector2Int, SCR_combat_unit> boardData = new Dictionary<Vector2Int, SCR_combat_unit>(); //Use unit in future
    private List<Vector2Int> unitKeys = new List<Vector2Int>();

    #region Unity / My Update
    public IEnumerator toggleableUpdate() { //Note: I made this to stop update from running when it shouldn't
        while (true) {
            yield return null;
            takeInput();
        }
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

                //Debug.Log($"Is in boardData: {currentPos}");

                bool isBound = currentPos.x == 0 || currentPos.x == size.x - 1 || currentPos.y == 0 || currentPos.y == size.y - 1;

                if (!isBound) {
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

        StartCoroutine(toggleableUpdate());
    }
    public void unload() {
        StopCoroutine(toggleableUpdate());
    }
    #endregion
    #region Logic
    private void addEnemy(setupEncounterEnemy enemyData) {
        SCR_combat_unit newEnemy =
            SCR_combat_unit.createInstance(enemyData.enemy);

        newEnemy.gameObject.transform.position = (Vector2)enemyData.position;
        changeData(enemyData.position, newEnemy);
    }
    public void changeData(Vector2Int pos, SCR_combat_unit unit) {
        boardData.Remove(pos);
        boardData.Add(pos, unit);
    }
    private void remove(Vector2Int pos) {
        changeData(pos, null);
    }
    private void takeInput() {

        RaycastHit2D hit = Physics2D.Raycast(IzzetMain.getMousePos(Camera.main), Vector2.down);

        if (Input.GetMouseButtonDown(1)) removeSelected(); //Right click removes selected

        if (hit.collider != null) {
            Vector2Int currentPos = IzzetMain.castToVector2Int(hit.point); //Format mouse pos to vec2Int

            if (Input.GetMouseButtonDown(0)) { setSelected(currentPos); return; } //If mouse down set as new selected


            if (currentPos == oldClickPos || !boardData.ContainsKey(currentPos)) return; //The stuff below just shows hover colour on board for useability

            updateBoard(currentPos, oldClickPos);

            oldClickPos = currentPos;
        }
    }
    private void updateBoard(Vector2Int isPos, Vector2Int wasPos) {
        if (!isSelected(isPos)) tilemap.SetColor((Vector3Int)isPos, hoverColour);
        if (!isSelected(wasPos)) tilemap.SetColor((Vector3Int)wasPos, Color.white);
    }
    private bool isSelected(Vector2Int toCheck) {
        return toCheck == selected;
    }
    private float returnDist(Vector2Int current, Vector2Int target) {
        return Vector2.Distance(current, target);
    }
    private void setSelected(Vector2Int toSet) {
        if(boardData.ContainsKey(toSet)) {
            if (selected != null && boardData[(Vector2Int)selected] != null) {
                float dist = returnDist((Vector2Int)selected, toSet);
                print(boardData[(Vector2Int)selected].returnAttributes().returnSpeed());
                if(dist <= boardData[(Vector2Int)selected].returnAttributes().returnSpeed()) {
                    tilemap.SetColor((Vector3Int)selected, Color.white);
                    boardData[(Vector2Int)selected].move(toSet);
                    selected = null;
                }
            }
            else {
                if (selected != null) tilemap.SetColor((Vector3Int)selected, Color.white);
                selected = toSet;
                tilemap.SetColor((Vector3Int)selected, selectedColour);
            }
        }
    }
    private void removeSelected() {
        if (selected != null) {
            tilemap.SetColor((Vector3Int)selected, Color.white);
            selected = null;
        }
    }
    #endregion
}
