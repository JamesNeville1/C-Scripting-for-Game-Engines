using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;
using IzzetUtils.IzzetAttributes;
using IzzetUtils;

public class SCR_master_map : MonoBehaviour {

    [System.Serializable]
    public struct PASS_gathableDataToPass {
        [Range(0, 1000)]
        public int id;
        public gatherableData dataToPass;
    }
    [System.Serializable]
    public struct gatherableData {
        public SCO_gatherable objectData;
        public int randomWieght;
    }

    [Header("Map Base")]
    [SerializeField] [Tooltip("Main tile for map")] private RuleTile groundTile;
    [SerializeField] [Tooltip("Where to place tile")] private Tilemap groundTilemap;
    [SerializeField] [Tooltip("Magnify how large perlin map is")] private int islandSize = 8;
    [SerializeField] [Tooltip("")] private List<Vector2Int> constantTiles;

    [Header("Map Gatherables")]
    [SerializeField] [Tooltip("Inspector friendly, passed to 'colorToType' dictionary on awake")] private PASS_gathableDataToPass[] gathables;
    [SerializeField] [MyReadOnly] [Tooltip("Distributes more evenly")] int distributionStep = 1;
    [SerializeField] [Tooltip("Reduce gatherables by amount")] private int reduceGatherablesBy;
    [SerializeField] [Tooltip("")] private string gatherablesParentName;

    [Header("Other")]
    [SerializeField] [MyReadOnly] [Tooltip("Temp start pos of player")] private Vector2 playerStartPos;

    //
    private Dictionary<int, gatherableData> idToGatherable = new Dictionary<int, gatherableData>(); //Maps colour to gatherable scriptable object
    private Dictionary<Vector2Int, int> mapData = new Dictionary<Vector2Int, int>();

    #region Set Instance
    private static SCR_master_map instance;

    private void Awake(){
        instance = this;
    }

    public static SCR_master_map returnInstance() {
        return instance;
    }
    #endregion

    #region Setup
    public void setup(string seedString, string groundTilemapName, Vector2Int size) { //Set up map in external scene
        foreach (PASS_gathableDataToPass item in gathables) { //Pass gatherables to dictionairy
            idToGatherable.Add(item.id, item.dataToPass);
        }
        gathables = null; //Set to null to avoid accidentally using it

        groundTilemap = GameObject.Find(groundTilemapName).GetComponent<Tilemap>(); //Get the tilemap because it is in another scene

        distributionStep = 1; //Set distributionStep to 1 to stop an error

        //If seed is empty, make random seed
        if(seedString == "") {
            seedString = randomSeed();
        }

        Vector2Int seed = readSeed(seedString); //Read seed, convert to vector2Int

        groundTilemap.ClearAllTiles(); //Clear to be sure

        optimisedGeneration(size, seed); //Main map gen (Optimised version, old is in Legacy region)
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

        Debug.Log($"Random Seed: {newSeed}");

        return newSeed;
    }

    //Read, authenticate seed, output Vector2
    public Vector2Int readSeed(string seed = "") {

        if(seed.Length > 10) { //Constain to 10
            seed = seed.Substring(1, 10);
        }

        int loopFor = seed.Length;
        string validString = "";
        for (int i = 0; i < loopFor; i++) {
            if (char.IsDigit(seed[i])) { 
                validString += seed[i];
            }
        }

        loopFor = 10 - (validString.Length);

        for (int i = 0; i < loopFor; i++) {
            validString += "0";
        }

        int x = 0;
        int y = 0;

        Int32.TryParse(validString.Substring(0, 5), out x);
        Int32.TryParse(validString.Substring(5), out y);

        Vector2Int offset = new Vector2Int(x, y);

        Debug.Log($"Valid Seed: {offset}");
        
        return offset;
    }
    #endregion
    #region Generate
    private void optimisedGeneration(Vector2Int size, Vector2Int seedValid) {
        GameObject gatherableParent = GameObject.Find(gatherablesParentName); //Get ref to gatherable parent, keep neat

        //Main
        for (int x = 0; x < size.x; x++) { //Loop through x
            for (int y = 0; y < size.y; y++) { //Loop through y
                Vector2Int pos = new Vector2Int(x, y);

                int perlin = getBasePerlinID(pos, seedValid, islandSize); //Get perlin
                if (perlin == 1) { //If perlin is 1, it's ground
                    
                    groundTilemap.SetTile((Vector3Int)pos,groundTile); //Set tile to ground

                    bool isBound = pos.x <= 0 || pos.y >= size.x - 1 || pos.y <= 0 || pos.x >= size.x - 1; //Dont put gatherable on bounds of map
                    int idToCheck = getGatherableID(pos, seedValid, calculateTotalWeight()); //Get gatherable ID

                    //If in bound and id is valid spawn gatherable
                    if (!isBound && !isBoundOfIsland(pos, seedValid, islandSize) && idToGatherable.ContainsKey(idToCheck)) {
                        idToGatherable[idToCheck].objectData.gatherableSetup(pos, gatherableParent.transform);
                    }
                }
            }
        }

        //Make Constant Tiles
        Vector2Int centre = returnCentre(size);
        foreach (Vector2Int pos in constantTiles) {
            Vector3Int centreAdjusted = (Vector3Int)pos + (Vector3Int)centre;
            if (groundTilemap.GetTile(centreAdjusted) == null) { //If tile is null, make it groundTile
                groundTilemap.SetTile(centreAdjusted, groundTile);
            }
        }

        //Mark Player Spawn
        playerStartPos = constantTiles[0] + centre;
    }
    #endregion
    #region Logic
    private int getBasePerlinID(Vector2Int v, Vector2 offset, int islandSize, int count = 1) { //Get Perlin
        float rawPerlin = Mathf.PerlinNoise( //Raw perlin position + seed, dividing it makes island size bigger
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        ); 

        //Clamp perlin between 0 and count
        float scaledPerlin = Mathf.Clamp01(rawPerlin) * (count + 1);

        return Mathf.FloorToInt(scaledPerlin);
    }
    
    private bool isBoundOfIsland(Vector2Int v, Vector2 offset, int islandSize) { //Check if neighbors are water, if yes, return true
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

        if (soundroundings.Contains<int>(0)) { //Return ground if water is near
            return true;
        }
        else return false; //Return valid for gatherable areas
    }
    private int getGatherableID(Vector2 v, Vector2 offset, int totalRandWeight) { //Get ID, return -1 on fail
        Random randomStart = new Random((int)(offset.magnitude + v.magnitude * Mathf.Pow(distributionStep, 4)));

        //Further "randomise" the gatherables as C#'s random class isn't perfect and does have patterns
        if (distributionStep > 50) distributionStep = 1; 
        else distributionStep++;

        int rand = randomStart.Next(1, totalRandWeight + reduceGatherablesBy); //(Adding an int to reduce by allows me to control how many spawn with one variable)

        if (rand > totalRandWeight) return -1;

        else return checkRandomAgainstID(randomStart);
    }
    private int checkRandomAgainstID(Random rand) { //Using random weight, get ID
        //Find gatherable in weight

        List<int> weights = new List<int>();
        foreach (var item in idToGatherable.Values) {
            weights.Add(item.randomWieght);
        }
        int index = IzzetMain.getRandomWeight(weights.ToArray(), rand);

        return index;

    }
    #endregion
    #region utils
    private Vector2Int returnCentre(Vector2Int size) {
        return IzzetMain.castToVector2Int(new Vector2(size.x / 2, size.y / 2));
    }
    private void removeTile(Vector2 pos) {
        groundTilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y), null);
    }
    public Vector2 startPos() {
        return playerStartPos;
    }
    private int calculateTotalWeight() {
        int totalWeight = 0;
        foreach (var weight in idToGatherable.Values) {
            totalWeight += weight.randomWieght;
        }
        return totalWeight;
    }
    public Tilemap returnGroundTilemap() {
        return groundTilemap;
    }
    #endregion
    #region Legacy (Map Gen System that used texture, very slow)
    //Generate map using tilemap
    //public void generate(Vector2Int size, Vector2Int seed) {
    //    Texture2D gatherablesTexture = generatePerlinTexture(seed, islandSize, size); //Generate distribued gatherables

    //    mapTex = gatherablesTexture;

    //    GameObject gatherableParent = GameObject.Find(gatherablesParentName);

    //    for (int x = 0; x < size.x; x++) {
    //        for (int y = 0; y < size.y; y++) {
    //            Vector3Int posInt = new Vector3Int((int)x, (int)y);

    //            Color currentColour = mapTex.GetPixel((int)posInt.x, (int)posInt.y); //Get pixel on texture

    //            //Debug.Log("Colours Contained = " + colorToTile.ContainsKey(currentColour));
    //            //Debug.Log("Current Pos: " + pos);


    //            //Final small check to see if the gatherable is out of bounds
    //            bool isBound = posInt.x <= 0 || posInt.y >= size.x - 1 || posInt.y <= 0 || posInt.x >= size.x - 1;

    //            if (currentColour != Color.black) {
    //                groundTilemap.SetTile(posInt, groundTile);

    //                if (currentColour != Color.white && !isBound) { //If pixel isn't white, place gatherable from dictionairy 
    //                    colorToType[currentColour].objectData.gatherableSetup((Vector2Int)posInt, gatherableParent.transform);
    //                }
    //                #region Temporary
    //                else
    //                {
    //                    playerStartPos = (Vector2Int)posInt;
    //                }
    //                #endregion
    //            }
    //            else {
    //                waterTilemap.SetTile(posInt, waterTile);
    //            }
    //        }
    //    }
    //}

    //Generate all textures
    //private Texture2D generatePerlinTexture(Vector2 seed, int islandSize, Vector2Int mapSize) {

    //    Texture2D tex = new Texture2D(mapSize.x, mapSize.y);

    //    int totalWeight = calculateTotalWeight();

    //    for (int x = 0; x < mapSize.x; x++) {
    //        for (int y = 0; y < mapSize.y; y++) {
    //            Vector2Int pos = new Vector2Int(x, y);

    //            mapTileState tileState = getSinglePixel(pos, seed, islandSize); //Get single pixel

    //            Color32 col;
    //            switch (tileState) {
    //                case mapTileState.EMPTY:
    //                    col = Color.black;
    //                    break;
    //                case mapTileState.GROUND:
    //                    col = Color.white;
    //                    break;
    //                default:
    //                    col = returnRandomGatherable(pos, seed, totalWeight);
    //                    break;
    //            }

    //            tex.SetPixel((int)pos.x, (int)pos.y, col);
    //        }
    //    }
    //    tex.filterMode = FilterMode.Point;
    //    tex.Apply();

    //    return tex;
    //}
    #endregion
}
