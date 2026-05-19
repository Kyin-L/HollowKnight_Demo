using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "CheckEdgeOrWall", menuName = "BT/CheckEdgeOrWall")]
    public class CheckEdgeOrWall : BTNode
    {
        public override State OnUpdate(EnemyContext context)
        {
            LayerMask wallLayer = context.Get<LayerMask>("wallLayer");
            LayerMask groundLayer = context.Get<LayerMask>("groundLayer");
            Collider2D wallDetectCollider = context.Get<Collider2D>("wallDetectCollider");
            Collider2D groundDetectCollider = context.Get<Collider2D>("groundDetectCollider");

            // 检测前方墙壁（墙面 + 地面）
            Collider2D[] wallResults = new Collider2D[1];
            int wallCount = Physics2D.OverlapCollider(
                wallDetectCollider,
                new ContactFilter2D() { layerMask = wallLayer, useLayerMask = true },
                wallResults
            );

            // 检测前方地面（仅地面）
            Collider2D[] groundResults = new Collider2D[1];
            int groundCount = Physics2D.OverlapCollider(
                groundDetectCollider,
                new ContactFilter2D() { layerMask = groundLayer, useLayerMask = true },
                groundResults
            );

            return (wallCount > 0 || groundCount == 0) ? State.Success : State.Failure;
        }
    }
}