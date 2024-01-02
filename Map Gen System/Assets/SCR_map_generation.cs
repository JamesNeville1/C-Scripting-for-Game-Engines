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

    public void generate() {
        tilemap.ClearAllTiles();
        mapData.Clear();

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                Vector2 pos = new Vector2(x, y);

                int perlin = getPerlinID(pos, Vector2.zero);
                if (perlin == 0) {
                    tilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y), tiles[perlin]);
                    mapData.Add(pos, tiles[perlin]);
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
