using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static CheckPoint local;


    public void Start()
    {
        local = this;
        if (PlayerController.local)
        {
            if (PlayerController.local.data.checkPoint)
            {
                PlayerController.local.transform.position = transform.position;
                PlayerController.local.transform.rotation = transform.rotation;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();

        if (player)
        {
            player.data.checkPoint = true;
        }
    }

}
