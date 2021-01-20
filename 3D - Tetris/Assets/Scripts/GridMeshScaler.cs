using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMeshScaler : MonoBehaviour
{
    private Material  _matWallsZ, _matWallsX, _matBottom;

    void Awake()
    {
        MeshRenderer meshRen = GetComponent<MeshRenderer>();

        _matWallsZ = meshRen.sharedMaterials[0];
        _matWallsX = meshRen.sharedMaterials[1];
        _matBottom = meshRen.sharedMaterials[2];
    }

    public void ScaleGrid(float width, float depth, float height)
    {
        transform.localScale = new Vector3(width, height, depth);

        _matWallsZ.mainTextureScale = new Vector2(width, height);
        _matWallsX.mainTextureScale = new Vector2(depth, height);
        _matBottom.mainTextureScale = new Vector2(width, depth);
    }
}
