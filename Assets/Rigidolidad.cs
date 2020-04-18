using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidolidad : MonoBehaviour
{
    public enum eje
    {
        Positiva,
        Negativa,
        Si,
        Hackeada,
    }

    [Header("Físicas de la rigibolidad")]
    public eje ejePreciso;
    public eje ejeImpreciso;
    public eje ejeRcicio;
    public eje ejeMplar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
