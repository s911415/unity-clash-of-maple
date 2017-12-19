using NTUT.CSIE.GameDev.Game;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUT.CSIE.GameDev.Component
{
    public class Explosion : CommonObject
    {
        protected void OnExplosionAnimationFinished()
        {
            Destroy(this.gameObject);
        }
    }
}
