using UnityEngine;

namespace UnityRPG.VFX
{
    public static class VfxSpawner
    {
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return null;

            return Object.Instantiate(prefab, position, rotation);
        }

        public static GameObject Spawn(GameObject prefab, Transform point)
        {
            if (prefab == null || point == null)
                return null;

            return Spawn(prefab, point.position, point.rotation);
        }
    }
}