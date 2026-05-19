using UnityEngine;
using UnityEngine.UI;

public class HUDSouls : MonoBehaviour
{
    private Animator animator;
    private Image image;
    private int maxSouls;
    private int souls;

    public int Souls
    {
        get
        {
            return souls;
        }
        set
        {
            souls = Mathf.Clamp(value, 0, maxSouls);
            image.material.SetFloat("_FillAmount", 1.0f * souls / maxSouls);
        }
    }

    private int showHash = Animator.StringToHash("Souls_Show");

    void Awake()
    {
        animator = GetComponent<Animator>();
        image = transform.Find("SoulsImage").GetComponent<Image>();
    }

    public void Initialized(int maxSouls)
    {
        this.maxSouls = maxSouls;
        souls = 0;
    }

    public void Show()
    {
        animator.Play(showHash);
    }

    public void Hide()
    {

    }
}
