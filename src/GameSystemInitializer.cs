using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
	internal class GameSystemInitializer
	{
		public HashSet<IGameSystem> Failed { get; private set; }
		public readonly HashSet<IGameSystem> AllSystems;
		public readonly List<IGameSystem> SystemsInInitialzationOrder;
		private List<IGameSystem> _systemsLeftToInitialize;
		private bool _verbose;

		public GameSystemInitializer(Transform root, bool verbose = false)
		{
			_verbose = verbose;
			Failed = new HashSet<IGameSystem>();

			AllSystems = new HashSet<IGameSystem>(GetAllGameSystems(root));
			Log("Found GameSystems:\n - " + string.Join("\n - ", AllSystems));

			SystemsInInitialzationOrder = new GameSystemInitializationOrderSorter(AllSystems).GetOrderedByDependencies().ToList();
			Log("Ordered GameSystems:\n - " + string.Join("\n - ", SystemsInInitialzationOrder));

			_systemsLeftToInitialize = new List<IGameSystem>(SystemsInInitialzationOrder);
		}

		public void Initialize()
		{
			foreach (IGameSystem gameSystem in _systemsLeftToInitialize)
			{
				Log($"Attempting to initialize {gameSystem.GetType().Name}");
				TryInitialize(gameSystem);
			}
		}

		private IEnumerable<IGameSystem> GetAllGameSystems(Transform root)
		{
			foreach (Transform child in root)
			{
				if (!child.gameObject.activeInHierarchy) continue;
				if (!child.TryGetComponent(out IGameSystem system)) continue;
				if (system.IsInstantiated()) continue;
				if (Failed.Contains(system)) continue;
				yield return system;
			}
		}

		private void TryInitialize(IGameSystem gameSystem)
		{
			try
			{
				gameSystem.Instantiate();
			}
			catch (Exception error)
			{
				Debug.LogError($"Could not instantiate {gameSystem.GetType().Name}: {error}");
				return;
			}

			if (!gameSystem.IsInstantiated())
			{
				string typeName = gameSystem.GetType().Name;
				Debug.LogError($"Could not instantiate {typeName}: {typeName}.IsInitialized() returned false!");
				Failed.Add(gameSystem);
				return;
			}

			Log($"Successfully initialized {gameSystem.GetType().Name}");
		}

		private void Log(string text)
		{
			if (_verbose)
			{
				Debug.Log($"[{GetType().Name}] {text}");
			}
		}
	}
}
