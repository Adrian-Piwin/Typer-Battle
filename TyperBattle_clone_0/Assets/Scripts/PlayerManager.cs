using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
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

    private PlayerCommands playerCommands;
    private TimeManager timeManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerCommands = GetComponent<PlayerCommands>();
        timeManager = GameObject.FindGameObjectWithTag("Time Manager").GetComponent<TimeManager>();

        // Assign camera animator
        cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();

        // Assign starting health
        startingHealth = health;

        // Assign UI to player
        GameObject[] healthUIs = GameObject.FindGameObjectsWithTag("Healthbar");
        foreach (GameObject hUI in healthUIs) 
        {
            if (!hUI.GetComponent<UIHealth>().isAssigned) 
            {
                uiHealth = hUI.GetComponent<UIHealth>();
                hUI.GetComponent<UIHealth>().isAssigned = true;
                break;
            }
        }
    }

    // Deal damage to opponent on the network
    public void DealDamage(GameObject opponent, int damage, float stunLength, float knockback, Vector2 knockbackDir) 
    {
        // Give damage
        opponent.GetComponent<PlayerManager>().TakeDamage(damage);
        // Apply stun
        opponent.GetComponent<PlayerCooldown>().ApplyCooldown(stunLength);
        // Apply knockback
        opponent.GetComponent<Rigidbody2D>().AddForce(knockback * knockbackDir);
    }

    // Take damage from opponent, play any effects for taking damage
    public void TakeDamage(int damage) 
    {
        // Take damage from opposing player
        health -= damage;
        if (health < 0) health = 0;

        // Stop typing when hit
        playerCommands.ClearCommand();

        // Screenshake effect
        cameraAnimator.Play("CameraShake");

        // Slow motion
        timeManager.DoSlowmotion();

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

    // Enable/Disable an animation indefinetly
    public IEnumerator ToggleAnimation(string anim, float time) 
    {
        animator.SetBool(anim, true);
        yield return new WaitForSeconds(time);
        animator.SetBool(anim, false);
    }

    // Current grounded state
    public bool IsGrounded() 
    {
        return Physics2D.Raycast(transform.position, Vector2.down, distanceFromGround, groundLayer);
    }

}
