using UnityEngine;

public class RandomTransform : MonoBehaviour
{
    public Vector2 rotationRange = new Vector2(0, 360);
    public Vector2 scaleRange = new Vector2(1, 1);
    private void OnEnable()
    {
        float rotation = Random.Range(rotationRange.x, rotationRange.y);
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        float scale = Random.Range(scaleRange.x, scaleRange.y);
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
