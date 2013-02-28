using System;
using Socona.Expor.Utilities.Options.Constraints;
namespace Socona.Expor.Utilities.Options.Parameters
{
    public interface IParameter
    {
        void AddConstraint(IParameterConstraint constraint);
        object GetDefaultValue();
        string GetDefaultValueAsString();
        string GetFullDescription();
        object GetGivenValue();
        string GetName();
        OptionDescription GetOptionDescription();
        string GetSyntax();
        object GetValue();
        string GetValueAsString();
        string GetValuesDescription();
        bool HasDefaultValue();
        bool HasValuesDescription();
        bool IsDefined();
        bool IsOptional();
        bool IsValid(object obj);
        void SetDefaultValue(object defaultValue);
        void SetOptional(bool opt);
        void SetValue(object obj);
        string ShortDescription { get; set; }
        bool TookDefaultValue();
        bool TryDefaultValue();
        void UseDefaultValue();
    }
}
