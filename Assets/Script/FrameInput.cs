using UnityEngine;
using Unity.Netcode;

public class FrameInput : NetworkBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    public KeyCode JumpKey => jumpKey;

    public bool Jumping { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }

    public delegate void RecieveFrameInput(FrameInput input);
    public event RecieveFrameInput OnFrameInput;

    private void OnNetworkInstantiate()
    {
        if (IsOwner) return;

        Destroy(this);
    }

    void Update()
    {
        X = Input.GetAxisRaw("Horizontal");
        Y = Input.GetAxisRaw("Vertical");
        Jumping = Input.GetKeyDown(jumpKey);

        OnFrameInput?.Invoke(this);
    }
}
