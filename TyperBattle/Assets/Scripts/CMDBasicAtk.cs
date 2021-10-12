using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDBasicAtk : MonoBehaviour
{
    [Header("Settings")]

    // Damage delt to other player if successfully hit
    [SerializeField]
    public int damage;

    // Time before attack is launched
    [SerializeField]
    public float chargeTime;

    // How long attack lasts in order to successfully hit
    [SerializeField]
    public float attackTime;

    // Force of attack
    [SerializeField]
    public float force;

    // Knockback force
    [SerializeField]
    public float knockback;

    // Direction to knockback
    [SerializeField]
    public Vector2 knockbackDir;

    // Stun length to other player
    [SerializeField]
    public float stunLength;

    // References
    private Rigidbody2D body;
    private Transform opponent;
    private PlayerManager playerManager;
    private CMDMovement cmdMovement;
    private PlayerCooldown playerCooldown;

    // Variables
    private bool isCharging;
    private List<DirectionCommand> dirCommand;
    private Command command;

    private void Start()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
        playerManager = transform.parent.GetComponent<PlayerManager>();
        cmdMovement = transform.GetComponent<CMDMovement>();
        playerCooldown = transform.parent.GetChild(1).GetComponent<PlayerCooldown>();

        // Find opponent
        if (transform.parent.tag == "Player1")
            opponent = GameObject.FindWithTag("Player2").transform;
        else
            opponent = GameObject.FindWithTag("Player1").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stop movement while charging
        if (isCharging == true)
            body.velocity = Vector2.zero;
    }

    public void DoCommand(List<DirectionCommand> dirCommand, Command command) 
    {
        this.dirCommand = dirCommand;
        this.command = command;

        StartCoroutine(Charge());
    }

    IEnumerator Attack() 
    {
        // Movement towards enemy if any
        if (dirCommand != null)
            cmdMovement.DoCommand(dirCommand, force);

        // Check if player is hit within attack time
        float timer = 0.0f;
        playerManager.isAttacking = true;
        while (timer < attackTime) 
        {
            // Player hit
            if (Time.time - playerManager.timeSinceLastOppCollision <= attackTime)
            {
                // Hit opponent within attack time
                PlayerManager oppPlayerManager = opponent.GetComponent<PlayerManager>();

                // Give damage
                oppPlayerManager.TakeDamage(damage);
                // Apply stun
                oppPlayerManager.playerCooldown.ApplyCooldown(stunLength);
                // Apply knockback
                opponent.GetComponent<Rigidbody2D>().AddForce(knockback * knockbackDir);
                
                playerManager.isAttacking = false;
                break;
            }

            yield return null;
        }

        playerManager.isAttacking = false;
    }

    IEnumerator Charge() 
    {
        // Apply cooldown for attack duration + any cooldown after that
        playerCooldown.ApplyCooldown(chargeTime + attackTime + command.cooldown);

        // Charge up and play charging animation
        isCharging = true;
        playerManager.EnableAnimation("isSpinning", true);
        yield return new WaitForSeconds(chargeTime);
        isCharging = false;
        playerManager.EnableAnimation("isSpinning", false);

        // Attack opponent
        StartCoroutine(Attack());
    }

}
