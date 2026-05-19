using UnityEngine;

[System.Flags]
public enum RecoilTag
{
    Up = 1 << 0,      // 1
    Forward = 1 << 1, // 2
    Down = 1 << 2     // 4
}

public class CustomTag : MonoBehaviour
{
    [SerializeField] private RecoilTag recoilTag;

    public bool HasTag(RecoilTag tag) => (recoilTag & tag) != 0;

}