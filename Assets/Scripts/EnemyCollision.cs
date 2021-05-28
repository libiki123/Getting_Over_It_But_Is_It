using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.4f;
    public float groundCheckRadius = 0.3f;

    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;
    public float backArgoDistance = 4f;

    public float minAgroRadius = 0.5f;
    public float closeRangeAttkRadius = 0.5f;
    public float backArgoRadius = 1f;

    public float closeRangeActionDistance = 1f;

    public GameObject hitParticle;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform groundCheck;

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, enemy.body.transform.right, wallCheckDistance, whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    public virtual bool CheckPlayerInMinArgoRange()
    {
        //return Physics2D.Raycast(playerCheck.position, enemy.body.transform.right, minAgroDistance, whatIsPlayer);
        return Physics2D.OverlapCircle(playerCheck.position + (Vector3)(enemy.body.transform.right * minAgroDistance), minAgroRadius, whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxArgoRange()
    {
        return Physics2D.Raycast(playerCheck.position, enemy.body.transform.right, maxAgroDistance, whatIsPlayer);
    }

	public void Update()
	{
        if (Physics2D.OverlapCircle(playerCheck.position + (Vector3)(-enemy.body.transform.right * closeRangeActionDistance), backArgoRadius, whatIsPlayer))
            enemy.Flip();
	}

	public virtual bool CheckPlayerInCloseRangeAction()
    {
        //return Physics2D.Raycast(playerCheck.position, enemy.body.transform.right, closeRangeActionDistance, whatIsPlayer);
        return Physics2D.OverlapCircle(playerCheck.position + (Vector3)(enemy.body.transform.right * closeRangeActionDistance), closeRangeAttkRadius, whatIsPlayer);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * enemy.facingDirection * wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * closeRangeActionDistance), closeRangeAttkRadius);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * minAgroDistance), minAgroRadius);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * maxAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.left * backArgoDistance), backArgoRadius);
    }
}
