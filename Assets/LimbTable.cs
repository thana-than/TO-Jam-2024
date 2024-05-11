using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Service/LimbTable")]
public class LimbTable : Service
{
    [SerializeField] LimbData[] _limbs;
    public Dictionary<Limb.Type, LimbData> limbs { get; private set; } = new Dictionary<Limb.Type, LimbData>();

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < _limbs.Length; i++)
        {
            limbs.TryAdd(_limbs[i].type, _limbs[i]);
        }
    }


}
