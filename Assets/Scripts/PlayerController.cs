using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform Gun;
    public Transform Hammer;
    public Transform body;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public LayerMask whatIsEnemy;

    [Space]
    public float fallingThreshold = -4f;
    public float maxBodyVelocity = 10f;

    [Header("Hammer")]
    public float hammerMoveSpeed = 0.2f;
    public float hammerForce = 80f;

    [Header("Gun")]
    public bool usingGun;
    public float recoilForce = 15f;
    public int bulletNum = 3;
    public Transform damgePos;
    public float damageRadius = 1f;

    [Header("Shell")]
    public Image[] shells;
    public Sprite loadShell;
    public Sprite emptyShell;
    public Vector2 aboveHeadOffset;

    [Header("Knockback")]
    public float knockbackDuration;
    public Vector2 knockbackSpeed;

    [HideInInspector] public Transform weapon;
    private Rigidbody2D weaponRB;
    private DistanceJoint2D weaponDJ;
    private Rigidbody2D bodyRB;

    private GameObject bulletShell;
    private Arm arm;

    private Vector3 mouseVec;

    private bool knockBack;
    private float knockbackStartTime;

    private bool isShooting;
    private bool isReloading;
    private int currentBulletNum;
    private float lastShotTime;

    void Start()
    {
        ChoseWeapon(usingGun);
        bodyRB = body.GetComponent<Rigidbody2D>();
        arm = GetComponent<Arm>();


        isShooting = false;
        isReloading = false;
        currentBulletNum = bulletNum;
    }

	private void Update()
	{
		if (!usingGun)
		{
           
		}

		if (Input.GetMouseButtonDown(0) && !isReloading && currentBulletNum > 0 && usingGun)
		{
            lastShotTime = Time.time;
            isShooting = true;
            AudioPlayer.Instance.PlaySound(SFX.Shot);
        }

        if (Time.time >= lastShotTime + 1.5f && currentBulletNum <= 2 && CheckGround())
		{
            Reload();
		}
            
        CheckKnockBack();
    }

	private void FixedUpdate()
	{
		float depth = Mathf.Abs(Camera.main.transform.position.z);                              // camera depth
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, depth);
        Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);       // mouse pos
        center = Camera.main.ScreenToWorldPoint(center);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
		mouseVec = Vector3.ClampMagnitude(mouse - center, weaponDJ.distance);                   // Clamp mouse pos to the hammer max reach distance (Distance Joiny)
                                                                                                // Use cam center to avoid sudden changes (like body)
		UpdateBodyPosition();
        UpdateHammerRotation();
        
    }

    void LateUpdate()
    {
        if(usingGun)
            bulletShell.transform.position = body.position + (Vector3)aboveHeadOffset;  // Move the healthUI base on the camera position
    }

    private void ChoseWeapon(bool useGun)
	{
        if (useGun)
        {
            weapon = Gun;
            Gun.gameObject.SetActive(true);
            Hammer.gameObject.SetActive(false);

            bulletShell = shells[0].transform.parent.gameObject;
            bulletShell.SetActive(true);
        }
        else
        {
            weapon = Hammer;
            Gun.gameObject.SetActive(false);
            Hammer.gameObject.SetActive(true);
        }

        weaponRB = weapon.GetComponent<Rigidbody2D>();
        weaponDJ = weapon.GetComponent<DistanceJoint2D>();
    }

    public void SwitchWeapon()
	{
        usingGun = !usingGun;
        ChoseWeapon(usingGun);
        arm.ChooseWeapon(usingGun);
	}

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, whatIsGround);
    }

    private void UpdateBodyPosition()
	{
		if (!usingGun)
		{
            if (weaponRB.IsTouchingLayers(whatIsGround))                                            // If collided with ground
            {
                if(bodyRB.velocity == Vector2.zero) AudioPlayer.Instance.PlaySound(SFX.Force);

                Vector3 targetBodyPos = weapon.position - mouseVec;                                 // getting body pos
                Debug.DrawLine(weapon.position, targetBodyPos, Color.red);
                targetBodyPos = targetBodyPos - body.position;
   
                bodyRB.velocity = Vector2.ClampMagnitude(bodyRB.velocity, maxBodyVelocity);         // limit the body velocity
 
                bodyRB.AddForce(targetBodyPos * hammerForce);

            }
		}
		else
		{
			if (isShooting)
			{
                float bodyPushForce;
                Vector3 targetBodyPos = body.position - weapon.position;

                if (currentBulletNum == 3)
                    bodyPushForce = recoilForce;
                else if(currentBulletNum == 2)
                    bodyPushForce = recoilForce * 1.2f;
                else
                    bodyPushForce = recoilForce * 1.5f;

                bodyRB.velocity = Vector3.zero;
                bodyRB.AddForce(targetBodyPos * bodyPushForce, ForceMode2D.Impulse);
                weapon.GetComponent<Animator>().SetBool("isShooting", true);
            }
		}

    }

    public void finishShooting()
	{
        weapon.GetComponent<Animator>().SetBool("isShooting", false);
        isShooting = false;
        currentBulletNum--;
        UpdateBulletUI();
        if (currentBulletNum == 0)
            Reload();
    }

	private void Reload()
	{
        if(!isReloading) AudioPlayer.Instance.PlaySound(SFX.Reload);

        isReloading = true;
        weapon.GetComponent<Animator>().SetBool("reload", true);
        
    }

    public void FinishReloading()
	{
        currentBulletNum = bulletNum;
        UpdateBulletUI();
        isReloading = false;
        weapon.GetComponent<Animator>().SetBool("reload", false);
    }

	private void UpdateHammerRotation()
	{
		Vector3 newHammerPos = body.position + mouseVec;                                            // New Hammer pos
		Vector3 hammerNewPosDicrection = newHammerPos - weapon.position;                            // Dicrection to the new pos
		newHammerPos = weapon.position + hammerNewPosDicrection * hammerMoveSpeed;                  // old to new pos with speed

		weaponRB.MovePosition(newHammerPos);                                                        // Update hammer pos
		weapon.rotation = Quaternion.FromToRotation(Vector3.right, newHammerPos - body.position);   // Update hammer rotation
	}

    public void KnockBack(int direction)
	{
        knockBack = true;
        knockbackStartTime = Time.time;
        bodyRB.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockBack()
    {
        if (Time.time > knockbackStartTime + knockbackDuration && knockBack)
        {
            knockBack = false;
            bodyRB.velocity = new Vector2(0.0f, bodyRB.velocity.y);     // stop the knockback
        }
    }

    void UpdateBulletUI()
	{
        for (int i = 0; i < bulletNum; i++)
        {
            if (i < currentBulletNum)
                shells[i].sprite = loadShell;
            else
                shells[i].sprite = emptyShell;
        }
    }

	private void OnDrawGizmos()
	{
        Gizmos.DrawWireSphere(damgePos.position + (Vector3)(Vector2.right * damageRadius), 0.2f);
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
	}
}
