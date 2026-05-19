using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTNode : ScriptableObject
    {
        public abstract State OnUpdate(EnemyContext context);

        public enum State { Success, Failure, Running }
    }
}