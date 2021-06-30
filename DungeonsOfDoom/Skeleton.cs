using System;
using System.Collections.Generic;
using System.Text;

namespace DungeonsOfDoom
{
    class Skeleton : Monster
    {
        public Skeleton() : base(5)
        {
        }

        public override void Attack(Character opponent)
        {
            if (opponent.Health < 30)
                opponent.Health -= 5;
        }
    }
}
