using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
	internal class GameSystemStarter
	{
		public HashSet<IGameSystem> Failed { get; private set; }
		public readonly HashSet<IGameSystem> AllSystems;
		public readonly List<IGameSystem> SystemsInInitialzationOrder;

		public GameSystemStarter(Transform root)
		{
			Failed = new HashSet<IGameSystem>();

			AllSystems = new HashSet<IGameSystem>(GetAllGameSystems(root));
			Debug.Log("Found GameSystems:\n - " + string.Join("\n - ", AllSystems));

			SystemsInInitialzationOrder = new GameSystemInitializationOrderSorter(AllSystems).GetOrderedByDependencies().ToList();
			Debug.Log("Ordered GameSystems:\n - " + string.Join("\n - ", SystemsInInitialzationOrder));
		}

		public void Initialize()
		{
			foreach (IGameSystem gameSystem in SystemsInInitialzationOrder)
			{
				TryInstantiate(gameSystem);
			}
		}

		private IEnumerable<IGameSystem> GetAllGameSystems(Transform root)
		{
			foreach (Transform child in root)
			{
				if (!child.gameObject.activeInHierarchy) continue;
				if (!child.TryGetComponent(out IGameSystem system)) continue;
				if (system.Initialized) continue;
				if (system.Awoken) continue;
				if (Failed.Contains(system)) continue;
				yield return system;
			}
		}

		private void TryInstantiate(IGameSystem gameSystem)
		{
			gameSystem.Initialize();
			try
			{
				gameSystem.AwakeSystem();
			}
			catch (Exception error)
			{
				Debug.LogError($"Could not instantiate {gameSystem.GetType().Name}: {error}");
				return;
			}

			if (!gameSystem.Initialized)
			{
				string typeName = gameSystem.GetType().Name;
				Debug.LogError($"Could not instantiate {typeName}: {typeName}.IsInitialized() returned false!");
				Failed.Add(gameSystem);
				return;
			}

			Debug.Log($"Successfully initialized {gameSystem.GetType().Name}");
		}
	}
}
