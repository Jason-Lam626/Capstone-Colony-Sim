using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private GameObject grid;
    private GameObject tileMapGameObject;
    private Tilemap tileMap;

    private List<TileMapLevel> mapLevels;
    private const int LEVEL_DISTANCE = 5;

    private CharacterManager characterManager;


    private void Awake()
    {
        mapLevels = new List<TileMapLevel>();
        CreateGrid();
        CreateTileMap();
        CreateLevel();
        
        characterManager = new CharacterManager(tileMap);
        characterManager.AddCharacter((Player)FindObjectOfType(typeof(Player)));
    }

    void Start()
    {
        //InitializeTrigMap(); // alternative to InitializeTileMap()
    }

    void Update()
    {
        updateGameObjects();
    }

    private void updateGameObjects()
    {
        Vector3Int tileLocation = getTileLocation();

        if (Input.GetMouseButtonDown(0)) { // Mouse left click
            // Set target location for characters
            characterManager.setCharactersPath(tileLocation, getTilesLevel(tileLocation));
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            // Add an additional level to map
            CreateLevel();
	    }
        characterManager.UpdateCharacters(mapLevels);
    }

    // Get the level the tile is located on
    private int getTilesLevel(Vector3Int tileLocation)
    {
        int level = tileLocation.x / LEVEL_DISTANCE;
        return level;
    }

    // Get the tile's location in the Tilemap
    private Vector3Int getTileLocation()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return tileMap.WorldToCell(mousePosition);
    }

    private void CreateGrid()
    {
        grid = new GameObject("Grid");
        grid.AddComponent<Grid>();
    }

    private void CreateTileMap()
    {
        tileMapGameObject = new GameObject("TileMap");
        tileMap = tileMapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tileMapGameObject.AddComponent<TilemapRenderer>();
        tileMapGameObject.transform.SetParent(grid.transform);
    }

    // Create a level within the map
    private void CreateLevel()
    {
        int xMin, xMax, yMin, yMax;
        if(mapLevels.Count == 0) { // First level
            xMin = 0; xMax = LEVEL_DISTANCE-1; yMin = 0; yMax = LEVEL_DISTANCE-1;
        } else { // Additional levels, seperated by 1 tile
            xMin = mapLevels[mapLevels.Count - 1].getXMax() + 1;
            xMax = xMin + LEVEL_DISTANCE-1;
            yMin = mapLevels[mapLevels.Count - 1].getYMin();
            yMax = yMin + LEVEL_DISTANCE-1;
	    }
        // Add new level to map levels list
        mapLevels.Add(new TileMapLevel(mapLevels.Count, xMin, xMax, yMin, yMax));

        // Set levels tiles
        for(int x = xMin; x<xMax; x++) { 
            for(int y = yMin; y<yMax; y++) {
                GrassTile grassTile = ScriptableObject.CreateInstance<GrassTile>();
                grassTile.InitializeTileData(x, y, TileType.GRASS, false);
                tileMap.SetTile(new Vector3Int(x, y, 0), grassTile);
	        }
	    }

        // Set stairs in random location in level
        int randomX = UnityEngine.Random.Range(xMin, xMax);
        int randomY = UnityEngine.Random.Range(yMin, yMax);
        Stairs stairs = ScriptableObject.CreateInstance<Stairs>();
        stairs.InitializeTileData(randomX, randomY, TileType.STAIRS, false);
        tileMap.SetTile(new Vector3Int(randomX, randomY, 0), stairs);
        mapLevels[mapLevels.Count - 1].setStairsPosition(new Vector3Int(randomX, randomY, 0));
    }

    // Create random test map -- for connors testing
    /*
    private void InitializeTrigMap()
    {
        GlobalInstance.Instance.prefabList.InitializePrefabDictionary();

        // coords go from -max to +max
        int maxX = 100;
        int maxY = 75;

        // create random parameters
        float a = UnityEngine.Random.Range(0.0f, 6.28318f);
        float b = UnityEngine.Random.Range(0.0f, 6.28318f);
        float c = UnityEngine.Random.Range(0.0f, 6.28318f);
        float d = UnityEngine.Random.Range(0.0f, 6.28318f);
        Debug.Log("Generating map with paramters: " + a + ", " + b + ", " + c + ", " + d);

        // precalculate
        float[] yp = new float[maxY * 2 + 1];
        float[] xp = new float[maxX * 2 + 1];
        for (int y = 0; y <= 2 * maxY; y++)
            yp[y] = 4.0f * (y-maxY) / maxY;
        for (int x = 0; x <= 2 * maxX; x++)
            xp[x] = 4.0f * (x-maxX) / maxX;

        float[,] yterms = new float[2, maxY * 2 + 1];
        float[,] xterms = new float[2, maxX * 2 + 1];
        for (int y = 0; y <= 2 * maxY; y++)
        {
            yterms[0, y] = Mathf.Cos(yp[y] + b);
            yterms[1, y] = Mathf.Sin(yp[y] - c);
        }
        for (int x = 0; x <= 2 * maxX; x++)
        {
            xterms[0, x] = Mathf.Cos(xp[x] + a);
            xterms[1, x] = Mathf.Sin(xp[x] - d);
        }
        int treecount = 0;

        // generate
        // land is defined as Mathf.Pow((xp+Mathf.Cos(xp+a)-Mathf.Sin(yp-c)),2) + Mathf.Pow((yp + Mathf.Cos(yp + b) - Mathf.Sin(xp - d)), 2) <= 4
        for (int y=-maxY; y<=maxY; y++)
        {
            for(int x=-maxX; x<=maxX; x++)
            {
                float term1 = xp[x + maxX] + xterms[0, x + maxX] - yterms[1, y + maxY];
                float term2 = yp[y + maxY] + yterms[0, y + maxY] - xterms[1, x + maxX];
                if (term1*term1+term2*term2 <= 4)
                {
                    GrassTile grassTile = ScriptableObject.CreateInstance<GrassTile>();
                    grassTile.InitializeTileData(x, y, TileType.GRASS, false);
                    tileMap.SetTile(new Vector3Int(x, y, 0), grassTile);

                    
                    if(UnityEngine.Random.Range(0,10)==0)   // create tree
                    {
                        GameObject tree = GlobalInstance.Instance.entityDictionary.InstantiateEntity("tree", "", new Vector3(x + 0.5f, y + 0.5f, 0f));
                        tree.transform.parent = grid.transform;
                        LaborOrderManager.addLaborOrder(new LaborOrder_Woodcut(tree)); // needs to be updated
                        treecount++;
                    }
                    else if (UnityEngine.Random.Range(0, 100) == 0)  // create pawn
                    {
                        GameObject pawn = GlobalInstance.Instance.entityDictionary.InstantiateEntity("pawn2", "", new Vector3(x + 0.5f, y + 0.5f, 0f));
                        pawn.transform.parent = grid.transform;
                        //LaborOrderManager.addPawn(pawn.GetComponent<Pawn>()); // needs to be updated
                    }

                        
                }
                else
                {
                    WaterTile waterTile = ScriptableObject.CreateInstance<WaterTile>();
                    waterTile.InitializeTileData(x, y, TileType.WATER, true);
                    tileMap.SetTile(new Vector3Int(x, y, 0), waterTile);
                    
                }
            }
        }

        //Debug.Log("trees: " + treecount);
        //Debug.Log("orders: " + LaborOrderManager.getNumOfLaborOrders());
    }*/
}
