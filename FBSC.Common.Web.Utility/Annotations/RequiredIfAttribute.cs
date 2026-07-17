using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace FBSC.Common.Web.Utility.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfAttribute(string otherProperty, params object[] expectedValues) : ValidationAttribute
    {
        // Enterprise Optimization: Cache PropertyInfo to eliminate expensive reflection overhead on subsequent calls
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _propertyCache = new();

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessage) && string.IsNullOrEmpty(ErrorMessageResourceName))
            {
                ErrorMessage = $"The field {name} is required.";
            }
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);

            var objectType = validationContext.ObjectType;

            // Retrieve from cache or use reflection to find the property, then cache it
            var otherPropertyInfo = _propertyCache.GetOrAdd(objectType, type => type.GetProperty(otherProperty));

            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property '{otherProperty}' not found on type '{objectType.Name}'.");
            }

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // Check if the current value matches ANY of the expected values (the "OR" logic)
            if (expectedValues.Contains(otherPropertyValue) && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }

    public class RequiredIfAttributeAdapter(RequiredIfAttribute attribute, IStringLocalizer? stringLocalizer)
        : AttributeAdapterBase<RequiredIfAttribute>(attribute, stringLocalizer)
    {
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));

            // Note: If you need strict client-side validation logic for the "OR" condition, 
            // you would also need to pass the expected values as a data attribute here 
            // and write a custom jQuery Unobtrusive validation adapter in Javascript.
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return Attribute.FormatErrorMessage(validationContext.ModelMetadata.GetDisplayName());
        }
    }
}