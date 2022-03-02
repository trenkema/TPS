using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPLocomotion : MonoBehaviour
{
    [SerializeField] float animationDamp = 0.1f;

    Animator animator;

    Vector2 movementInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("InputX", movementInput.x, animationDamp, Time.fixedDeltaTime);
        animator.SetFloat("InputY", movementInput.y, animationDamp, Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
}
