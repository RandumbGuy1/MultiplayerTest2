using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float speed;

    [Header("Refrences")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FrameInput frameInput;

    private void OnNetworkInstantiate()
    {
        if (IsOwner) return;

        Destroy(this);
    }

    void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(frameInput.X, 0f, frameInput.Y);

        //Clamp Speed
        {
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float coefficientOfFriction = acceleration / speed;

            if (vel.sqrMagnitude > speed * speed) rb.AddForce(coefficientOfFriction * -vel);
        }

        rb.AddForce(moveDir * acceleration);
    }
}
