using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("Doors")]
    public Door firstDoor;
    public Door secondDoor;
    public Door thirdDoor;
    public Door fourthDoor;
    public Door fifthDoor;
    public Door ceilingDoor;
    public Door fakeDog;


    [Header("PlayerListeners")]
    public PlayerListener hookGrabListener;
    public PlayerListener secondDoorListener;
    public PlayerListener thirdDoorListener;
    public PlayerListener fourthDoorListener;

    [Header("Others")]
    public PlayerController player;
    public UIController UIController;
    public GameObject hook;

    public AudioSource speaker;
    public LinesData linesData;

    bool hookGrabbed = false;

    int currentLine = 0;

    bool playerInDogRoom;
    bool dialogeReady;

    bool frozenPlayer;

    // Start is called before the first frame update
    void Start()
    {
        UIController.HideUI();
        player.canMove = false;
        player.canShoot = false;
        AddNewLine();

        Invoke("OpenFirstDoor", speaker.clip.length);

        

        


 
    }

    public void OpenFirstDoor()
    {
        firstDoor.OpenDoor();
        player.canMove = true;        
    }



    public void AddNewLine() 
    {
        speaker.clip = linesData.lines[currentLine];
        currentLine++;
        if (currentLine < 6)
        {
            Invoke("AddNewLine", speaker.clip.length);
        }
        else if(currentLine == 6)
        {
            dialogeReady = true;
        }
        else if(currentLine == 7)
        {

            Invoke("AddNewLine", speaker.clip.length);
        }
        else if(currentLine == 8)
        {
            RaiseDog();
            Invoke("AddNewLine", speaker.clip.length);
        }
        else if(currentLine == 9)
        {            
            Invoke("AddNewLine", speaker.clip.length);
            fourthDoor.OpenDoor();
            player.canMove = true;
            player.canShoot = true;
        }
        else if(currentLine == 10)
        {
            Invoke("OpenFifthDoor", speaker.clip.length);

        }


        

        speaker.Play();

    }

    public void OpenCeilingDoor()
    {
        ceilingDoor.OpenDoor(); 
        Invoke("LowDog", 1f);
    }


    public void OpenFifthDoor()
    {
        fifthDoor.OpenDoor();
    }

    public void LowDog()
    {
        fakeDog.OpenDoor();
    }

    public void RaiseDog()
    {
        fakeDog.CloseDoor();
        Invoke("CloseCeilingDoor", 3f);
    }

    public void CloseCeilingDoor()
    {
        ceilingDoor.CloseDoor();
    }



    // Update is called once per frame
    void Update()
    {

        if (!hookGrabbed)
        {
            if (hookGrabListener.active)
            {
                player.hook.gameObject.SetActive(true);
                hook.SetActive(false);
                hookGrabbed = true;
                player.canShoot = true;

            }
        }


        if (secondDoorListener.active)
        {
            secondDoor.OpenDoor();
        }


        if (!fourthDoorListener.active)
        {
            if (thirdDoorListener.active)
            {
                thirdDoor.OpenDoor();
            }
        }
        else
        {
            thirdDoor.CloseDoor();
            if (!frozenPlayer)
            {
                player.canMove = false;
                player.canShoot = false;
                frozenPlayer = true;
            }
            playerInDogRoom = true;
  
        }

        if(playerInDogRoom && dialogeReady)
        {
            
            dialogeReady = false;
            AddNewLine();
            OpenCeilingDoor();

        }

    }
}
