using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFixer : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    Material material;
    //public Color tileColor;
    //public Color lineColor;


    public enum Type
    {
        Wall,
        Floor
    }

    public Type surfaceType;

    //_zFlooryWallTileDensity
    //_xFloorzWallTileDensity


    void Start()
    {


        material = Instantiate(meshRenderer.material);
        meshRenderer.material = material;

        /*material.SetColor("_baseTileColor", tileColor);
        material.SetColor("_lineTileColor", lineColor);*/

        material.SetInt("_zFlooryWallTileDensity",  Mathf.RoundToInt(5 * transform.lossyScale.z));
        material.SetInt("_xFloorzWallTileDensity",  Mathf.RoundToInt(5 * transform.lossyScale.x));


        /*switch (surfaceType)
        {
            case Type.Floor:
                material.SetInt("_zFlooryWallTileDensity", 5 * Mathf.RoundToInt(transform.lossyScale.z));
                material.SetInt("_xFloorzWallTileDensity", 5 * Mathf.RoundToInt(transform.lossyScale.x));
                break;


            case Type.Wall:
                material.SetInt("_zFlooryWallTileDensity", 5 * Mathf.RoundToInt(transform.lossyScale.y));
                material.SetInt("_xFloorzWallTileDensity", 5 * Mathf.RoundToInt(transform.lossyScale.z));


                break;
        }*/


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
