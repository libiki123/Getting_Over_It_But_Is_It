using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;

	private void Update()
	{
		transform.position = Vector3.Lerp(pos1.position, pos2.position, Mathf.PingPong(Time.time * speed, 1f));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.gameObject.tag == "Player")
		{
			collision.collider.transform.SetParent(transform);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.collider.gameObject.tag == "Player")
		{
			collision.collider.transform.SetParent(null);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(pos1.position, pos2.position);
	}
}
