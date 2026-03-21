using System;

namespace JonathonOH.UnityTools.SystemsManagement
{
    public interface IGameSystemDependencies
    {
        Type[] RequiredSystems { get; }
    }
}
