using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "TurnAround", menuName = "BT/TurnAround")]
    public class TurnAround : BTNode
    {
        public override State OnUpdate(EnemyContext context)
        {
            int facing = context.Get<int>("facingDirection");
            facing *= -1;
            context.Set("facingDirection", facing);

            var scale = context.enemy.transform.localScale;
            scale.x = -scale.x;
            context.enemy.transform.localScale = scale;
            return State.Success;
        }
    }
}