using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    public GameObject MapGenerator;
    public buildQueue BuildQueue;
    public gameManager GameManager;

    void Start()
    {
        BuildQueue = MapGenerator.GetComponent<buildQueue>();
    }

    void Update()
    {
        if (Input.GetKey("up"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            float columns = MapGenerator.GetComponent<MapGeneratorScript>().columns + 1;
            if (transform.position.y > columns) { transform.position = new Vector3(transform.position.x, columns, transform.position.z); }

        }

        if (Input.GetKey("down"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            if (transform.position.y < 0) { transform.position = new Vector3(transform.position.x, 0f, transform.position.z); }
        }

        if (Input.GetKey("left"))
        {
            transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
            if (transform.position.x < 0) { transform.position = new Vector3(0f, transform.position.y, transform.position.z); }
        }

        if (Input.GetKey("right"))
        {
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
            float rows = MapGenerator.GetComponent<MapGeneratorScript>().rows + 1;
            if (transform.position.x > rows) { transform.position = new Vector3(rows, transform.position.y, transform.position.z); }
        }

        if (Input.GetKey("[-]"))
        {
            this.GetComponent<Camera>().orthographicSize += 0.05f;
            if (this.GetComponent<Camera>().orthographicSize > 12f) { this.GetComponent<Camera>().orthographicSize = 12f; }
        }

        if (Input.GetKey("[+]"))
        {
            this.GetComponent<Camera>().orthographicSize -= 0.05f;
            if (this.GetComponent<Camera>().orthographicSize < 4f) { this.GetComponent<Camera>().orthographicSize = 4f; }
        }

        if (Input.GetKeyDown("1"))
        {
            BuildQueue.optionAClicked = true;
        }

        if (Input.GetKeyDown("2"))
        {
            BuildQueue.optionBClicked = true;
        }

        if (Input.GetKeyDown("3"))
        {
            BuildQueue.optionCClicked = true;
        }

        if (Input.GetKeyDown("4"))
        {
            BuildQueue.optionDeleteClicked = true;
        }

        if (Input.GetKeyDown("d"))
        {
            GameManager.openDescription();
        }


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            for (int x = 0; x < hits.Length; x++)
            {
                RaycastHit2D hit = hits[x];
                if (hit.collider != null)
                {
                    Debug.Log("Target Position: " + hit.collider.gameObject.transform.position + " Target Hit: " + hit.collider.gameObject);
                }
                if (hit.collider.gameObject == GameObject.Find("Option A"))
                {
                    BuildQueue.optionAClicked = true;
                }
                if (hit.collider.gameObject == GameObject.Find("Option B"))
                {
                    BuildQueue.optionBClicked = true;
                }
                if (hit.collider.gameObject == GameObject.Find("Option C"))
                {
                    BuildQueue.optionCClicked = true;
                }
                if (hit.collider.gameObject == GameObject.Find("Delete"))
                {
                    BuildQueue.optionDeleteClicked = true;
                }
            }
        }


    }
}
