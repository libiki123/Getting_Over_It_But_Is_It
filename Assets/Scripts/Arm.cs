using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

public class Arm : MonoBehaviour
{
	[Header("Hammer")]
	public Transform hammerLeftHandle;
	public Transform hammerRightHandle;

	[Header("Gun")]
	public Transform gunLeftHandle;
	public Transform gunRightHandle;

	[Header("Arm")]
	public Transform leftArmTarget;
	public Transform rightArmTarget;
	public Transform weaponVFX;
	public float journeyTime = 0.5f;
	

	private PlayerController lc;
	private LimbSolver2D  leftArmLimbSolver;
	private LimbSolver2D  rightArmLimbSolver;
	private bool usingGun = false;

	private Transform leftHandle;
	private Transform rightHandle;

	private bool onRight;
	private bool onLeft;

	private void Awake()
	{
		lc = GetComponent<PlayerController>();
		leftArmLimbSolver = leftArmTarget.parent.GetComponent<LimbSolver2D>();
		rightArmLimbSolver = rightArmTarget.parent.GetComponent<LimbSolver2D>();

		usingGun = lc.usingGun;

		ChooseWeapon(usingGun);
	}

	public void ChooseWeapon(bool useGun)
	{
		if (useGun)
		{
			leftArmLimbSolver.flip = true;
			rightArmLimbSolver.flip = true;

			leftHandle = gunLeftHandle;
			rightHandle = gunRightHandle;

			if (lc.weapon.position.x < lc.body.position.x)
			{
				onLeft = true;
				onRight = false;
				Flip();
			}
			else if (lc.weapon.position.x > lc.body.position.x)
			{
				onLeft = false;
				onRight = true;
			}
		}
		else
		{
			leftArmLimbSolver.flip = true;
			rightArmLimbSolver.flip = false;

			leftHandle = hammerLeftHandle;
			rightHandle = hammerRightHandle;
		}

		usingGun = lc.usingGun;
	}

	void Update()
	{
		
		if (lc.weapon.position.x < lc.body.position.x)
		{
			if (!onLeft && onRight && usingGun)
				Flip();

			leftArmTarget.position = Vector3.Lerp(leftArmTarget.position, rightHandle.position, journeyTime * Time.deltaTime);
			rightArmTarget.position = Vector3.Lerp(rightArmTarget.position, leftHandle.position, journeyTime * Time.deltaTime);
			onLeft = true;
			onRight = false;

		}
		else if (lc.weapon.position.x > lc.body.position.x)
		{
			if (onLeft && !onRight && usingGun)
				Flip();

			leftArmTarget.position = Vector3.Lerp(leftArmTarget.position, leftHandle.position, journeyTime * Time.deltaTime);
			rightArmTarget.position = Vector3.Lerp(rightArmTarget.position, rightHandle.position, journeyTime * Time.deltaTime);
			onLeft = false;
			onRight = true;
		}
	}

	void Flip()
	{
		leftArmLimbSolver.flip = !leftArmLimbSolver.flip;
		rightArmLimbSolver.flip = !rightArmLimbSolver.flip;
		weaponVFX.Rotate(180, 0, 0);
	}
}
