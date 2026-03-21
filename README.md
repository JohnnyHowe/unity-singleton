Before using this or any other singleton implementation please consider whether it is right for your project. Singletons have a lot of drawbacks. 

# Overview
Create `MonoBehaviour` singletons (called `GameSystem`).
* Serialize fields and accessing them in the inspector
* Hook into Unity's loop (`Update`, `FixedUpdate`, etc)
* **No more initialization scene!**

# Install (UPM)
1. In Unity, open the Package Manager\
**Window** > **Package Management** > **Package Manager**

2. Install Package from git URL\
**Package Manager** > **+** (in top left) > **Install Package from git URL**

3. Paste `https://github.com/JohnnyHowe/unity-gamesystem-singleton.git` and press **Install**

**(Recommended)** If you want a specific version, use \
`https://github.com/JohnnyHowe/unity-gamesystem-singleton.git#<version>`

# Usage

## Create Your Class
```csharp
public class ExampleGameSystem: GameSystem<PlayerInventory>
{
	/// AwakeSystem is called similarly to MonoBehaviour.Awake.
	/// This is where you put your startup/initialization
	protected override void AwakeSystem() { }

	public void DoSomething() { }
}
```

## Create Game Systems Prefab
Just one of these in the project. \
Each individual `GameSystem` will live as a child of this.

* Add the `SystemsStarter` script.

* Place this in some resource folder.

## Add Your System to the Prefab
Create a child object and put your `GameSystem` script on it.

## Access it From Anywhere
```csharp
public class ThingThatUsesExampleGameSystem 
{
	public void CallDoSomething()
	{
		ExampleGameSystem.Instance.DoSomething();
	}
}
```
