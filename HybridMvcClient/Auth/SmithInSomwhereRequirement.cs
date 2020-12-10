using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridMvcClient.Auth
{
    /// <summary>
    /// 描述性类名
    /// </summary>
    public class SmithInSomwhereRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 构造函数用于传值
        /// </summary>
        public SmithInSomwhereRequirement()
        {

        }
    }
}
