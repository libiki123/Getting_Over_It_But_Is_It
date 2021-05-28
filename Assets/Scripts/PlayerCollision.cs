using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] PlayerController pc;

    private void Damage(Vector3 attkPos)
    {
        int direction = attkPos.x < transform.position.x ? 1 : -1;
        pc.KnockBack(direction);
    }
}
