using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
public class RespawnPoint : MonoBehaviour
{
    public int id = -1;
    public Vector2 position;
    public TimelineAsset enterTimeline;

    private RespawnManager respawnManager;

    private readonly string playerTag = "Player";

    void Start()
    {
        position = transform.GetChild(0).position;

        respawnManager = GetComponentInParent<RespawnManager>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if(collider.CompareTag(playerTag))
        {
            if(collision.contacts.Length > 0 && collision.contacts[0].normal.y < -0.5f)
            {
                respawnManager.SetReplacePoint(this);
            }
        }    
    }
}