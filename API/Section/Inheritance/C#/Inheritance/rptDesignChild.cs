using System.Resources;

namespace GrapeCity.ActiveReports.Sample.Inheritance
{
	/// <summary>
	/// A description of the overview of the rptDesignChild.
	/// </summary>
	public partial class rptDesignChild : Inheritance.rptDesignBase
	{
		private ResourceManager _resource;
		public rptDesignChild()
		{
			_resource = new ResourceManager(typeof(rptDesignChild));
			//
			// Designer support is required to ActiveReport.
			//

			InitializeComponent();
		}
	}
}
