using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    Rigidbody rigidBody;

    public float bulletSpeed = 15f;

    public AudioClip BulletHitAudio;

    public GameObject bulletImpactEffect;

    public int damage = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        transform.forward = originalDirection;
        rigidBody.linearVelocity = transform.forward * bulletSpeed;
    }

    private void onCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);

        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);

        Destroy(gameObject); 
    }

}
