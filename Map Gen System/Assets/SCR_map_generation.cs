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

    [SerializeField]
    private string seedString;

    public Dictionary<Vector2, RuleTile> mapData = new Dictionary<Vector2, RuleTile>();

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
        print(validString);

        int x = 0;
        int y = 0;

        Int32.TryParse(validString.Substring(0, 5), out x);
        Int32.TryParse(validString.Substring(5), out y);

        Vector2 offset = new Vector2(x, y);

        return offset;
    }

    public void generate() {
        Vector2 seed = readSeed(seedString);
        tilemap.ClearAllTiles();
        mapData.Clear();

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int perlin = getPerlinID(pos, seed);
                if (perlin != 0) {
                    tilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y), tiles[perlin - 1]);
                    mapData.Add(pos, tiles[perlin - 1]);
                }
                else {
                    mapData.Add(pos, null);
                }
            }
        }
    }
    private int getPerlinID(Vector2 v, Vector2 offset) {
        float raw_perlin = Mathf.PerlinNoise(
            (v.x + offset.x) / islandSize,
            (v.y + offset.y) / islandSize
        );
        int scaleBy = tiles.Count + 1;
        float scaled_perlin = Mathf.Clamp01(raw_perlin) * scaleBy;

        return Mathf.FloorToInt(scaled_perlin);
    }
    #region utils
    public Vector2 mapCentre(bool round = false) {
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
