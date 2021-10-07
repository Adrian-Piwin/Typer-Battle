using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDMovement : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private float force;

    public void DoCommand(DirectionCommand command) 
    {
        body.velocity = Vector2.zero;
        body.AddForce(GetDirection(command.direction) * force);
    }

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
                direction = (new Vector2(transform.position.x + 1, transform.position.y)) - (Vector2)transform.position;
                break;

            case "away":
                direction = (new Vector2(transform.position.x - 1, transform.position.y)) - (Vector2)transform.position;
                break;
        }

        return direction.normalized;
    }
}
