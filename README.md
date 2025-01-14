# Top-Down Survival Game

### This is a simple survival game I made for my C# Scripting for Game Engines module.
<br /><br />
Contains:
- Seeded Map Generation
- Crafting
- Inventory Management Mechanic
- Character Selection & Attribute Variation
<br /><br />


(My main code can be found [here](https://github.com/JamesNeville1/C-Scripting-for-Game-Engines/tree/main/TDSG/Assets/James%20Neville%20-%20Student%20Work/Scripts))
<br /><br />

## Seeded Map Generation
The SCR_master_map script uses the Perlin Noise algorithm to generate a seeded map. The use can also input a seed to play the same map multiple times.
<br /><br />

Here is the logic for authenticating the seed. Since it comes in as a string, we need to sanitize it and output a vector to be used. We then add 0 if the string inputted was too small. So 123456 would become X: 12345, Y: 60000.
```
//Read, authenticate seed, output as Vector
public Vector2Int ReadSeed(string seed = "") {

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
```
<br /><br />

If the string is empty, we generate a random seed.
```
//Randomly generates 10 digit seed
public string RandomSeed() {
    string newSeed = "";
    for (int i = 0; i < 10; i++) {
        int currentNum = UnityEngine.Random.Range(0, 10);
        newSeed += currentNum;
    }

    Debug.Log($"Random Seed: {newSeed}");

    return newSeed;
}
```
<br /><br />

We then Generate. This is a similar system to my map generator, although we use Rule Tiles here which automatically adapt to the surrounding edges.
```
private void OptimisedGeneration(Vector2Int size, Vector2Int seedValid) {
    GameObject gatherableParent = GameObject.Find(gatherablesParentName); //Get ref to gatherable parent, keep neat

    //Main
    for (int x = 0; x < size.x; x++) { //Loop through x
        for (int y = 0; y < size.y; y++) { //Loop through y
            Vector2Int pos = new Vector2Int(x, y);

            int perlin = GetBasePerlinID(pos, seedValid, islandSize); //Get perlin
            if (perlin == 1) { //If perlin is 1, it's ground
                
                groundTilemap.SetTile((Vector3Int)pos,groundTile); //Set tile to ground

                bool isBound = pos.x <= 0 || pos.y >= size.x - 1 || pos.y <= 0 || pos.x >= size.x - 1; //Dont put gatherable on bounds of map
                int idToCheck = GetGatherableID(pos, seedValid, CalculateTotalWeight()); //Get gatherable ID

                //If in bound and id is valid spawn gatherable
                if (!isBound && !IsBoundOfIsland(pos, seedValid, islandSize) && idToGatherable.ContainsKey(idToCheck)) {
                    idToGatherable[idToCheck].objectData.gatherableSetup(pos, gatherableParent.transform);
                }
            }
        }
    }

    //Make Constant Tiles
    Vector2Int centre = ReturnCentre(size);
    foreach (Vector2Int pos in constantTiles) {
        Vector3Int centreAdjusted = (Vector3Int)pos + (Vector3Int)centre;
        if (groundTilemap.GetTile(centreAdjusted) == null) { //If tile is null, make it groundTile
            groundTilemap.SetTile(centreAdjusted, groundTile);
        }
    }

    //Mark Player Spawn
    playerStartPos = centre + constantTiles[0];
}
```
We also make some "Constant Tiles", this is to make sure the player starts in the centre with no water.

<br /><br />

## Crafting
The crafting combines two items. We check if a and b are something, if not, we check if b and a are something. If one creates something, we destroy them and spawn the item.
```
private void Craft(SCR_inventory_piece a, SCR_inventory_piece b) {
    recipeData data = new recipeData();
    data.itemA = a.ReturnItem();
    data.itemB = b.ReturnItem();

    if (recipes.ContainsKey(data)) {
        CraftExecute(a, b, data);
        return;
    }
    if (recipes.ContainsKey(data.flip())) {
        CraftExecute(b, a, data);
        return;
    }
}

private void CraftExecute(SCR_inventory_piece a, SCR_inventory_piece b, recipeData key) {
    Destroy(a.gameObject);
    Destroy(b.gameObject);

    SCR_inventory_piece createdItem = SCR_inventory_piece.CreateInstance(recipes[key], craftingSlots[(int)craftingArrayPosName.OUTPUT].vec, slotsParent, false);
    Place(createdItem, craftingArrayPosName.OUTPUT);

    return;
}
```

## Inventory Management Mechanic
This is the setup for the Tetris-style item. As you can see I use scriptable objects to get the data of this specific item. 
```
private void Setup(SCO_item source, bool startActive) { //Called from create instance. It creates children acording to the source (item)
    active = startActive; mouseOver = startActive;
    
    Vector2Int[] blocks = source.ReturnSpaces();

    Sprite itemSprite = source.ReturnItemSprite();

    foreach (Vector2 blockPos in blocks) {
        GameObject newBlock = new GameObject("Block:" + blockPos.x + ", " + blockPos.y, typeof(SpriteRenderer));
        newBlock.transform.parent = transform;
        newBlock.transform.localPosition = blockPos;

        srs.Add(newBlock.GetComponent<SpriteRenderer>());
        int arrayPos = srs.Count - 1;

        myName = source.ReturnName();

        srs[arrayPos].sprite = itemSprite;
        srs[arrayPos].sortingOrder = 2;
        srs[arrayPos].sortingLayerName = "Inventory Piece";

        //newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        newBlock.AddComponent<BoxCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
    }

    
    gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    CompositeCollider2D compCol = gameObject.AddComponent<CompositeCollider2D>();
    compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
    compCol.isTrigger = true;

    slots = source.ReturnSpaces();

    pieceItem = source;
}
```
<br /><br />

When we try to place a piece, we check if we can, if not we return false, if we find a space we return true and put it into place.
```
public bool TryPlaceGrid(SCR_inventory_piece toManipulate) { //Try to place in grid, return false if fail
    toManipulate.transform.parent = cellParent;

    //Check if it can even be placed?
    Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toManipulate.transform.localPosition.x), Mathf.RoundToInt(toManipulate.transform.localPosition.y));
    if(!CheckPiece(toManipulate,pos)) {
        return false;
    }

    //If True
    toManipulate.transform.localPosition = (Vector2)pos;

    pieceData.Add(toManipulate, toManipulate.ReturnChildren(pos));

    return true;
}
```
<br /><br />

To check the piece, we go through the grid and check that the area on the grid exists and we check if it is occupied by another piece.
```
 private bool CheckPiece(SCR_inventory_piece piece, Vector2Int pos) { 
        
    //Loop through children, check all slots are free and valid
    Vector2Int[] children = piece.ReturnChildren(pos);
    foreach(Vector2Int vec in children) {
        bool invalid = !gridData.ContainsKey(vec) || gridData[vec] == cellState.OCCUPIED;
        if (invalid) return false;
    }

    //If passes the validation, set as occupied
    AdjustGridState(children, cellState.OCCUPIED);
    return true;
}
```
