using System;
using System.Collections.Generic;
using System.Linq;

namespace Tinyfx.Models
{
    public class User
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}
