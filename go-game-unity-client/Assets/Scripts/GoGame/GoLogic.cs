using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum StoneType
{
    White,
    Black
}

public interface IGoLogic
{
    event Action<StoneType> WinEvent;
    void PlaceStone(StoneType stoneType, int2 xy);
    bool IsIntersectionOccupied(int2 xy);
    void ClearBoard();
}

public class GoLogic : MonoBehaviour, IGoLogic
{
    public event Action<StoneType> WinEvent;

    private struct Intersection
    {
        public bool IsOccupied { get; private set; }
        public StoneType? OccupingStoneType { get; private set; }

        public void Clear()
        {
            IsOccupied = false;
            OccupingStoneType = null;
        }

        public void Place(StoneType stoneType)
        {
            IsOccupied = true;
            OccupingStoneType = stoneType;
        }
    }

    private readonly Intersection[,] _boardIntersections = new Intersection[19, 19];

    public void PlaceStone(StoneType stoneType, int2 xy)
    {
        // check for exception
        // 1. is indices between 0~19
        // 2. Is this intersection already occupied
        if (xy.x < 0 || xy.x >= 19 || xy.y < 0 || xy.y >= 19)
        {
            throw new IndexOutOfRangeException("Intersection index out of range");
        }

        if (_boardIntersections[xy.x, xy.y].IsOccupied)
        {
            throw new InvalidOperationException("Cant place stone on a already occupied intersection\n");
        }

        // update board state
        _boardIntersections[xy.x, xy.y].Place(stoneType);

        // check win condition
        CheckWinCondition(stoneType, xy);
    }

    public bool IsIntersectionOccupied(int2 xy)
    {
        return _boardIntersections[xy.x, xy.y].IsOccupied;
    }
    
    public void ClearBoard()
    {
        // set all intersection to Unoccupied
        for (int x = 0; x < 19; x++)
        {
            for (int y = 0; y < 19; y++)
            {
                _boardIntersections[x, y].Clear();
            }
        }
    }

    private void CheckWinCondition(StoneType stoneType, int2 xy)
    {
        int x = xy.x, y = xy.y;
        // [direction, index] => [up, 0] [right, 2], [down, 4], [left, 6] 
        int[] countStone = new int[8];
        // [direction, x, y] (direction up -> right -> down -> left)
        int[,,] searchIndices = new[,,]
        {
            {{x, y + 1}, {x, y + 2}, {x, y + 3}, {x, y + 4}}, // up
            {{x + 1, y + 1}, {x + 2, y + 2}, {x + 3, y + 3}, {x + 4, y + 4}}, // up right
            {{x + 1, y}, {x + 2, y}, {x + 3, y}, {x + 4, y}}, // right
            {{x + 1, y - 1}, {x + 2, y - 2}, {x + 3, y - 3}, {x + 4, y - 4}}, // down right
            {{x, y - 1}, {x, y - 2}, {x, y - 3}, {x, y - 4}}, // down
            {{x - 1, y - 1}, {x - 2, y - 2}, {x - 3, y - 3}, {x - 4, y - 4}}, // down left
            {{x - 1, y}, {x - 2, y}, {x - 3, y}, {x - 4, y}}, // left
            {{x - 1, y + 1}, {x - 2, y + 2}, {x - 3, y + 3}, {x - 4, y + 4}} // left up
        };

        // count continuous of same stone type 
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int xx = searchIndices[i, j, 0];
                int yy = searchIndices[i, j, 1];
                // check if out of board
                if (xx < 0 || xx >= 19 || yy < 0 || yy >= 19) break;
                // check if different stone type
                if (stoneType != _boardIntersections[xx, yy].OccupingStoneType) break;

                // same stone type, add count
                countStone[i]++;
            }
        }
        
        // count if any direction have >= 5 continuous of same stone type
        for (int i = 0; i < 4; i++)
        {
            int cnt = countStone[i] + countStone[i + 4] + 1;
            if (cnt >= 5)
            {
                // win, emit win event
                WinEvent?.Invoke(stoneType);
                return;
            }
        }
    }
}