using System;
using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
	public class SystemsStarter : MonoBehaviour
	{
		public enum InitializationState
		{
			NotStarted,
			Initializing,
			Done,
		}

		private const string systemPrefabName = "Systems";
		private static SystemsStarter _instance;

		[SerializeField] private bool _verbose = false;

		private GameSystemStarter _gameSystemStarter;
		private InitializationState _initializationState = InitializationState.NotStarted;

		#region Static

		/// <summary>Ensures the Systems prefab is instantiated and initialized.</summary>
		public static void PromptLoad()
		{
			EnsureInstanceExistsAndSystemsStarted();
		}

		private static void EnsureInstanceExistsAndSystemsStarted()
		{
			EnsureInstanceExists();
			_instance.StartSystemsIfRequired();
		}

		/// <summary>Finds or creates the Systems prefab and returns its initializer.</summary>
		private static void EnsureInstanceExists()
		{
			// Instance already exists?
			if (_instance != null) return;

			// Object exists but instance not populated?
			if (DoesSystemObjectExist(out SystemsStarter existing))
			{
				_instance = existing;
			}

			// Object doesn't exist -> make it
			else
			{
				CreateSystemsObject();
			}
		}

		/// <summary>Checks for an existing Systems root and returns its initializer.</summary>
		/// <param name="systemsStarter">Initializer found on the Systems root.</param>
		private static bool DoesSystemObjectExist(out SystemsStarter systemsStarter)
		{
			GameObject systemObject = GameObject.Find(systemPrefabName);
			if (systemObject != null && systemObject.TryGetComponent(out SystemsStarter found))
			{
				systemsStarter = found;
				return true;
			}

			systemsStarter = null;
			return false;
		}

		private static void CreateSystemsObject()
		{
			GameObject prefab = SystemsPrefabLoader.GetPrefab(systemPrefabName);
			GameObject systems = Instantiate(prefab);
			systems.name = systemPrefabName;

			if (!systems.TryGetComponent(out SystemsStarter systemsStarter))
			{
				throw new Exception($"{typeof(SystemsStarter).Name} does not exist on Systems prefab ({prefab})");
			}

			_instance = systemsStarter;
		}

		#endregion
		#region Instance

		private void StartSystemsIfRequired()
		{
			if (_initializationState == InitializationState.NotStarted)
			{
				StartSystems();
			}
		}

		/// <summary>Initializes child systems in order with dependency checks.</summary>
		private void StartSystems()
		{
			if (_verbose) Debug.Log($"[SystemsManagement][{GetType().Name}] InitialzingSystems");

			_initializationState = InitializationState.Initializing;
			_gameSystemStarter = new GameSystemStarter(transform, _verbose);
			_gameSystemStarter.Initialize();
			_initializationState = InitializationState.Done;
		}

		#endregion
	}
}
