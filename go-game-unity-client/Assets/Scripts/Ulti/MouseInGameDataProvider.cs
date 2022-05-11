using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IMouseInGameDataProvider
{
    Vector2 MouseWOrldPosition { get; }
    GameObject MouseHit { get; }
}

[RequireComponent(typeof(PlayerInput))]
public class MouseInGameDataProvider : MonoBehaviour, IMouseInGameDataProvider
{
    public Vector2 MouseWOrldPosition { get; private set; }

    public GameObject MouseHit { get; private set; }

    // cache
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnPoint(InputValue inputValue)
    {
        // get Mouse Position in world
        Vector2 val = inputValue.Get<Vector2>();
        Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(val);
        MouseWOrldPosition = mouseWorldPosition;
        

    }

    private void FixedUpdate()
    {
        // get mouse hit in world
        RaycastHit2D hit = Physics2D.Raycast(MouseWOrldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            MouseHit = hit.collider.gameObject;
        }
    }
            
}