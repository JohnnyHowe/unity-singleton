using System;
using UnityEngine;

namespace JonathonOH.Unity.Singletons
{
	public class SingletonMaster : MonoBehaviour
	{
		public enum InitializationState
		{
			NotStarted,
			Initializing,
			Done,
		}

		private static SingletonMaster _instance;

		private SingletonBootstrap _singletonsBootstrap;
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
			if (DoesSystemObjectExist(out SingletonMaster existing))
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
		private static bool DoesSystemObjectExist(out SingletonMaster systemsStarter)
		{
			systemsStarter = FindFirstObjectByType<SingletonMaster>();
			return systemsStarter != null;
		}

		private static void CreateSystemsObject()
		{
			GameObject prefab = SystemsPrefabLoader.GetPrefab();
			GameObject systems = Instantiate(prefab);

			if (!systems.TryGetComponent(out SingletonMaster systemsStarter))
			{
				throw new Exception($"{typeof(SingletonMaster).Name} does not exist on Systems prefab ({prefab})");
			}

			DontDestroyOnLoad(systemsStarter);
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
			Debug.Log($"[SystemsManagement][{GetType().Name}] InitialzingSystems");

			_initializationState = InitializationState.Initializing;
			_singletonsBootstrap = new SingletonBootstrap(transform);
			_singletonsBootstrap.Initialize();
			_initializationState = InitializationState.Done;
		}

		#endregion
	}
}
