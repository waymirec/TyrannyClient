using UnityEngine;

namespace Tyranny.Client.System
{
    public class Registry : MonoBehaviour
    {
        public static Configuration Configuration => Singleton<Configuration>.Instance;

        public static T Get<T>() where T : Singleton<T>
        {
            return Singleton<T>.Instance;
        }
    }
}