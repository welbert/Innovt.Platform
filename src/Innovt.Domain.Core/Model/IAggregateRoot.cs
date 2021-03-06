﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Domain.Core
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

namespace Innovt.Domain.Core.Model
{
    public interface IAggregateRoot<T>
    {
        public T Id { get; set; }
    }

    public interface IAggregateRoot
    {
        public int Id { get; set; }
    }
}