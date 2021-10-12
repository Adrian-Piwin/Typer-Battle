using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Player Stats")]

    [SerializeField]
    private int health;
    private int startingHealth;

    [Header("Raycasting")]

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float distanceFromGround;

    [Header("References")]

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CMDMovement cmdMovement;

    [SerializeField]
    public PlayerCooldown playerCooldown;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Animator cameraAnimator;

    // Variables
    public float timeSinceLastOppCollision;
    [System.NonSerialized]
    public bool isAttacking;

    private UIHealth uiHealth;

    [System.NonSerialized]
    public Transform opponent;

    // Start is called before the first frame update
    void Start()
    {
        // Assign starting health
        startingHealth = health;

        // Assign UI to player
        uiHealth = GameObject.FindWithTag(transform.tag + " Healthbar").GetComponent<UIHealth>();

        // Find opponent
        if (transform.tag == "Player1")
            opponent = GameObject.FindWithTag("Player2").transform;
        else
            opponent = GameObject.FindWithTag("Player1").transform;
    }

    public void TakeDamage(int damage) 
    {
        // Take damage from opposing player
        health -= damage;
        if (health < 0) health = 0;

        // Play hit animation for enenmy
        opponent.GetComponent<PlayerManager>().PlayOneShotAnimation("Hit");

        // Screenshake effect
        cameraAnimator.Play("CameraShake");

        // Update UI
        uiHealth.TakeDamage((float)health / startingHealth);

        // Die if health reaches 0
        if (health == 0) Die();
    }

    private void Die() 
    {
        Debug.Log("ded");
    }

    public void PlayOneShotAnimation(string anim) 
    {
        switch (anim) 
        {
            case "Hit":
                animator.Play("Hit");
                break;
        }
    }

    public void EnableAnimation(string anim, bool enable) 
    {
        animator.SetBool(anim, enable);
    }

    public bool isGrounded() 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceFromGround, groundLayer);
        if (hit != null) return true;
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Attacking enemy
        if (collision.gameObject == opponent.gameObject)
        {
            timeSinceLastOppCollision = Time.time;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Attacking enemy
        if (collision.gameObject == opponent.gameObject)
        {
            timeSinceLastOppCollision = Time.time;
        }
    }

}
