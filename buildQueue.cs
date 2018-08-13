using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class buildQueue : MonoBehaviour {

    public string currentlyPlacingTile = "Tile";

    public bool tileJustPlaced = false;

    public gameManager GameManager;

    public GameObject optionAIndicator;
    public GameObject optionBIndicator;
    public GameObject optionCIndicator;
    public GameObject optionDeleteIndicator;

    public GameObject optionAHighlight;
    public GameObject optionBHighlight;
    public GameObject optionCHighlight;
    public GameObject optionDeleteHighlight;

    public Text populationInfoText;
    public Text ammenityInfoText;
    public Text jobsInfoText;
    public Text costInfoText;
    public Text effectsDescriptionText;

    public bool optionAClicked = false;
    public bool optionBClicked = false;
    public bool optionCClicked = false;
    public bool optionDeleteClicked = false;

    public List<string> options = new List<string>();
    public List<string> buildQueueList = new List<string>();


    public void updateInfoPanel()
    {
        GameObject placingTile = Resources.Load("Prefabs/" + currentlyPlacingTile) as GameObject;
        populationInfoText.text = placingTile.GetComponent<tileAttributes>().population.ToString();
        ammenityInfoText.text = placingTile.GetComponent<tileAttributes>().ammenitiesProvided.ToString();
        jobsInfoText.text = placingTile.GetComponent<tileAttributes>().jobsProvided.ToString();
        costInfoText.text = placingTile.GetComponent<tileAttributes>().placementCost.ToString();
        effectsDescriptionText.text = placingTile.GetComponent<tileAttributes>().effectsDescription;
    }

    public void updateBuildQueueVisuals()
    {
        for (int y = 0; y <= 14; y++)
        {
            string buildQueueEntry = buildQueueList[y];
            GameObject tileToGetSpriteFrom = Resources.Load("Prefabs/" + buildQueueEntry) as GameObject;

            if (y == 0)
            {
                optionAIndicator.GetComponent<SpriteRenderer>().sprite = tileToGetSpriteFrom.GetComponent<SpriteRenderer>().sprite;
            }
            else if (y == 1)
            {
                optionBIndicator.GetComponent<SpriteRenderer>().sprite = tileToGetSpriteFrom.GetComponent<SpriteRenderer>().sprite;
            }
            else if (y == 2)
            {
                optionCIndicator.GetComponent<SpriteRenderer>().sprite = tileToGetSpriteFrom.GetComponent<SpriteRenderer>().sprite;
            }

            else
            {
                int buildQueueRow = Mathf.RoundToInt(y / 3);
                int buildQueueColumn = (y % 3) + 1;
                string nameOfPosition = "Q" + buildQueueRow + buildQueueColumn;

                GameObject.Find(nameOfPosition).GetComponent<SpriteRenderer>().sprite = tileToGetSpriteFrom.GetComponent<SpriteRenderer>().sprite;
            }
        }
        updateInfoPanel();
    }

    public void populateBuildQueueOptionsFromFile()
    {
        options.Clear();
        string filePath = "Assets/Resources/SaveFiles/prototype.txt";
        StreamReader reader = new StreamReader(filePath);
        string line;

        int numberOfDashesFinding = GameManager.ageNumber;
        int numberOfDashesFound = 0;

        while (numberOfDashesFound < numberOfDashesFinding)
        {
            line = reader.ReadLine();
            if (line == "-") { numberOfDashesFound += 1; }
        }

        // Populate Possible Build Options From File
        while ((line = reader.ReadLine()) != null)
        {
            if (line == "-") { break; }

            string tileToAddToList = line;
            int numberOfTimesToAdd = int.Parse(reader.ReadLine());
            for (int x = 1; x <= numberOfTimesToAdd; x++)
            {
                options.Add(tileToAddToList);
            }
        }
        reader.Close();
    }

    void Start () {
        populateBuildQueueOptionsFromFile();

        // Pre-populate Build Queue
        for (int y = 0; y <= 14; y++)
        {
            int randomListPosition = Random.Range(0, options.Count);
            string tileToAddToBuildList = options[randomListPosition];

            buildQueueList.Add(tileToAddToBuildList);
        }

        updateBuildQueueVisuals();

        optionAClicked = true;
    }
	
	void Update () {
        // Change tile to place based on selected build option.
        if (optionAClicked == true || optionBClicked == true || optionCClicked == true || optionDeleteClicked == true)
        {
            if (GameManager.tileNameDescDisplay.IsActive())
            {
                GameManager.openDescription();
            }

            if (optionAClicked == true)
            {
                optionAClicked = false;
                optionAHighlight.GetComponent<Renderer>().enabled = true;
                optionBHighlight.GetComponent<Renderer>().enabled = false;
                optionCHighlight.GetComponent<Renderer>().enabled = false;
                optionDeleteHighlight.GetComponent<Renderer>().enabled = false;
                currentlyPlacingTile = buildQueueList[0];
            }
            if (optionBClicked == true)
            {
                optionBClicked = false;
                optionAHighlight.GetComponent<Renderer>().enabled = false;
                optionBHighlight.GetComponent<Renderer>().enabled = true;
                optionCHighlight.GetComponent<Renderer>().enabled = false;
                optionDeleteHighlight.GetComponent<Renderer>().enabled = false;
                currentlyPlacingTile = buildQueueList[1];
            }
            if (optionCClicked == true)
            {
                optionCClicked = false;
                optionAHighlight.GetComponent<Renderer>().enabled = false;
                optionBHighlight.GetComponent<Renderer>().enabled = false;
                optionCHighlight.GetComponent<Renderer>().enabled = true;
                optionDeleteHighlight.GetComponent<Renderer>().enabled = false;
                currentlyPlacingTile = buildQueueList[2];
            }
            if (optionDeleteClicked == true)
            {
                optionDeleteClicked = false;
                optionAHighlight.GetComponent<Renderer>().enabled = false;
                optionBHighlight.GetComponent<Renderer>().enabled = false;
                optionCHighlight.GetComponent<Renderer>().enabled = false;
                optionDeleteHighlight.GetComponent<Renderer>().enabled = true;
                currentlyPlacingTile = "Tile";
            }
            updateInfoPanel();
        }


        // If a tile has been placed, update build queue.
		if (tileJustPlaced == true)
        {
            GameManager.tileJustPlaced = true;
            tileJustPlaced = false;

            for (int x = 1; x <= 3; x++)
            {
                buildQueueList.RemoveAt(0);

                if (buildQueueList.Count < 15)
                {
                    int randomListPosition = Random.Range(0, options.Count);
                    string tileToAddToBuildList = options[randomListPosition];
                    buildQueueList.Add(tileToAddToBuildList);
                }
            }

            updateBuildQueueVisuals();
            optionAClicked = true;

        }
	}
}
