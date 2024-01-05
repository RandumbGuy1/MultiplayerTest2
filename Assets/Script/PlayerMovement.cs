using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float speed;
    [Space(10)]
    [SerializeField] private float friction;
    [SerializeField] private int counterThresold;
    private Vector2Int readyToCounter = Vector2Int.zero;

    public Vector3 RelativeVel { get; private set; }

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
        RelativeVel = rb.velocity;

        Friction();
        //Clamp Speed
        {
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float coefficientOfFriction = acceleration / speed;

            if (vel.sqrMagnitude > speed * speed) rb.AddForce(coefficientOfFriction * -vel * 0.9f, ForceMode.Impulse);
        }

        rb.AddForce(moveDir * acceleration, ForceMode.Impulse);
    }

    private void Friction()
    {
        if (frameInput.Jumping) return;

        Vector3 frictionForce = Vector3.zero;

        if (Mathf.Abs(RelativeVel.x) > 0f && frameInput.X == 0f && readyToCounter.x > counterThresold) frictionForce -= Vector3.right * RelativeVel.x;
        if (Mathf.Abs(RelativeVel.z) > 0f && frameInput.Y == 0f && readyToCounter.y > counterThresold) frictionForce -= Vector3.forward * RelativeVel.z;

        if (CounterMomentum(frameInput.X, RelativeVel.x)) frictionForce -= Vector3.right * RelativeVel.x;
        if (CounterMomentum(frameInput.Y, RelativeVel.z)) frictionForce -= Vector3.forward * RelativeVel.z;

        frictionForce = Vector3.ProjectOnPlane(frictionForce, Vector3.up);
        if (frictionForce != Vector3.zero) rb.AddForce(0.2f * friction * acceleration * frictionForce);

        readyToCounter.x = frameInput.X == 0f ? readyToCounter.x + 1 : 0;
        readyToCounter.y = frameInput.Y == 0f ? readyToCounter.y + 1 : 0;
    }
    private bool CounterMomentum(float input, float mag, float threshold = 0.05f)
        => input > 0 && mag < -threshold || input < 0 && mag > threshold;
}
