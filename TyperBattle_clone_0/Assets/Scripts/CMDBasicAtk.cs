using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CMDBasicAtk : NetworkBehaviour
{
    [Header("Settings")]

    // Damage delt to other player if successfully hit
    [SerializeField]
    public int damage;

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
    private PlayerManager playerManager;
    private CMDMovement cmdMovement;
    private PlayerCooldown playerCooldown;

    // Variables
    private bool isAttacking;
    private List<DirectionCommand> dirCommand;
    private Command command;

    private void Start()
    {
        playerManager = transform.GetComponent<PlayerManager>();
        cmdMovement = transform.GetComponent<CMDMovement>();
        playerCooldown = transform.GetComponent<PlayerCooldown>();
    }

    public void DoCommand(List<DirectionCommand> dirCommand, Command command) 
    {
        this.dirCommand = dirCommand;
        this.command = command;

        // Attack opponent
        StartCoroutine(Attack());
    }

    IEnumerator Attack() 
    {
        // Apply cooldown for attack duration + any cooldown after that
        playerCooldown.ApplyCooldown(attackTime + command.cooldown);

        // Movement towards enemy if any
        if (dirCommand != null)
            cmdMovement.DoCommand(dirCommand, force);

        // Check if player is hit within attack time
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit enemy while attacking
        if (collision.gameObject.tag == "Player" && isAttacking)
        {
            // Deal damage
            playerManager.DealDamage(collision.gameObject, damage, stunLength, knockback, knockbackDir);

            isAttacking = false;
        }
    }

}
