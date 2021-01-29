using System;
using Managers;
using UnityEngine;

public class SystemSetup : MonoBehaviour
{
    private void Awake()
    {
        Registry.Get<Logging>();
        Registry.Get<NetworkEventManager>();
        Registry.Get<WorldEntityManager>();
        Registry.Get<GameManager>();
    }
}
