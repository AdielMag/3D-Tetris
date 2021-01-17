using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScaler : MonoBehaviour
{
    private Material  _matWallsZ, _matWallsX, _matBottom;

    // Start is called before the first frame update
    void Awake()
    {
        MeshRenderer meshRen = GetComponent<MeshRenderer>();

        _matWallsZ = meshRen.sharedMaterials[0];
        _matWallsX = meshRen.sharedMaterials[1];
        _matBottom = meshRen.sharedMaterials[2];

        ScaleGrid(
            TetrisApplication.instance.model.game.boardWidth,
            TetrisApplication.instance.model.game.boardDepth,
            TetrisApplication.instance.model.game.boardHeight);
    }

    public void ScaleGrid(float width, float depth, float height)
    {
        transform.localScale = new Vector3(width, height, depth);

        _matWallsZ.mainTextureScale = new Vector2(width, height);
        _matWallsX.mainTextureScale = new Vector2(depth, height);
        _matBottom.mainTextureScale = new Vector2(width, height);

        SetGridYPosition(height);
    }

    private void SetGridYPosition(float height)
    {
        float targetHeight = -height / 2;
        transform.localPosition = new Vector3(0, targetHeight, 0);
    }
}
