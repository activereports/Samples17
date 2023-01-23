Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Authorization
Imports Microsoft.AspNetCore.Hosting

<ApiController>
<Authorize(AuthenticationSchemes := "BasicAuthentication")>
<Route("[controller]/[action]")>
Public Class Customers
	Inherits ControllerBase
	Private ReadOnly _env As IWebHostEnvironment
	
	Sub New (env As IWebHostEnvironment)
		_env = env
	End Sub
	
	<HttpGet>
	Public Function GetJson() As Task (Of IActionResult)
		Return Task.FromResult (Of IActionResult)(new PhysicalFileResult(_env.ContentRootPath + $"/data/customers.json", "application/json"))
	End Function
End Class