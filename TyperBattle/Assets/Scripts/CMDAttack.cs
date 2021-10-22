using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType
{
    // Launch towards direction
    Launch,
    // Stay still
    Still,
    // Stay still and check for collision in front of direction
    DirectionalStill
};

public class CMDAttack : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    public string commandName;

    // Cool down if attack is unsuccessful
    [SerializeField]
    public int cooldown;

    // Damage delt to other player if successfully hit
    [SerializeField]
    public int damage;

    // How long attack lasts in order to successfully hit
    [SerializeField]
    public float attackTime;

    // Force of attack if launching
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

    // Type of attack
    [SerializeField]
    private CollisionType collisionType;

    // References
    private PlayerManager playerManager;
    private CMDMovement cmdMovement;
    private PlayerCooldown playerCooldown;
    private CollisionDetection collisionDetection;

    // Variables
    private List<DirectionCommand> dirCommand;

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
        StartCoroutine(Attack());
    }

    // Attempt to attack
    IEnumerator Attack() 
    {
        // Movement for launch
        if (collisionType == CollisionType.Launch && dirCommand != null)
        {
            // Movement towards given direction
            cmdMovement.DoCommand(dirCommand, force);
        }

        // Select directional collider
        if (collisionType == CollisionType.DirectionalStill)
        {
            collisionDetection = transform.parent.GetChild(0).GetChild(0).GetComponent<CollisionDetection>();

            // Get direction for collider
            Vector2 direction = Vector2.zero;
            foreach (DirectionCommand dir in dirCommand)
            {
                direction += dir.direction;
            }

            collisionDetection.SetDirection(direction);
            collisionDetection.ToggleCollisionDetection("Player");
        }
        // Select player collider
        else 
        {
            collisionDetection = transform.parent.GetComponent<CollisionDetection>();
            collisionDetection.ToggleCollisionDetection("Player");
        }

        // Check if player is hit within attack time
        bool hit = false;
        float timer = 0.0f;
        while (timer < attackTime)
        {
            timer += Time.deltaTime;

            // Check if player hit other player
            if (collisionDetection.GetCollisionResults() != null)
            {
                // Deal damage
                playerManager.DealDamage(collisionDetection.GetCollisionResults(), damage, stunLength, knockback, knockbackDir);
                hit = true;
                break;
            }

            yield return null;
        }

        // Apply cooldown if did not hit other player
        if (!hit)
            playerCooldown.ApplyCooldown(cooldown);

        yield return null;
    }

}
