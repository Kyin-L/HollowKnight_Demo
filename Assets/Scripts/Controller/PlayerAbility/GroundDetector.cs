using UnityEngine;

public class GroundDetector : AbilityBase
{
    private BoxCollider2D collider;
    private Transform transform;
    private int groundLayer;

    private float checkRadius = 0.1f;
    private Collider2D[] groundHits = new Collider2D[5];

    private Vector2 leftOffset;
    private Vector2 rightOffset;

    private GameObject roarDustEffect;

    private readonly int groundHash = Animator.StringToHash("Ground");

    public GroundDetector(PlayerController playerController, PlayerContext context) : base(playerController, context)
    {
        collider = playerController.GetComponent<BoxCollider2D>();
        transform = playerController.transform;
        groundLayer = LayerMask.GetMask("Ground");

        roarDustEffect = playerController.transform.Find("RoarDustEffect").gameObject;

        CalculateOffsets();
    }

    private void CalculateOffsets()
    {
        Bounds localBounds = collider.bounds;
        Vector3 size = localBounds.size;
        Vector2 offset = collider.offset;

        float bottomY = -size.y / 2f;

        float leftX = -size.x / 2f + checkRadius;
        float rightX = size.x / 2f - checkRadius;

        leftOffset = new Vector2(leftX, bottomY) + offset;
        rightOffset = new Vector2(rightX, bottomY) + offset;
    }

    public void DetectGround()
    {
        Vector2 pos = transform.position;

        int hitCount = Physics2D.OverlapCircleNonAlloc(pos + leftOffset, checkRadius, groundHits, groundLayer)
            + Physics2D.OverlapCircleNonAlloc(pos + rightOffset, checkRadius, groundHits, groundLayer);

        context.isGround = hitCount > 0;
        roarDustEffect.SetActive(context.isGround);
        animator.SetBool(groundHash, context.isGround);
    }

    public override void OnFixedUpdate()
    {
        DetectGround();
    }

    public override void OnUpdate()
    {
        
    }
}

