using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] GameObject vfx;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(vfx, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

        Destroy(gameObject);
    }
}
