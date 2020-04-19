using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform openPosion;
    public Transform closedPosion;
    public Transform doorTransform;
    public float timeToOpen;
    float timer = 0f;

    bool isOpening = false;
    bool isClosing = false;


    public enum DoorState
    {
        Closed,
        Open
    }

    public DoorState doorState;

    // Start is called before the first frame update
    void Start()
    {
        switch (doorState)
        {
            case DoorState.Closed:
                doorTransform.position = closedPosion.position;
                break;

            case DoorState.Open:
                doorTransform.position = openPosion.position;
                break;
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (isOpening)
        {
            timer += Time.deltaTime;
            doorTransform.position = Vector3.Lerp(closedPosion.position, openPosion.position, timer / timeToOpen);

            if(timer >= timeToOpen)
            {
                doorState = DoorState.Open;
                isOpening = false;
                timer = 0f;
            }

        }else if (isClosing)
        {
            timer += Time.deltaTime;

            doorTransform.position = Vector3.Lerp(openPosion.position, closedPosion.position, timer / timeToOpen);

            if(timer >= timeToOpen)
            {
                doorState = DoorState.Closed;
                isClosing = false;
                timer = 0f;
            }


        }




    }

    public void OpenDoor()
    {
        if(doorState != DoorState.Open)
        {
            isOpening = true;
        }
        
    }

    public void CloseDoor()
    {
        if(doorState != DoorState.Closed)
        {
            isClosing = true;
        }
    }

}
