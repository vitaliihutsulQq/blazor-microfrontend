using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesApiController : ControllerBase
    {

        [HttpPost("create")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleRequest request)
        {
            if (string.IsNullOrEmpty(request.ModuleName))
                return BadRequest("Name of module must be not empty");

            var scriptPath = "./Scripts/CreateModule.sh";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"{scriptPath} {request.ModuleName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            return Ok(new { Message = "Module succeful created!" });
        }
    }
}
