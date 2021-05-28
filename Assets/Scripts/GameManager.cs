using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Button resumeButton;
	private bool pause = false;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			resumeButton.gameObject.SetActive(true);
			Time.timeScale = 0;
		}


	}

	public void OnResumeClick()
	{
		Time.timeScale = 1;
		resumeButton.gameObject.SetActive(false);
	}


}
