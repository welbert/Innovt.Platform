// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalh�es
// Project: Innovt.Cloud.AWS.Dynamo.Tests
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using Innovt.Cloud.AWS.Configuration;
using Innovt.Core.CrossCutting.Log;

namespace Innovt.Cloud.AWS.Dynamo.Tests
{
    public class BaseRepository : Repository
    {
        public BaseRepository(ILogger logger, IAwsConfiguration configuration) : base(logger, configuration)
        {
        }

        public BaseRepository(ILogger logger, IAwsConfiguration configuration, string region) : base(logger,
            configuration, region)
        {
        }
    }
}