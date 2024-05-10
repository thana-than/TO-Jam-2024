#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
//using UnityEngine.InputSystem;

//taken from https://www.youtube.com/watch?v=WSw82nKXibc  
//https://github.com/Srfigie/UnityInputSystem_ControlerRumble
public class Rumbler : SingletonBehaviour<Rumbler>
{
    public enum RumblePattern
    {
        Constant,
        Pulse,
        Linear,
        Curve
    }

    public RumblePattern activeRumblePattern { get; private set; }
    //private Gamepad gamepad;
    IHandleRumble rumbleHandler;
    public float rumbleDuration { get; private set; } = 0;
    private float currentTime = 0;
    private float pulseDuration;
    private float lowA;
    private float lowStep;
    private float highA;
    private float highStep;
    private float rumbleStep;
    private float last_low = float.MinValue;
    private float last_high = float.MinValue;
    public bool isMotorActive { get; private set; } = false;
    public AnimationCurve lowCurve { get; private set; }
    public AnimationCurve highCurve { get; private set; }

    [System.Serializable]
    public struct Curve
    {
        public AnimationCurve high;
        public AnimationCurve low;
        public float defaultDuration;
    }

    public float RumbleDampen
    {
        get
        {
            float value = rumbleDampen_master;

            return value;
        }
    }

    [Range(0.0f, 1.0f)] public float rumbleDampen_master = 0.5f;
    [Range(0.0f, 1.0f)] public float rumbleDampen_joycon = 0.5f;

    #region Rumble Patterns
    public void RumbleConstant(float low, float high, float duration)
    {
        activeRumblePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        currentTime = 0;

        rumbleDuration = duration;
    }

    public void RumblePulse(float low, float high, float burstTime, float duration)
    {
        activeRumblePattern = RumblePattern.Pulse;
        lowA = low;
        highA = high;
        rumbleStep = burstTime;
        pulseDuration = burstTime;
        rumbleDuration = duration;
        currentTime = 0;

        SetMotorSpeeds(lowA, highA);
    }
    public void RumbleLinear(float lowStart, float lowEnd, float highStart, float highEnd, float duration)
    {
        activeRumblePattern = RumblePattern.Linear;
        lowA = lowStart;
        highA = highStart;
        lowStep = (lowEnd - lowStart) / duration;
        highStep = (highEnd - highStart) / duration;
        currentTime = 0;

        rumbleDuration = duration;
    }
    public void RumbleCurve(AnimationCurve lowCurve, AnimationCurve highCurve, float duration)
    {
        activeRumblePattern = RumblePattern.Curve;
        this.lowCurve = lowCurve;
        this.highCurve = highCurve;
        lowA = lowCurve.Evaluate(0);
        highA = highCurve.Evaluate(0);
        currentTime = 0;

        rumbleDuration = duration;
    }
    public void RumbleCurve(Curve curves)
    {
        RumbleCurve(curves.low, curves.high, curves.defaultDuration);
    }
    #endregion

    #region Event system methods

    public void BothRumbleSoft(float time)
    {
        StopRumble();
        RumbleConstant(0.25f, 0.5f, time);

    }
    public void BothRumbleHeavy(float time)
    {
        RumbleConstant(0.5f, 1f, time);
    }
    #endregion

    public void StopRumble()
    {
        ResetInternalValues();
        rumbleHandler.ResetHaptics();
    }

    void ResetInternalValues()
    {
        isMotorActive = false;
        rumbleDuration = 0;
        last_low = float.MinValue;
        last_high = float.MinValue;
    }

    public override void Awake()
    {
        base.Awake();
        rumbleHandler = GetNewRumbleHandler();
        rumbleHandler.Init();
    }

    IHandleRumble GetNewRumbleHandler()
    {
        //! SteamInput just doesn't communicate with the InputSystem in any proper way.
        //!If we want to use rumble we'll have to convert the whole Steam version of the game to SteamInput :(
        //! #if !DISABLESTEAMWORKS
        //! if (SteamDeck_RumbleHandler.IsSteamDeck())
        //!    return new SteamDeck_RumbleHandler();
#if UNITY_SWITCH
        return new NintendoSwitch_RumbleHandler();
#endif
        return new UnityInputSystem_RumbleHandler();
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0 || currentTime > rumbleDuration || !rumbleHandler.IsGamepadCurrentScheme())
        {
            StopRumble();
            return;
        }

        UpdateHandler();

        if (!rumbleHandler.GamepadExists())
            return;

        switch (activeRumblePattern)
        {
            case RumblePattern.Constant:
                if (!isMotorActive)
                {

                    isMotorActive = true;
                    SetMotorSpeeds(lowA, highA);
                }
                break;

            case RumblePattern.Pulse:

                if (currentTime > pulseDuration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDuration = currentTime + rumbleStep;
                    if (!isMotorActive)
                    {

                        SetMotorSpeeds(0, 0);
                    }
                    else
                    {

                        SetMotorSpeeds(lowA, highA);
                    }
                }
                break;
            case RumblePattern.Linear:
                if (!isMotorActive)
                {
                    isMotorActive = true;
                    SetMotorSpeeds(lowA, highA);
                    lowA += (lowStep * Time.deltaTime);
                    highA += (highStep * Time.deltaTime);
                }
                break;

            case RumblePattern.Curve:
                if (!isMotorActive)
                    isMotorActive = true;

                float t = currentTime / rumbleDuration;
                lowA = lowCurve.Evaluate(t);
                highA = highCurve.Evaluate(t);
                SetMotorSpeeds(lowA, highA);
                break;
            default:
                break;
        }

        currentTime += Time.deltaTime;
    }

    void SetMotorSpeeds(float low, float high)
    {
        if (last_low == low && last_high == high)
            return;

        last_low = low;
        last_high = high;

        rumbleHandler.SetMotorSpeeds(lowA * RumbleDampen, highA * RumbleDampen);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        StopRumble();
        rumbleHandler.ResetHaptics();
    }

    private void UpdateHandler()
    {
        RUMBLEUPDATE_RESULT result = rumbleHandler.Update();
        if (result == RUMBLEUPDATE_RESULT.INPUT_CHANGED)
            ResetInternalValues();
    }

    public enum RUMBLEUPDATE_RESULT
    {
        OK,
        INPUT_CHANGED
    }
}


public interface IHandleRumble
{
    bool IsGamepadCurrentScheme();
    bool GamepadExists();
    void Init();
    void SetMotorSpeeds(float lowFrequency, float highFrequency);
    void ResetHaptics();
    Rumbler.RUMBLEUPDATE_RESULT Update();
}