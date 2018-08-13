using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour {

    public GameObject MapGenerator;
    public buildQueue BuildQueue;
    public MapGeneratorScript MapGenScript;

    public GameObject itemDescription;

    public Text populationDisplay;
    public Text ammenitiesDisplay;
    public Text jobsDisplay;
    public Text costDisplay;

    public Text tileNameDescDisplay;
    public Text tileAbilitiesDescDisplay;

    public bool transportFixNeeded = false;
    public bool tileJustPlaced = false;

    public float totalAmmenitiesOnBoard;
    public float totalPopulationOnBoard;
    public float totalJobsOnBoard;

    public float populationBonuses;
    public float ammenityBonuses;
    public float jobBonuses;

    public int costUntilNextAge = 100;
    public int ageNumber = 1;
    public float requiredAmmenities;
    public float requiredAmmenitiesOnAgeAdvancement;

    public bool singleUpdateLock = false;

    private IEnumerator updateGameInfo()
    {        
        yield return new WaitForFixedUpdate();
        Debug.Log("Updating Game Info...");
        tileJustPlaced = false;

        float runningAmmenityTotal = 0f;
        float runningPopulationTotal = 0f;
        float runningJobsTotal = 0f;
        foreach (GameObject boardTile in MapGenScript.gridTiles)
        {
            tileAttributes tilesAttributes = boardTile.GetComponent<tileAttributes>();
            runningAmmenityTotal += tilesAttributes.ammenitiesProvided;
            runningPopulationTotal += tilesAttributes.population;
            runningJobsTotal += tilesAttributes.jobsProvided;
        }

        totalAmmenitiesOnBoard = (float)System.Math.Round(runningAmmenityTotal, 1) + (float)System.Math.Round(ammenityBonuses,1);
        totalPopulationOnBoard = (float)System.Math.Round(runningPopulationTotal, 1) + (float)System.Math.Round(populationBonuses,1); 
        totalJobsOnBoard = (float)System.Math.Round(runningJobsTotal, 0) + (float)System.Math.Round(jobBonuses,0);

        populationBonuses = 0f;
        ammenityBonuses = 0f;
        jobBonuses = 0f;

        populationDisplay.text = (totalPopulationOnBoard).ToString();
        ammenitiesDisplay.text = (totalAmmenitiesOnBoard).ToString();
        jobsDisplay.text = (totalJobsOnBoard).ToString();

        costUntilNextAge -= (int)(Resources.Load("Prefabs/" + BuildQueue.currentlyPlacingTile) as GameObject).GetComponent<tileAttributes>().placementCost;
        costDisplay.text = (costUntilNextAge).ToString();
        Debug.Log("Cost Till Next Age: " + (costUntilNextAge).ToString());

        if (costUntilNextAge <= 0)
        {
            costUntilNextAge += 100;
            ageNumber += 1;
            BuildQueue.populateBuildQueueOptionsFromFile();
        }

        requiredAmmenities = (float) (totalPopulationOnBoard * (ageNumber - 0.5f)) + ((Mathf.Abs(totalPopulationOnBoard - totalJobsOnBoard)) * ageNumber);
        requiredAmmenitiesOnAgeAdvancement = (float)(totalPopulationOnBoard * (ageNumber + 0.5f)) + ((Mathf.Abs(totalPopulationOnBoard - totalJobsOnBoard)) * (ageNumber + 1)); 
        ammenitiesDisplay.text = (totalAmmenitiesOnBoard).ToString() + " / " + (requiredAmmenities).ToString() + " / " + (requiredAmmenitiesOnAgeAdvancement).ToString();
        singleUpdateLock = false;
    }

    public void updateDescription()
    {
        Debug.Log("Updating Desciption Window");
        string filePath = "Assets/Resources/Text/" + BuildQueue.currentlyPlacingTile + ".txt";
        StreamReader reader = new StreamReader(filePath);

        tileNameDescDisplay.text = reader.ReadLine();
        tileAbilitiesDescDisplay.text = reader.ReadLine();

        reader.Close();
    }

    public void openDescription()
    {
        if (itemDescription.activeSelf == true)
        {
            itemDescription.SetActive(false);
        }
        else
        {
            updateDescription();
            itemDescription.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(updateGameInfo());
    }

    private void Update()
    {
        if (tileJustPlaced == true && singleUpdateLock == false)
        {
            singleUpdateLock = true;
            Debug.Log("TilePlaced!");
            StartCoroutine(updateGameInfo());
        }

        if (transportFixNeeded == true)
        {
            transportFixNeeded = false;
            foreach (var transportTile in MapGenScript.gridTiles)
            {
                int transportTileX = Mathf.RoundToInt(transportTile.GetComponent<tileAttributes>().Xaxis);
                int transportTileY = Mathf.RoundToInt(transportTile.GetComponent<tileAttributes>().Yaxis);

                bool checkTop = false;
                bool checkBottom = false;
                bool checkLeft = false;
                bool checkRight = false;

                if (transportTile.GetComponent<tileAttributes>().tileName == "path")
                {
                    if (transportTileY != MapGenScript.rows)
                    {
                        if (MapGenScript.findTileAtLocation(transportTileX, transportTileY + 1).GetComponent<tileAttributes>().tileName == "path" ||
                            MapGenScript.findTileAtLocation(transportTileX, transportTileY + 1).GetComponent<tileAttributes>().tileName == "bridge") { checkTop = true; }
                    }
                    if (transportTileY != 1)
                    {
                        if (MapGenScript.findTileAtLocation(transportTileX, transportTileY - 1).GetComponent<tileAttributes>().tileName == "path" ||
                            MapGenScript.findTileAtLocation(transportTileX, transportTileY - 1).GetComponent<tileAttributes>().tileName == "bridge") { checkBottom = true; }
                    }
                    if (transportTileX != 1)
                    {
                        if (MapGenScript.findTileAtLocation(transportTileX - 1, transportTileY).GetComponent<tileAttributes>().tileName == "path" ||
                            MapGenScript.findTileAtLocation(transportTileX - 1, transportTileY).GetComponent<tileAttributes>().tileName == "bridge") { checkLeft = true; }
                    }
                    if (transportTileX != MapGenScript.columns)
                    {
                        if (MapGenScript.findTileAtLocation(transportTileX + 1, transportTileY).GetComponent<tileAttributes>().tileName == "path" ||
                            MapGenScript.findTileAtLocation(transportTileX + 1, transportTileY).GetComponent<tileAttributes>().tileName == "bridge") { checkRight = true; }
                    }

                    // paths decision tree
                    if (checkTop == true && checkRight == true && checkBottom == true && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathX", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == true && checkBottom == true && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNES", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == true && checkBottom == false && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNE", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == false && checkBottom == false && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNS", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == false && checkBottom == false && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathWE", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == false && checkBottom == true && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathSW", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == true && checkBottom == true && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathESW", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == true && checkBottom == false && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathWE", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == false && checkBottom == true && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNS", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == false && checkBottom == true && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNS", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == true && checkBottom == false && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathWE", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == true && checkBottom == false && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNEW", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == false && checkBottom == true && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNSW", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == true && checkRight == false && checkBottom == false && checkLeft == true)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathNW", typeof(Sprite)) as Sprite;
                    }
                    if (checkTop == false && checkRight == true && checkBottom == true && checkLeft == false)
                    {
                        transportTile.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/PathES", typeof(Sprite)) as Sprite;
                    }
                }
            }
        }
    }
}
