using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private string collideTag;
    private GameObject collision;

    // Set direction to face
    public void SetDirection(Vector2 dir)
    {
        Vector2 difference = ((Vector2)transform.parent.position + dir) - (Vector2)transform.parent.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.parent.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
    }

    // Listen for collision
    public void ToggleCollisionDetection(string tag)
    {
        Debug.Log(gameObject.name);
        collision = null;
        collideTag = tag;
    }

    // Return if collision was recieved
    public GameObject GetCollisionResults()
    {
        return collision;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == collideTag)
        {
            this.collision = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == collideTag)
        {
            this.collision = collision.gameObject;
        }
    }
}
