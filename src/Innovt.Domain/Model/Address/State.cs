﻿// Company: INNOVT
// Project: Innovt.Domain
// Created By: Michel Borges
// Date: 2016/10/18

using System;
using System.Collections.Generic;

namespace Innovt.Domain.Model.Address
{
    public class State : ValueObject
    {
        public string Description { get; set; }

        public string Acronym { get; set; }

        public string UtcOffset { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual IList<City> Cities { get; set; }
    }
}