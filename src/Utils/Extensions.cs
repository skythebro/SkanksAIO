using Bloodstone.API;
using Unity.Entities;
using Il2CppInterop.Runtime;

namespace SkanksAIO.Utils;

public static class Extensions
{
    public delegate void ActionRefTest<T>(ref T item);
    
    public static void WithComponentDataAOT<T>(this Entity entity, ActionRefTest<T> action) where T : unmanaged
    {
        var componentData = VWorld.Game.EntityManager.GetComponentDataAOT<T>(entity);
        action(ref componentData);
        VWorld.Game.EntityManager.SetComponentData<T>(entity, componentData);
    }
    
    private static Il2CppSystem.Type GetType<T>() => Il2CppType.Of<T>();
    
    public static unsafe T GetComponentDataAOT<T>(this EntityManager entityManager, Entity entity) where T : unmanaged
    {
        var type = TypeManager.GetTypeIndex(GetType<T>());
        var result = (T*)entityManager.GetComponentDataRawRW(entity, type);
        return *result;
    }
}