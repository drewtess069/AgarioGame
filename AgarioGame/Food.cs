using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioGame
{
    internal class Food
    {
        public int x, y;
        public int size = 10;

        public Food (int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public bool collision(Enemy m)
        {
            Rectangle foodRec = new Rectangle(x, y, size, size);
            Rectangle enemyRec = new Rectangle(m.x, m.y, m.size, m.size);

            if (foodRec.IntersectsWith(enemyRec))
            {
                m.size += 1;
                return true;
            }

            return false;
        }

        public bool collision(Player p)
        {
            Rectangle foodRec = new Rectangle(x, y, size, size);
            Rectangle playerRec = new Rectangle(p.x, p.y, p.size, p.size);

            if (foodRec.IntersectsWith(playerRec))
            {
                p.size += 1;
                return true;
            }
            return false;
        }
    }


}

