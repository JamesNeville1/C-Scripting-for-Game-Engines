using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using Color = UnityEngine.Color;
using Random = System.Random;

public class SCR_map_generation : MonoBehaviour {

    public Dictionary<Vector2, RuleTile> mapData = new Dictionary<Vector2, RuleTile>(); //Hold tile data

    [Header("Map Base")]
    [Tooltip("Main tile for map")] [SerializeField]
    private RuleTile groundTile;

    //[SerializeField]
    //private RuleTile boundsTile;

    [SerializeField]
    private RuleTile waterTile;

    [Tooltip("Where to place tile")] [SerializeField]
    private Tilemap tilemap;
    [Tooltip("Where to place water tile")] [SerializeField]
    private Tilemap waterTilemap;
    //[Tooltip("Where to place water tile")] [SerializeField]
    //private Tilemap boundsTileMap;

    [Tooltip("Magnify how large perlin map is")] [SerializeField]
    private int islandSize = 8;

    [Tooltip("Number of tiles (x)")] [SerializeField]
    private int sizeX;

    [Tooltip("Number of tiles (y)")] [SerializeField]
    private int sizeY;
    

    [System.Serializable]
    public struct gathableData { 
        public Color color;
        public SCO_gatherable data;
    }

    [Header("Map Gatherables")]
    [Tooltip("Inspector friendly, passed to 'colorToType' dictionary on awake")] [SerializeField]
    private List<gathableData> gathables;
    public Dictionary<Color32, SCO_gatherable> colorToType = new Dictionary<Color32, SCO_gatherable>(); //Maps colour to gatherable scriptable object

    private Dictionary<Vector2, Color32> posToColour = new Dictionary<Vector2, Color32>();

    [Tooltip("Holds pixels to be used for the end map")] [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private Texture2D mapTex;

    [Tooltip("Holds pixels to be used for the end map")] [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    int distributionStep;

    [Tooltip("Reduce gatherables by amount")] [SerializeField]
    private int reduceGatherablesBy;

    [Header("Other")]
    [Tooltip("")][SerializeField]
    private Vector2 playerStartPos;

    [Header("TEMP")]
    public Texture2D test;

    private void Awake() {
        foreach (gathableData item in gathables) {
            colorToType.Add(item.color, item.data);
        }

        tilemap.transform.position = new Vector3(-0.5f, -0.5f);

        distributionStep = 1;
    }

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

                    if (currentColour != Color.white && !isBound)
                    { //If pixel isn't white, place gatherable from dictionairy 
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

        //tex = cleanUp(tex); //Fix

        return tex;
    }

    //Final Clean-Up
    private Texture2D cleanUp(Texture2D raw) {
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeX; y++) {
                if(checkPixel(x, y)) {
                    raw.SetPixel(x, y, Color.white);
                }
            }
        }
        return raw;
    }

    private bool checkPixel(int x, int y) {
        Vector2 pos = new Vector2(x, y);

        Vector2[] toCheck = {
            pos + Vector2.left,
            pos + Vector2.right,
            pos + Vector2.up,
            pos + Vector2.down,
                    
            pos + Vector2.left + Vector2.up,
            pos + Vector2.right + Vector2.up,
            pos + Vector2.left + Vector2.down,
            pos + Vector2.right + Vector2.down,
        };

        foreach (Vector2 v in toCheck) {
            if (posToColour[v] == Color.black) {
                return true;
            }
        }
        return false;
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
    #region utils
    public Vector2 mapCentre(bool round = false) {
        if(mapData.Count == 0) return new Vector2(0, 0);
        Vector2 dist = (mapData.ElementAt(0).Key + mapData.ElementAt(mapData.Count - 1).Key) / 2;

        if (round) {
            return new Vector2(Mathf.Round(dist.x), Mathf.Round(dist.y));
        }
        return dist;
    }
    public void removeTile(Vector2 pos) {
        tilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y), null);
    }
    public Vector2 startPos() {
        return playerStartPos;
    }
    #endregion
}