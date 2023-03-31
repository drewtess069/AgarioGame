using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioGame
{
    internal class Player
    {
        public int size, speed, x, y;

        public Player(int _size, int _speed, int _x, int _y)
        {
            size = _size;
            speed = _speed;
            x = _x;
            y = _y;
        }

        public void Move(string direction)
        {
            if (direction == "up")
            {
                y -= speed;
            }
            if (direction == "down")
            {
                y += speed;
            }
            if (direction == "right")
            {
                x += speed;
            }
            if (direction == "left")
            {
                x -= speed;
            }
        }
    }
}
