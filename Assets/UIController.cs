using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static Image pointer;
    public Image pointerLocal;

    // Start is called before the first frame update
    void Start()
    {
        pointer = pointerLocal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
