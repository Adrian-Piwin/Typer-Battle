using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowmo : MonoBehaviour
{
    [SerializeField]
    private float timeScale;

    [SerializeField]
    private Rigidbody2D body;

    private Vector2 curVelocity;
    private bool slowmoState;

    private void FixedUpdate()
    {
        if (slowmoState) 
        {
            body.velocity = curVelocity * timeScale;
        }
    }

    public void EnterSlowmo() 
    {
        curVelocity = body.velocity;
        slowmoState = true;
    }

    public void ExitSlowmo()
    {
        slowmoState = false;

    }
}
