using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "HurtkHandler", menuName = "BT/HurtkHandler")]
    public class HurtkHandler : BTNode
    {
        public float hurtDuration = 0.3f;

        private readonly int hurtHash = Animator.StringToHash("Hurt");
        public override State OnUpdate(EnemyContext context)
        {

            if (context.Has("isHurt"))
            {
                context.Remove("isHurt");

                context.Set("hurtTimer", Time.time);

                return State.Running;
            }
            
            if (context.Has("hurtTimer"))
            {
                float timer = context.Get<float>("hurtTimer");

                if (Time.time - timer < hurtDuration)
                {
                    return State.Running;
                }

                context.Remove("hurtTimer");
            }

            return State.Failure;
        }
    }
}