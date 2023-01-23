using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataEndPoint.Models;

namespace ODataEndPoint.Controllers
{
	public class CustomersController : ODataController
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;

		public CustomersController(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}
		
		[EnableQuery]
		public IActionResult Get()
		{
			var connStr = _configuration.GetSection("ConnectionStrings")["Nwind"].Replace("$appPath$", _env.ContentRootPath);
			var conn = new SqliteConnection(connStr);
			conn.Open();
			var customers = new Collection<Customer>();
			var cmd = new SqliteCommand("select customers.Customerid, customers.CompanyName, customers.ContactName, customers.Address from customers", conn);
			var dataReader = cmd.ExecuteReader();
			while (dataReader.Read())
			{
				customers.Add(new Customer
				{
					CustomerID = dataReader.GetValue(0).ToString()!,
					CompanyName = dataReader.GetValue(1).ToString()!,
					ContactName = dataReader.GetValue(2).ToString()!,
					Address = dataReader.GetValue(3).ToString()!,
				});
			}
			conn.Close();
			return Ok(customers.AsQueryable());
		}
	}
}
