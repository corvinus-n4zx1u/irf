using nyolcadik.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nyolcadik.Entities
{
    public class BallFactory : IToyFactory
    {
        public Ball CreateNew()
        {
            return new Ball();
        }
    }
}
