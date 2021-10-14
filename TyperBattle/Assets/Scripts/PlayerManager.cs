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
    public PlayerCooldown playerCooldown;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Animator cameraAnimator;

    // Health UI
    private UIHealth uiHealth;

    // Opponent
    [System.NonSerialized]
    public Transform opponent;

    // Collision with opponent
    public bool hitOpponent;

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

    // Take damage from opponent, play any effects for taking damage
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

    // Player dies
    private void Die() 
    {
        Debug.Log("ded");
    }

    // Play animation once
    public void PlayOneShotAnimation(string anim) 
    {
        switch (anim) 
        {
            case "Hit":
                animator.Play("Hit");
                break;
        }
    }

    // Enable/Disable an animation indefinetly
    public void ToggleAnimation(string anim, bool enable) 
    {
        animator.SetBool(anim, enable);
    }

    // Current grounded state
    public bool IsGrounded() 
    {
        return Physics2D.Raycast(transform.position, Vector2.down, distanceFromGround, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enter collision with enemy
        if (collision.gameObject == opponent.gameObject)
        {
            hitOpponent = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Exit collision with enemy
        if (collision.gameObject == opponent.gameObject)
        {
            hitOpponent = false;
        }
    }

}
