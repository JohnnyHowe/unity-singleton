using UnityEngine;

namespace JonathonOH.Unity.Singletons
{
	public class Singleton<T> : MonoBehaviour, ISingleton
	{
		public static T Instance { get => GetInstance(); }
		private static T _instance;

		public static bool IsInitialized => _instance != null;
		bool ISingleton.IsInitialized => _instance != null;

		public static bool IsAwoken => IsInitialized && ((ISingleton)_instance).IsAwoken;
		bool ISingleton.IsAwoken => _awoken;
		private bool _awoken = false;

		private static T GetInstance()
		{
			if (_instance is null) SingletonMaster.PromptLoad();
			return _instance;
		}

		void ISingleton.Initialize()
		{
			if (IsAwoken)
			{
				Debug.LogError($"{GetType().Name}.AwakeSystem called but it is already Awoken!");
				return;
			}
			AwakeSystem();
			_awoken = true;
		}

		protected virtual void AwakeSystem() { }

		void ISingleton.SetInstance()
		{
			if (IsInitialized)
			{
				Debug.LogError($"{GetType().Name}.Initialize called but it is already Initialized!");
				return;
			}
			_instance = GetComponent<T>();
		}
	}
}
