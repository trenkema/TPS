using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem[] muzzleFlash;

    [SerializeField] ParticleSystem hitEffect;

    [SerializeField] TrailRenderer tracerEffect;

    [SerializeField] Transform raycastOrigin;

    public WeaponSlot weaponSlot;

    public string weaponName;

    public Transform raycastDestination;

    public MeshCollider weaponCollider;

    [Header("Settings")]
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] float bulletSpeed = 1000f;
    [SerializeField] float bulletDrop = 0f;
    [SerializeField] float maxLifetime = 3f;

    [SerializeField] bool canHold = true;

    bool readyToFire = true;

    bool isFiring = false;

    List<BulletNew> bullets = new List<BulletNew>();

    Ray ray;

    RaycastHit hitInfo;


    private void Update()
    {
        MyInput();
    }

    private void LateUpdate()
    {
        UpdateBullets();
    }

    private void MyInput()
    {
        if (readyToFire && isFiring)
        {
            Fire();
        }
    }

    public void StartFiring()
    {
        isFiring = true;
    }
    
    public void StopFiring()
    {
        isFiring = false;
    }

    private void ResetShot()
    {
        readyToFire = true;

        if (!canHold && isFiring)
            isFiring = false;
    }

    private void UpdateBullets()
    {
        SimulateBullets();

        DestroyBullets();
    }

    private void SimulateBullets()
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetBulletPosition(bullet);
            bullet.time += Time.deltaTime;
            Vector3 p1 = GetBulletPosition(bullet);

            RaycastSegment(p0, p1, bullet);
        });
    }

    private void RaycastSegment(Vector3 start, Vector3 end, BulletNew bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time > maxLifetime);
    }

    private void Fire()
    {
        readyToFire = false;

        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;

        var bullet = CreateBullet(raycastOrigin.position, velocity);

        bullets.Add(bullet);

        Invoke("ResetShot", timeBetweenShots);
    }

    private Vector3 GetBulletPosition(BulletNew bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;

        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    private BulletNew CreateBullet(Vector3 position, Vector3 velocity)
    {
        BulletNew bullet = new BulletNew();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }
}

[System.Serializable]
public class BulletNew
{
    public float time;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public TrailRenderer tracer;
}