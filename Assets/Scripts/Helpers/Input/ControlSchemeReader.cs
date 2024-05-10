#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
#define DISABLESWITCH
#endif

using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;

#if !DISABLESWITCH
using UnityEngine.InputSystem.Switch;
#endif

public class ControlSchemeReader : SingletonBehaviour<ControlSchemeReader>
{
    public static bool isController { get; private set; }
    [System.Flags]
    public enum ControllerGamepadType
    {
        keyboard = 1 << 1,
        generic = 1 << 2,
        XInput = 1 << 3,
        Dualshock = 1 << 4,
        SwitchPro = 1 << 5,
        SwitchJoy = 1 << 6
    }
    public static ControllerGamepadType GamepadType { get; private set; }
    public static event Action controlChange;
    public static event Action lastInputTypeChanged;


    public static MovementInputType lastMovementType = MovementInputType.arrowKeys;//MovementInputType.dpad;
    public static ActionInputType lastActionType = ActionInputType.ctrlKey;//ActionInputType.faceButtons;

    int lastInputDeviceID = -1;
    InputDevice lastInputDevice = null;
    Gamepad lastGamepad = null;
    Keyboard lastKeyboard = null;




    public enum MovementInputType
    {

        //* Following bitwise operations, the second bit determines if the action is a keyboard (0X) or gamepad (1X)
        arrowKeys = 0,       //* 00
        wasd = 1,            //* 01
        dpad = 2,            //* 10
        analog = 3,          //* 11
        keyboard = 0,        //* 00

        gamepad = 2,         //* 10
    }

    public enum ActionInputType
    {
        //* Following bitwise operations, the second bit determines if the action is a keyboard (0X) or gamepad (1X)
        shiftKey = 0,        //* 00
        ctrlKey = 1,         //* 01
        faceButtons = 2,     //* 10
        shoulderButtons = 3, //* 11

        gamepad = 2,         //* 10
    }

    public bool IsGamepad(MovementInputType type) => (type & MovementInputType.gamepad) == MovementInputType.gamepad;
    public bool IsGamepad(ActionInputType type) => (type & ActionInputType.gamepad) == ActionInputType.gamepad;

    protected new void Awake()
    {
        base.Awake();
        lastKeyboard = Keyboard.current;
        lastGamepad = Gamepad.current;
    }

    private void OnEnable()
    {
        InputSystem.onEvent -= ReadDevice;
        InputSystem.onEvent += ReadDevice;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= ReadDevice;
    }

    void UpdateLastInputType(Keyboard keyboard)
    {
        MovementInputType buffer_lastMovement = lastMovementType;
        ActionInputType buffer_lastAction = lastActionType;

        //*Update (set to default) both action and movement to be platform relevant, to ensure they are always both the same device
        if (IsGamepad(lastMovementType))
            lastMovementType = MovementInputType.arrowKeys;
        if (IsGamepad(lastActionType))
            lastActionType = ActionInputType.shiftKey;

        if (keyboard.wKey.IsPressed() || keyboard.aKey.IsPressed() || keyboard.sKey.IsPressed() || keyboard.dKey.IsPressed())
        {
            lastMovementType = MovementInputType.wasd;
        }
        else if (keyboard.upArrowKey.IsPressed() || keyboard.leftArrowKey.IsPressed() || keyboard.downArrowKey.IsPressed() || keyboard.rightArrowKey.IsPressed())
        {
            lastMovementType = MovementInputType.arrowKeys;
        }
        else if (keyboard.ctrlKey.IsPressed())
        {
            lastActionType = ActionInputType.ctrlKey;
        }
        else if (keyboard.shiftKey.IsPressed())
        {
            lastActionType = ActionInputType.shiftKey;
        }

        if (buffer_lastMovement != lastMovementType || buffer_lastAction != lastActionType)
            lastInputTypeChanged?.Invoke();
    }

    void UpdateLastInputType(Gamepad gamepad)
    {
        MovementInputType buffer_lastMovement = lastMovementType;
        ActionInputType buffer_lastAction = lastActionType;

        //*Update (set to default) both action and movement to be platform relevant, to ensure they are always both the same device
        if (!IsGamepad(lastMovementType))
            lastMovementType = MovementInputType.dpad;
        if (!IsGamepad(lastActionType))
            lastActionType = ActionInputType.faceButtons;

        if (gamepad.dpad.IsPressed())
        {
            lastMovementType = MovementInputType.dpad;
        }
        else if (gamepad.leftStick.IsPressed() || gamepad.rightStick.IsPressed())
        {
            lastMovementType = MovementInputType.analog;
        }
        else if (gamepad.buttonNorth.IsPressed() || gamepad.buttonSouth.IsPressed() || gamepad.buttonEast.IsPressed() || gamepad.buttonWest.IsPressed())
        {
            lastActionType = ActionInputType.faceButtons;
        }
        else if (gamepad.leftShoulder.IsPressed() || gamepad.rightShoulder.IsPressed() || gamepad.leftTrigger.IsPressed() || gamepad.rightTrigger.IsPressed())
        {
            lastActionType = ActionInputType.shoulderButtons;
        }

        if (buffer_lastMovement != lastMovementType || buffer_lastAction != lastActionType)
            lastInputTypeChanged?.Invoke();
    }

    void ReadDevice(InputEventPtr eventPtr, InputDevice device)
    {
        //* Ignore anything that isn't a state event.
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (device.deviceId != lastInputDeviceID)
            RegisterDeviceChange(device);

        if (isController)
            UpdateLastInputType(lastGamepad);
        else
            UpdateLastInputType(lastKeyboard);
    }

    void RegisterDeviceChange(InputDevice device)
    {
        lastInputDeviceID = device.deviceId;
        lastInputDevice = device;



        Gamepad gamepad; Keyboard keyboard;

        if ((keyboard = device as Keyboard) != null)
        {
            lastKeyboard = keyboard;
            UpdateLastInputType(keyboard);

            if (GamepadType != ControllerGamepadType.keyboard)
            {
                GamepadType = ControllerGamepadType.keyboard;
                InputSystem.PauseHaptics();
                isController = false;
                controlChange?.Invoke();
            }
        }
        else if ((gamepad = device as Gamepad) != null)
        {
            lastGamepad = gamepad;
            isController = true;
            ControllerGamepadType _gamepadType = GetGamepadType(device);

            if (GamepadType != _gamepadType)
            {
                GamepadType = _gamepadType;
                controlChange?.Invoke();
            }
        }
    }

#pragma warning disable CS0162
    ControllerGamepadType GetGamepadType(InputDevice device)
    {
#if UNITY_SWITCH

        NPad npad = device as NPad;
        if (npad != null && npad.styleMask != NPad.NpadStyles.FullKey)
            return ControllerGamepadType.SwitchJoy;

        return ControllerGamepadType.SwitchPro;
#endif

        if (!isController)
            return ControllerGamepadType.keyboard;

        if (device is XInputController)
            return ControllerGamepadType.XInput;
        else if (device is DualShockGamepad)
            return ControllerGamepadType.Dualshock;
        ////         else
        ////         {
        //// #if !DISABLESWITCH
        ////             if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
        ////                 return ControllerGamepadType.Switch;
        //// #endif
        ////         }

        return ControllerGamepadType.generic;
    }
}
#pragma warning restore CS0162