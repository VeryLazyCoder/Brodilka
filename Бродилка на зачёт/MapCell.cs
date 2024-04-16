using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    [Flags]
    public enum MapCell
    {
        Treasure = 'X',
        ArmorBonus = 'A',
        DamageBonus = 'D',
        HealthBonus = 'H',
        AngryDog = '@',
        Empty = ' ',
        HorizontalWall = '-',
        VerticalWall = '|',
    }
}
