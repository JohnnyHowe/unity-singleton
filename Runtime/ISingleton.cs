namespace JonathonOH.Unity.Singletons
{
	internal interface ISingleton
	{
		internal bool Initialized { get; }
		internal bool Awoken { get; }
		internal void SetInstance();
		internal void Initialize();
	}
}
