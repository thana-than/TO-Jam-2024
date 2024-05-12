using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartSelector : MonoBehaviour
{
    Controls controls;
    Body player;

    public GameObject[] enableWhenActive;

    bool active = false;

    Limb.Type startingLimb = Limb.Type.normal;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Body>();
        controls = new Controls();
        player.left_limb = startingLimb;
        player.right_limb = startingLimb;
        player.LimbsChanged();
    }

    void Start()
    {
        OnRestart();
    }

    void OnEnable()
    {
        controls.Character.Reset.Enable();
        controls.Character.Reset.started += OnRestart;
        controls.UI.Enable();
        controls.UI.Confirm.started += OnConfirm;
        controls.UI.Right.started += OnRight;
        controls.UI.Left.started += OnLeft;
    }
    void OnDisable()
    {
        controls.Character.Reset.Disable();
        controls.Character.Reset.started += OnRestart;
        controls.UI.Disable();
        controls.UI.Confirm.started -= OnConfirm;
        controls.UI.Right.started -= OnRight;
        controls.UI.Left.started -= OnLeft;
    }

    private void OnRestart(InputAction.CallbackContext context)
    {
        OnRestart();
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        OnConfirm();
    }

    private void OnLeft(InputAction.CallbackContext context)
    {
        OnLeft();
    }

    private void OnRight(InputAction.CallbackContext context)
    {
        OnRight();
    }

    public void OnRight()
    {
        if (!active)
            return;
        int len = Enum.GetNames(typeof(Limb.Type)).Length;
        player.right_limb = (Limb.Type)(((int)player.right_limb + 1) % len);
        player.LimbsChanged();
    }

    public void OnLeft()
    {
        if (!active)
            return;
        int len = Enum.GetNames(typeof(Limb.Type)).Length;
        player.left_limb = (Limb.Type)(((int)player.left_limb + 1) % len);
        player.LimbsChanged();
    }

    void OnConfirm()
    {
        active = false;
        player.brain.enabled = true;
        controls.UI.Disable();
        foreach (GameObject obj in enableWhenActive)
        {
            obj.SetActive(false);
        }
    }

    void OnRestart()
    {
        active = true;
        player.brain.enabled = false;
        controls.UI.Enable();
        foreach (GameObject obj in enableWhenActive)
        {
            obj.SetActive(true);
        }
    }
}
