using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public interface IPlayer
{
    string Id { get; }
}


public class Player : MonoBehaviour, IPlayer
{
    public string Id => _id;
    [SerializeField] private string _id;

    private void Awake()
    {
        _id = Guid.NewGuid().ToString();
    }
}