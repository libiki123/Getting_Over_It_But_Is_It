using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
	Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2(Random.Range(1,-1), 5);
		rb.AddTorque(15 * Random.Range(1, -1), ForceMode2D.Impulse);
		Destroy(gameObject, 4f);
	}

}
