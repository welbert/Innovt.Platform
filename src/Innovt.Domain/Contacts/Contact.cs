﻿using Innovt.Domain.Core.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Innovt.Domain.Contacts
{
    public class Contact : ValueObject<int>, IValidatableObject
    {
        [Required] public string Name { get; set; }

        /// <summary>
        /// For example Home, Office etc
        /// </summary>
        [Required]
        public string Description { get; set; }

        [Required] public ContactType Type { get; set; }

        [Required] public string Value { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == null)
            {
                yield return new ValidationResult("Contact type is required");
            }
            else
            {
                if (Type.Validate(Value))
                    yield return new ValidationResult($"The value {Value} is not valid for {Type.Name}.");
            }
        }
    }
}