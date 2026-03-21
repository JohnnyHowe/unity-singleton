using System;

namespace JonathonOH.Unity.Singletons
{
    public interface ISingletonDependencies
    {
        Type[] RequiredSystems { get; }
    }
}
