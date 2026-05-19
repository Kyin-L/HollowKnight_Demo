using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "Scriptable/SceneConfig")]
public class SceneConfig : ScriptableObject
{
    public AudioClip BGM;
    public AudioClip effectPressed;
    public AudioClip effectSelect;
}
