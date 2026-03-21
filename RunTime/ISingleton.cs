namespace JonathonOH.Unity.Singletons
{
	internal interface ISingleton
	{
		public bool Initialized { get; }
		public bool Awoken { get; }
		internal void SetInstance();
		internal void Initialize();
	}
}
