using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "Selector", menuName = "BT/Selector")]
    public class Selector : BTNode
    {
        public List<BTNode> children;

        public override State OnUpdate(EnemyContext context)
        {
            foreach (var child in children)
            {
                if (child == null) continue;
                var state = child.OnUpdate(context);
                if (state != State.Failure)
                    return state;
            }
            return State.Failure;
        }
    }
}