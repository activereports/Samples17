
namespace GrapeCity.ActiveReports.Samples.IListBinding.DataLayer
{
	/// <summary>
	/// Summary description for Product.
	/// </summary>
	public class Product
	{
		internal Product()
		{
		}

		//******
		//The properties listed below define a product in the datalayer. 
		//******

		public int ProductID { get; set; }

		public string ProductName { get; set; }

		public int SupplierID { get; set; }

		public int CategoryID { get; set; }


		public string QuantityPerUnit { get; set; }

		public decimal UnitPrice { get; set; }

		public int UnitsInStock { get; set; }

		public int UnitsOnOrder { get; set; }

		public int ReorderLevel { get; set; }

		public bool Discontinued { get; set; }
	}
}
