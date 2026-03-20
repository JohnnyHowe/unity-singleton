using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
    public static class SystemsPrefabLoader
    {
        /// <summary>Loads the Systems prefab from Resources by name.</summary>
        /// <param name="prefabName">Name of the Systems prefab.</param>
        public static GameObject GetPrefab(string prefabName)
        {
            GameObject[] prefabs = Resources.LoadAll<GameObject>("");
            GameObject systemPrefab = null;

            foreach (GameObject prefab in prefabs)
            {
                if (prefab.name != prefabName) continue;
                systemPrefab = prefab;
                break;
            }

            if (systemPrefab is null) Debug.LogError($"[SystemsManagement][{typeof(SystemsPrefabLoader).Name}] Could not find prefab: " + prefabName);
            return systemPrefab;
        }
    }
}
