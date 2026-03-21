using UnityEngine;

namespace JonathonOH.Unity.Singletons
{
	public static class MasterPrefabLoader
	{
		/// <summary>Loads the Systems prefab from Resources by name.</summary>
		/// <param name="prefabName">Name of the Systems prefab.</param>
		public static GameObject GetPrefab()
		{
			GameObject[] prefabs = Resources.LoadAll<GameObject>("");
			GameObject systemPrefab = null;

			foreach (GameObject prefab in prefabs)
			{
				if (!prefab.TryGetComponent(out SingletonMaster _starter)) continue;
				systemPrefab = prefab;
				break;
			}

			if (systemPrefab is null)
			{
				Debug.LogError($"[SystemsManagement][{typeof(MasterPrefabLoader).Name}] Could not find prefab in Resources folder with script {typeof(SingletonMaster).Name}");
			}
			return systemPrefab;
		}
	}
}
