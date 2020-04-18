using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        if (player)
        {
            GameManager.NextLevel();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
