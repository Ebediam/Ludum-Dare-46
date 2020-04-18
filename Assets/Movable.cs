using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public Rigidbody rb;
    // Start is called before the first frame update
    public void Move(Vector3 targetPosition, bool towards)
    {

        Vector3 direction = targetPosition - transform.position;

        if (!towards)
        {
            direction *= -1f;
        }
        rb.AddForce(direction.normalized*12f, ForceMode.VelocityChange);    
    }

}
