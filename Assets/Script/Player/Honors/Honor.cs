using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUT.CSIE.GameDev.Player.Honors
{
    public class Honor
    {
        private string _name;
        public  Honor(string name)
        {
            this._name = name;
        }

        public string Name => _name;

        public override string ToString() => _name;
        public override int GetHashCode() => _name.GetHashCode();

        private static Honor _松果大帝Honor = new Honor("松果大帝");
        private static Honor _衛生股長Honor = new Honor("衛生股長");
        private static Honor _燈泡大師Honor = new Honor("燈泡大師");
        private static Honor _除錯大師Honor = new Honor("除錯大師");
        private static Honor _開發者模式Honor = new Honor("開發者模式");

        public static Honor 松果大帝 => _松果大帝Honor;
        public static Honor 衛生股長 => _衛生股長Honor;
        public static Honor 燈泡大師 => _燈泡大師Honor;
        public static Honor 除錯大師 => _除錯大師Honor;
        public static Honor 開發者模式 => _開發者模式Honor;
    }
}
