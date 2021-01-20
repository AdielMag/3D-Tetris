using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameModel
{
    public Transform[,,] grid;

    [Header("Board dimensions")]
    public int boardWidth = 7;
    public int boardDepth = 7;
    public int boardHeight = 10;

    [Header("Game speed")]
    // Game speed
    public float timeBetweenUpdates = 1;
    public float timeBetweenUpdatesFast = .1f;
    public float rowClearSpeedMultiplier = 1.5f;

    // Prefabs
    [HideInInspector] public GameObject cubePrefab;
    [HideInInspector] public GameObject fallIndicatorCubePrefab;

    // Current game available lists
    [HideInInspector] public List<ShapeModel> availableShapes;
    [HideInInspector] public List<Material> availableFillMats;

    // Other materials
    [HideInInspector] public Material cubeSquaresMatt;
    [HideInInspector] public Material fallIndicatorMat;

    // Private data changed in the inspector
    [SerializeField] private ShapeModel[] shapes;
    [SerializeField] private Color[] colors;

    // Cubes fill base material array - used to reset 'availableFillMats' list
    private Material[] cubeFillMats;

    public void Init()
    {
        SetAvailableShapes();

        // Load prefabs
        cubePrefab =
            Resources.Load<GameObject>("Prefabs/CubePrefab");
        fallIndicatorCubePrefab =
            Resources.Load<GameObject>("Prefabs/FallIndicatorCubePrefab");

        // Create grid
        grid = new Transform[boardWidth, boardHeight, boardDepth];

        // Set materials
        CreateCubeFillMats();
        SetAvailableFillmats();

        cubeSquaresMatt = cubePrefab
            .GetComponent<MeshRenderer>().sharedMaterials[0];
        fallIndicatorMat = fallIndicatorCubePrefab
            .GetComponent<MeshRenderer>().sharedMaterials[1];
    }

    public void SetAvailableShapes()
    {
        // Make sure the list is empty
        availableShapes.Clear();

        // Create list based on shapes data
        foreach (ShapeModel shape in shapes)
            for (int x = 0; x < shape.appearanceOdds; x++)
                availableShapes.Add(shape);

    }

    public void CreateCubeFillMats()
    {
        // Grab cube fill material
        Material referenceMat =
            cubePrefab.GetComponent<MeshRenderer>().sharedMaterials[1];

        // Set array length by the colors length 
        cubeFillMats = new Material[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            cubeFillMats[i] = new Material(referenceMat);
            cubeFillMats[i].color = colors[i];

            // Add number to the material - to differentiate between them
            cubeFillMats[i].name += i.ToString();
        }
    }
    public void SetAvailableFillmats()
    {
        availableFillMats = cubeFillMats.ToList<Material>();
    }
}

[System.Serializable]
public struct Score
{
    public string name;
    public int score;
}
