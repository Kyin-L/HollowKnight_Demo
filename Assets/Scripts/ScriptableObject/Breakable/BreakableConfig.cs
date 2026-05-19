using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableConfig", menuName = "Scriptable/Hitable/BreakableConfig")]
public class BreakableConfig : HitableConfig
{
    [Header("基础属性")]
    [MinValue(1)]
    public int hp = 1;

    [Header("反馈列表")]
    public List<Feedback> feedbacks = new List<Feedback>();

    [System.Serializable]
    public class DropsItem
    {
        public GameObject prefab;
        public Vector2Int countRange = new Vector2Int(1, 1);
        public Vector2 forceRange = new Vector2(500, 1000);
        
        public static DropsItem GetDefault()
        {
            return new DropsItem();
        }
    }

    [System.Serializable]
    public class Feedback
    {
        public AudioClip sound;
        public string animation = "Hit";
        public GameObject[] effectPrefabs;
        public DropsItem[] dropItemsPrefabs;
        
        public static Feedback GetDefault(bool isBreak = false)
        {
            var defaultFeedback = new Feedback();
            defaultFeedback.animation = isBreak ? "Break" : "Hit";
            defaultFeedback.sound = null;
            defaultFeedback.effectPrefabs = new GameObject[0];
            defaultFeedback.dropItemsPrefabs = new DropsItem[0];
            return defaultFeedback;
        }
    }
}