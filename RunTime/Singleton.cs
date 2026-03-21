using UnityEngine;

namespace JonathonOH.Unity.Singletons
{
	public class Singleton<T> : MonoBehaviour, ISingleton
	{
		public static T Instance { get => GetInstance(); }
		private static T _instance;

		bool ISingleton.Initialized => _initialized;
		private bool Initialized => ((ISingleton)this).Initialized;
		private bool _initialized = false;

		bool ISingleton.Awoken => _awoken;
		private bool Awoken => ((ISingleton)this).Awoken;
		private bool _awoken = false;

		private static T GetInstance()
		{
			if (_instance is null) SingletonMaster.PromptLoad();
			return _instance;
		}

		void ISingleton.AwakeSystem()
		{
			if (Awoken)
			{
				Debug.LogError($"{GetType().Name}.AwakeSystem called but it is already Awoken!");
				return;
			}
			AwakeSystem();
		}

		protected virtual void AwakeSystem() { }

		void ISingleton.Initialize()
		{
			if (Initialized)
			{
				Debug.LogError($"{GetType().Name}.Initialize called but it is already Initialized!");
				return;
			}
			_instance = GetComponent<T>();
			_initialized = true;
		}
	}
}
