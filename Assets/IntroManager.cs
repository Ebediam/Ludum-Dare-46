using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public Door firstDoor;
    public Door secondDoor;

    public Door ceilingDoor;

    public Door fakeDog;

    public PlayerController player;
    public UIController UIController;
    public GameObject hook;
    public PlayerListener hookGrab;
    

    bool hookGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {

        UIController.HideUI();
        player.canMove = false;
        player.canShoot = false;
        Invoke("OpenCeilingDoor", 1f);


 
    }

    public void OpenCeilingDoor()
    {
        ceilingDoor.OpenDoor(); 
        Invoke("LowDog", 1f);
    }


    public void LowDog()
    {
        fakeDog.OpenDoor();
        Invoke("RaiseDog", 6f);

    }

    public void RaiseDog()
    {
        fakeDog.CloseDoor();
        Invoke("CloseCeilingDoor", 3f);
    }

    public void CloseCeilingDoor()
    {
        ceilingDoor.CloseDoor();
        Invoke("OpenFirstDoor", 2f);
    }

    public void OpenFirstDoor()
    {
        firstDoor.OpenDoor();
        player.canMove = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (!hookGrabbed)
        {
            if (hookGrab.active)
            {
                player.hook.gameObject.SetActive(true);
                hook.SetActive(false);
                hookGrabbed = true;
                secondDoor.OpenDoor();
                player.canShoot = true;

            }
        }


    }
}
