using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameModel
{
    public Transform[,,] grid;

    public float timeBetweenUpdates = 5;
    public int boardWidth, boardDepth, boardHeight;

    [SerializeField] private ShapeModel[] shapes;
    [SerializeField] private Color[] colors;
    private Material[] cubeFillMats;


    [HideInInspector] public GameObject cubePrefab;
    [HideInInspector] public GameObject fallIndicatorCubePrefab;

    [HideInInspector] public List<ShapeModel> availableShapes;
    [HideInInspector] public List<Material> availableFillMats;

    [HideInInspector] public Material cubeSquaresMatt;
    [HideInInspector] public Material fallIndicatorMat;

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
    public void SetAvailableShapes()
    {
        foreach (ShapeModel shape in shapes)
            for (int x = 0; x < shape.appearanceOdds; x++)
                availableShapes.Add(shape);
        
    }
    public void SetAvailableFillmats()
    {
        availableFillMats = cubeFillMats.ToList<Material>();
    }
}
