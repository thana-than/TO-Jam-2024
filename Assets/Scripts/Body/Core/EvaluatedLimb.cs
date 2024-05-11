using UnityEngine;

public abstract class EvaluatedLimb : Limb
{
    [SerializeField] AnimationCurve evaluationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float off_speed = 10;
    [SerializeField] float on_speed = 10;

    public float time_percent { get; private set; } = 0;
    public float evaluation { get; private set; } = 0;

    protected virtual void Start()
    {
        time_percent = Active.ToInt();
        evaluation = evaluationCurve.Evaluate(time_percent);
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!Application.isPlaying)
        {
            time_percent = Active.ToInt();
            evaluation = evaluationCurve.Evaluate(time_percent);
            LimbFixedUpdate(evaluation);
        }
    }

    protected virtual void Update()
    {
        UpdatePercent();
        LimbUpdate(evaluation);
    }

    protected virtual void FixedUpdate()
    {
        LimbFixedUpdate(evaluation);
    }

    void UpdatePercent()
    {
        float speed = Active ? on_speed : -off_speed;
        time_percent = Mathf.Clamp01(time_percent + speed * Time.deltaTime);
        evaluation = evaluationCurve.Evaluate(time_percent);
    }

    protected virtual void LimbUpdate(float evaluation) { }
    protected virtual void LimbFixedUpdate(float evaluation) { }
}
