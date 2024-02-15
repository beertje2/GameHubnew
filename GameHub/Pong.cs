using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace GameHub
{
    public partial class Pong : Form
    {
        private const int PaddleWidth = 10;
        private const int PaddleHeight = 70;
        private const int BallSize = 10;
        private const int PaddleSpeed = 10;
        private const int BallSpeed = 8;

        private int player1Score = 0;
        private int player2Score = 0;

        private Rectangle player1Paddle;
        private Rectangle player2Paddle;
        private Rectangle ball;

        private int player1PaddleSpeed = 0;
        private int player2PaddleSpeed = 0;
        private int ballSpeedX = BallSpeed;
        private int ballSpeedY = BallSpeed;

        private Timer gameTimer;

        private SoundPlayer player1Sound;
        private SoundPlayer player2Sound;
        private SoundPlayer goalScoredSound;

        public Pong()
        {
            InitializeComponent();
            InitializeGame();
            InitializeSounds();
        }

        private void InitializeGame()
        {
            this.Width = 600;
            this.Height = 900;

            player1Paddle = new Rectangle(ClientSize.Width / 2 - PaddleHeight / 2, 20, PaddleHeight, PaddleWidth);
            player2Paddle = new Rectangle(ClientSize.Width / 2 - PaddleHeight / 2, ClientSize.Height - 30, PaddleHeight, PaddleWidth);
            ball = new Rectangle(ClientSize.Width / 2 - BallSize / 2, ClientSize.Height / 2 - BallSize / 2, BallSize, BallSize);

            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void InitializeSounds()
        {
            player1Sound = new SoundPlayer(Properties.Resources.paddle1);
            player2Sound = new SoundPlayer(Properties.Resources.paddle2);
            goalScoredSound = new SoundPlayer(Properties.Resources.victory);
        }

        private void PlaySound(bool player1Hit)
        {
            if (player1Hit)
                player1Sound.Play();
            else
                player2Sound.Play();
        }

        private void PlayGoalScoredSound()
        {
            goalScoredSound.Play();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MovePaddles();
            MoveBall();
            CheckCollision();
            Invalidate();
        }

        private void MovePaddles()
        {
            player1Paddle.X += player1PaddleSpeed;
            player2Paddle.X += player2PaddleSpeed;

            player1Paddle.X = Math.Max(0, Math.Min(ClientSize.Width - PaddleHeight, player1Paddle.X));
            player2Paddle.X = Math.Max(0, Math.Min(ClientSize.Width - PaddleHeight, player2Paddle.X));
        }

        private void MoveBall()
        {
            ball.X += ballSpeedX;
            ball.Y += ballSpeedY;

            if (ball.Left <= 0 || ball.Right >= ClientSize.Width)
                ballSpeedX = -ballSpeedX;

            if (ball.Top <= 0 || ball.Bottom >= ClientSize.Height)
                ballSpeedY = -ballSpeedY;

            if (ball.IntersectsWith(player1Paddle))
            {
                PlaySound(true);
                ballSpeedY = -ballSpeedY;
            }
            else if (ball.IntersectsWith(player2Paddle))
            {
                PlaySound(false);
                ballSpeedY = -ballSpeedY;
            }

            if (ball.Top <= 0)
            {
                player2Score++;
                PlayGoalScoredSound();
                ResetBall();
                ResetPaddles();
            }
            else if (ball.Bottom >= ClientSize.Height)
            {
                player1Score++;
                PlayGoalScoredSound();
                ResetBall();
                ResetPaddles();
            }
        }

        private void ResetBall()
        {
            ball.X = ClientSize.Width / 2 - BallSize / 2;
            ball.Y = ClientSize.Height / 2 - BallSize / 2;
            ballSpeedX = BallSpeed;
            ballSpeedY = BallSpeed;
        }

        private void ResetPaddles()
        {
            player1Paddle.X = ClientSize.Width / 2 - PaddleHeight / 2;
            player2Paddle.X = ClientSize.Width / 2 - PaddleHeight / 2;
        }

        private void CheckCollision()
        {
            if (player1Paddle.Left <= 0 || player1Paddle.Right >= ClientSize.Width)
                player1PaddleSpeed = 0;

            if (player2Paddle.Left <= 0 || player2Paddle.Right >= ClientSize.Width)
                player2PaddleSpeed = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.FillRectangle(Brushes.Black, player1Paddle);
            g.FillRectangle(Brushes.Black, player2Paddle);
            g.FillEllipse(Brushes.Red, ball);

            string player1Text = "Player 1: " + player1Score;
            SizeF player1TextSize = g.MeasureString(player1Text, Font);
            float player1TextX = 20;
            float player1TextY = (ClientSize.Height - player1TextSize.Height) / 2;
            g.DrawString(player1Text, Font, Brushes.Black, player1TextX, player1TextY);

            string player2Text = "Player 2: " + player2Score;
            SizeF player2TextSize = g.MeasureString(player2Text, Font);
            float player2TextX = ClientSize.Width - player2TextSize.Width - 20;
            float player2TextY = (ClientSize.Height - player2TextSize.Height) / 2;
            g.DrawString(player2Text, Font, Brushes.Black, player2TextX, player2TextY);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.A)
                player1PaddleSpeed = -PaddleSpeed;
            if (e.KeyCode == Keys.D)
                player1PaddleSpeed = PaddleSpeed;
            if (e.KeyCode == Keys.Left)
                player2PaddleSpeed = -PaddleSpeed;
            if (e.KeyCode == Keys.Right)
                player2PaddleSpeed = PaddleSpeed;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.D)
                player1PaddleSpeed = 0;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                player2PaddleSpeed = 0;
        }
    }
}
