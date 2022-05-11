using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public interface IBoard
{
    void Clear();
    void PlaceStone(StoneType stoneType, int2 xy);
    int2 NearestIntersectionIndex(Vector2 position);
}

public class Board : MonoBehaviour, IBoard
{
    // Points for the position of board
    [SerializeField] private Transform _point00;
    [SerializeField] private Transform _point1919;
    [SerializeField] private GameObject _whiteStonePrefab;
    [SerializeField] private GameObject _blackStonePrefab;

    private Vector2[,] _intersectionPositions = new Vector2[19, 19];
    private List<GameObject> _whiteStones = new List<GameObject>();
    private List<GameObject> _blackStones = new List<GameObject>();

    private void Awake()
    {
        // set _intersectionPositions
        Vector2 diagonalVec = _point1919.position - _point00.position;
        float xInterval = diagonalVec.x / 18f;
        float yInterval = diagonalVec.y / 18f;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                Vector2 position = _point00.position + new Vector3(xInterval * i, yInterval * j);
                _intersectionPositions[i, j] = position;
            }
        }
    }

    public void Clear()
    {
        foreach (var stone in _whiteStones)
        {
            Destroy(stone);
        }

        foreach (var stone in _blackStones)
        {
            Destroy(stone);
        }
        
        _whiteStones.Clear();
        _blackStones.Clear();
    }
    
    public void PlaceStone(StoneType stoneType, int2 xy)
    {
        GameObject stone;
        // Instantiate stone and place it in board 
        switch (stoneType)
        {
            case StoneType.White:
                stone = Instantiate(_whiteStonePrefab, _intersectionPositions[xy.x, xy.y], _whiteStonePrefab.transform.rotation);
                _whiteStones.Add(stone);
                break;
            case StoneType.Black:
                stone = Instantiate(_blackStonePrefab, _intersectionPositions[xy.x, xy.y], _blackStonePrefab.transform.rotation);
                _blackStones.Add(stone);
                break;
        }
        
    }

    public int2 NearestIntersectionIndex(Vector2 position)
    {
        float closestX = float.MaxValue, closestY = float.MaxValue;
        int indexX = 0, indexY = 0;
        for (int i = 0; i < 19; i++)
        {
            float dist = Mathf.Abs(_intersectionPositions[i, 0].x - position.x);
            if (dist < closestX)
            {
                closestX = dist;
                indexX = i;
            }

            dist = Mathf.Abs(_intersectionPositions[0, i].y - position.y);
            if (dist < closestY)
            {
                closestY = dist;
                indexY = i;
            }
        }

        return new int2(indexX, indexY);
    }
}