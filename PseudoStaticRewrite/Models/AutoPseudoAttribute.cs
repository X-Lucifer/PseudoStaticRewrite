using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PseudoStaticRewrite.Models
{
    /// <summary>
    /// 自动伪静态属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class AutoPseudoAttribute : Attribute
    {

    }
}
