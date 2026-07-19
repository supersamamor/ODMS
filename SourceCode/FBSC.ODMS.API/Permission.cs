namespace FBSC.ODMS.API;

public static class Permission
{
    public static class DataSource
	{
		public const string View = "P.DataSource.V";
		public const string Create = "P.DataSource.C";
		public const string Edit = "P.DataSource.E";
		public const string Delete = "P.DataSource.D";
		public const string Upload = "P.DataSource.U";
		public const string History = "P.DataSource.H";
		public const string Approve = "P.DataSource.A";
	}
	public static class DataUpload
	{
		public const string View = "P.DataUpload.V";
		public const string Create = "P.DataUpload.C";
		public const string Edit = "P.DataUpload.E";
		public const string Delete = "P.DataUpload.D";
		public const string Upload = "P.DataUpload.U";
		public const string History = "P.DataUpload.H";
	}
	
}