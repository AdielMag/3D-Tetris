using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class GameModel
{
    public Transform[,,] grid;

    public float timeBetweenUpdates = 5;
    public int boardWidth, boardDepth, boardHeight;

    public Dictionary<string, ShapeModel> shapes;

    [HideInInspector] public GameObject cubePrefab;
    [HideInInspector] public GameObject fallIndicatorCubePrefab;

    public Color[] colors;
    [HideInInspector] public Material[] cubeFillMats;
    [HideInInspector] public List<Material> availableFillMats;
    [HideInInspector] public Material cubeSquaresMatt;

    [HideInInspector] public Material fallIndicatorMat;

    public Dictionary<string, ShapeModel> LoadShapes()
    {
        Dictionary<string, ShapeModel> parsedData = new Dictionary<string, ShapeModel>();

        // Get CSV raw data
        TextAsset csvData = Resources.Load<TextAsset>("ShapesData");

        // Split data to raws
        string[] rawData = csvData.text.Split(new char[] { '\n' });

        // Loop thorugh all data except the first row 
        //(which is used for ease of use and not for data) and the last  row (which is empty)
        for (int i = 1; i< rawData.Length - 1; i++) 
        {
            // Split the row to section and ignore comas inside double quotes
            string[] row = Regex.Split(rawData[i], ",(?=([^\"]*\"[^\"]*\")*[^\"]*$)");

            // Remove Double quotes
            row[1] = row[1].Substring(1, row[1].Length - 2);

            // Split block positions data
            string[] blocksPosData = row[1].Split(new char[] { '|' });

            // Create array to store all the data and pass it later
            Vector3[] blockPositionTarget = new Vector3[blocksPosData.Length];

            for (int o = 0; o < blocksPosData.Length; o++)
            {
                // Split axis values
                string[] blockData = blocksPosData[o].Split(new char[] { ',' });

                int.TryParse(blockData[0], out int x);
                int.TryParse(blockData[1], out int y);
                int.TryParse(blockData[2], out int z);

                blockPositionTarget[o] = new Vector3(x, y, z);
            }

            ShapeModel shapeData = new ShapeModel
            {
                blocksPositions = blockPositionTarget
            };

            parsedData.Add(row[0], shapeData);
        }

        return parsedData;
    }

    public void CreateCubeMats()
    {
        Material referenceMat =
            cubePrefab.GetComponent<MeshRenderer>().sharedMaterials[1];

        cubeFillMats = new Material[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            cubeFillMats[i] = new Material(referenceMat);
            cubeFillMats[i].color = colors[i];
            cubeFillMats[i].name += i.ToString();
        }
    }
}
