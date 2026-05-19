using UnityEngine;

[CreateAssetMenu(fileName = "MenuConfig", menuName = "Scriptable/MenuConfig")]
public class MenuConfig : ScriptableObject
{
    public AudioClip BGM;
    public AudioClip effectPressed;
    public AudioClip effectSelect;
}
