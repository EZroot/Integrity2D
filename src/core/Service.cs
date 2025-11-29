using System.Diagnostics;

public static class Service
{
    private static Dictionary<Type, object?>? m_Registry = null;

    public static void ClearRegistry()
    {
        m_Registry?.Clear();
    }

    public static void Register<T>() where T : IService
    {
        m_Registry ??= [];

        if (m_Registry.ContainsKey(typeof(T))) return;

        var service = GetClassFromAssemblyByInterface(typeof(T));

        Debug.Assert(service != null, $"No implementation found for service interface {typeof(T).FullName}");

        object? instance = Activator.CreateInstance(service);
        m_Registry[typeof(T)] = instance;
        Logger.Log($"Registered service: <color=green>{typeof(T).FullName}</color>", Logger.LogSeverity.Info);
    }

    public static T? Get<T>() where T : IService
    {
        Register<T>();
        return (T?)m_Registry?[typeof(T)];
    }

    private static Type? GetClassFromAssemblyByInterface(Type interfaceType)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                if (!type.IsClass) continue;

                foreach (var iInterface in type.GetInterfaces())
                {
                    if (iInterface != interfaceType) continue;
                    return type;
                }
            }
        }

        return null;
    }
}
