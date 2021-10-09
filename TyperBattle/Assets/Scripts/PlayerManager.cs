using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Player Stats")]

    [SerializeField]
    private int health;

    [Header("References")]

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CMDMovement cmdMovement;

    [SerializeField]
    private PlayerCooldown playerCooldown;

    [SerializeField]
    private Animator animator;

    // Variables
    private bool isAttacking;
    private int damage;
    private float stunLength;

    [System.NonSerialized]
    public Transform opponent;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.tag == "Player1")
            opponent = GameObject.FindWithTag("Player2").transform;
        else
            opponent = GameObject.FindWithTag("Player1").transform;
    }

    public void EnterAttackState(int damage, float stunLength) 
    {
        // Enter state in which a player can damage another player
        this.damage = damage;
        this.stunLength = stunLength;
        isAttacking = true;
    }

    public void ExitAttackState() 
    {
        // Exit attack state
        isAttacking = false;
    }

    public void TakeDamage(int damage, float stunLength) 
    {
        // Take damage from opposing player
        health -= damage;
        if (health <= 0) Die();

        // Apply stun
        playerCooldown.ApplyCooldown(stunLength);
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

    // On collision enter
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Attacking enemy
        if (isAttacking && collision.gameObject == opponent.gameObject) 
        {
            PlayerManager enemyManager = collision.gameObject.GetComponent<PlayerManager>();
            // Apply damage and stun
            enemyManager.TakeDamage(damage, stunLength);

            // Stop movement
            cmdMovement.isMoving = false;

            // Play hit animation for enenmy
            enemyManager.PlayOneShotAnimation("Hit");

            isAttacking = false;
        }

        // Hit enemy without attacking
        if (collision.gameObject == opponent.gameObject) 
        {
            // Stop movement
            cmdMovement.isMoving = false;
        }
    }
}
