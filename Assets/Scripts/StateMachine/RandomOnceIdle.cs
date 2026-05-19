using UnityEngine;

public class RandomOnceIdle : StateMachineBehaviour
{
    public float minInterval = 5f;
    public float maxInterval = 10f;
    public int idleCount = 3;
    private float timer;
    private float nextInterval;
    private Animator animator;
    private int currentLayer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        this.currentLayer = layerIndex;
        nextInterval = Random.Range(minInterval, maxInterval);
        timer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer >= nextInterval)
        {
            PlayRandomAnimation();
            timer = 0f;
            nextInterval = Random.Range(minInterval, maxInterval);
        }
    }

    void PlayRandomAnimation()
    {
        int randomIndex = Random.Range(0, idleCount);
        animator.SetTrigger("Idle" + randomIndex);
    }
}