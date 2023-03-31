using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlTypes;

namespace AgarioGame
{
    public partial class GameScreen : UserControl
    {
        //Set up Lists
        Player hero;
        List<Enemy> enemies = new List<Enemy>();
        List<Food> food = new List<Food>();
        List<int> enemyColor = new List<int>();

        //Create lists and variables for AI movement
        int foodIndex = 0;
        int enemyIndex = 0;
        int foodMax;
        int enemyMax;
        int centerDistancePlayer;

        //Set up keystroke booleans
        bool leftArrowDown = false, rightArrowDown = false, upArrowDown = false, downArrowDown = false;

        public static int heroSize = 100;
        int heroSpeed = 0;

        //Set up variable to track time
        public static int time = 0;

        double newSize;
        int timertick = 0;

        Random randGen = new Random();

        //Create brushes
        SolidBrush greenBrush = new SolidBrush(Color.DarkGreen);
        SolidBrush redBrush = new SolidBrush(Color.DarkRed);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
        SolidBrush yellowBrush = new SolidBrush(Color.Goldenrod);
        SolidBrush purpleBrush = new SolidBrush(Color.Purple);
        SolidBrush brushColor;

        public GameScreen()
        {
            InitializeComponent();
            InitializeGame();
        }

        public void InitializeGame()
        {
            heroSize = 100;

            //Randomize player and enemy spawn
            int x = randGen.Next(10, this.Width - heroSize);
            int y = randGen.Next(10, this.Height - heroSize);
            int size = randGen.Next(20, 100);
            time = 0;

            //Build Enemies and Food
            for (int i = 0; i < 5; i++)
            {
                NewEnemy();
            }


            for (int i = 0; i < 40; i++)
            {
                NewFood();
            }

            hero = new Player(heroSize, heroSpeed, x, y);
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //Set up Key presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;

            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;

            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            timertick++;
            //heroSpeed = 500 / heroSize;

            //make speeds proportional to size
            hero.speed = 800 / hero.size;
            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].xSpeed = 800 / enemies[i].size;
                enemies[i].ySpeed = 800 / enemies[i].size;
            }

            //Create new food
            if (timertick % 12 == 0)
            {
                NewFood();
            }

            //Stopwatch to see how long player takes to win
            if (timertick % 50 == 0)
            {
                time++;
            }

            timeLabel.Text = time.ToString();
            scoreLabel.Text = $"{hero.size}";

            //Move Player
            if (upArrowDown && hero.y > 0)
            {
                hero.Move("up");
            }
            if (leftArrowDown && hero.x > 0)
            {
                hero.Move("left");
            }
            if (rightArrowDown && hero.x < this.Width - hero.size)
            {
                hero.Move("right");
            }
            if (downArrowDown && hero.y < this.Height - hero.size)
            {
                hero.Move("down");
            }

            //move player and enemies away from wall
            if(hero.x < 5)
            {
                hero.x = 5;
            }
            if(hero.x > this.Width - (hero.size + 5))
            {
                hero.x = this.Width - (hero.size + 5);
            }
            if(hero.y < 5)
            {
                hero.y = 5;
            }
            if(hero.y > this.Height - (hero.size + 5))
            {
                hero.y = this.Height - (hero.size + 5);
            }

            foreach(Enemy m in enemies)
            {
                if (m.x < 5)
                {
                    m.x = 5;
                }
                if (m.x > this.Width - (m.size + 5))
                {
                    m.x = this.Width - (m.size + 5);
                }
                if (m.y < 5)
                {
                    m.y = 5;
                }
                if (m.y > this.Height - (m.size + 5))
                {
                    m.y = this.Height - (m.size + 5);
                }
            }

            foreach (Enemy m in enemies)
            {

                //Find center point of enemy
                int enemyCenterX = m.x + (m.size / 2);
                int enemyCenterY = m.y + (m.size / 2);

                //Find closest objects to enemy
                objectDistance(enemyCenterX, enemyCenterY);

                //Compare other objects to determine the closest
                if (foodMax < enemyMax && foodMax < centerDistancePlayer)
                {
                    //Adjust enemy x and y speeds to move towards food if its the closest object
                    if (food[foodIndex].x < m.x)
                    {
                        m.xSpeed = -(Math.Abs(m.xSpeed));
                    }
                    else
                    {
                        m.xSpeed = Math.Abs(m.xSpeed);
                    }

                    if (food[foodIndex].y < m.y)
                    {
                        m.ySpeed = -(Math.Abs(m.ySpeed));
                    }
                    else
                    {
                        m.ySpeed = Math.Abs(m.ySpeed);
                    }
                }
                //Adjust enemy x and y speeds to move towards an enemy if its the closest object
                else if (enemyMax < foodMax && enemyMax < centerDistancePlayer)
                {
                    if (enemies[enemyIndex].x < m.x)
                    {
                        m.xSpeed = -(Math.Abs(m.xSpeed));
                    }
                    else
                    {
                        m.xSpeed = Math.Abs(m.xSpeed);
                    }

                    if (enemies[enemyIndex].y < m.y)
                    {
                        m.ySpeed = -(Math.Abs(m.ySpeed));
                    }
                    else
                    {
                        m.ySpeed = Math.Abs(m.ySpeed);
                    }

                    //If other enemy is bigger run away rather than towards
                    if (enemies[enemyIndex].size > m.size)
                    {
                        m.xSpeed = -m.xSpeed;
                        m.ySpeed = -m.ySpeed;
                    }
                }

                //Move towards or away from hero
                else
                {
                    if (hero.x < m.x)
                    {
                        m.xSpeed = -(Math.Abs(m.xSpeed));
                    }
                    else
                    {
                        m.xSpeed = Math.Abs(m.xSpeed);
                    }

                    if (hero.y < m.y)
                    {
                        m.ySpeed = -(Math.Abs(m.ySpeed));
                    }
                    else
                    {
                        m.ySpeed = Math.Abs(m.ySpeed);
                    }

                    //Change direction if smaller than hero
                    if (hero.size > m.size)
                    {
                        m.xSpeed = -m.xSpeed;
                        m.ySpeed = -m.ySpeed;
                    }
                }
                //Call move function in enemy class
                m.move(this.Width, this.Height);
            }

            //Check if player eats food
            for (int i = 0; i < food.Count; i++)
            {
                food[i].collision(hero);
                if (food[i].collision(hero) == true)
                {
                    food.RemoveAt(i);
                    //NewFood();
                }
            }

            //Check if enemies eat food
            foreach (Enemy m in enemies)
            {
                for (int i = 0; i < food.Count; i++)
                {
                    food[i].collision(m);

                    if (food[i].collision(m) == true)
                    {
                        food.RemoveAt(i);
                        //  NewFood();
                    }
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                //Find center point of enemy 1
                int enemy1CenterX = enemies[i].x + (enemies[i].size / 2);
                int enemy1CenterY = enemies[i].y + (enemies[i].size / 2);

                for (int y = 0; y < enemies.Count; y++)
                {
                    //Find center of enemy 2
                    int enemy2CenterX = enemies[y].x + (enemies[y].size / 2);
                    int enemy2CenterY = enemies[y].y + (enemies[y].size / 2);

                    //Find distance between enemy centers
                    int distance = Convert.ToInt32(Math.Pow(enemy1CenterX - enemy2CenterX, 2) + Math.Pow(enemy1CenterY - enemy2CenterY, 2));
                    int radius = Convert.ToInt32(Math.Sqrt(distance));

                    //Allow slight overlap before enemy is eaten
                    if (radius <= (enemies[i].size / 3 + hero.size / 3))
                    {
                        //Check which one is bigger, remove the other when they collide
                        if (enemies[i].Collision(enemies[y]) == 1)
                        {
                            newSize = enemies[i].size + 0.25 * (enemies[y].size);
                            enemies[i].size = Convert.ToInt32(newSize);
                            enemies.RemoveAt(y);

                            NewEnemy();
                        }
                        else if (enemies[i].Collision(enemies[y]) == 2)
                        {
                            newSize = enemies[y].size + 0.25 * (enemies[i].size);
                            enemies[y].size = Convert.ToInt32(newSize);
                            enemies.RemoveAt(i);

                            NewEnemy();
                        }
                    }
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                //Find enemy and player centers
                int enemyCenterX = enemies[i].x + (enemies[i].size / 2);
                int enemyCenterY = enemies[i].y + (enemies[i].size / 2);

                int playerCenterX = hero.x + (hero.size / 2);
                int playerCenterY = hero.y + (hero.size / 2);

                int distance = Convert.ToInt32(Math.Pow(playerCenterX - enemyCenterX, 2) + Math.Pow(playerCenterY - enemyCenterY, 2));
                int radius = Convert.ToInt32(Math.Sqrt(distance));

                //Allow overlap before elimination of either player or enemy depending on size
                if (radius <= ((enemies[i].size / 3) + (hero.size / 3)))
                {
                    if (enemies[i].collision(hero) == 1)
                    {
                        //Play sound
                        SoundPlayer killPlayer = new SoundPlayer(Properties.Resources.kill);
                        killPlayer.Play();

                        //Change player size
                        newSize = hero.size + 0.25 * (enemies[i].size);
                        hero.size = Convert.ToInt32(newSize);
                        enemies.RemoveAt(i);
                        NewEnemy();
                    }
                    else if (enemies[i].collision(hero) == 2)
                    {
                        //Save hero size for gae stats
                        heroSize = hero.size;

                        //Play death sound
                        SoundPlayer gameOverPlayer = new SoundPlayer(Properties.Resources.GameOverSound);
                        gameOverPlayer.Play();

                        //Open end screen
                        Form1.ChangeScreen(this, new GameOverScreen());

                        //Stop game
                        gameTimer.Stop();
                    }
                }
            }

            //Stop game once player reaches target size
            if (hero.size >= 500)
            {
                gameTimer.Stop();

                //Change to win screen
                Form1.ChangeScreen(this, new WinScreen());
            }

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            //Draw food
            foreach (Food f in food)
            {
                e.Graphics.FillEllipse(greenBrush, f.x, f.y, f.size, f.size);
            }
            //draw enemies
            foreach (Enemy m in enemies)
            {
                e.Graphics.FillEllipse(redBrush, m.x, m.y, m.size, m.size);
            }

            e.Graphics.FillEllipse(whiteBrush, hero.x, hero.y, hero.size, hero.size);
        }

        public void NewEnemy()
        {
            //Find random values for location and size of enemies
            int size = randGen.Next(40, 110);
            int x = randGen.Next(10, this.Width - size);
            int y = randGen.Next(10, this.Height - size);

            //Make speed proportionate to size
            int xSpeed = 300 / size;
            int ySpeed = 300 / size;

            Enemy newEnemy = new Enemy(x, y, size, xSpeed, ySpeed);
            enemies.Add(newEnemy);
        }

        public void NewFood()
        {
            //Find random locations or food
            int x = randGen.Next(10, this.Width);
            int y = randGen.Next(10, this.Height);

            Food newFood = new Food(x, y);

            food.Add(newFood);
        }

        public void objectDistance(int x, int y)
        {
            int playerCenterX = hero.x + (hero.size / 2);
            int playerCenterY = hero.y + (hero.size / 2);

            int distance = Convert.ToInt32(Math.Pow(playerCenterX - x, 2) + Math.Pow(playerCenterY - y, 2));
            centerDistancePlayer = Convert.ToInt32(Math.Sqrt(distance));

            foodMax = 99999;

            for (int i = 0; i < food.Count; i++)
            {

                int foodX = food[i].x + (food[i].size / 2);
                int foodY = food[i].y + (food[i].size / 2);

                distance = Convert.ToInt32(Math.Pow(x - foodX, 2) + Math.Pow(y - foodY, 2));
                int radius = Convert.ToInt32(Math.Sqrt(distance));

                if (radius < foodMax)
                {
                    foodMax = radius;
                    foodIndex = i;
                }
            }

            enemyMax = 99999;
            for (int i = 0; i < enemies.Count; i++)
            {
                int enemyX = enemies[i].x + (enemies[i].size / 2);
                int enemyY = enemies[i].y + (enemies[i].size / 2);

                distance = Convert.ToInt32(Math.Pow(x - enemyX, 2) + Math.Pow(y - enemyY, 2));
                int radius = Convert.ToInt32(Math.Sqrt(distance));

                if (radius < enemyMax && radius != 0)
                {
                    enemyMax = radius;
                    enemyIndex = i;
                }
            }
        }
    }
}
