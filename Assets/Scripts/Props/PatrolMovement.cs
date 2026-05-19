using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    [Header("固定移动范围（在Scene中拖拽调整）")]
    public Vector2 minBounds = new Vector2(-5, -3);
    public Vector2 maxBounds = new Vector2(5, 3);

    [Header("移动参数")]
    public float speed = 1f;
    public float waitTime = 0.8f;

    private Vector2 targetPosition;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // 添加重置范围的方法
    [ContextMenu("将范围设置为以物体为中心")]
    public void SetBoundsToCenter()
    {
        float halfWidth = (maxBounds.x - minBounds.x) / 2;
        float halfHeight = (maxBounds.y - minBounds.y) / 2;
        Vector3 center = transform.position;

        minBounds = new Vector2(center.x - halfWidth, center.y - halfHeight);
        maxBounds = new Vector2(center.x + halfWidth, center.y + halfHeight);
    }

    // 添加重置范围并保持大小的快捷方法
    [ContextMenu("重置范围（以物体为中心，大小保持不变）")]
    public void ResetBounds()
    {
        float width = maxBounds.x - minBounds.x;
        float height = maxBounds.y - minBounds.y;
        Vector3 center = transform.position;

        minBounds = new Vector2(center.x - width / 2, center.y - height / 2);
        maxBounds = new Vector2(center.x + width / 2, center.y + height / 2);
    }

    [ContextMenu("重置范围（以物体为中心，默认大小10x6）")]
    public void ResetBoundsDefault()
    {
        Vector3 center = transform.position;
        minBounds = new Vector2(center.x - 5, center.y - 3);
        maxBounds = new Vector2(center.x + 5, center.y + 3);
    }

    void Start()
    {
        SetRandomTarget();
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                SetRandomTarget();
            }
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    void SetRandomTarget()
    {
        targetPosition = new Vector2(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y)
        );
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2,
            (minBounds.y + maxBounds.y) / 2,
            0
        );
        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y,
            0
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawCube(center, size);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }
    }
}