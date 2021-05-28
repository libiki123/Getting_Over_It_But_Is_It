using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerController pc;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.gameObject.tag == "Player")
		{
			pc.SwitchWeapon();
			AudioPlayer.Instance.PlayerBackgroundMusic();

			Destroy(gameObject);
		}
	}
}
