using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Attributes
{
    public class ValidEnumValueAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumValueAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            return Enum.IsDefined(_enumType, value);
        }
    }
}

