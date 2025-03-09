using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesApiController : ControllerBase
    {
        private readonly string _modulesPath;

        public ModulesApiController()
        {
            _modulesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dynamic");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleRequest request)
        {
            if (string.IsNullOrEmpty(request.ModuleName))
                return BadRequest("Name of module must be not empty");



            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var modulesPath = Path.Combine(solutionDirectory, "Modules");
            var moduleDirectory = Path.Combine(modulesPath, request.ModuleName);

            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dynamic");

            if (Directory.Exists(moduleDirectory))
                return BadRequest($"Module {request.ModuleName} already exists.");

            try
            {
                Console.WriteLine($"📁 Creating module {request.ModuleName}...");

                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "CreateModule.bat");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C \"{scriptPath} {request.ModuleName}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"❌ Module creation failed: {error}");
                    return StatusCode(500, $"Module creation failed: {error}");
                }

                Console.WriteLine($"✅ Module {request.ModuleName} created successfully!");


                if (BuildModule(moduleDirectory))
                {
                    var dllPath = Path.Combine(moduleDirectory, "bin", "Release", "net9.0", $"{request.ModuleName}.dll");

                    if (!System.IO.File.Exists(dllPath))
                    {
                        Console.WriteLine($"❌ Error: Built DLL not found at {dllPath}");
                        return StatusCode(500, "Module build completed, but DLL not found.");
                    }

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    var destinationFile = Path.Combine(destinationPath, $"{request.ModuleName}.dll");
                    System.IO.File.Copy(dllPath, destinationFile, true);
                    Console.WriteLine($"✅ Module DLL copied to {destinationFile}");
                }
                else
                {
                    return StatusCode(500, "Module build failed.");
                }

                return Ok(new { Message = $"Module {request.ModuleName} successfully created, built, and deployed!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the module.");
            }

        }

        [HttpGet("list")]
        public IActionResult GetModulesList()
        {
            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var modulesPath = Path.Combine(solutionDirectory, "Modules");

            if (!Directory.Exists(modulesPath))
            {
                return NotFound("Modules folder not found.");
            }

            var moduleDirectories = Directory.GetDirectories(modulesPath)
                                             .Select(Path.GetFileName)
                                             .ToList();

            return Ok(moduleDirectories);
        }

        [HttpGet("get/{moduleName}")]
        public IActionResult GetModule(string moduleName)
        {
            var modulePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dynamic", $"{moduleName}.dll");

            if (!System.IO.File.Exists(modulePath))
            {
                Console.WriteLine($"❌ ERROR: Module {moduleName} not found at {modulePath}");
                return NotFound($"Module {moduleName} not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(modulePath);
            return File(fileBytes, "application/octet-stream", $"{moduleName}.dll");
        }

        private bool BuildModule(string moduleDirectory)
        {
            try
            {
                Console.WriteLine($"Building module in {moduleDirectory}...");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = "build --configuration Release",
                        WorkingDirectory = moduleDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"✅ Module built successfully: {moduleDirectory}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Module build failed: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception while building module: {ex.Message}");
                return false;
            }
        }
    }
}
