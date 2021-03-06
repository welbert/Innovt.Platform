﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Cloud.AWS.Cognito
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Innovt.Cloud.AWS.Cognito.Model
{
    public abstract class RequestBase : IRequestBase
    {
#pragma warning disable CA2227 // Collection properties should be read only
        [JsonIgnore] public Dictionary<string, string> HttpHeader { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        [JsonIgnore] public string IpAddress { get; set; }

        [JsonIgnore] public string ServerPath { get; set; }

        [JsonIgnore] public string ServerName { get; set; }

        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}