using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUT.CSIE.GameDev.Game
{
    public class Difficulty
    {
        public enum Level
        {
            None = 0,
            Easy = 1,
            Normal = 2,
            Hard = 3,
            Demo = 9
        }

        public const int MIN_LEVEL = 1;
        public const int MAX_LEVEL = 3;
    }
}
