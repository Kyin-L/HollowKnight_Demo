using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "PlayerDetected", menuName = "BT/PlayerDetected")]
    public class PlayerDetected : BTNode
    {
        public float detectedWaitDuring = 0.5f;

        private readonly int waitHash = Animator.StringToHash("Wait");
        private readonly string playerTag = "Player";
        public override State OnUpdate(EnemyContext context)
        {
            if (context.Has("player"))
            {
                float timer = context.Get<float>("detectedWaitTime");
                if (Time.time - timer < detectedWaitDuring)
                {
                    return State.Running;
                }

                context.Remove("detectedWaitTime");
                return State.Success;
            }

            LayerMask playerLayer = context.Get<LayerMask>("playerLayer");
            Collider2D playerDetectCollider = context.Get<Collider2D>("playerDetectCollider");

            Collider2D[] playerResults = new Collider2D[1];
            int hit = Physics2D.OverlapCollider(
                playerDetectCollider,
                new ContactFilter2D() { layerMask = playerLayer, useLayerMask = true },
                playerResults
            );


            if (hit > 0 && playerResults[0].CompareTag(playerTag))
            {
                context.Set("player", playerResults[0].transform);
                context.anim.SetTrigger(waitHash);
                context.Set("detectedWaitTime", Time.time);
                return State.Running;
            }

            return State.Failure;
        }
    }
}