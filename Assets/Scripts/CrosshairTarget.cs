using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    [SerializeField] Camera cam;

    Ray ray;
    RaycastHit hitInfo;

    private void Update()
    {
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;

        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = ray.origin + ray.direction * 1000f;
        }
    }
}
