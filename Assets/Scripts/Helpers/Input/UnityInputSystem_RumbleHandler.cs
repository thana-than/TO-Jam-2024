using UnityEngine.InputSystem;

public class UnityInputSystem_RumbleHandler : IHandleRumble
{
    Gamepad gamepad;
    bool gamepadExists = false;

    public virtual bool IsGamepadCurrentScheme()
    {
        return ControlSchemeReader.isController;
    }
    public bool GamepadExists()
    {
        return gamepadExists;
    }

    public void Init()
    {
        Update();
    }

    public void ResetHaptics()
    {
        if (GamepadExists())
            gamepad.ResetHaptics();
    }

    public virtual void SetMotorSpeeds(float lowFrequency, float highFrequency)
    {
        if (GamepadExists())
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
    }

    public Rumbler.RUMBLEUPDATE_RESULT Update()
    {
        Rumbler.RUMBLEUPDATE_RESULT result = Rumbler.RUMBLEUPDATE_RESULT.OK;

        Gamepad newGamepad = Gamepad.current;
        if (gamepad != newGamepad)
        {
            result = Rumbler.RUMBLEUPDATE_RESULT.INPUT_CHANGED;
            ResetHaptics();
        }

        gamepad = newGamepad;
        gamepadExists = newGamepad != null;

        return result;
    }
}
