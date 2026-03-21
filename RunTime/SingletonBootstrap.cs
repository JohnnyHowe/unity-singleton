using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.Unity.Singletons
{
	internal class SingletonBootstrap
	{
		public HashSet<ISingleton> Failed { get; private set; }
		public readonly HashSet<ISingleton> AllSystems;
		public readonly List<ISingleton> SystemsInInitialzationOrder;

		public SingletonBootstrap(Transform root)
		{
			Failed = new HashSet<ISingleton>();

			AllSystems = new HashSet<ISingleton>(GetAllChildSingletons(root));
			Debug.Log("Found Singletons:\n - " + string.Join("\n - ", AllSystems));

			SystemsInInitialzationOrder = new SingletonInitializationOrderSorter(AllSystems).GetOrderedByDependencies().ToList();
			Debug.Log("Ordered Singletons:\n - " + string.Join("\n - ", SystemsInInitialzationOrder));
		}

		public void StartSingletons()
		{
			foreach (ISingleton singleton in SystemsInInitialzationOrder)
			{
				TryStart(singleton);
			}
		}

		private IEnumerable<ISingleton> GetAllChildSingletons(Transform root)
		{
			foreach (Transform child in root)
			{
				if (!child.gameObject.activeInHierarchy) continue;
				if (!child.TryGetComponent(out ISingleton system)) continue;
				if (system.Initialized) continue;
				if (system.Awoken) continue;
				if (Failed.Contains(system)) continue;
				yield return system;
			}
		}

		private void TryStart(ISingleton singleton)
		{
			singleton.SetInstance();
			try
			{
				singleton.Initialize();
			}
			catch (Exception error)
			{
				Debug.LogError($"Could not instantiate {singleton.GetType().Name}: {error}");
				return;
			}

			if (!singleton.Initialized)
			{
				string typeName = singleton.GetType().Name;
				Debug.LogError($"Could not instantiate {typeName}: {typeName}.IsInitialized() returned false!");
				Failed.Add(singleton);
				return;
			}

			Debug.Log($"Successfully initialized {singleton.GetType().Name}");
		}
	}
}
