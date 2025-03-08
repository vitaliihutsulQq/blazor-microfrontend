using Microsoft.AspNetCore.Components;

namespace NewModule.Components;

public partial class NewModuleComponent : ComponentBase
{
    [Parameter]
    public string Message { get; set; } = "Hello from NewModuleComponent!";
}
