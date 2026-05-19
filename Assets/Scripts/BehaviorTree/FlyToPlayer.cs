using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "FlyToPlayer", menuName = "BT/FlyToPlayer")]
    public class FlyToPlayer : BTNode
    {
        public override State OnUpdate(EnemyContext context)
        {
            if (!context.Has("player"))
                return State.Failure;

            Transform player = context.Get<Transform>("player");
            float speed = context.Get<float>("flySpeed");

            Vector2 direction = (player.position - context.transform.position).normalized;

            var scale = context.enemy.transform.localScale;
            scale.x = direction.x > 0 ? 1 : -1;
            context.enemy.transform.localScale = scale;

            context.rb.velocity = direction * speed;

            return State.Running;
        }
    }
}