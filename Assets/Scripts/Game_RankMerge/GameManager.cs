using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 7;
    public float cellSize = 1.4f;
    public GameObject cellPrefabs;
    public Transform gridContainer;

    public GameObject rankPrefabs;
    public Sprite[] rankSprites;
    public int maxRankLevel = 7;

    public GridCell[,] grid;

    void InitializeGrid()
    {
        grid=new GridCell[gridWidth, gridHeight];
        for(int x=0; x<gridWidth; x++)
        {
            for(int y=0; y<gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * cellSize - (gridWidth * cellSize / 2) + cellSize / 2,
                    y * cellSize -(gridHeight * cellSize / 2) + cellSize / 2,
                    1f
                );

                GameObject cellobj = Instantiate(cellPrefabs, position, Quaternion.identity, gridContainer);
                GridCell cell = cellobj.AddComponent<GridCell>();
                cell.initialize(x, y);

                grid[x, y] = cell;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();

        for (int i = 0; i < 4; i++)
        {
            SpawnNewRank();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggableRank CreateRankInCell(GridCell cell, int level)
    {
        if (cell == null || !cell.IsEmpty()) return null;

        level=Mathf.Clamp(level, 1, maxRankLevel);

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);

        GameObject rankObj=Instantiate(rankPrefabs,rankPosition, Quaternion.identity, gridContainer);
        rankObj.name = "Rank_Lvl" + level;

        DraggableRank rank=rankObj.AddComponent<DraggableRank>();
        rank.SetRankLevel(level);

        cell.SetRank(rank);
        
        return rank;
    }

    private GridCell FindEmptyCell()
    {
        List<GridCell> emptyCells = new List<GridCell>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].IsEmpty())
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    public bool SpawnNewRank()
    {
        GridCell emptyCell = FindEmptyCell();
        if (emptyCell == null) return false;

        int ranklevel = Random.Range(0, 100) < 80 ? 1 : 2;

        CreateRankInCell(emptyCell, ranklevel);

        return true;
    }

    public GridCell FindClosestCell(Vector3 position)
    {
        for(int x=0; x < gridWidth; ++x)
        {
            for (int y=0;  y < gridHeight; ++y)
            {
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }
        GridCell closestCell = null;
        float closestDistance = float.MaxValue;
        for(int x=0;x < gridWidth; ++x)
        {
            for(int y=0; y < gridHeight; ++y)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = grid[x, y];
                }
            }
        }

        if (closestDistance > cellSize * 2)
        {
            return null;
        }

        return closestCell;
    }

    public void mergeRanks(DraggableRank draggedRank, DraggableRank targetRank)
    {
        if(draggedRank == null || targetRank == null || draggedRank.rankLevel != targetRank.rankLevel)
        {
            if(draggedRank != null) draggedRank.ReturnToOriginalPosition();
            return;
        }

        int newLevel=targetRank.rankLevel+1;
        if(newLevel > maxRankLevel)
        {
            RemoveRank(draggedRank);
            return;
        }

        targetRank.SetRankLevel(newLevel);
        RemoveRank(draggedRank);

        if(Random.Range(0, 100) < 60)
        {
            SpawnNewRank();
        }
    }

    public void RemoveRank(DraggableRank rank)
    {
        if (rank == null) return;
        if (rank.currentCell != null)
        {
            rank.currentCell.currentRank = null;
        }
        
        Destroy(rank.gameObject);
    }
}
