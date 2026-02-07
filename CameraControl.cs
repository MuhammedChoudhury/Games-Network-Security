using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class CameraControl : MonoBehaviour
{

    public Transform playerCharacter;

    Vector3 cameraOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraOffset = transform.position - playerCharacter.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerCharacter.position + cameraOffset;
    }
}
