using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private LayerMask whatIsPlayer;
	[SerializeField] private Transform damagePos;
	public float gravity;
	public float damageRadius;
	public float selfDestruckAfter;


	private Rigidbody2D rb;

	private float speed;
	private float travelDistance;
	private float xStartPos;

	private bool isGravityOn;
	private bool hasHitGround;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0f;
		rb.velocity = transform.right * speed;

		xStartPos = transform.position.x;
		isGravityOn = false;
	}

	private void Update()
	{
		if (!hasHitGround)
		{
			if (isGravityOn)
			{
				float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;        // Atan2 return angle from 2 lines
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);      // AngleAxis rotate base on the angle
			}
		}
	}

	private void FixedUpdate()
	{
		if (!hasHitGround)
		{
			Collider2D damageHit = Physics2D.OverlapCircle(damagePos.position, damageRadius, whatIsPlayer);
			Collider2D groundHit = Physics2D.OverlapCircle(damagePos.position, damageRadius, whatIsGround);

			if (damageHit)
			{
				damageHit.SendMessage("Damage", transform.position);
				Destroy(gameObject);
			}

			if (groundHit)
			{
				hasHitGround = true;
				rb.gravityScale = 0;
				rb.velocity = Vector2.zero;

				Destroy(gameObject, selfDestruckAfter);
			}


			if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
			{
				isGravityOn = true;
				rb.gravityScale = gravity;
			}

		}

	}

	public void FireProjectile(float speed, float travelDistance)
	{
		this.speed = speed;
		this.travelDistance = travelDistance;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(damagePos.position, damageRadius);
	}
}
