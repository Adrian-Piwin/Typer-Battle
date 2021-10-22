using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDMovement : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private float defaultForce = 10;

    [Header("References")]

    [SerializeField]
    private Rigidbody2D body;

    private Vector2 oldVel;

    private bool isFrozen;

    private void Update() 
    {
        if (isFrozen)
            body.velocity = Vector2.zero;
    }

    // Set up movement
    public void DoCommand(List<DirectionCommand> dirCommand, float force) 
    {
        if (force == 0)
            force = defaultForce;

        // Do player movement
        body.velocity = Vector2.zero;

        // Add force in all directions
        foreach (DirectionCommand cmd in dirCommand)
        {
            body.AddForce(GetDirection(cmd).normalized * force);
        }
    }

    // Freeze player movement
    public void FreezeMovement(bool isFrozen)
    {
        if (isFrozen)
        {
            oldVel = body.velocity;
            body.gravityScale = 0f;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 1f;
            body.velocity = oldVel;
        }

        this.isFrozen = isFrozen;
    }

    // Return direction from command
    private Vector2 GetDirection(DirectionCommand dir) 
    {
        Vector2 direction = ((Vector2)transform.position + dir.direction) - (Vector2)transform.position;
        return direction.normalized;
    }
}
