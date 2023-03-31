using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgarioGame
{
    public partial class GameOverScreen : UserControl
    {
        public GameOverScreen()
        {
            InitializeComponent();
            titleLabel.Text = "Game Over";

            descriptionLabel.Text = $"Your final score was {GameScreen.heroSize} \n\nWould you like to play again?";
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            Agario.ChangeScreen(this, new MenuScreen());
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
