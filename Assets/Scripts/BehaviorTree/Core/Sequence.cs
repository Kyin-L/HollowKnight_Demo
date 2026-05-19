using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "Sequence", menuName = "BT/Sequence")]
    public class Sequence : BTNode
    {
        public List<BTNode> children;

        public override State OnUpdate(EnemyContext context)
        {
            foreach (var child in children)
            {
                if (child == null) continue;
                var state = child.OnUpdate(context);
                if (state != State.Success)
                    return state;
            }
            return State.Success;
        }
    }
}