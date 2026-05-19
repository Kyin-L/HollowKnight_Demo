using UnityEngine;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class EnemyContext
    {
        public Enemy enemy { get; private set; }
        public Transform transform { get; private set; }
        public Rigidbody2D rb { get; private set; }
        public Animator anim { get; private set; }

        private Dictionary<string, object> data = new Dictionary<string, object>();

        public EnemyContext(Enemy enemy, Rigidbody2D rb, Animator anim)
        {
            this.enemy = enemy;
            transform = enemy.transform;
            this.rb = rb;
            this.anim = anim;
        }


        public void Set(string key, object value)
        {
            data[key] = value;
        }

        public T Get<T>(string key)
        {
            if (data.TryGetValue(key, out object value))
                return (T)value;

            Debug.LogError($"Key {key} not found in context!");
            return default;
        }

        public void Remove(string key)
        {
            if (data.ContainsKey(key))
                data.Remove(key);
        }

        public bool Has(string key) => data.ContainsKey(key);
    }
}