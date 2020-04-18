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

    [Header("Air movement settings")]
    public float maxAirHorizontalSpeed;
    public float maxAirVerticalSpeed;
    public float airAcceleration;
    public float velocityDamper;

    [Header("Cable settings")]
    public float pullForce;
    public float cableTension;
    public float cableLaunchSpeed;
    public float maxCableLength;
    public float cablePullSpeed;
    public float cableReleaseSpeed;
    public LayerMask attachableLayer;



    [Header("Modifiers")]
    public float movementBoost;

    [Header("Settings")]
    public bool completedLevel;
    public LayerMask groundLayer;

    [Header("SFX")]
    public AudioClip jumpSFX;
    public AudioClip shotSFX;
    public AudioClip pullSFX;
    public AudioClip landSFX;


}
