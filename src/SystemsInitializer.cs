using System;
using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
	public class SystemsInitializer : MonoBehaviour
	{
		public enum InitializationState
		{
			NotStarted,
			Initializing,
			Done,
		}

		private const string systemPrefabName = "Systems";
		private static SystemsInitializer _instance;
		private GameSystemInitializer _gameSystemInitializer;
		private InitializationState _initializationState = InitializationState.NotStarted;

		#region Static

		/// <summary>Ensures the Systems prefab is instantiated and initialized.</summary>
		public static void PromptLoad()
		{
			EnsureInstanceExistsAndSystemsInitialized();
		}

		private static void EnsureInstanceExistsAndSystemsInitialized()
		{
			EnsureInstanceExists();
			_instance.InitializeSystemsIfRequired();
		}

		/// <summary>Finds or creates the Systems prefab and returns its initializer.</summary>
		private static void EnsureInstanceExists()
		{
			// Instance already exists?
			if (_instance != null) return;

			// Object exists but instance not populated?
			if (DoesSystemObjectExist(out SystemsInitializer existing))
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
		/// <param name="initializer">Initializer found on the Systems root.</param>
		private static bool DoesSystemObjectExist(out SystemsInitializer initializer)
		{
			GameObject systemObject = GameObject.Find(systemPrefabName);
			if (systemObject != null && systemObject.TryGetComponent(out SystemsInitializer found))
			{
				initializer = found;
				return true;
			}

			initializer = null;
			return false;
		}

		private static void CreateSystemsObject()
		{
			GameObject prefab = SystemsPrefabLoader.GetPrefab(systemPrefabName);
			GameObject systems = Instantiate(prefab);
			systems.name = systemPrefabName;

			if (!systems.TryGetComponent(out SystemsInitializer initializer))
			{
				throw new Exception($"{typeof(SystemsInitializer).Name} does not exist on Systems prefab ({prefab})");
			}

			_instance = initializer;
		}

		#endregion
		#region Instance

		private void InitializeSystemsIfRequired()
		{
			if (_initializationState == InitializationState.NotStarted)
			{
				InitializeSystems();
			}
		}

		/// <summary>Initializes child systems in order with dependency checks.</summary>
		private void InitializeSystems()
		{
			_initializationState = InitializationState.Initializing;
			_gameSystemInitializer = new GameSystemInitializer(transform);
			_gameSystemInitializer.Initialize();
			_initializationState = InitializationState.Done;
		}

		#endregion
	}
}
