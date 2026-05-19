using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable/Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public int hp = 1;
    public int damage = 1;
    public float moveSpeed = 1;

    public BTNode rootNode;
    public AudioClip hitaudio;

    public List<HitEffect> hitEffects;

    public HitFeedback hitFeedback;
    public DeadFeedback deadFeedback;

    public void Initialize(EnemyContext context)
    {
        SetNodeContext(rootNode, context);
    }

    private void SetNodeContext(BTNode node, EnemyContext context)
    {
        if (node == null) 
            return;
        

        if (node is Selector selector && selector.children != null)
            foreach (var child in selector.children) SetNodeContext(child, context);
        else if (node is Sequence sequence && sequence.children != null)
            foreach (var child in sequence.children) SetNodeContext(child, context);
    }

    public void OnUpdate(EnemyContext context)
    {
        rootNode?.OnUpdate(context);
    }


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
    public class HitFeedback
    {
        public AudioClip[] sound;
        public string animation;
        public GameObject[] effectPrefabs;
    }

    [System.Serializable]
    public class DeadFeedback : HitFeedback
    {
        public DropsItem[] dropItems;
    }

    [System.Serializable]
    public class HitEffect
    {
        public GameObject hitEffectPrefab;
        public bool needLookAt = false;
    }
}