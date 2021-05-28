using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	private enum State { Moving, Idle, Dead, PlayerDetected, LookForPlayer, MeleeAttack, RangeAttack }

	private State currentState;
	private Vector2 movement;

	private float startTime;
	protected bool isAnimationFinished;

	// Idle State Data
	protected bool flipAfterIdle;
	protected bool isIdleTimeOver;
	protected float idleTIme;

	// Look For Player Data
	protected bool turnImmediately;
	protected bool isAllTurnDone;
	protected bool isAllTurnTimeDone;
	protected float lastTurnTime;
	protected int amountOfTurnDone;

	// Range Attack Data
	protected GameObject projectileContainer;
	protected Projectile projectileScript;

	//heal bar
	private Transform healBar;
	private Image healSlider;

	[SerializeField] private Transform meleeAttackPos;
	[SerializeField] private Transform rangedAttackPos;
	[SerializeField] private EnemyCollision coll;

	[Header("Movement")]
	public float movementSpeed;

	[Header("IdleStateData")]
	public float minIdleTime = 1f;
	public float maxIdleTime = 2f;

	[Header("PlayerDetectedStateData")]
	public float longRangeActionTime = 1.5f;

	[Header("LookForPlayerStateData")]
	public int amountOfTurn = 2;
	public float timeBetweenTurns = 0.75f;

	[Header("RangeAttackStateData")]
	public GameObject projectile;
	public float projectileSpeed = 12f;
	public float projectileTravelDistance = 5f;

	[Header("Particles")]
	public GameObject hitParticle;
	public GameObject deathChunkParticle;
	public GameObject deathBloodParticle;

	[Header("Other")]
	public Rigidbody2D bodyRb;
    public Animator anim;
    public GameObject body;
    public int facingDirection;
    public int lastDamageDirection;
    public float attackRadius = 0.5f;

	private void Start()
	{
		facingDirection = 1;

		startTime = Time.time;
		currentState = State.Idle;
		EnterIdleState();
	}

	private void Update()
	{
		UpdateStates();

	}

	#region MoveState
	//-------------------------- Moving State --------------------------------//

	private void EnterMovingState()
	{
		anim.SetBool("move", true);
	}

	private void UpdateMovingState()
	{
		SetVelocity(movementSpeed);

		if (coll.CheckPlayerInMinArgoRange())
		{
			SwitchState(State.PlayerDetected);
		}
		else if (coll.CheckWall() || !coll.CheckLedge())
		{
			SetFlipAfterIdle(true);
			SwitchState(State.Idle);
		}

	}

	private void ExistMovingState()
	{
		anim.SetBool("move", false);
	}

	#endregion

	#region IdleState
	//-------------------------- Idle State --------------------------------//

	private void EnterIdleState()
	{
		anim.SetBool("idle", true);
		SetVelocity(0f);
		isIdleTimeOver = false;
		SetRandomIdleTime();
	}

	private void UpdateIdleState()
	{
		if (Time.time >= startTime + idleTIme)
			isIdleTimeOver = true;

		if (coll.CheckPlayerInMinArgoRange())
		{
			SwitchState(State.PlayerDetected);
		}
		else if (isIdleTimeOver)
		{
			SwitchState(State.Moving);
		}
	}

	private void ExistIdleState()
	{
		if (flipAfterIdle) Flip();
		anim.SetBool("idle", false);
	}

	public void SetFlipAfterIdle(bool flip)
	{
		flipAfterIdle = flip;
	}

	public void SetRandomIdleTime()
	{
		idleTIme = UnityEngine.Random.Range(minIdleTime, maxIdleTime);

	}

	#endregion

	#region PlayerDetectedState
	//-------------------------- Player Detected State --------------------------------//
	private void EnterPlayerDetectedState()
	{
		anim.SetBool("playerDetected", true);
		SetVelocity(0f);
	}
	private void UpdatePlayerDetectedState()
	{

		if (coll.CheckPlayerInCloseRangeAction())
		{
			SwitchState(State.MeleeAttack);

		}
		else if (Time.time >= startTime + longRangeActionTime)
		{
			SwitchState(State.RangeAttack);
		}
		else if (!coll.CheckPlayerInMaxArgoRange())
		{
			SwitchState(State.LookForPlayer);
		}
	}
	private void ExitPlayerDetectedState()
	{
		anim.SetBool("playerDetected", false);
	}

	#endregion

	#region LookForPlayerState
	//-------------------------- Look For Player State --------------------------------//

	private void EnterLookForPlayerState()
	{
		anim.SetBool("lookForPlayer", true);
		isAllTurnDone = false;
		isAllTurnTimeDone = false;
		lastTurnTime = startTime;
		amountOfTurnDone = 0;
		SetVelocity(0f);
	}

	private void UpdateLookForPlayerState()
	{
		if (turnImmediately)
		{
			Flip();
			lastTurnTime = Time.time;
			amountOfTurnDone++;
			turnImmediately = false;
		}
		else if (Time.time >= lastTurnTime + timeBetweenTurns && !isAllTurnDone)      // it time to turn and theres still turns left
		{
			Flip();
			lastTurnTime = Time.time;
			amountOfTurnDone++;
		}

		if (amountOfTurnDone >= amountOfTurn)
		{
			isAllTurnDone = true;
		}

		if (Time.time >= lastTurnTime + timeBetweenTurns && isAllTurnDone)
		{
			isAllTurnTimeDone = true;
		}

		if (coll.CheckPlayerInMinArgoRange())
		{
			SwitchState(State.PlayerDetected);
		}
		else if (isAllTurnTimeDone)
		{
			SwitchState(State.Moving);
		}
	}

	private void ExitLookForPlayerState()
	{
		anim.SetBool("lookForPlayer", false);
	}

	public void SetTurnImmediately(bool flip)
	{
		turnImmediately = flip;
	}

	#endregion

	#region RangeAttackState
	//-------------------------- Range Attack State --------------------------------//

	private void EnterRangeAttackState()
	{
		anim.SetBool("rangeAttack", true);
		isAnimationFinished = false;
	}

	private void UpdateRangeAttackState()
	{
		if (isAnimationFinished)
		{
			if (coll.CheckPlayerInCloseRangeAction())
			{
				SwitchState(State.MeleeAttack);
			}
			else if (coll.CheckPlayerInMinArgoRange())
			{
				SwitchState(State.PlayerDetected);
			}
			else
			{
				SwitchState(State.LookForPlayer);
			}
		}
	}

	private void ExitRangeAttackState()
	{
		anim.SetBool("rangeAttack", false);
	}

	#endregion

	#region MeleeAttackState
	//-------------------------- Melee Attack State --------------------------------//

	private void EnterMeleeAttackState()
	{
		anim.SetBool("meleeAttack", true);
		isAnimationFinished = false;
	}

	private void UpdateMeleeAttackState()
	{
		if (isAnimationFinished)
		{
			if (coll.CheckPlayerInMinArgoRange())
			{
				SwitchState(State.PlayerDetected);
			}
			else if (!coll.CheckPlayerInMinArgoRange())
			{
				SwitchState(State.LookForPlayer);
			}
		}
	}


	private void ExitMeleeAttackState()
	{
		anim.SetBool("meleeAttack", false);
	}

	#endregion

	#region DeadState

	//-------------------------- Dead State --------------------------------//

	private void EnterDeadState()
	{
		//Instantiate(deathChunkParticle, alive.transform.position, deathChunkParticle.transform.rotation);
		//Instantiate(deathBloodParticle, alive.transform.position, deathBloodParticle.transform.rotation);

		Destroy(gameObject);
	}

	private void UpdateDeadState()
	{

	}

	private void ExistDeadState()
	{

	}

	#endregion

	public void TriggerAttack()
	{
		if (currentState == State.MeleeAttack)
		{
			Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(meleeAttackPos.position, attackRadius, coll.whatIsPlayer);

			foreach (Collider2D collider in detectedObjects)
			{
				collider.transform.SendMessage("Damage", body.transform.position);
			}
		}
		else if (currentState == State.RangeAttack)
		{
			projectileContainer = GameObject.Instantiate(projectile, rangedAttackPos.position, rangedAttackPos.rotation);
			projectileScript = projectileContainer.GetComponent<Projectile>();
			projectileScript.FireProjectile(projectileSpeed, projectileTravelDistance);
		}

	}
	public void FinishAttack()
	{
		isAnimationFinished = true;
	}

	public virtual void SetVelocity(float velocity)
	{
		movement.Set(facingDirection * velocity, bodyRb.velocity.y);
		bodyRb.velocity = movement;
	}

	public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
	{
		angle.Normalize();
		movement.Set(angle.x * velocity * direction, angle.y * velocity);
		bodyRb.velocity = movement;
	}

	private void SwitchState(State state)
	{
		switch (currentState)
		{
			case State.Moving:
				ExistMovingState();
				break;
			case State.Idle:
				ExistIdleState();
				break;
			case State.PlayerDetected:
				ExitPlayerDetectedState();
				break;
			case State.LookForPlayer:
				ExitLookForPlayerState();
				break;
			case State.MeleeAttack:
				ExitMeleeAttackState();
				break;
			case State.RangeAttack:
				ExitRangeAttackState();
				break;
			case State.Dead:
				ExistDeadState();
				break;
		}

		startTime = Time.time;
		currentState = state;

		switch (state)
		{
			case State.Moving:
				EnterMovingState();
				break;
			case State.Idle:
				EnterIdleState();
				break;
			case State.PlayerDetected:
				EnterPlayerDetectedState();
				break;
			case State.LookForPlayer:
				EnterLookForPlayerState();
				break;
			case State.MeleeAttack:
				EnterMeleeAttackState();
				break;
			case State.RangeAttack:
				EnterRangeAttackState();
				break;
			case State.Dead:
				EnterDeadState();
				break;
		}
	}

	private void UpdateStates()
	{
		switch (currentState)
		{
			case State.Moving:
				UpdateMovingState();
				break;
			case State.Idle:
				UpdateIdleState();
				break;
			case State.PlayerDetected:
				UpdatePlayerDetectedState();
				break;
			case State.LookForPlayer:
				UpdateLookForPlayerState();
				break;
			case State.MeleeAttack:
				UpdateMeleeAttackState();
				break;
			case State.RangeAttack:
				UpdateRangeAttackState();
				break;
			case State.Dead:
				UpdateDeadState();
				break;
		}
	}

	public virtual void Flip()
    {
        facingDirection = -facingDirection;
        body.transform.Rotate(0.0f, 180f, 0.0f);
    }



    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(meleeAttackPos.position, attackRadius);
    }
}
