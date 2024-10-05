using Microsoft.AspNetCore.Mvc;
using Nullposters.DataInspector.Api.Services;

namespace Nullposters.DataInspector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchemaController(SchemaService schemaService) : ControllerBase
    {
        private readonly SchemaService _schemaService = schemaService;

        [HttpPost("GetTables")]
        public async Task<IActionResult> GetTables([FromBody] ConnectionRequest request)
        {
            try
            {
                var tables = await _schemaService.GetTableSchemasAsync(request.ConnectionString, request.Provider);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ConnectionRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}

