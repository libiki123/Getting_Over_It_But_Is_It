using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSpawner : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject shell;

    private void SpawnShell()
	{
        GameObject s = Instantiate(shell, spawnPos.position, Quaternion.identity);
	}
}
