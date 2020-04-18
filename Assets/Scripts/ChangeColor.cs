using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{

    public Material newColor;


     

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer myMesh = gameObject.GetComponent<MeshRenderer>();

        myMesh.material = newColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
