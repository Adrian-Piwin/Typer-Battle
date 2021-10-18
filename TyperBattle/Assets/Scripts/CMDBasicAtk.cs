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
    private PlayerManager playerManager;
    private CMDMovement cmdMovement;
    private PlayerCooldown playerCooldown;

    // Variables
    private bool isCharging;
    private bool isAttacking;
    private List<DirectionCommand> dirCommand;
    private Command command;

    private void Start()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
        playerManager = transform.parent.GetComponent<PlayerManager>();
        cmdMovement = transform.GetComponent<CMDMovement>();
        playerCooldown = transform.parent.GetComponent<PlayerCooldown>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stop movement while charging
        if (isCharging == true)
            body.velocity = Vector2.zero;
    }

    virtual public void DoCommand(List<DirectionCommand> dirCommand, Command command) 
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
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    IEnumerator Charge() 
    {
        // Apply cooldown for attack duration + any cooldown after that
        playerCooldown.ApplyCooldown(chargeTime + attackTime + command.cooldown);

        // Charge up and play charging animation
        isCharging = true;
        playerManager.ToggleAnimation("isSpinning", true);
        yield return new WaitForSeconds(chargeTime);
        isCharging = false;
        playerManager.ToggleAnimation("isSpinning", false);

        // Attack opponent
        StartCoroutine(Attack());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit enemy while attacking
        if (collision.gameObject.tag == "player" && isAttacking)
        {
            // Give damage
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
            // Apply stun
            collision.gameObject.GetComponent<PlayerCooldown>().ApplyCooldown(stunLength);
            // Apply knockback
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockback * knockbackDir);

            isAttacking = false;
        }
    }

}
