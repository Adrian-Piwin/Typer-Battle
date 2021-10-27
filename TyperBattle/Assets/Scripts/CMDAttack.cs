using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    // Launch towards direction and touch for hit
    Launch,
    // Stay still and touch for hit
    Touch,
    // Stay still and check for hit in front of direction
    Infront,
    // Stay still and check all around for hit
    Around
};

public enum KnockbackType 
{
    Up,
    Down,
    Left,
    Right,
    Towards,
    Away
};

public class CMDAttack : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    public string commandName;

    // Damage delt to other player if successfully hit
    [SerializeField]
    public int damage;

    // Cool down if attack is unsuccessful
    [SerializeField]
    public int cooldown;

    // Stun length to other player
    [SerializeField]
    public float stunLength;

    // Knockback force
    [SerializeField]
    public float knockbackForce;

    // Direction to knockback
    [SerializeField]
    public KnockbackType knockbackType;

    // Type of attack
    [SerializeField]
    private AttackType attackType;

    [Header("Touch/Launch Attack Settings")]

    // How long attack lasts in order to successfully hit
    [SerializeField]
    public float attackTime;

    // Force of attack if launching
    [SerializeField]
    public float force;

    [Header("Infront/Around Attack Settings")]

    // Distance for infront attack
    [SerializeField]
    public float attackDistance;

    // References
    private PlayerManager playerManager;
    private CMDMovement cmdMovement;
    private PlayerCooldown playerCooldown;

    // Variables
    private List<DirectionCommand> dirCommand;
    private bool didHit;
    private bool isTouchingOpponent;

    private void Start()
    {
        playerManager = transform.parent.GetComponent<PlayerManager>();
        cmdMovement = transform.GetComponent<CMDMovement>();
        playerCooldown = transform.parent.GetComponent<PlayerCooldown>();
    }

    // Do this command
    public void DoCommand(List<DirectionCommand> dirCommand) 
    {
        this.dirCommand = dirCommand;

        // Attack opponent
        Attack();
    }

    // Attempt to attack
    private void Attack() 
    {
        // Movement for launch
        if (attackType == AttackType.Launch && dirCommand != null)
        {
            // Movement towards given direction
            cmdMovement.DoCommand(dirCommand, force);
        }

        didHit = false;

        // Attack based on attack type
        if (attackType == AttackType.Launch || attackType == AttackType.Touch)
            StartCoroutine(TouchAttack());
        else if (attackType == AttackType.Around)
            AroundAttack();
        else if (attackType == AttackType.Infront)
            InfrontAttack();

        // Deal damage on hit
        if (didHit)
            playerManager.DealDamage(FindOpponent(), damage, stunLength, knockbackForce, GetKnockbackDirection(knockbackType));
        else
            playerCooldown.ApplyCooldown(cooldown);
    }

    // Do attack infront of player
    private void InfrontAttack() 
    {
        // Get direction for collider
        Vector2 direction = Vector2.zero;
        foreach (DirectionCommand dir in dirCommand)
        {
            direction += dir.direction;
        }
        direction = direction.normalized;

        Debug.DrawRay(transform.parent.position, direction * attackDistance, Color.red, 1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, direction, attackDistance);
        if (hit.collider != null && hit.transform.tag == "Player")
            didHit = true;
    }

    // Do attack all around player
    private void AroundAttack()
    {
        List<Vector2> dirs = new List<Vector2>
        {
            Vector2.up,
            Vector2.down,
            Vector2.right,
            Vector2.left,
            Vector2.up + Vector2.right,
            Vector2.up + Vector2.left,
            Vector2.down + Vector2.right,
            Vector2.down + Vector2.left
        };

        foreach (Vector2 dir in dirs) 
        {
            Debug.DrawRay(transform.parent.position, dir * attackDistance, Color.red, 1f);
            RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, dir, attackDistance);
            if (hit.collider != null && hit.transform.tag == "Player")
            {
                didHit = true;
                break;
            }
        }
    }

    // Do attack based on touching another player
    IEnumerator TouchAttack() 
    {
        // Check if player is hit within attack time
        float timer = 0.0f;
        while (timer < attackTime)
        {
            timer += Time.deltaTime;

            // Check if player hit other player
            if (isTouchingOpponent)
            {
                // Deal damage
                didHit = true;
                break;
            }

            yield return null;
        }

        yield return null;
    }

    // Return direction for knockback
    private Vector2 GetKnockbackDirection(KnockbackType kbType) 
    {
        switch (kbType) 
        {
            case KnockbackType.Up:
                return Vector2.up;
            case KnockbackType.Down:
                return Vector2.down;
            case KnockbackType.Left:
                return Vector2.left;
            case KnockbackType.Right:
                return Vector2.right;
            case KnockbackType.Away:
                return ((Vector2)(FindOpponent().transform.position - transform.parent.position)).normalized;
            case KnockbackType.Towards:
                return ((Vector2)((FindOpponent().transform.position - transform.parent.position) * -1)).normalized;
        }

        return Vector2.right;
    }

    // Find opponent
    private GameObject FindOpponent() 
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) 
        {
            if (player != transform.parent.gameObject)
                return player;
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isTouchingOpponent = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isTouchingOpponent = false;
    }

}
