using System.ComponentModel.DataAnnotations;

namespace ODataEndPoint.Models
{
	public class Customer
	{
		[Key]
		public string? CustomerID { get; set; }
		public string? CompanyName { get; set; }
		public string? ContactName { get; set; }
		public string? Address { get; set; }
	}
}