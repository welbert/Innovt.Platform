﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Cloud
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

namespace Innovt.Cloud
{
    public interface IConfiguration
    {
        string SecretKey { get; set; }

        string AccessKey { get; set; }

        string Region { get; set; }
    }
}