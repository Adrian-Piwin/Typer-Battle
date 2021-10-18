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

    // References

    // Player animator
    private Animator animator;

    // Camera animator
    private Animator cameraAnimator;

    // Health UI
    private UIHealth uiHealth;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // Assign camera animator
        cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();

        // Assign starting health
        startingHealth = health;

        // Assign UI to player
        uiHealth = GameObject.FindWithTag(transform.tag + " Healthbar").GetComponent<UIHealth>();
    }

    // Take damage from opponent, play any effects for taking damage
    public void TakeDamage(int damage) 
    {
        // Take damage from opposing player
        health -= damage;
        if (health < 0) health = 0;

        // Play hit animation for enenmy
        PlayOneShotAnimation("Hit");

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

}
