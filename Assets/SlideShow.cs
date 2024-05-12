using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
{
    public Image image;
    public Sprite[] slides;
    int index = 0;
    public UnityEvent onFinish;

    Controls controls;

    void Awake()
    {
        controls = new Controls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.UI.Right.started += OnButton;
        controls.UI.Confirm.started += OnButton;
        controls.UI.Select.started += OnButton;
        // controls.
    }
    void OnDisable()
    {
        controls.Disable();
        controls.UI.Right.started -= OnButton;
        controls.UI.Confirm.started -= OnButton;
        controls.UI.Select.started -= OnButton;
        // controls.
    }

    private void OnButton(InputAction.CallbackContext context)
    {
        Next();
    }

    // Start is called before the first frame update
    void Start()
    {
        image.sprite = slides[index];
    }

    public void Next()
    {
        index++;
        if (index == slides.Length)
        {
            onFinish?.Invoke();
        }

        if (index >= slides.Length)
            return;

        image.sprite = slides[index];
    }
}
