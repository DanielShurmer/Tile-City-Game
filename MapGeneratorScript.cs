using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorScript : MonoBehaviour {

    public gameManager GameManger;

    public GameObject tile;
    public int columns;
    public int rows;
    public List<GameObject> gridTiles = new List<GameObject>();

    public int riverAmount;
    public int riverBendiness;
    public int mountainAmount;
    public int forestAmount;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();


	void Start () {
        GenerateGridPositions();
        GenerateBlankMap();
        RandomiseMap();
	}

    public void GenerateGridPositions()
    {
        gridPositions.Clear();

        for (int x = 1; x <= columns; x++)
        {
            for (int y = 1; y <= rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }

    }

    public void GenerateBlankMap()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = 1; x <= columns; x++)
        {
            for (int y = 1; y <= rows; y++)
            {
                GameObject toInstantiate = tile;
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.name = "TileX" + x + "Y" + y;
                instance.transform.SetParent(boardHolder);
                gridTiles.Add(instance);
            }
        }
    }

    public GameObject findTileAtLocation(int Xaxis, int Yaxis)
    {
        foreach (var tile in gridTiles)
        {
             if (tile.GetComponent<tileAttributes>().Xaxis == Xaxis && tile.GetComponent<tileAttributes>().Yaxis == Yaxis)
            {
                return tile;
            }
        }

        Debug.LogError("No tile found at searched location - X" + Xaxis + " Y" + Yaxis);
        return null;
    }

    public GameObject selectRandomEdge()
    {
        //Determine edge: 1 = Xaxis, 2 = Yaxis
        int XorY = Random.Range(1, 3);

        if (XorY == 1)
        {
            int randomColumn = Random.Range(2, columns);
            var tile = findTileAtLocation(randomColumn, 1);
            return tile;
        }
        else
        {
            int randomRow = Random.Range(2, rows);
            var tile = findTileAtLocation(1, randomRow);
            return tile;
        }
    }

    public GameObject selectRandomTile()
    {
        int randomX = Random.Range(1, columns + 1);
        int randomY = Random.Range(1, rows + 1);
        var tile = findTileAtLocation(randomX, randomY);
        return tile;
    }

    public GameObject selectRandomGroundTile()
    {
        int emergencyExit = 1;

        while (emergencyExit < 99)
        {
            GameObject possibleTile = selectRandomTile();
            string tileSurface = possibleTile.GetComponent<tileAttributes>().tileName;
            emergencyExit += 1;
            if (tileSurface == "ground")
            {
                return possibleTile;
            }   
        }
        return null;
    }

    public void SwapTiles(GameObject tileToReplace, int xToReplace, int yToReplace, string nameOfTileToInsert)
    {
        if (tileToReplace == null) { tileToReplace = findTileAtLocation(xToReplace, yToReplace); }
        else if (xToReplace == 0)
        {
            xToReplace = Mathf.RoundToInt(tileToReplace.GetComponent<tileAttributes>().Xaxis);
            yToReplace = Mathf.RoundToInt(tileToReplace.GetComponent<tileAttributes>().Yaxis);
        }

        Destroy(tileToReplace);
        gridTiles.Remove(tileToReplace);
        

        GameObject tileToInsert = Resources.Load("Prefabs/" + nameOfTileToInsert) as GameObject;

        GameObject instance = Instantiate(tileToInsert, new Vector3(xToReplace, yToReplace, 0f), Quaternion.identity) as GameObject;
        instance.name = "TileX" + xToReplace + "Y" + yToReplace;
        instance.transform.SetParent(boardHolder);
        gridTiles.Add(instance);

    }

    public bool checkIfValidBridgeLocation(GameObject tileToCheck, int xToCheck, int yToCheck)
    {
        try
        {
            if (tileToCheck == null) { tileToCheck = findTileAtLocation(xToCheck, yToCheck); }
            else if (xToCheck == 0)
            {
                xToCheck = Mathf.RoundToInt(tileToCheck.GetComponent<tileAttributes>().Xaxis);
                yToCheck = Mathf.RoundToInt(tileToCheck.GetComponent<tileAttributes>().Yaxis);
            }

            GameObject tileRight = findTileAtLocation((int)xToCheck + 1, (int)yToCheck);
            Debug.Log(tileRight.ToString());

            if (findTileAtLocation((int)xToCheck + 1, (int)yToCheck).GetComponent<tileAttributes>().tileName != "river" &&
                findTileAtLocation((int)xToCheck - 1, (int)yToCheck).GetComponent<tileAttributes>().tileName != "river") { Debug.Log("Returned A"); return true; }
            if (findTileAtLocation((int)xToCheck, (int)yToCheck + 1).GetComponent<tileAttributes>().tileName != "river" &&
                findTileAtLocation((int)xToCheck, (int)yToCheck - 1).GetComponent<tileAttributes>().tileName != "river") { Debug.Log("Returned B"); return true; }
            Debug.Log("Location Invalid!");
            return false;
        }
        catch
        {
            Debug.Log("Returned C");
            return false;
        }
    }

    public void fixRivers()
    {
        foreach (var riverTile in gridTiles)
        {
            int riverTileX = Mathf.RoundToInt(riverTile.GetComponent<tileAttributes>().Xaxis);
            int riverTileY = Mathf.RoundToInt(riverTile.GetComponent<tileAttributes>().Yaxis);

            bool checkTop = false;
            bool checkBottom = false;
            bool checkLeft = false;
            bool checkRight = false;

            if (riverTile.GetComponent<tileAttributes>().tileName == "river")
            {
                if (riverTileY != rows)
                {
                    if (findTileAtLocation(riverTileX, riverTileY + 1).GetComponent<tileAttributes>().tileName == "river") { checkTop = true; }
                }
                if (riverTileY != 1)
                {
                    if (findTileAtLocation(riverTileX, riverTileY - 1).GetComponent<tileAttributes>().tileName == "river") { checkBottom = true; }
                }
                if (riverTileX != 1)
                {
                    if (findTileAtLocation(riverTileX - 1, riverTileY).GetComponent<tileAttributes>().tileName == "river") { checkLeft = true; }
                }
                if (riverTileX != columns)
                {
                    if (findTileAtLocation(riverTileX + 1, riverTileY).GetComponent<tileAttributes>().tileName == "river") { checkRight = true; }
                }

                // Rivers decision tree
                if (checkTop == true && checkRight == true && checkBottom == true && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNES", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == true && checkBottom == false && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNE", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == false && checkBottom == false && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNS", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == false && checkBottom == false && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverWE", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == false && checkBottom == true && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverSW", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == true && checkBottom == true && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverESW", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == true && checkBottom == false && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverWE", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == false && checkBottom == true && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNS", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == false && checkBottom == true && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNS", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == true && checkBottom == false && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverWE", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == true && checkBottom == false && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNEW", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == false && checkBottom == true && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNSW", typeof(Sprite)) as Sprite;
                }
                if (checkTop == true && checkRight == false && checkBottom == false && checkLeft == true)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverNW", typeof(Sprite)) as Sprite;
                }
                if (checkTop == false && checkRight == true && checkBottom == true && checkLeft == false)
                {
                    riverTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/RiverES", typeof(Sprite)) as Sprite;
                }
            }
        }
    }

    public void RandomiseMap()
    {
        // Generate Rivers
        if (riverAmount > 0)
        {

            // Check River Bendiness
            if (riverBendiness <= 0) { riverBendiness = 1; }
            if (riverBendiness > 20) { riverBendiness = 20; }


            for (int river = 1; river <= riverAmount; river++)
            {
                string tileSurface = "Null";
                int emergencyExit = 1;

                while (tileSurface != "ground" && emergencyExit < 20)
                {
                    GameObject tileFound = selectRandomEdge();
                    tileSurface = tileFound.GetComponent<tileAttributes>().tileName;
                    emergencyExit += 1;

                    if (tileSurface == "ground")
                    {
                        // Set start position of new river
                        SwapTiles(tileFound, 0, 0, "RiverTile");

                        int currentTileX = Mathf.RoundToInt(tileFound.GetComponent<tileAttributes>().Xaxis);
                        int currentTileY = Mathf.RoundToInt(tileFound.GetComponent<tileAttributes>().Yaxis);

                        // Generate river
                        if (currentTileX == 1)
                        {
                            currentTileX += 1;
                            GameObject firstTile = findTileAtLocation(currentTileX, currentTileY);
                            SwapTiles(firstTile, 0, 0, "RiverTile");

                            while (currentTileX < columns)
                            {
                                int rollD6 = Random.Range(1, (20 - riverBendiness) + 6);
                                //Up
                                if (rollD6 == 1 || rollD6 == 2 || rollD6 == 3)
                                {
                                    bool continuation = true;

                                    while (continuation == true)
                                    {
                                        if (currentTileY < columns)
                                        {
                                            currentTileY += 1;
                                            GameObject editingTile2 = findTileAtLocation(currentTileX, currentTileY);
                                            SwapTiles(editingTile2, 0, 0, "RiverTile");
                                            int rollD3 = Random.Range(1, riverBendiness + 3);
                                            if (rollD3 == 1 || rollD3 == 2 || rollD3 == 3)
                                            {
                                                continuation = false;
                                            }
                                        }
                                        else
                                        {
                                            continuation = false;
                                        }
                                    }
                                    currentTileX += 1;
                                    GameObject editingTile3 = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile3, 0, 0, "RiverTile");
                                }
                                //Down
                                else if (rollD6 == 4 || rollD6 == 5 || rollD6 == 6)
                                {
                                    bool continuation = true;

                                    while (continuation == true)
                                    {
                                        if (currentTileY > 1)
                                        {
                                            currentTileY -= 1;
                                            GameObject editingTile2 = findTileAtLocation(currentTileX, currentTileY);
                                            SwapTiles(editingTile2, 0, 0, "RiverTile");
                                            int rollD3 = Random.Range(1, riverBendiness + 3);
                                            if (rollD3 == 1 || rollD3 == 2 || rollD3 == 3)
                                            {
                                                continuation = false;
                                            }
                                        }
                                        else
                                        {
                                            continuation = false;
                                        }
                                    }
                                    currentTileX += 1;
                                    GameObject editingTile3 = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile3, 0, 0, "RiverTile");
                                }

                                if (currentTileX < columns)
                                {
                                    //Right
                                    currentTileX += 1;
                                    GameObject editingTile = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile, 0, 0, "RiverTile");
                                }
                            }
                        }
                        else
                        {
                            currentTileY += 1;
                            GameObject firstTile = findTileAtLocation(currentTileX, currentTileY);
                            SwapTiles(firstTile, 0, 0, "RiverTile");

                            while (currentTileY < rows)
                            {
                                int rollD6 = Random.Range(1, (20 - riverBendiness) + 6);
                                //Left
                                if (rollD6 == 1 || rollD6 == 2 || rollD6 == 3)
                                {
                                    bool continuation = true;

                                    while (continuation == true)
                                    {
                                        if (currentTileX > 1)
                                        {
                                            currentTileX -= 1;
                                            GameObject editingTile2 = findTileAtLocation(currentTileX, currentTileY);
                                            SwapTiles(editingTile2, 0, 0, "RiverTile");
                                            int rollD3 = Random.Range(1, riverBendiness + 3);
                                            if (rollD3 == 1 || rollD3 == 2 || rollD3 == 3)
                                            {
                                                continuation = false;
                                            }
                                        }
                                        else
                                        {
                                            continuation = false;
                                        }
                                    }
                                    currentTileY += 1;
                                    GameObject editingTile3 = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile3, 0, 0, "RiverTile");
                                }

                                //Right
                                else if (rollD6 == 4 || rollD6 == 5 || rollD6 == 6)
                                {
                                    bool continuation = true;

                                    while (continuation == true)
                                    {
                                        if (currentTileX < rows)
                                        {
                                            currentTileX += 1;
                                            GameObject editingTile2 = findTileAtLocation(currentTileX, currentTileY);
                                            SwapTiles(editingTile2, 0, 0, "RiverTile");
                                            int rollD3 = Random.Range(1, riverBendiness + 3);
                                            if (rollD3 == 1 || rollD3 == 2 || rollD3 == 3)
                                            {
                                                continuation = false;
                                            }
                                        }
                                        else
                                        {
                                            continuation = false;
                                        }
                                    }
                                    currentTileY += 1;
                                    GameObject editingTile3 = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile3, 0, 0, "RiverTile");
                                }

                                if (currentTileY < rows)
                                {
                                    //Up
                                    currentTileY += 1;
                                    GameObject editingTile = findTileAtLocation(currentTileX, currentTileY);
                                    SwapTiles(editingTile, 0, 0, "RiverTile");
                                }
                            }
                        }
                    }
                }
            }

            // Fix river sprites
            fixRivers();
            
        }

        // Mountains
        if (mountainAmount > 0)
        {
            int mountainsRemaining = mountainAmount;
            
            while (mountainsRemaining > 0)
            {
                GameObject mountainTile = selectRandomGroundTile();
                if (mountainTile != null)
                {
                    SwapTiles(mountainTile, 0, 0, "MountainTile");
                    mountainsRemaining -= 1;
                }
                else
                {
                    break;
                }
            }
        }

        // Forests
        if (forestAmount > 0)
        {
            int forestsRemaining = forestAmount;

            while (forestsRemaining > 0)
            {
                GameObject forestTile = selectRandomGroundTile();
                if (forestTile != null)
                {

                    SwapTiles(forestTile, 0, 0, "ForestTile");
                    forestsRemaining -= 1;
                }
                else
                {
                    break;
                }
            }
        }

        // Town Centre & Starting Path
        int lowerXAxisSpawnbound = (int)((columns / 2) - 5);
        int upperXAxisSpawnbound = (int)((columns / 2) + 5);
        int lowerYAxisSpawnbound = (int)((rows / 2) - 5);
        int upperYAxisSpawnbound = (int)((rows / 2) + 5);
        int randomX = 0;
        int randomY = 0;

        bool centrePlaced = false;
        int placementAttempts = 0;

        while (centrePlaced == false && placementAttempts < 25)
        {
            randomX = Random.Range(lowerXAxisSpawnbound, upperXAxisSpawnbound);
            randomY = Random.Range(lowerYAxisSpawnbound, upperYAxisSpawnbound);

            GameObject possibleTile = findTileAtLocation(randomX, randomY);

            if (possibleTile.GetComponent<tileAttributes>().tileName == "ground")
            {
                centrePlaced = true;
  
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (findTileAtLocation(randomX + x, randomY + y).GetComponent<tileAttributes>().tileName == "river") { centrePlaced = false; }
                    }
                }

                if (centrePlaced == true)
                {
                    SwapTiles(possibleTile, 0, 0, "TownCentreTile");
                }
            }
            placementAttempts += 1;
        }

        if (centrePlaced == false)
        {
            GameObject possibleTile = selectRandomGroundTile();
            randomX = (int)possibleTile.GetComponent<tileAttributes>().Xaxis;
            randomY = (int)possibleTile.GetComponent<tileAttributes>().Yaxis;
            SwapTiles(possibleTile, 0, 0, "TownCentreTile");
        }

        for(int x = -3; x <= 3; x++ )
        {
            if (x != 0)
            {
                GameObject tileToChange = findTileAtLocation(randomX + x, randomY);
                if (tileToChange.GetComponent<tileAttributes>().tileName != ("river"))
                {
                    SwapTiles(null, randomX + x, randomY, "PathTile");
                }
                else
                {
                    if (checkIfValidBridgeLocation(null, randomX + x, randomY) == true)
                    {
                        SwapTiles(null, randomX + x, randomY, "BridgeTile");
                        findTileAtLocation(randomX + x, randomY).GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/BridgeHor", typeof(Sprite)) as Sprite;
                    }
                }
            }           
        }

        for (int y = -3; y <= 3; y++)
        {
            if (y != 0)
            {
                GameObject tileToChange = findTileAtLocation(randomX, randomY + y);
                if (tileToChange.GetComponent<tileAttributes>().tileName != ("river"))
                {
                    SwapTiles(null, randomX, randomY + y, "PathTile");
                }
                else
                {
                    if (checkIfValidBridgeLocation(null, randomX, randomY + y) == true)
                    {
                        SwapTiles(null, randomX, randomY + y, "BridgeTile");
                    }
                }
            }
        }

        GameObject cornerTileToChange = findTileAtLocation(randomX - 1, randomY - 1);
        SwapTiles(cornerTileToChange,0,0,"PathTile");
        cornerTileToChange = findTileAtLocation(randomX - 1, randomY + 1);
        SwapTiles(cornerTileToChange, 0, 0, "PathTile");
        cornerTileToChange = findTileAtLocation(randomX + 1, randomY + 1);
        SwapTiles(cornerTileToChange, 0, 0, "PathTile");
        cornerTileToChange = findTileAtLocation(randomX + 1, randomY - 1);
        SwapTiles(cornerTileToChange, 0, 0, "PathTile");

        GameManger.transportFixNeeded = true;

    }
	

}
