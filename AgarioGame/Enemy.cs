using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioGame
{
    internal class Enemy
    {
        public int x, y, size, xSpeed, ySpeed;

        public Enemy(int _x, int _y, int _size, int _xSpeed, int _ySpeed)
        {
            x = _x;
            y = _y;
            size = _size;
            xSpeed = _xSpeed;
            ySpeed = _ySpeed;
        }

        public void move(int width, int height)
        {
            //if(fDistance < pDistance && fDistance < eDistance)

            x += xSpeed;
            y += ySpeed;

            if (x > width - size || x < 0)
            {
                if (xSpeed > 0)
                {
                    x = width - size;
                }
                else
                {
                    x = 0;
                }
                xSpeed *= -1;
            }

            if (y > height - size || y < 0)
            {
                if (ySpeed > 0)
                {
                    y = height - size;
                }
                else
                {
                    y = 0;
                }

                ySpeed *= -1;
            }
        }

        public int Collision(Enemy m)
        {
            Rectangle enemyRec = new Rectangle(x, y, size, size);
            Rectangle enemy2Rec = new Rectangle(m.x, m.y, m.size, m.size);

            if (enemyRec.IntersectsWith(enemy2Rec))
            {
                if (size > m.size - 10 && size < m.size + 10)
                {
                    return 3;
                }
                if (size > m.size)
                {
                    return 1;
                }
                else if (size < m.size)
                {
                    return 2;
                }
            }
            return 3;
        }

        public int collision(Player p)
        {
            Rectangle enemyRec = new Rectangle(x, y, size, size);
            Rectangle playerRec = new Rectangle(p.x, p.y, p.size, p.size);

            if (enemyRec.IntersectsWith(playerRec))
            {
                if(size > p.size - 10 && size < p.size + 10)
                {
                    return 3;
                }
                if(size < p.size)
                {
                    return 1;
                }
                else if(size > p.size)
                {
                    return 2;
                }
            }
            return 3;
        }
    }
}
