using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisController : TetrisElement
{
    // Controllers
    [HideInInspector] public CameraController cam;
    [HideInInspector] public UIController ui;
    private FallLocationIndicatorController _fallLocIndic;

    [SerializeField] private GridMeshScaler _gridScaler;

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
        MoveApplicationPosition();

        // Scale grid enviorment
        _gridScaler.ScaleGrid(
            app.model.game.boardWidth,
            app.model.game.boardDepth,
            app.model.game.boardHeight);

        // Get controllers
        cam = GetComponentInChildren<CameraController>();
        ui = GetComponentInChildren<UIController>();
        _fallLocIndic =
             GetComponentInChildren<FallLocationIndicatorController>();

        cam.Init();
        _fallLocIndic.Init();

        // Make sure script is disabled
        // (To disable update, movement & rotation)
        enabled = false;
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

        // Create new shape and set it as current one
        SetCurrrentShape(NewShape());

        // Set next shape preview
        CreateNextShapePreview();

        // Make sure script is enabled
        // (To enable update, movement & rotation)
        enabled = true;
    }

    #region Input functions
    public void OnMovementInput(Vector2 input)
    {
        // Make sure script is enabled
        // (used to disable movement when game isn't started)
        if (!enabled) 
            return;

        MoveCurrentShape(input);
    }
    public void OnRotationInput(UserInputView.RotationAxis axis)
    {
        // Make sure script is enabled
        // (used to disable rotation when game isn't started)
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
    public void OnRegisterdHighScore(string _name, int _score)
    {
        // Check if the highScores list is empty
        if (app.model.highScores.Count == 0)
            app.model.highScores.Add(
                new Score { name = _name, score = _score });
        else
        {
            // Boolean used to check if placed the score as one of the high scores
            bool placedScore = false;

            // Loop high scores list to check where this score needs to be placed
            for (int i = 0; i < app.model.highScores.Count; i++)
            {
                // If score is higher than this high score - place it above that one
                if (_score > app.model.highScores[i].score)
                {
                    app.model.highScores.Insert(i,
                        new Score { name = _name, score = _score });

                    placedScore = true;
                }
            }

            if(!placedScore)
                app.model.highScores.Add(
                    new Score { name = _name, score = _score });
        }

        ui.UpdateHighScoresWindow();
    }
    #endregion


    // Private function
    private void MoveApplicationPosition()
    {
        // Set the correct location of the application to make sure
        // all position based functions will work properly

        // Add offset to the board position if axis dimension is even
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

            // Shape reached bottom or has another
            // shape below that blocks falling
            ShapeCollided();
        }
    }
    
    private void IncreaseGameSpeed()
    {
        // Update times to wait between frames
        app.model.game.timeBetweenUpdatesFast
            /= app.model.game.rowClearSpeedMultiplier;
        app.model.game.timeBetweenUpdates
            /= app.model.game.rowClearSpeedMultiplier;
    }
    private void GameLost() 
    {
        // Make sure script is disabled
        // (To disable update, movement & rotation)
        enabled = false;

        ui.OnGameLost();

        _fallLocIndic.HideIndicatorCubes();
    }
    private void UpdateScore(int value)
    {
        app.model.currentScore = value;

        ui.UpdateInGameScore();
    }


    #region Shape down collision functions
    private void ShapeCollided()
    {
        // Check if one of the shapes blocks is higher the grid height
        bool reachedTop = AddCurrentShapeBlocksToGrid();

        if (reachedTop)
            GameLost();
        else
        {
            // Check for full rows
            CheckForRows();

            // Set new curernt shape (grab preview shape)
            SetCurrrentShape(app.model.shapePreviewParent.GetChild(0));

            // Create next shape preview
            CreateNextShapePreview();
        }
    }
    private void CheckForRows()
    {
        int maxHeightToCheck = 0;
        int minHeightToCheck = app.model.game.boardHeight;

        for (int i = 0; i < app.model.currentShape.childCount; i++)
        {
            float yPos = app.model.currentShape.GetChild(i).position.y;

            if (yPos > maxHeightToCheck)
                maxHeightToCheck = Mathf.RoundToInt(yPos);

            if (yPos < minHeightToCheck)
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
    }// NOT CHECKED
    private void ClearRow(int rowHeight)
    {
        int boardWidth = app.model.game.boardWidth;
        int boardDepth = app.model.game.boardDepth;

        // Loop grid row, empty it and disable it's cubes
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
        // Loop grid until min row (the row that was deleted)
        for (int height = minRow; height < app.model.game.boardHeight; height++)
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
    private bool RowIsFull(int rowHeight)
    {
        // Loop row and check if has empty variable
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
    #endregion

    #region Shape manipulation functions
    // Current shape functions
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

        // Set fall indicator
        _fallLocIndic.SetNewIndicator();
    }
    private void MoveCurrentShape(Vector2 input)
    {
        // Move current shape postion based on input & grid snapped direciton
        app.model.currentShape.transform.position +=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;

        // Check if shape new blocks position isn't valid
        if (!ValidBlocksPosition())
            // Move back
            app.model.currentShape.transform.position -=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;
        else
            // Update fall location indicator
            _fallLocIndic.UpdateIndicator();
    }
    private void RotateCurrentShape(Vector3 actualAxis)
    {
        // Rotate current shape on a given axis
        app.model.currentShape.transform.Rotate(actualAxis, 90,Space.World);

        // Check if shape new blocks position isn't valid
        if (!ValidBlocksPosition())
            // Rotate back
            app.model.currentShape.transform.Rotate(actualAxis, -90, Space.World);
        else
            // Update fall location indicator
            _fallLocIndic.UpdateIndicator();
    }

    private void CreateNextShapePreview()
    {
        // Create new shape
        Transform nextShape = NewShape();

        // Set new shape parent
        nextShape.SetParent(app.model.shapePreviewParent);

        // Reset position
        nextShape.localPosition = Vector3.zero;
    }

    private Transform NewShape()
    {
        // Get random number based on available shapes list count
        int shapeNum = Random.Range(0, app.model.game.availableShapes.Count);

        // Grab shape data
        ShapeModel shapeData = app.model.game.availableShapes[shapeNum];

        // Create new transform
        Transform shape = new GameObject(shapeData.name).transform;

        Material targetMat = CubeMat();

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
    private Material CubeMat()
    {
        // Get random material from availableFillMats
        Material targetMat = app.model.game.availableFillMats
            [Random.Range((int)0, (int)app.model.game.availableFillMats.Count)];

        // Remove material from availableFillMats
        // (to make sure you loop through
        // the entrie list before you get the same color)
        app.model.game.availableFillMats.Remove(targetMat);

        // if availableFillMats list count is 0 - Reset it
        if (app.model.game.availableFillMats.Count == 0)
            app.model.game.SetAvailableFillmats();

        return targetMat;
    }
    #endregion

    // Used to check if current shape blocks positions are valid
    private bool ValidBlocksPosition() 
    {
        // Validate if current shape blocks positions are allowed

        // Loop current shape blocks
        foreach (Transform block in app.model.currentShape)
        {
            int roundX = Mathf.RoundToInt(block.position.x);
            int roundY = Mathf.RoundToInt(block.position.y);
            int roundZ = Mathf.RoundToInt(block.position.z);

            // Check if it's within width dimensions
            if (roundX >= app.model.game.boardWidth || roundX < 0)
                return false;
            // Check if it's within depth dimensions
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
