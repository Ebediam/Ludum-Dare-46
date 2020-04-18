using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform spawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player)
        {
            player.data.levelFailed = true;
            GameManager.RestartGame();
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
