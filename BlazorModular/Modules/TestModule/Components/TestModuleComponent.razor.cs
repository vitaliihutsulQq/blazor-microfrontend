using Microsoft.AspNetCore.Components;

namespace TestModule.Components;

public partial class TestModuleComponent : ComponentBase
{
    [Parameter]
    public string Message { get; set; } = "Hello from TestModuleComponent!";
}
