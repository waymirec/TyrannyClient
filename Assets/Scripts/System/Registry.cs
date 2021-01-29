using UnityEngine;

namespace System
{
    public class Registry : MonoBehaviour
    {
        public static GameConfiguration Configuration => Singleton<GameConfiguration>.Instance;

        public static T Get<T>() where T : Singleton<T>
        {
            return Singleton<T>.Instance;
        }
    }
}