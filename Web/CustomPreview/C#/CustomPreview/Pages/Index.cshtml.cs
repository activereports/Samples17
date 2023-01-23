using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomPreview.Pages;

public class Index : PageModel
{
	private readonly IWebHostEnvironment _environment;
	private readonly string[] _exts = { ".rdlx", ".rpx" };
	public ICollection<string> Files { get; set; } = new List<string>();
	public Index(IWebHostEnvironment environment)
	{
		_environment = environment;
	}

	public IActionResult OnGet()
	{
		var filePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "Reports"))
			.Where(x => _exts.Any(c => x.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)));

		Files = new List<string>();
		foreach (var filePath in filePaths)
		{
			Files.Add(Path.GetFileName(filePath));
		}
		return Page();
	}
}