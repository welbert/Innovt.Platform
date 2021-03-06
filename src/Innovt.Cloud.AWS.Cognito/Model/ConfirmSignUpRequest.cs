﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Cloud.AWS.Cognito
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Innovt.Cloud.AWS.Cognito.Resources;
using Innovt.Core.Collections;

namespace Innovt.Cloud.AWS.Cognito.Model
{
    public class ConfirmSignUpRequest : RequestBase
    {
        [Required] public virtual string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public virtual string ConfirmationCode { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.IsNullOrEmpty())
                yield return new ValidationResult(Messages.UserNameIsRequired, new[] {nameof(UserName)});

            if (ConfirmationCode.IsNullOrEmpty())
                yield return new ValidationResult(Messages.ConfirmationCodeRequired, new[] {nameof(ConfirmationCode)});
        }
    }
}