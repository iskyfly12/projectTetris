using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BoardBehaviour : MonoBehaviour
{
    [Header("Board")]
    public Block[] blocks;

    [Header("Settings")]
    public float fallTime = 0.8f;

    [Header("References")]
    [SerializeField] private Transform pointNextBlock;
    [SerializeField] private TextMeshProUGUI levelText;

    private bool activeInput;
    private int levelSpeed;
    private int linesCleaned;
    private float previousTime;
    private const int width = 10;
    private const int height = 24;

    private Transform[,] grid = new Transform[width, height];
    private Block currentBlock;
    private Block nextBlock;

    private event Action<int, int> onScore;
    private event Action onLose;

    public void Init(Action<int, int> onScoreAction, Action onLoseAction)
    {
        activeInput = true;
        enabled = true;

        StartCoroutine(CreateBoard());

        levelSpeed = 1;
        levelText.text = levelSpeed.ToString();

        linesCleaned = 0;

        onScore = onScoreAction;
        onLose = onLoseAction;
    }

    void Update()
    {
        if (!currentBlock)
            return;

        InputBoard();
    }

    #region Input
    //INPUT 
    void InputBoard()
    {
        if (!activeInput)
            return;

        //MOVE BLOCK
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentBlock.transform.position += new Vector3(-1f, 0, 0);
            if (!ValidInput())
                currentBlock.transform.position -= new Vector3(-1f, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentBlock.transform.position += new Vector3(1f, 0, 0);
            if (!ValidInput())
                currentBlock.transform.position -= new Vector3(1f, 0, 0);
        }

        //ROTATE BLOCK
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentBlock.transform.RotateAround(currentBlock.transform.TransformPoint(currentBlock.PointRotation()), new Vector3(0, 1, 0), 90);
            if (!ValidInput())
                currentBlock.transform.RotateAround(currentBlock.transform.TransformPoint(currentBlock.PointRotation()), new Vector3(0, 1, 0), -90);
        }

        //FALL DOWN BLOCK
        float fallTimeByLevel = fallTime / levelSpeed;
        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTimeByLevel / 10 : fallTimeByLevel))
        {
            currentBlock.transform.position += new Vector3(0, 0, -1f);

            //END OF THE BOARD
            if (!ValidInput())
            {
                currentBlock.transform.position -= new Vector3(0, 0, -1f);

                SetBlockToGrid();
                CheckRows();
                StartCoroutine(GenerateNewBlock());
            }
            previousTime = Time.time;
        }
    }

    //CHECK IF IS IN THE LIMIT OF THE BOARD
    bool ValidInput()
    {
        for (int i = 0; i < currentBlock.Children().Length; i++)
        {
            int xPosition = Mathf.RoundToInt(currentBlock.Children()[i].position.x);
            int zPosition = Mathf.RoundToInt(currentBlock.Children()[i].position.z);

            if (xPosition < 0 || xPosition >= width || zPosition < 0 || zPosition >= height)
                return false;

            if (grid[xPosition, zPosition])
                return false;
        }

        return true;
    }
    #endregion

    #region Grid
    //ADD EACH BLOCK TO THE GRID
    void SetBlockToGrid()
    {
        for (int i = 0; i < currentBlock.Children().Length; i++)
        {
            int xPosition = Mathf.RoundToInt(currentBlock.Children()[i].position.x);
            int zPosition = Mathf.RoundToInt(currentBlock.Children()[i].position.z);

            if (xPosition >= 0 && xPosition < width && zPosition >= 0 && zPosition < height)
                grid[xPosition, zPosition] = currentBlock.Children()[i].transform;

            if (zPosition >= height - 1)
                onLose();
        }
    }

    //CHECK IF THE ROWS ARE COMPLETED AND INCREASE THE SCORE
    void CheckRows()
    {
        int points = 0;
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasRowComplete(i))
            {
                points++;
                StartCoroutine(CleanRow(i));
                MoveRowDown(i, points);
            }
        }

        linesCleaned += points;

        //INCREASE SCORE
        if (points > 0)
            if (onScore != null)
            {
                onScore(levelSpeed, points);
                IncreaseLevel();
            }
    }

    //CHECK IF THE ROW IS COMPLETE
    bool HasRowComplete(int row)
    {
        for (int j = 0; j < width; j++)
            if (grid[j, row] == null)
                return false;

        return true;
    }

    //MOVE THE ROW DOWN
    void MoveRowDown(int row, int amountMove)
    {
        for (int i = row; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, i] != null)
                {
                    grid[j, i - 1] = grid[j, i];
                    grid[j, i] = null;
                    grid[j, i - 1].DOMoveZ(grid[j, i - 1].transform.position.z - amountMove, .25f);
                }
            }
        }
    }

    //CLEAN THE ROW
    IEnumerator CleanRow(int row)
    {
        Transform[] blocksToDestroy = new Transform[width];
        for (int j = 0; j < width; j++)
        {
            grid[j, row].DOScale(Vector3.zero, .25f);
            blocksToDestroy[j] = grid[j, row];
            grid[j, row] = null;
        }

        yield return new WaitForSeconds(.25f);

        for (int k = 0; k < blocksToDestroy.Length; k++)
            Destroy(blocksToDestroy[k].gameObject);
    }
    #endregion

    #region Board
    //CREATE BOARD BLOCKS 
    IEnumerator CreateBoard()
    {
        ClearBoard();

        Block newBlock = blocks[UnityEngine.Random.Range(0, blocks.Length)];
        currentBlock = Instantiate(newBlock, new Vector3(4, 0, 23), Quaternion.identity);

        while (newBlock.BlockID() == currentBlock.BlockID())
            newBlock = blocks[UnityEngine.Random.Range(0, blocks.Length)];

        nextBlock = Instantiate(newBlock, pointNextBlock.position, Quaternion.identity);

        yield break;
    }

    //CLEAR GRID AND BLOCKS ON BOARD
    void ClearBoard()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (grid[j, i] != null)
                    Destroy(grid[j, i].gameObject);

        if (currentBlock != null)
            Destroy(currentBlock.gameObject);

        if (nextBlock != null)
            Destroy(nextBlock.gameObject);
    }

    //CREATE NEW 'NEXT BLOCK' AND SET THE CURRENT BLOCK
    IEnumerator GenerateNewBlock()
    {
        activeInput = false;
        currentBlock = nextBlock;
        currentBlock.transform.DOMove(new Vector3(width / 2, 0, height - 1), .25f);

        Block newBlock = blocks[UnityEngine.Random.Range(0, blocks.Length)];
        while (newBlock.BlockID() == currentBlock.BlockID() && newBlock.BlockID() == nextBlock.BlockID())
            newBlock = blocks[UnityEngine.Random.Range(0, blocks.Length)];
        nextBlock = Instantiate(newBlock, pointNextBlock.position, Quaternion.identity);

        yield return new WaitForSeconds(.25f);

        activeInput = true;
    }

    //INCREASE BOARD LEVEL EVERY 10 CLEAN LINES
    void IncreaseLevel()
    {
        if (linesCleaned % 10 == 0)
        {
            levelSpeed++;
            levelText.text = levelSpeed.ToString();
        }
    }
    #endregion
}
