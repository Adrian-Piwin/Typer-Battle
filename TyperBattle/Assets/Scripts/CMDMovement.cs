using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDMovement : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private float timeScale;

    [SerializeField]
    private float defaultSpeed;

    [SerializeField]
    private float defaultDistance;

    // Slow mo
    private bool slowmoState;
    private Vector2 curVelocity;

    // Movement
    [System.NonSerialized] public bool isMoving;
    private float curSpeed;
    private float curDistance;
    private Vector2 orgPosition;
    private Vector2 curDirection;

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Slow mo during command movement
            if (slowmoState)
                body.velocity = (curDirection * curSpeed) * timeScale;
            else
            // Non slow mo movement
                body.velocity = curDirection * curSpeed;

            // Keep going until reaching max distance
            if (Vector2.Distance(transform.position, orgPosition) > curDistance || curDirection == Vector2.zero)
            {
                isMoving = false;
                body.velocity = Vector2.zero;
            }
        }
        else if (!isMoving && slowmoState) 
        {
            if (curVelocity == Vector2.zero)
                curVelocity = body.velocity;

            // Slow mo during gravity
            body.velocity = curVelocity * timeScale;
        }

    }

    // Set up default movement
    public void DoCommand(List<DirectionCommand> dirCommand) 
    {
        body.velocity = Vector2.zero;
        curSpeed = defaultSpeed;
        curDistance = defaultDistance;

        // Add all directions together
        curDirection = Vector2.zero;
        foreach (DirectionCommand cmd in dirCommand)
        {
            curDirection += GetDirection(cmd.direction);
        }
        curDirection = curDirection.normalized;

        orgPosition = transform.position;
        isMoving = true;
    }

    // Setup movement for other command
    public void DoCommand(List<DirectionCommand> dirCommand, float speed, float distance)
    {
        body.velocity = Vector2.zero;
        curSpeed = speed;
        curDistance = distance;

        // Add all directions together
        curDirection = Vector2.zero;
        foreach (DirectionCommand cmd in dirCommand) 
        {
            curDirection += GetDirection(cmd.direction);
        }
        curDirection = curDirection.normalized;
        
        orgPosition = transform.position;
        isMoving = true;
    }

    // Return direction from command
    private Vector2 GetDirection(string dir) 
    {
        Vector2 direction = Vector2.zero;
        switch (dir) 
        {
            case "up":
                direction = (new Vector2(transform.position.x, transform.position.y + 1)) - (Vector2)transform.position;
                break;

            case "down":
                direction = (new Vector2(transform.position.x, transform.position.y - 1)) - (Vector2)transform.position;
                break;

            case "right":
                direction = (new Vector2(transform.position.x + 1, transform.position.y)) - (Vector2)transform.position;
                break;

            case "left":
                direction = (new Vector2(transform.position.x - 1, transform.position.y)) - (Vector2)transform.position;
                break;

            case "toward":
                direction = (Vector2)playerManager.opponent.position - (Vector2)transform.position;
                break;

            case "away":
                direction = -1 * ((Vector2)playerManager.opponent.position - (Vector2)transform.position);
                break;
        }

        return direction.normalized;
    }

    public void EnterSlowmo()
    {
        curVelocity = Vector2.zero;
        slowmoState = true;
    }

    public void ExitSlowmo()
    {
        slowmoState = false;
    }
}
