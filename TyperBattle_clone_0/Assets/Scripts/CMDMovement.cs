using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CMDMovement : NetworkBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private float timeScale;

    [SerializeField]
    private float defaultForce = 10;

    [Header("References")]

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private PlayerManager playerManager;

    private Vector2 oldVel;

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
    }

    // Return direction from command
    private Vector2 GetDirection(DirectionCommand dir) 
    {
        Vector2 direction = ((Vector2)transform.position + dir.direction) - (Vector2)transform.position;
        return direction.normalized;
    }
}
