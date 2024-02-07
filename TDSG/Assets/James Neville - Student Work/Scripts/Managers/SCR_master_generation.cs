using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;
using Random = System.Random;
using IzzetUtils.IzzetAttributes;
using IzzetUtils;
using System.Drawing;

public class SCR_master_generation : MonoBehaviour {

    //Structure & Enums
    private enum mapTileState {
        EMPTY,
        GROUND,
        CANGATHERABLE
    }

    [System.Serializable]
    public struct gathableDataToPass {
        public Color color;
        public gatherableData dataToPass;
    }
    [System.Serializable]
    public struct gatherableData {
        public SCO_gatherable objectData;
        public int randomWieght;
    }

    [Header("Map Base")]
    [SerializeField] [Tooltip("Main tile for map")] private RuleTile groundTile;
    [SerializeField] [Tooltip("Water tile for map")] private RuleTile waterTile;
    [SerializeField] [Tooltip("Where to place tile")] private Tilemap groundTilemap;
    [SerializeField] [Tooltip("Where to place water tile")] private Tilemap waterTilemap;
    [SerializeField] [Tooltip("Magnify how large perlin map is")] private int islandSize = 8;
    

    [Header("Map Gatherables")]
    [SerializeField] [Tooltip("Inspector friendly, passed to 'colorToType' dictionary on awake")] private gathableDataToPass[] gathables;
    [SerializeField] [MyReadOnly] [Tooltip("Holds pixels to be used for the end map")] private Texture2D mapTex;
    [SerializeField] [MyReadOnly] [Tooltip("Distributes more evenly")] int distributionStep = 1;
    [SerializeField] [Tooltip("Reduce gatherables by amount")] private int reduceGatherablesBy;
    [SerializeField] [Tooltip("")] private string gatherablesParentName;

    [Header("Other")]
    [SerializeField] [MyReadOnly] [Tooltip("Temp start pos of player")] private Vector2 playerStartPos;

    #region Won't be Serialised
    private Dictionary<Color32, gatherableData> colorToType = new Dictionary<Color32, gatherableData>(); //Maps colour to gatherable scriptable object
    //private Dictionary<Vector2, Color32> posToColour = new Dictionary<Vector2, Color32>();
    #endregion
    #region Set Instance
    private static SCR_master_generation instance;

    private void Awake(){
        instance = this;
    }

    public static SCR_master_generation returnInstance() {
        return instance;
    }
    #endregion

    #region Setup
    public void setup(string seedString, string groundTilemapName, string waterTilemapName, Vector2Int size) {
        foreach (gathableDataToPass item in gathables) {
            colorToType.Add(item.color, item.dataToPass);
        }
        gathables = null;

        groundTilemap = GameObject.Find(groundTilemapName).GetComponent<Tilemap>();
        waterTilemap = GameObject.Find(waterTilemapName).GetComponent<Tilemap>();

        groundTilemap.transform.position = new Vector3(-0.5f, -0.5f);

        distributionStep = 1;

        generate(size, seedString);
    }
    #endregion
    #region Seed Generation & Authentication
    //Randomly generates 10 digit seed
    public string randomSeed() {
        string newSeed = "";
        for (int i = 0; i < 10; i++) {
            int currentNum = UnityEngine.Random.Range(0, 10);
            newSeed += currentNum;
        }
        return newSeed;
    }

    //Read, authenticate seed, output Vector2
    public Vector2 readSeed(string seed = "") {

        if(seed.Length > 10) {
            seed = seed.Substring(1, 10);
        }

        int loopFor = seed.Length - 1;
        string validString = "";
        for (int i = 0; i < loopFor; i++) {
            if (char.IsDigit(seed[i])) { 
                validString += seed[i];
            }
        }

        loopFor = 10 - validString.Length;

        for (int i = 0; i < loopFor; i++) {
                validString += "0";
        }

        int x = 0;
        int y = 0;

        Int32.TryParse(validString.Substring(0, 5), out x);
        Int32.TryParse(validString.Substring(5), out y);

        Vector2 offset = new Vector2(x, y);

        return offset;
    }
    #endregion
    #region Generate From Texture
    //Generate map using tilemap
    public void generate(Vector2Int size, string seedString) {
        
        //Clear all tiles just incase
        groundTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();

        Vector2 seed = readSeed(seedString);


        Texture2D gatherablesTexture = generatePerlinTexture(seed, islandSize, size); //Generate distribued gatherables

        mapTex = gatherablesTexture;

        GameObject gatherableParent = GameObject.Find(gatherablesParentName);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector2 pos = new Vector2(x, y);
                Vector3Int posInt = new Vector3Int((int)pos.x, (int)pos.y);

                Color currentColour = mapTex.GetPixel((int)pos.x, (int)pos.y); //Get pixel on texture

                //Debug.Log("Colours Contained = " + colorToTile.ContainsKey(currentColour));
                //Debug.Log("Current Pos: " + pos);


                //Final small check to see if the gatherable is out of bounds
                bool isBound = pos.x <= 0 || pos.y >= size.x - 1 || pos.y <= 0 || pos.x >= size.x - 1;

                if (currentColour != Color.black) {
                    groundTilemap.SetTile(posInt, groundTile);

                    if (currentColour != Color.white && !isBound) { //If pixel isn't white, place gatherable from dictionairy 
                        colorToType[currentColour].objectData.gatherableSetup(pos, gatherableParent.transform);
                    }
                    #region Temporary
                    else
                    {
                        playerStartPos = pos;
                    }
                    #endregion
                }
                else {
                    waterTilemap.SetTile(posInt, waterTile);
                }
            }
        }
    }
    #endregion
    #region Generate Texture
    //Generate all textures
    private Texture2D generatePerlinTexture(Vector2 seed, int islandSize, Vector2Int mapSize) {

        Texture2D tex = new Texture2D(mapSize.x, mapSize.y);

        int totalWeight = calculateTotalWeight();

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                Vector2 pos = new Vector2(x, y);

                mapTileState tileState = getSinglePixel(pos, seed, islandSize); //Get single pixel
                
                Color32 col;
                switch (tileState) {
                    case mapTileState.EMPTY:
                        col = Color.black;
                        break;
                    case mapTileState.GROUND:
                        col = Color.white;
                        break;
                    default:
                        col = returnRandomGatherable(pos, seed, totalWeight);
                        break;
                }

                tex.SetPixel((int)pos.x, (int)pos.y, col);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        return tex;
    }
    //Return perlin
    private int getBasePerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        float rawPerlin = Mathf.PerlinNoise( //Raw perlin position + seed, dividing it makes island size bigger
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        ); 

        //Clamp perlin between 0 and count
        float scaledPerlin = Mathf.Clamp01(rawPerlin) * (count + 1);

        return Mathf.FloorToInt(scaledPerlin);
    }
    
    //Return single pixel as enum
    private mapTileState getSinglePixel(Vector2 v, Vector2 offset, int islandSize) {
        bool isGround = getBasePerlinID(v, offset, islandSize, 1) == 1; //Is this pixel ground?

        int[] soundroundings = { //All surounding pixels
            getBasePerlinID(v, offset+Vector2.left, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.down, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.left+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.left+Vector2.down, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right+Vector2.down, islandSize, 1),
        };

        if (isGround) {
            if (soundroundings.Contains<int>(0)) { //Return ground if water is near
                return mapTileState.GROUND;
            }
            else return mapTileState.CANGATHERABLE; //Return valid for gatherable areas
        }
        return mapTileState.EMPTY;
    }
    private Color32 returnRandomGatherable(Vector2 v, Vector2 offset, int totalRandWeight) {
        Random randomStart = new Random((int)(offset.magnitude + v.magnitude * Mathf.Pow(distributionStep, 4)));
        _ = (distributionStep > 50) ? distributionStep = 1 : distributionStep++; //Further "randomise" the gatherables as C#'s random class isn't perfect and does have patterns

        int rand = randomStart.Next(1, totalRandWeight + reduceGatherablesBy);

        if (rand > totalRandWeight) return Color.white; //If the random value is greater than the total, tile is empty

        else return checkIntAgainstColour(randomStart);
    }
    private Color32 checkIntAgainstColour(Random rand) {
        //Find gatherable in weight

        List<int> weights = new List<int>();
        foreach (var item in colorToType.Values) {
            weights.Add(item.randomWieght);
        }
        int index = IzzetMain.getRandomWeight(weights.ToArray(), rand);

        return colorToType.ElementAt(index).Key;

    }
    #endregion
    #region utils
    public void removeTile(Vector2 pos) {
        groundTilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y), null);
    }
    public Vector2 startPos() {
        return playerStartPos;
    }
    private int calculateTotalWeight() {
        int totalWeight = 0;
        foreach (var weight in colorToType.Values) {
            totalWeight += weight.randomWieght;
        }
        return totalWeight;
    }
    #endregion
}
