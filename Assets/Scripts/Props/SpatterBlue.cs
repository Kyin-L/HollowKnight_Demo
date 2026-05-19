using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatterBlue : MonoBehaviour
{
    private Animator animator;
    private new Collider2D collider;

    private int groundLayer;
    private readonly int groundHash = Animator.StringToHash("Ground");
    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }


    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            animator.SetBool(groundHash, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            animator.SetBool(groundHash, false);
        }
    }
}
