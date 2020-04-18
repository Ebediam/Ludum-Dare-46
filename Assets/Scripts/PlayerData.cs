using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [Header("Camera settings")]
    public float xMouseSensitivity;
    public float yMouseSensitivity;


    [Header("Ground movement settings")]
    public float maxSpeed;
    public float acceleration;
    public float jumpForce;
    public LayerMask groundLayer;

    [Header("Air movement settings")]
    public float maxAirHorizontalSpeed;
    public float maxAirVerticalSpeed;
    public float airAcceleration;
    public float velocityDamper;
    public float airBoostForce;
    public float cableReleaseBoost;



    [Header("Cable settings")]
    public float pullForce;
    public float cableTension;
    public float cableLaunchSpeed;
    public float maxCableLength;
    public float minCableLenght;
    public float cablePullSpeed;
    public float cableReleaseSpeed;
    public LayerMask attachableLayer;
    public float maxCableActivationDelay;
    public float undeattachableTimer;



    [Header("Modifiers")]
    public float movementBoost;

    [Header("Persistent info")]
    public bool levelFailed;
    public bool checkPoint;


    [Header("SFX")]
    public AudioClip jumpSFX;
    public AudioClip shotSFX;
    public AudioClip pullSFX;
    public AudioClip landSFX;
    public AudioClip airBoostSFX;
    public AudioClip deathSFX;


}
