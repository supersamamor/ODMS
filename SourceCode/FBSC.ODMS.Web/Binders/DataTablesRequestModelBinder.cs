using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FBSC.ODMS.Web.Binders
{
    internal class DataTablesRequestModelBinder : IModelBinder
    {
        //
        // Summary:
        //     Attempts to bind a model.
        //
        // Parameters:
        //   bindingContext:
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return Task.Factory.StartNew(delegate
            {
                BindModel(bindingContext);
            });
        }

        //
        // Summary:
        //     Attempts to bind a model.
        //
        // Parameters:
        //   bindingContext:
        private static void BindModel(ModelBindingContext bindingContext)
        {
            IValueProvider valueProvider = bindingContext.ValueProvider;
            ValueProviderResult value = valueProvider.GetValue("draw");
            if (valueProvider == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            _ = DataTablesRequestModelBinderHelpers.TryParse<int>(value, out int result);
            _ = DataTablesRequestModelBinderHelpers.TryParse<int>(valueProvider.GetValue("start"), out int result2);
            _ = DataTablesRequestModelBinderHelpers.TryParse<int>(valueProvider.GetValue("length"), out int result3);
            DataTablesRequest model = new(result, result2, result3, TryGetSearch(valueProvider), DataTablesRequestModelBinderHelpers.TryGetOrders(valueProvider), DataTablesRequestModelBinderHelpers.TryGetColumns(valueProvider));
            bindingContext.Result = ModelBindingResult.Success(model);
        }

        //
        // Summary:
        //     Gets the search part of query
        private static Search? TryGetSearch(IValueProvider valueProvider)
        {
            if (DataTablesRequestModelBinderHelpers.TryParse<string>(valueProvider.GetValue("search[value]"), out var result) && !string.IsNullOrEmpty(result))
            {
                _ = DataTablesRequestModelBinderHelpers.TryParse(valueProvider.GetValue("search[regex]"), out bool result2);
                return new Search(result, result2);
            }
            return null;
        }
    }
}
