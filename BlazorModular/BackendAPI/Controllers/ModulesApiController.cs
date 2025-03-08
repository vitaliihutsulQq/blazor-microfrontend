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

            //var scriptPath = "./Scripts/CreateModule.sh";
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
            await process.WaitForExitAsync();

            return Ok(new { Message = "Module succeful created!" });
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

        [HttpGet("get/{moduleName}.dll")]
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
