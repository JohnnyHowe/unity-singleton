using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
	public interface IGameSystem
	{
		public bool Initialized { get; }
		public bool Awoken { get; }
		internal void Initialize();
		internal void AwakeSystem();
	}

	public class GameSystem<T> : MonoBehaviour, IGameSystem
	{
		public static T Instance { get => GetInstance(); }
		private static T _instance;

		bool IGameSystem.Initialized => _initialized;
		private bool Initialized => ((IGameSystem)this).Initialized;
		private bool _initialized = false;

		bool IGameSystem.Awoken => _awoken;
		private bool Awoken => ((IGameSystem)this).Awoken;
		private bool _awoken = false;

		private bool _appQuitting = false;

		private static T GetInstance()
		{
			if (_instance is null) SystemsStarter.PromptLoad();
			return _instance;
		}

		void IGameSystem.AwakeSystem()
		{
			if (Awoken)
			{
				Debug.LogError($"[SystemsManagement][GameSystem<T>] {GetType().Name}.AwakeSystem called but it is already Awoken!");
				return;
			}
			AwakeSystem();
		}

		protected virtual void AwakeSystem() { }

		void IGameSystem.Initialize()
		{
			if (Initialized)
			{
				Debug.LogError($"[SystemsManagement][GameSystem<T>] {GetType().Name}.Initialize called but it is already Initialized!");
				return;
			}
			_instance = GetComponent<T>();
			_initialized = true;
		}
	}
}
