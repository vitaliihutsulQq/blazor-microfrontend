@echo off
set MODULE_NAME=%1
set MODULE_PATH=..\Modules\%MODULE_NAME%

if "%MODULE_NAME%"=="" (
    echo ❌ Error: Please specify a module name!
    exit /b 1
)

echo 🔹 Creating new module %MODULE_NAME%...
dotnet new razorclasslib -o "%MODULE_PATH%"

:: Waiting when .csproj file will be created
echo ⏳ Waiting for .csproj file to be created...
:waitLoop
if not exist "%MODULE_PATH%\%MODULE_NAME%.csproj" (
    timeout /t 1 >nul
    goto waitLoop
)

echo 🔹 Setting up module structure...
mkdir "%MODULE_PATH%\Components"
move "%MODULE_PATH%\Component1.razor" "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor"

echo 🔹 Removing unnecessary files...
del "%MODULE_PATH%\Component1.razor.css" >nul 2>&1

echo 🔹 Creating scoped CSS file...
(
    echo h3 {>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.css"
    echo     color: blue;>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.css"
    echo }>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.css"
)

echo 🔹 Creating C# code-behind file...
(
    echo using Microsoft.AspNetCore.Components;>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo.>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo namespace %MODULE_NAME%.Components;>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo.>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo public partial class %MODULE_NAME%Component : ComponentBase>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo {>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo     [Parameter]>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo     public string Message { get; set; } = "Hello from %MODULE_NAME%Component!";>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
    echo }>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs"
)

echo 🔹 Updating namespace...
(
    echo @namespace %MODULE_NAME%.Components>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor"
    echo.>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor"
    echo <h3>@Message</h3>> "%MODULE_PATH%\Components\%MODULE_NAME%Component.razor"
)


echo ✅ Module %MODULE_NAME% successfully created and configured!