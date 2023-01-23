using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = nameof(BasicAuthentication))]
[Route("[controller]/[action]")]
public class CustomersController : ControllerBase
{
	private readonly IWebHostEnvironment _env;
	public CustomersController(IWebHostEnvironment env)
	{
		_env = env;
	}
	
	[HttpGet]
	public Task<IActionResult> GetJson() =>
		Task.FromResult<IActionResult>(new PhysicalFileResult(_env.ContentRootPath + @"/data/customers.json", "application/json"));
}