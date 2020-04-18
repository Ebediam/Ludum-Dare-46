using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    bool onCooldown = false;
    public float bouncyMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!onCooldown)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();

            if (player)
            {
                player.UnClampVerticalSpeed();
                player.rb.velocity = new Vector3(player.rb.velocity.x, player.rb.velocity.y*-bouncyMultiplier, player.rb.velocity.z);
                

                onCooldown = true;
                Invoke("EndCooldown", 0.5f);
            }
        }



    }


    void EndCooldown()
    {
        onCooldown = false;
    }
}
