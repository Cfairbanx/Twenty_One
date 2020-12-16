using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino.Interfaces
{
    interface IWalkAway
    {
        //everything is public in an interface, no need to strongly state it

        void WalkAway(Player player);
    }
}
