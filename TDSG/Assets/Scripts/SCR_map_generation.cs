using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;
using Random = System.Random;
using IzzetUtils.IzzetAttributes;

public class SCR_map_generation : MonoBehaviour {

    [Header("Map Base")]
    [SerializeField] [Tooltip("Main tile for map")] private RuleTile groundTile;
    [SerializeField] [Tooltip("Water tile for map")] private RuleTile waterTile;
    [SerializeField] [Tooltip("Where to place tile")] private Tilemap tilemap;
    [SerializeField] [Tooltip("Where to place water tile")] private Tilemap waterTilemap;
    [SerializeField] [Tooltip("Magnify how large perlin map is")] private int islandSize = 8;
    [SerializeField] [Tooltip("Number of tiles (x)")] private int sizeX;
    [SerializeField] [Tooltip("Number of tiles (y)")] private int sizeY;
    

    [Header("Map Gatherables")]
    [SerializeField] [Tooltip("Inspector friendly, passed to 'colorToType' dictionary on awake")] private List<gathableData> gathables;
    [SerializeField] [MyReadOnly] [Tooltip("Holds pixels to be used for the end map")] private Texture2D mapTex;
    [SerializeField] [MyReadOnly] [Tooltip("Holds pixels to be used for the end map")] int distributionStep = 1;
    [SerializeField] [Tooltip("Reduce gatherables by amount")] private int reduceGatherablesBy;

    [Header("Other")]
    [SerializeField] [Tooltip("Temp start pos of player")] private Vector2 playerStartPos;

    #region Won't be Serialised
    public Dictionary<Color32, SCO_gatherable> colorToType = new Dictionary<Color32, SCO_gatherable>(); //Maps colour to gatherable scriptable object
    private Dictionary<Vector2, Color32> posToColour = new Dictionary<Vector2, Color32>();

    [System.Serializable]
    public struct gathableData {
        public Color color;
        public SCO_gatherable data;
    }
    #endregion

    private void Awake() {
        passToDictionary();

        tilemap.transform.position = new Vector3(-0.5f, -0.5f);

        distributionStep = 1;
    }

    #region Setup
    private void passToDictionary() {
        foreach (gathableData item in gathables) {
            colorToType.Add(item.color, item.data);
        }
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
    public void generate(string seedString = "") {
        tilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();

        if (seedString == "") { //If string is empty, create one
            seedString = randomSeed();
        }
        Vector2 seed = readSeed(seedString);

        List<Color32> colours = colorToType.Keys.ToList();
        colours.Add(Color.white);


        Texture2D gatherablesTexture = generatePerlinTexture(seed, islandSize, colours); //Generate distribued gatherables

        mapTex = gatherablesTexture;

        GameObject gatherableParent = new GameObject("Gatherables Parent");

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);
                Vector3Int posInt = new Vector3Int((int)pos.x, (int)pos.y);

                Color currentColour = mapTex.GetPixel((int)pos.x, (int)pos.y); //Get pixel on texture

                //Debug.Log("Colours Contained = " + colorToTile.ContainsKey(currentColour));
                //Debug.Log("Current Pos: " + pos);


                //If bounds, can't place gatherable
                bool isBound = false;
                if (pos.x == 0 || pos.y >= sizeX - 1) {
                    isBound = true;
                }
                else if (pos.y == 0 || pos.x >= sizeY - 1) {
                    isBound = true;
                }

                if (currentColour != Color.black) {
                    tilemap.SetTile(posInt, groundTile);

                    if (currentColour != Color.white && !isBound) { //If pixel isn't white, place gatherable from dictionairy 
                        colorToType[currentColour].gatherableSetup(pos, gatherableParent.transform);
                    }
                    //TEMP
                    else {
                        playerStartPos = pos;
                    }
                    //TEMP
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
    private Texture2D generatePerlinTexture(Vector2 seed, int islandSize, List<Color32> successColours) {

        Texture2D tex = new Texture2D(sizeX, sizeY);

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int seededID = getUnorderedPerlinID(pos, seed, islandSize, successColours.Count);
                if (seededID != 0) {
                    posToColour.Add(pos, successColours[seededID-1]);
                    tex.SetPixel((int)pos.x, (int)pos.y, successColours[seededID - 1]);
                }
                else {
                    tex.SetPixel((int)pos.x, (int)pos.y, Color.black);
                }
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        return tex;
    }
    //Return perlin
    private int getBasePerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        float rawPerlin = Mathf.PerlinNoise(
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        );

        int scaleBy = count + 1;
        float scaledPerlin = Mathf.Clamp01(rawPerlin) * scaleBy;

        if (scaledPerlin > count) return count;

        return Mathf.FloorToInt(scaledPerlin);
    }
    
    //Return "random" seeded noise
    private int getUnorderedPerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        int bounds = getBasePerlinID(v, offset, islandSize, 1);
        int[] soundroundings = {
            getBasePerlinID(v, offset+Vector2.left, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.down, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.left+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.left+Vector2.down, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right+Vector2.up, islandSize, 1),
            getBasePerlinID(v, offset+Vector2.right+Vector2.down, islandSize, 1),
        };

        int rand = 0;
        if (bounds == 1) {
            Random seedFormat = new Random((int)(offset.magnitude + v.magnitude * Mathf.Pow(distributionStep, 4)));
            rand = seedFormat.Next(0, count + reduceGatherablesBy);
            if (rand > count || rand < 1 || soundroundings.Contains<int>(0)) {
                return count;
            } 
        }
        _ = (distributionStep > 50) ? distributionStep = 1 : distributionStep++;
        return rand;
    }
    #endregion
    #region utils
    public void removeTile(Vector2 pos) {
        tilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y), null);
    }
    public Vector2 startPos() {
        return playerStartPos;
    }
    #endregion
}
