using Tyranny.Client.System;
using UnityEngine;

public class SystemSetup : MonoBehaviour
{
    private void Awake()
    {
        Registry.Get<Logging>();
    }
}
