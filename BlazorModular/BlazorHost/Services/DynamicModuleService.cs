using System.Reflection;

namespace BlazorHost.Services
{
    public class DynamicModuleService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, Type> _loadedComponents = new();

        public DynamicModuleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Type> GetComponentAsync(string moduleName)
        {
            var moduleUrl = $"{_httpClient.BaseAddress}api/ModulesApi/get/{moduleName}.dll";

            try
            {
                var assemblyBytes = await _httpClient.GetByteArrayAsync(moduleUrl);
                var assembly = Assembly.Load(assemblyBytes);

                var componentFullName = $"{moduleName}.Components.{moduleName}Component";
                var loadedComponent = assembly.GetType(componentFullName);

                if (loadedComponent == null)
                {
                    Console.WriteLine($"❌ Component {componentFullName} not found in assembly.");
                    return null;
                }

                _loadedComponents[moduleName] = loadedComponent; // Cashing
                return loadedComponent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading module {moduleName}: {ex.Message}");
                return null;
            }
        }
    }
}
