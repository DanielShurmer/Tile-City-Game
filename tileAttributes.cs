using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class tileAttributes : MonoBehaviour {

    public GameObject MapGenerator;
    public MapGeneratorScript MapScript;
    public buildQueue QueueScript;
    public gameManager GameManager;
    public GameObject boardArea;

    public string currentlyPlacingTile;

    public float Xaxis;
    public float Yaxis;

    public string tileName;
    public float population;
    public float ammenitiesProvided;
    public float powerUse;
    public float jobsProvided;
    public float placementCost;
    public string effectsDescription;
    public string tileType;
    public List<string> canBePlacedOn;


    private void Awake()
    {
        Xaxis = this.transform.position.x;
        Yaxis = this.transform.position.y;

        MapGenerator = GameObject.Find("MapGenerator");
        MapScript = (MapGeneratorScript)MapGenerator.GetComponent(typeof(MapGeneratorScript));
        QueueScript = (buildQueue)MapGenerator.GetComponent(typeof(buildQueue));
        GameManager = (gameManager)MapGenerator.GetComponent(typeof(gameManager));
    }

    public int numberOfAdjacentRoads(int targetXaxis, int targetYaxis)
    {
        int pathTilesAdj = 0;
        if (MapScript.findTileAtLocation((int)targetXaxis + 1, (int)targetYaxis).GetComponent<tileAttributes>().tileType == "transport") { pathTilesAdj += 1; }
        if (MapScript.findTileAtLocation((int)targetXaxis - 1, (int)targetYaxis).GetComponent<tileAttributes>().tileType == "transport") { pathTilesAdj += 1; }
        if (MapScript.findTileAtLocation((int)targetXaxis, (int)targetYaxis + 1).GetComponent<tileAttributes>().tileType == "transport") { pathTilesAdj += 1; }
        if (MapScript.findTileAtLocation((int)targetXaxis, (int)targetYaxis - 1).GetComponent<tileAttributes>().tileType == "transport") { pathTilesAdj += 1; }
        return pathTilesAdj;
    }

    public bool checkIfTileCanBeDeleted()
    {
        bool canDelete = true;
        if (tileType == "natural" || tileName == "townCentre") { canDelete = false; }
        if (tileType == "transport")
        {
            if (numberOfAdjacentRoads((int)Xaxis,(int)Yaxis) != 1) { canDelete = false; }

            if (MapScript.findTileAtLocation((int)Xaxis + 1, (int)Yaxis).GetComponent<tileAttributes>().tileType != "transport" && 
                MapScript.findTileAtLocation((int)Xaxis + 1, (int)Yaxis).GetComponent<tileAttributes>().tileType != "natural")
            { if (numberOfAdjacentRoads((int)Xaxis + 1, (int)Yaxis) == 1) { canDelete = false;  } }

            if (MapScript.findTileAtLocation((int)Xaxis - 1, (int)Yaxis).GetComponent<tileAttributes>().tileType != "transport" &&
                MapScript.findTileAtLocation((int)Xaxis - 1, (int)Yaxis).GetComponent<tileAttributes>().tileType != "natural")
            { if (numberOfAdjacentRoads((int)Xaxis - 1, (int)Yaxis) == 1) { canDelete = false;  } }

            if (MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis + 1).GetComponent<tileAttributes>().tileType != "transport" &&
                MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis + 1).GetComponent<tileAttributes>().tileType != "natural")
            { if (numberOfAdjacentRoads((int)Xaxis, (int)Yaxis + 1) == 1) { canDelete = false;  } }

            if (MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis - 1).GetComponent<tileAttributes>().tileType != "transport" &&
                MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis - 1).GetComponent<tileAttributes>().tileType != "natural")
            { if (numberOfAdjacentRoads((int)Xaxis, (int)Yaxis - 1) == 1) { canDelete = false;  } }

        }

        return canDelete;
    }

    private void OnMouseDown()
    {
        bool validPlacementTile = false;
        currentlyPlacingTile = QueueScript.currentlyPlacingTile;
        GameObject currentlyPlacingTileObject = Resources.Load("Prefabs/" + currentlyPlacingTile) as GameObject;

        foreach (string tileCheck in currentlyPlacingTileObject.GetComponent<tileAttributes>().canBePlacedOn)
        {
            if (tileCheck == tileName) { validPlacementTile = true; }
            if (tileCheck == "delete")
            {
                validPlacementTile = checkIfTileCanBeDeleted();
            }
        }

        if (validPlacementTile == true && EventSystem.current.IsPointerOverGameObject() == false)
        {
            validPlacementTile = false;

            GameObject checkingTile = MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis + 1);
            if (checkingTile != null)
            {
                if (checkingTile.GetComponent<tileAttributes>().tileType == "transport") { validPlacementTile = true; }
            }

            checkingTile = MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis - 1);
            if (checkingTile != null)
            {
                if (checkingTile.GetComponent<tileAttributes>().tileType == "transport") { validPlacementTile = true; }
            }

            checkingTile = MapScript.findTileAtLocation((int)Xaxis + 1, (int)Yaxis);
            if (checkingTile != null)
            {
                if (checkingTile.GetComponent<tileAttributes>().tileType == "transport") { validPlacementTile = true; }
            }

            checkingTile = MapScript.findTileAtLocation((int)Xaxis - 1, (int)Yaxis);
            if (checkingTile != null)
            {
                if (checkingTile.GetComponent<tileAttributes>().tileType == "transport") { validPlacementTile = true; }
            }

            if (validPlacementTile == true)
            {
                if (currentlyPlacingTile == "BridgeTile")
                {
                    if (MapScript.checkIfValidBridgeLocation(null,(int)Xaxis,(int)Yaxis) == true)
                    {
                        MapScript.SwapTiles(null, (int)Xaxis, (int)Yaxis, currentlyPlacingTile);
                        if (MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis + 1).GetComponent<tileAttributes>().tileName == "river")
                        { MapScript.findTileAtLocation((int)Xaxis, (int)Yaxis).GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/BridgeHor", typeof(Sprite)) as Sprite; }
                        QueueScript.tileJustPlaced = true;
                    }
                }

                if (tileName == "bridge")
                {
                    MapScript.SwapTiles(null, (int)Xaxis, (int)Yaxis, "RiverTile");
                    QueueScript.tileJustPlaced = true;
                    GameManager.transportFixNeeded = true;
                    MapScript.fixRivers();
                }

                else
                {
                    MapScript.SwapTiles(null, (int)Xaxis, (int)Yaxis, currentlyPlacingTile);
                    QueueScript.tileJustPlaced = true;
                }

                if (currentlyPlacingTileObject.GetComponent<tileAttributes>().tileType == "transport" ||
                    currentlyPlacingTileObject.GetComponent<tileAttributes>().tileName == "ground")
                {
                    GameManager.transportFixNeeded = true;
                }

            }
        }
    }
}
