using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Cores
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class TinyException : Exception
    {
        public TinyException(string message) : base(message)
        {

        }
    }
}
