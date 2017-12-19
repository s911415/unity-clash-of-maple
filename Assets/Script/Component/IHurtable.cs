using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUT.CSIE.GameDev.Component
{
    public interface IHurtable
    {
        int MAX_HP
        {
            get;
        }
        bool Alive
        {
            get;
        }
        void Damage(int damage);
        void Recovery(int recover);


    }
}
