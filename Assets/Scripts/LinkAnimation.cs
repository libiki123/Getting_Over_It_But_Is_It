using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkAnimation : MonoBehaviour
{
	public Enemy enemy;
	public PlayerController player;

	private void TriggerAttack()
	{
		if(enemy != null)
			enemy.TriggerAttack();
	}

	private void FinishAttack()
	{
		if (enemy != null)
			enemy.FinishAttack();
	}

	private void FinishShooting()
	{
		if (player != null)
			player.finishShooting();
	}

	private void FinishReloading()
	{
		if (player != null)
			player.FinishReloading();
	}

}
