using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;
using Color = UnityEngine.Color;
using Random = System.Random;

public class SCR_map_generation : MonoBehaviour {

    [SerializeField]
    private RuleTile tiles;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int islandSize = 8;

    [SerializeField]
    private int sizeX;

    [SerializeField]
    private int sizeY;

    public Dictionary<Vector2, RuleTile> mapData = new Dictionary<Vector2, RuleTile>();
    public Dictionary<Color32, SCO_gatherable> colorToTile = new Dictionary<Color32, SCO_gatherable>();

    [System.Serializable]
    public struct gathableData {
        public Color color;
        public SCO_gatherable data;
    }

    [SerializeField]
    private List<gathableData> gathables;

    [SerializeField]
    private Texture2D mapTex;

    [SerializeField]
    private int reduceGatherablesBy;

    public Texture2D test;

    private void Awake() {
        foreach (gathableData item in gathables) {
            colorToTile.Add(item.color, item.data);
        }

        tilemap.transform.position -= new Vector3(0.5f, 0.5f);
    }
    public string randomSeed() {
        string newSeed = "";
        for (int i = 0; i < 10; i++) {
            int currentNum = UnityEngine.Random.Range(0, 10);
            newSeed += currentNum;
        }
        return newSeed;
    }

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

    public void generate(string seedString = "") {
        tilemap.ClearAllTiles();

        if(seedString == "") {
            seedString = randomSeed();
        }
        Vector2 seed = readSeed(seedString);
        Texture2D perlinTexture = generatePerlinTexture(seed, islandSize, getBasePerlinID);
        Texture2D gatherablesTexture = generatePerlinTexture(seed, islandSize, getUnorderedPerlinID, colorToTile.Keys.ToList(), colorToTile.Keys.ToList().Count);
        test = gatherablesTexture;

        mapTex = (Texture2D)mergeTextures(perlinTexture, gatherablesTexture);

        GameObject gatherableParent = new GameObject("Gatherables Parent");

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);
                Vector3Int posInt = new Vector3Int((int)pos.x, (int)pos.y);

                Color currentColour = mapTex.GetPixel((int)pos.x, (int)pos.y);
                //Debug.Log("Colours Contained = " + colorToTile.ContainsKey(currentColour));

                if (currentColour != Color.black) {
                    tilemap.SetTile(posInt, tiles);

                    if(currentColour != Color.white) {
                        colorToTile[currentColour].gatherableSetup(pos, gatherableParent.transform);                        
                    }
                }
            }
        }
    }

    private Texture mergeTextures(Texture2D perlin, Texture2D gatherables) {
        Texture2D tex = perlin;

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                if (perlin.GetPixel(x, y) != Color.black && gatherables.GetPixel(x, y) != Color.black) {
                    tex.SetPixel(x, y, gatherables.GetPixel(x,y));
                }
            }
        }
        tex.filterMode = FilterMode.Point;

        tex.Apply();
        return tex;
    }

    private Texture2D generatePerlinTexture(Vector2 seed, int islandSize, Func<Vector2,Vector2,int,int,int> action, 
        List<Color32> successColours = null, int scaleBy = 1, Func<Texture2D, Texture2D> finalCheck = null) {

        Texture2D tex = new Texture2D(sizeX, sizeY);
        if (successColours == null) {
            successColours = new List<Color32>{
                Color.white
            };
        }

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int seededID = action(pos, seed, islandSize, scaleBy);
                if (seededID != 0) {
                    //Debug.Log("seededID: " + seededID + " SuccessColours: " + successColours.Count);
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
    
    private int getBasePerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        float rawPerlin = Mathf.PerlinNoise(
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        );

        int scaleBy = count + 1;
        float scaledPerlin = Mathf.Clamp01(rawPerlin) * scaleBy;

        return Mathf.FloorToInt(scaledPerlin);
    }
    
    private int getUnorderedPerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        int bounds = getBasePerlinID(v, offset, islandSize, 1);
        if(bounds == 1) {
            int rand = UnityEngine.Random.Range(0, count + reduceGatherablesBy);
            if (rand > count) return 0;
            
            return rand;
        }
        return bounds;
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
    #endregion
}
