using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitableConfig", menuName = "Scriptable/Hitable/HitableConfig")]
public class HitableConfig : ScriptableObject
{
    public AudioClip sound;
    public string animation;
    public List<HitEffect> hitEffects;

    [System.Serializable]
    public class HitEffect
    {
        public GameObject hitEffectPrefab;
        public bool needLookAt = false;
    }
}