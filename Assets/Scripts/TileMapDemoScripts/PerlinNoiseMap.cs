using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject grass;
    public GameObject rock;
    public GameObject sand;
    public GameObject water;

    int map_width = 100;
    int map_height = 100;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    //Best between 4 and 20
    float magnification = 14.0f;

    int x_offset = 0; //<- +>
    int y_offset = 0; //down- ^+

    void Start()
    {
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
    }

    public void CreateTileset()
    {
        /** Assigns the ID codes to each of the tile prefabs to make them easier to access **/
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, rock); 
        tileset.Add(1, grass); 
        tileset.Add(2, sand); 
        tileset.Add(3, water);
    }

    public void CreateTileGroups()
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    public void GenerateMap()
    {
        for(int x = 0; x < map_width; x++)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());

            for(int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x,y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }

    public int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification);
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);
        float scaled_perlin = clamp_perlin * tileset.Count;
        if(scaled_perlin == 4)
        {
            scaled_perlin = 3;
        }
        return Mathf.FloorToInt(scaled_perlin);
    }

    public void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);

        tile_grid[x].Add(tile);
    }
}
