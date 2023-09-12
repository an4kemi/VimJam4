using Reflex.Core;
using Source.Scripts;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(new TestClass());
    }
}
