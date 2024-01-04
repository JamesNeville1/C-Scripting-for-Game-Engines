using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SCR_map_generation : MonoBehaviour {

    [SerializeField]
    private List<RuleTile> tiles = new List<RuleTile>();

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int islandSize = 8;

    [SerializeField]
    private int sizeX;

    [SerializeField]
    private int sizeY;

    public Dictionary<Vector2, RuleTile> mapData = new Dictionary<Vector2, RuleTile>();

    [SerializeField]
    private Texture2D mapTex;

    private int randomPerlin;

    private void Awake() {
        randomPerlin = 0;
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
        if(seedString == "") {
            seedString = randomSeed();
        }
        Vector2 seed = readSeed(seedString);
        Texture2D perlinTexture = generatePerlinTexture(seed, islandSize, Color.white, getPerlinID,tiles.Count);
        Vector2 gatherableseed = readSeed(seedString) * 5;
        Texture2D gatherablesTexture = generatePerlinTexture(seed, 2, Color.green, getUnorderedPerlinID);

        mapTex = (Texture2D)mergeTextures(perlinTexture, gatherablesTexture);

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                Color currentColour = mapTex.GetPixel((int)pos.x, (int)pos.y);

                if (currentColour != Color.black) {
                    tilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y), tiles[0]);
                    print("white");

                    if(currentColour == Color.green) { //Map colour to gatherable, this is just a demo
                        tilemap.SetColor(new Vector3Int((int)pos.x, (int)pos.y), Color.green);
                        print("Green");
                    }
                }
                else {
                    print("black");
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
    private Texture2D generateGatherablesTexture(Vector2 seed) {
        Texture2D tex = new Texture2D(sizeX, sizeY);

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int randDemo = UnityEngine.Random.Range(0, 20);
                if (randDemo == 0) {
                    tex.SetPixel((int)pos.x, (int)pos.y, Color.green);
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
    private Texture2D generatePerlinTexture(Vector2 seed, int islandSize, Color successColour, Func<Vector2,Vector2,int,int,int> action, int scaleBy = 1) { //add border
        Texture2D tex = new Texture2D(sizeX, sizeY);

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int perlin = action(pos, seed, islandSize, scaleBy);
                if (perlin != 0) {
                    tex.SetPixel((int)pos.x, (int)pos.y, successColour);
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
    private int getPerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        float raw_perlin = Mathf.PerlinNoise(
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        );
        int scaleBy = count + 1;
        float scaled_perlin = Mathf.Clamp01(raw_perlin) * scaleBy;

        return Mathf.FloorToInt(scaled_perlin);
    }
    private int getUnorderedPerlinID(Vector2 v, Vector2 offset, int islandSize, int count = 1) {
        int rawPerlin = getPerlinID(v, offset, islandSize, count);
        randomPerlin ++;
        _ = (randomPerlin > 20) ? randomPerlin = 1 : randomPerlin++;
        print(randomPerlin);
        return rawPerlin;
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
