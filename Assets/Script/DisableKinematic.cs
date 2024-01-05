using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableKinematic : NetworkBehaviour
{
    private void OnNetworkInstantiate()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
