namespace JonathonOH.Unity.Singletons
{
	internal interface ISingleton
	{
		internal bool IsInitialized { get; }
		internal bool IsAwoken { get; }
		internal void SetInstance();
		internal void Initialize();
	}
}
