using System;
using UnityEngine;

namespace Tyranny.Client.Events
{
    public class WorldAuthEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
    }

    public class EnterWorldEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Vector3 Position { get; set; }
    }

    public class SpawnEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Vector3 Position { get; set; }
    }
}
