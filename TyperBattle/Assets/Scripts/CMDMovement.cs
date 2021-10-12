using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDMovement : MonoBehaviour
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

    // Slow mo
    private bool slowmoState;
    private Vector2 curVelocity;

    void FixedUpdate()
    {
        if (slowmoState) 
        {
            // Slow mo during gravity
            body.velocity = curVelocity * timeScale;
        }
    }

    // Set up default movement
    public void DoCommand(List<DirectionCommand> dirCommand) 
    {
        body.velocity = Vector2.zero;

        // Add force in all directions
        foreach (DirectionCommand cmd in dirCommand)
        {
            body.AddForce(GetDirection(cmd).normalized * defaultForce);
        }
    }

    // Setup movement for other command
    public void DoCommand(List<DirectionCommand> dirCommand, float force)
    {
        body.velocity = Vector2.zero;

        // Add force in all directions
        foreach (DirectionCommand cmd in dirCommand)
        {
            body.AddForce(GetDirection(cmd).normalized * force);
        }
    }

    // Return direction from command
    private Vector2 GetDirection(DirectionCommand dir) 
    {
        Vector2 direction;

        if (dir.directionName == "toward")
        {
            direction = (Vector2)playerManager.opponent.position - (Vector2)transform.position;
        }
        else if (dir.directionName == "away")
        {
            direction = -1 * (Vector2)playerManager.opponent.position - (Vector2)transform.position;
        }
        else 
        {
            direction = ((Vector2)transform.position + dir.direction) - (Vector2)transform.position;
        }

        return direction.normalized;
    }

    public void EnterSlowmo()
    {
        curVelocity = body.velocity;
        slowmoState = true;
    }

    public void ExitSlowmo()
    {
        body.velocity = curVelocity;
        slowmoState = false;
    }
}
