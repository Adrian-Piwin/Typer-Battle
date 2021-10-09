using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDBasicAtk : MonoBehaviour
{
    [Header("Settings")]
    // Time before attack is launched
    [SerializeField]
    private float chargeTime;

    // How long attack lasts in order to successfully hit
    [SerializeField]
    private float attackTime;

    // Speed of attack
    [SerializeField]
    private float speed;

    // Range of attack
    [SerializeField]
    private float distance;

    // Damage delt to other player if successfully hit
    [SerializeField]
    private int damage;

    // Knockback to other player
    [SerializeField]
    private float knockBack;

    // Stun length to other player
    [SerializeField]
    private float stunLength;

    [Header("References")]

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private CMDMovement cmdMovement;

    [SerializeField]
    private PlayerCooldown playerCooldown;

    // Variables
    private bool isCharging;
    private List<DirectionCommand> dirCommand;
    private Command command;

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
        cmdMovement.DoCommand(dirCommand, speed, distance);
        playerManager.EnterAttackState(damage, stunLength);
        yield return new WaitForSeconds(attackTime);
        playerManager.ExitAttackState();
    }

    IEnumerator Charge() 
    {
        playerCooldown.ApplyCooldown(chargeTime + attackTime + command.cooldown);
        isCharging = true;
        playerManager.EnableAnimation("isSpinning", true);
        yield return new WaitForSeconds(chargeTime);
        isCharging = false;
        playerManager.EnableAnimation("isSpinning", false);
        StartCoroutine(Attack());
    }

}
