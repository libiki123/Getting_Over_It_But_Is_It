using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform hammer;
    public Transform body;
    public LayerMask whatIsGround;
    public Image cooldownSlider;

    public float hammerMoveSpeed = 0.2f;
    public float normalForce = 80f;
    public float superForce = 1000f;
    public float maxBodyVelocity = 10f;
    public float superJumpCdTime = 1f;

    private Rigidbody2D hammerRB;
    private DistanceJoint2D hammerDJ;
    private Rigidbody2D bodyRB;

    Vector3 mouseVec;

    private float superJumpTimer;
    private bool hasSuperJump;
    private bool enableSuperJump;

    void Start()
    {
        hammerRB = hammer.GetComponent<Rigidbody2D>();
        hammerDJ = hammer.GetComponent<DistanceJoint2D>();
        bodyRB = body.GetComponent<Rigidbody2D>();

        superJumpTimer = 0f;
    }

	private void Update()
	{
        if (!hasSuperJump)
        {
            superJumpTimer += Time.deltaTime;
            cooldownSlider.fillAmount = Mathf.Clamp(superJumpTimer / superJumpCdTime, 0, 1);
            if (superJumpTimer >= superJumpCdTime)
                hasSuperJump = true;

        }

		if (Input.GetKeyDown(KeyCode.Z))
		{
            enableSuperJump = true;
		}
        else if (Input.GetKeyUp(KeyCode.Z))
		{
            enableSuperJump = false;
        }


    }

	private void FixedUpdate()
	{
        float depth = Mathf.Abs(Camera.main.transform.position.z);                              // camera depth
        Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);       // mouse pos
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouseVec = Vector3.ClampMagnitude(mouse - body.position, hammerDJ.distance);            // Clamp mouse pos to the hammer max reach distance (Distance Joiny)

        UpdateBodyPosition();
        UpdateHammerRotation();
    }

    private void UpdateBodyPosition()
	{
        float bodyPushForce = normalForce;
        if (hammerRB.IsTouchingLayers(whatIsGround))                                            // If collided with ground
        {
            if (enableSuperJump && hasSuperJump)                                
            {
                bodyPushForce = superForce;
                superJumpTimer = 0f;
                hasSuperJump = false;
            }

            Vector3 targetBodyPos = hammer.position - mouseVec;                               // Update body pos
            Debug.DrawLine(hammer.position, targetBodyPos, Color.red);
            targetBodyPos = targetBodyPos - body.position;
            
            bodyRB.AddForce(targetBodyPos * bodyPushForce);
            bodyRB.velocity = Vector2.ClampMagnitude(bodyRB.velocity, maxBodyVelocity);
        }
    }

    private void UpdateHammerRotation()
	{
        Vector3 newHammerPos = body.position + mouseVec;                                            // New Hammer pos
        Vector3 hammerNewPosDicrection = newHammerPos - hammer.position;                            // Dicrection to the new pos
        newHammerPos = hammer.position + hammerNewPosDicrection * hammerMoveSpeed;                  // old to new pos with speed

        hammerRB.MovePosition(newHammerPos);                                                        // Update hammer pos
        hammer.rotation = Quaternion.FromToRotation(Vector3.right, newHammerPos - body.position);   // Update hammer rotation
    }
}
