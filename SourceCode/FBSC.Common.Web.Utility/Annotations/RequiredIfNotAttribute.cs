using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace FBSC.Common.Web.Utility.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfNotAttribute : ValidationAttribute
    {
        private readonly string OtherProperty;
        private readonly object ExpectedValue;

        public RequiredIfNotAttribute(string otherProperty, object expectedValue)
        {
            OtherProperty = otherProperty;
            ExpectedValue = expectedValue;
        }
        public override string FormatErrorMessage(string name)
        {
            if (ErrorMessage == null && ErrorMessageResourceName == null)
            {
                ErrorMessage = $"The field {name} is required";
            }
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property {OtherProperty} not found.");
            }
            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);
            if (!object.Equals(otherPropertyValue, ExpectedValue) && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
    public class RequiredIfNotAttributeAdapter : AttributeAdapterBase<RequiredIfNotAttribute>
    {
        public RequiredIfNotAttributeAdapter(RequiredIfNotAttribute attribute, IStringLocalizer? stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));
        }
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return Attribute.FormatErrorMessage(validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
