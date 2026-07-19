using FBSC.ODMS.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Attributes
{
    public class DataTablesRequestAttribute : ModelBinderAttribute
    {
        //
        // Summary:
        //     Initialize a new instance of DataTables.AspNetCore.Mvc.Binder.DataTablesRequestAttribute
        public DataTablesRequestAttribute()
            : base(typeof(DataTablesRequestModelBinder))
        {
        }
    }
}
