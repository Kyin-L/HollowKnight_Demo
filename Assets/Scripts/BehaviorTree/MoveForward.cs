using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "MoveForward", menuName = "BT/MoveForward")]
    public class MoveForward : BTNode
    {
        public override State OnUpdate(EnemyContext context)
        {
            float speed = context.Get<float>("moveSpeed");
            int facing = context.Get<int>("facingDirection");
            context.rb.velocity = new Vector2(speed * facing, context.rb.velocity.y);
            return State.Success;
        }
    }
}