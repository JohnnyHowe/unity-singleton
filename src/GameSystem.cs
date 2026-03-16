using UnityEngine;

namespace JonathonOH.UnityTools.SystemsManagement
{
    public interface IGameSystem
    {
        public bool IsInstantiated();
        internal void Instantiate();
    }

    public class GameSystem<T> : MonoBehaviour, IGameSystem
    {
        private bool instantiated = false;
        private static T instance;
        public static T Instance { get => GetInstance(); }

        private static T GetInstance()
        {
            if (instance is null) SystemsInitializer.PromptLoad();
            return instance;
        }

        public bool IsInstantiated()
        {
            return instantiated;
        }

		void IGameSystem.Instantiate()
        {
            instance = GetComponent<T>();
            AwakeSystem();
            instantiated = true;
        }

        protected virtual void AwakeSystem() { }
	}
}
