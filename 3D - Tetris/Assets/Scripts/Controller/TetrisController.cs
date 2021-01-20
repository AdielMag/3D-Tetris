using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisController : TetrisElement
{
    [HideInInspector] public CameraController cam;
    [HideInInspector] public UIController ui;

    [SerializeField] private GridMeshScaler gridScaler;
    private FallLocationIndicatorController _fallLocIndic;

    // MonoBehaviour functions
    private float _previousUpdateTime;
    private void Update()
    {
        float timeToWait = app.view.input.fallFaster ?
            app.model.game.timeBetweenUpdatesFast
            : app.model.game.timeBetweenUpdates;

        if (Time.time - _previousUpdateTime > timeToWait)
        {
            _previousUpdateTime = Time.time;
            UpdateGame();
        }
    }

    // Public functions
    public void Init()
    {
        // Add offset to the board if axis dimension is even
        float boardXOffset =
            app.model.game.boardWidth % 2 != 0 ? 0 : -.5f;
        float boardZOffset =
            app.model.game.boardDepth % 2 != 0 ? 0 : -.5f;
        float boardYOffset =
            app.model.game.boardHeight % 2 != 0 ? 0 : -.5f;


        app.transform.position +=
            Vector3.forward * ((app.model.game.boardWidth / 2) + boardXOffset)
            + Vector3.right * ((app.model.game.boardDepth / 2) + boardZOffset)
            + Vector3.up * ((app.model.game.boardHeight / 2) + boardYOffset);

        cam = GetComponentInChildren<CameraController>();
        cam.Init();

        _fallLocIndic =
            GetComponentInChildren<FallLocationIndicatorController>();
        _fallLocIndic.Init();

        ui = GetComponentInChildren<UIController>();

        enabled = false;

        gridScaler.ScaleGrid(
            app.model.game.boardWidth,
            app.model.game.boardDepth,
            app.model.game.boardHeight);
    }

    public void StartGame()
    {
        // Remove all shapes inside the grid
        for (int i = app.model.shapesParent.childCount - 1; i >= 0; i--)
            Destroy(app.model.shapesParent.GetChild(i).gameObject);


        // Remove shape on preview if has one
        if (app.model.shapePreviewParent.childCount > 0)
            Destroy(app.model.shapePreviewParent.GetChild(0).gameObject);

        // Reset score
        UpdateScore(0);

        SetCurrrentShape(NewShape());

        CreateNextShapePreview();

        enabled = true;
    }
    public void OnMovementInput(Vector2 input)
    {
        if (!enabled)
            return;

        MoveCurrentShape(input);
    }
    public void OnRotationInput(UserInputView.RotationAxis axis)
    {
        if (!enabled)
            return;

        Vector3 targetAxis;
        switch (axis)
        {
            default:
                targetAxis = cam.SnappedRight;
                break;
            case UserInputView.RotationAxis.Up:
                targetAxis = -Vector3.up;
                break;
            case UserInputView.RotationAxis.Forward:
                targetAxis = -cam.SnappedForward;
                break;
        }

        RotateCurrentShape(targetAxis);
    }

    public void RegisterHighScore(string _name, int _score)
    {
        int scoresCount =
            app.model.highScores.Count > 0 ? app.model.highScores.Count : 1;

        // Check if the highScores list is empty
        if (app.model.highScores.Count == 0)
            app.model.highScores.Add(new Score { name = _name, score = _score });
        else
            for (int i = 0; i < scoresCount; i++)
                if (_score > app.model.highScores[i].score)
                    app.model.highScores.Insert(i,
                        new Score { name = _name, score = _score });

        // Update high scores window
        for (int i = 0; i < app.model.highScores.Count; i++)
        {
            string target;

            target = (i + 1).ToString() + ". " + app.model.highScores[i].name
                + ": " + app.model.highScores[i].score;

            Text highScoreTxt =
                app.model.ui.highScoresParent.GetChild(i).GetComponent<Text>();

            highScoreTxt.text = target;
        }
    }

    // Private function
    private void SetCurrrentShape(Transform shape)
    {
        // Set shape parent
        shape.transform.SetParent(app.model.shapesParent, true);

        // Reset Rotation
        shape.localRotation = Quaternion.Euler(Vector3.zero);

        #region Set shape pos
        Vector3 initialStartingPos;

        // Set initial starting pos
        initialStartingPos = new Vector3(
            app.model.game.boardWidth / 2,
            app.model.game.boardHeight,
            app.model.game.boardDepth / 2);

        shape.transform.position = initialStartingPos;
        #endregion

        app.model.currentShape = shape;

        // Set new indicator
        _fallLocIndic.SetNewIndicator();
        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }
    private void UpdateGame() 
    {
        // Move shape down
        app.model.currentShape.transform.position -= Vector3.up;

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();

        if (!ValidBlocksPosition()) // Check if its not valid
        {
            // Move shape up
            app.model.currentShape.transform.position += Vector3.up;

            ShapeColided();
        }
    }

    private void ShapeColided()
    {
        bool reachedTop = AddCurrentShapeBlocksToGrid();

        if (reachedTop)
            GameLost();
        else
        {
            CheckForRows();

            SetCurrrentShape(app.model.shapePreviewParent.GetChild(0));

            CreateNextShapePreview();
        }
    }
    
    private void CheckForRows()
    {
        int maxHeightToCheck = 0;
        int minHeightToCheck = app.model.game.boardHeight;

        for (int i = 0;i< app.model.currentShape.childCount; i++)
        {
            float yPos = app.model.currentShape.GetChild(i).position.y;

            if (yPos > maxHeightToCheck)
                maxHeightToCheck = Mathf.RoundToInt(yPos);

            if(yPos< minHeightToCheck)
                minHeightToCheck = Mathf.RoundToInt(yPos);
        }

        // Loop through max grid height to min (the shape cube max & min heights)
        for (int height = maxHeightToCheck; height >= minHeightToCheck; height--)
            if (RowIsFull(height))
            {
                // Remove row
                ClearRow(height);
                // Move top rows down
                MoveRowsDown(height);
                // Increase game speed
                IncreaseGameSpeed();
            }
    }
    private void ClearRow(int rowHeight) 
    {
        int boardWidth = app.model.game.boardWidth;
        int boardDepth = app.model.game.boardDepth;

        for (int width = 0; width < boardWidth; width++)
            for (int depth = 0; depth < boardDepth; depth++)
            {
                // Disable object - will remove all shpes when lost
                // (dont delete now it will affect performance)
                app.model.game.grid[width, rowHeight, depth].gameObject.SetActive(false);
                app.model.game.grid[width, rowHeight, depth] = null;
            }

        UpdateScore(app.model.currentScore + (boardWidth * boardDepth));
    }
    private void MoveRowsDown(int minRow)
    {
        for (int height = minRow ; height < app.model.game.boardHeight; height++)
            for (int width = 0; width < app.model.game.boardWidth; width++)
                for (int depth = 0; depth < app.model.game.boardDepth; depth++)
                {
                    // Grab cube transform
                    Transform cube = app.model.game.grid[width, height, depth];
                    if (cube != null)
                    {
                        // Remove cube from array
                        app.model.game.grid[width, height, depth] = null;

                        // Place the cube one row below
                        app.model.game.grid[width, height - 1, depth] = cube;

                        // Move down the cube
                        cube.position -= Vector3.up;
                    }
                }
    }
    private void IncreaseGameSpeed()
    {
        app.model.game.timeBetweenUpdatesFast
            /= app.model.game.rowClearSpeedMultiplier;
        app.model.game.timeBetweenUpdates
            /= app.model.game.rowClearSpeedMultiplier;
    }

    private void GameLost() 
    {
        enabled = false;

        app.model.ui.inGameWindow.Disable();

        app.model.ui.scoreWindow.gameObject.SetActive(true);

        app.model.ui.lostScore.text = "Score: " + app.model.currentScore.ToString();

        _fallLocIndic.HideIndicatorCubes();
    }

    private void UpdateScore(int value)
    {
        app.model.currentScore = value;

        app.model.ui.inGameScore.text = "Score: " + value.ToString();
    }

    private void MoveCurrentShape(Vector2 input)
    {
        app.model.currentShape.transform.position +=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;

        if(!ValidBlocksPosition())
            app.model.currentShape.transform.position -=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }
    private void RotateCurrentShape(Vector3 actualAxis)
    {
        app.model.currentShape.transform.Rotate(actualAxis, 90,Space.World);
        if (!ValidBlocksPosition())
            app.model.currentShape.transform.Rotate(actualAxis, -90, Space.World);

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }

    private void CreateNextShapePreview()
    {
        Transform nextShape = NewShape();

        nextShape.SetParent(app.model.shapePreviewParent);

        nextShape.localPosition = Vector3.zero;
    }

    private Transform NewShape()
    {
        int shapeNum = Random.Range(0, app.model.game.availableShapes.Count);

        ShapeModel shapeData = app.model.game.availableShapes[shapeNum];

        // Create new transform
        Transform shape = new GameObject(shapeData.name).transform;

        Material targetMat = GetCubeMat();

        // Create cubes & set color
        for (int i = 0; i < shapeData.blocksPositions.Length; i++)
        {
            // Create cube
            Transform cube =
                Instantiate(app.model.game.cubePrefab, shape.transform).transform;

            // Place cube based on data
            cube.localPosition = shapeData.blocksPositions[i];

            Material[] mats = new Material[2];

            mats[0] = app.model.game.cubeSquaresMatt;
            mats[1] = targetMat;

            cube.GetComponent<MeshRenderer>().materials = mats;
        }

        return shape;
    }

    private Material GetCubeMat()
    {
        Material targetMat = app.model.game.availableFillMats
        [Random.Range((int)0, (int)app.model.game.availableFillMats.Count)];

        app.model.game.availableFillMats.Remove(targetMat);

        if (app.model.game.availableFillMats.Count == 0)
                app.model.game.SetAvailableFillmats();

        return targetMat;
    }

    private bool RowIsFull(int rowHeight)
    {
        // Loop thourgh entire row and check if has empty variable
        for (int width = 0; width < app.model.game.boardWidth; width++)
            for (int depth = 0; depth < app.model.game.boardDepth; depth++)
                if (app.model.game.grid[width, rowHeight, depth] == null)
                    return false;

        return true;
    }
    private bool AddCurrentShapeBlocksToGrid()
    {
        foreach (Transform block in app.model.currentShape)
        {
            int x = Mathf.RoundToInt(block.transform.position.x);
            int y = Mathf.RoundToInt(block.transform.position.y);
            int z = Mathf.RoundToInt(block.transform.position.z);

            if (y >= app.model.game.boardHeight)
                return true;
            else // Not necessary but helps readability
                app.model.game.grid[x, y, z] = block.transform;
        }
        return false;
    }
    private bool ValidBlocksPosition() 
    {
        // Validate if current shape blocks positions are allowed

        // Itertae
        foreach (Transform block in app.model.currentShape)
        {
            int roundX = Mathf.RoundToInt(block.position.x);
            int roundY = Mathf.RoundToInt(block.position.y);
            int roundZ = Mathf.RoundToInt(block.position.z);

            // Check if its withing width dimensions
            if (roundX >= app.model.game.boardWidth || roundX < 0)
                return false;
            // Check if its withing depth dimensions
            if (roundZ >= app.model.game.boardDepth || roundZ < 0)
                return false;
            // Check if it touched the ground
            if (roundY < 0)
                return false;

            // Check if block is inside the grid
            // (When you create shapes there are some blocks that are a bit higher than the grid)
            if (roundY < app.model.game.boardHeight)
                // Check if block position is free
                if (app.model.game.grid[roundX, roundY, roundZ] != null)
                    return false;
        }

        return true;
    }
}
