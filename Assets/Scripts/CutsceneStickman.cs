using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneStickman : MonoBehaviour
{
    public LevelLoader levelLoader;

    private void OnAnimationFinish()
	{
		levelLoader.LoadNextLevel();
	}

	private void OnEndGame()
	{
		levelLoader.loadEndGame();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			levelLoader.LoadNextLevel();
		}
	}
}
