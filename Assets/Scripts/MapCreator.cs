using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapCreator : MonoBehaviour
{
    public TileType[] tileTypes;
    int[,] tiles;
    static Node[,] graph;
    List<Node> currentPath = null;
    static public int mapWidth = 25;
    static public int mapHeight = 12; //minimum 9 rows for zoom to work correctly!!!
    public GameObject hexTilePrefab;
    static public float xOffSet = 1f;
    static public float yOffSet = 0.88f;
    void Start()
    {
        initializeTileTypes();
        GeneratePathFindingGraph();
        CreateHexTileMap();
    }

    void initializeTileTypes()
    {
        tiles = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tiles[x, y] = 0;
            }
        }
        //river
        tiles[4, 4] = 2;
        tiles[5, 4] = 2;
        tiles[6, 4] = 2;
        tiles[7, 4] = 2;
        tiles[8, 4] = 2;
        tiles[9, 4] = 2;
        tiles[9, 5] = 2;
        tiles[10, 5] = 2;
        tiles[10, 6] = 2;

        //mountains
        for (int x = 3; x < 5; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tiles[x, y] = 1;
            }
        }
    }

    void CreateHexTileMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject hex = Instantiate(tileTypes[tiles[x, y]].tileVisualPreFab);
                if (y % 2 == 0)
                {
                    hex.transform.position = new Vector3(x * xOffSet, y * yOffSet, 0);
                }
                else
                {
                    hex.transform.position = new Vector3(x * xOffSet + xOffSet / 2, y * yOffSet, 0);
                }
                SetTileInfo(hex, x, y);
            }
        }
    }
    void SetTileInfo(GameObject hex, int column, int row)
    {
        hex.transform.parent = transform;
        hex.name = "Hex_" + column.ToString() + "_" + row.ToString();
        hex.GetComponent<Hex>().tileX = column;
        hex.GetComponent<Hex>().tileY = row;
    }

    float CostToEnterTile(int x, int y)
    {
        TileType tileType = tileTypes[tiles[x, y]];

        return tileType.costToEnter;
    }
    void GeneratePathFindingGraph()
    {
        //initialize the array
        graph = new Node[mapWidth, mapHeight];
        //initialize each node in the array
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }
        //populate neigbours
        populateNeigbours();
    }

    private void populateNeigbours()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (y % 2 != 0)
                {
                    if (x < mapWidth - 1 && y > 0)
                    {
                        graph[x, y].neighoubrs.Add(graph[x + 1, y - 1]);
                    }
                    if (x < mapWidth - 1 && y < mapHeight - 1)
                    {
                        graph[x, y].neighoubrs.Add(graph[x + 1, y + 1]);
                    }
                }
                else
                {
                    if (x > 0 && y > 0)
                    {
                        graph[x, y].neighoubrs.Add(graph[x - 1, y - 1]);
                    }
                    if (x > 0 && y < mapHeight - 1)
                    {
                        graph[x, y].neighoubrs.Add(graph[x - 1, y + 1]);
                    }
                }
                if (y > 0)
                {
                    graph[x, y].neighoubrs.Add(graph[x, y - 1]);
                }
                if (x > 0)
                {
                    graph[x, y].neighoubrs.Add(graph[x - 1, y]);
                }
                if (x < mapWidth - 1)
                {
                    graph[x, y].neighoubrs.Add(graph[x + 1, y]);
                }
                if (y < mapHeight - 1)
                {
                    graph[x, y].neighoubrs.Add(graph[x, y + 1]);
                }
            }
        }
    }

    public void GeneratePathTo(int x, int y, GameObject selectedUnit)
    {
        //clear old path
        selectedUnit.GetComponent<MovingUnit>().currentPath = null;

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        //create list of nodes which we have'nt checked yet
        List<Node> unvisited = new List<Node>();
        Node source = graph[selectedUnit.GetComponent<MovingUnit>().tileX, selectedUnit.GetComponent<MovingUnit>().tileY];
        Node target = graph[x, y];
        dist[source] = 0;
        prev[source] = null;

        //initialize to infinity distance 
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }


        while (unvisited.Count > 0)
        {
            //u will be the unvisited node with the smallest possible distance
            Node u = null;
            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }    
            }

            //exit loop if found target
            if (u == target)
            {
                break;
            }
            
            unvisited.Remove(u);

            foreach (Node v in u.neighoubrs)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }    
            }    
        }

        //found shortest route or no route available
        if (prev[target] == null) 
        {
            return; //no route to target
        }
        List<Node> currentPath = new List<Node>();
        Node current = target;
        //step through prev chain and add to path
        while(prev[current] != null)
        {
            currentPath.Add(current);
            current = prev[current];
        }
        currentPath.Add(source);
        //inverting the path to the correct order
        currentPath.Reverse();
        selectedUnit.GetComponent<MovingUnit>().currentPath = currentPath;
    }
    public Vector3 hexToWorldCoord(int x, int y)
    {
        string hexName = "Hex_" + x + "_" + y;
        GameObject hex = GameObject.Find(hexName);
        return new Vector3(hex.transform.position.x, hex.transform.position.y, -1);
    }
}
