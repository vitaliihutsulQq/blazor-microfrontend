@echo off
set MODULE_NAME=%1
set MODULE_PATH=..\Modules\%MODULE_NAME%

if "%MODULE_NAME%"=="" (
    echo Error: Please specify a module name!
    exit /b 1
)

echo Creating new module %MODULE_NAME%...
dotnet new razorclasslib -o %MODULE_PATH%

:: Waiting when file will be created
echo Waiting for .csproj file to be created...
:waitLoop
if not exist %MODULE_PATH%\%MODULE_NAME%.csproj (
    timeout /t 1 >nul
    goto waitLoop
)

echo Setting up module structure...
mkdir %MODULE_PATH%\Components
move %MODULE_PATH%\Component1.razor %MODULE_PATH%\Components\%MODULE_NAME%Component.razor

echo Removing unnecessary files...
del %MODULE_PATH%\Component1.razor.css >nul 2>&1

echo Creating scoped CSS file...
(
    echo h3 {
    echo     color: blue;
    echo }
) > %MODULE_PATH%\Components\%MODULE_NAME%Component.razor.css

echo Creating C# code-behind file...
(
    echo using Microsoft.AspNetCore.Components;
    echo.
    echo namespace %MODULE_NAME%.Components;  
    echo.
    echo public partial class %MODULE_NAME%Component : ComponentBase
    echo {
    echo     [Parameter] 
    echo     public string Message { get; set; } = "Hello from %MODULE_NAME%Component!";
    echo }
) > %MODULE_PATH%\Components\%MODULE_NAME%Component.razor.cs

echo Updating namespace...
(
    echo @namespace %MODULE_NAME%.Components  
    echo.
    echo <h3>@Message</h3>
) > %MODULE_PATH%\Components\%MODULE_NAME%Component.razor

:: ✅ Edit `.csproj`, добавляя `PostBuild`
powershell -Command "& {
    $csprojPath = '%MODULE_PATH%\%MODULE_NAME%.csproj';
    
    # Читаем существующий .csproj
    $content = Get-Content -Path $csprojPath -Raw;
    
    # Проверяем, есть ли уже PostBuild, чтобы не дублировать
    if ($content -notmatch '<Target Name=\"PostBuild\"') {
        # Добавляем PostBuild перед закрывающим тегом </Project>
        $postBuild = @'
  <Target Name=\"PostBuild\" AfterTargets=\"Build\">
    <Copy SourceFiles=\"$(OutputPath)$(AssemblyName).dll\" DestinationFolder=\"$(SolutionDir)BackendAPI/wwwroot/dynamic\" />
  </Target>
'@;
        $content = $content -replace '</Project>', \"$postBuild`n</Project>\";
        # Записываем обратно
        Set-Content -Path $csprojPath -Value $content -Encoding utf8;
    }
}"

echo ✅ Module %MODULE_NAME% successfully created and configured!