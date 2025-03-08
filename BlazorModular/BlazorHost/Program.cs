using BlazorHost;
using BlazorHost.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<DynamicModuleService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppConfig.ApiBaseUrl()) });
builder.Services.AddRadzenComponents();
await builder.Build().RunAsync();
