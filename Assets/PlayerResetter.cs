using System.Collections;
using System.Collections.Generic;
using Than.Input;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerResetter : MonoBehaviour
{
    Body player;
    Vector3 spawnPos;

    Controls controls;

    void Awake()
    {
        controls = new Controls();
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Body>();
        spawnPos = player.transform.position;
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Character.Reset.started += OnResetPressed;
    }

    void OnDisable()
    {
        controls.Character.Reset.started -= OnResetPressed;
    }

    void OnResetPressed(InputAction.CallbackContext context)
    {
        OnReset();
    }

    public void OnReset()
    {
        player.gameObject.SetActive(true);
        player.rb.velocity = Vector2.zero;
        player.transform.rotation = Quaternion.identity;
        player.transform.position = spawnPos;
    }
}
