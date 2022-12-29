using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Western
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>

    public partial class Game : Window
    {
        //general variables
        static double angle_rad;
        static double x1_ori;
        static double y1_ori;
        static double x2_ori;
        static double y2_ori;
        static double x3_ori;
        static double y3_ori;
        static double speed = 0.4;
        static double bullet_speed = 1;
        static double CosA;
        static double SinA;
        static double deltaTime = 1;
        static bool last_bullet_collised = false;
        static double max_height = 750;
        static double max_width = 600;
        static double min_height = 50;
        static double min_width = 50;
        static double players_scale = 0;

        //player 1 variables
        static int angle_p1 = 0;
        static double x0_p1 = min_width + 50;
        static double y0_p1 = max_height - 50;
        static double x1_p1 = x0_p1 - 15 * players_scale; //left
        static double y1_p1 = y0_p1 + 27 * players_scale;
        static double x2_p1 = x0_p1; //nose
        static double y2_p1 = y0_p1 - 8 * players_scale;
        static double x3_p1 = x0_p1 + 15 * players_scale; //right
        static double y3_p1 = y0_p1 + 27 * players_scale;
        static double velocityX_p1 = 0;
        static double velocityY_p1 = 0;
        static int bullet_p1_count = 0;
        Polygon p1 = new Polygon();
        Point left_p1 = new Point(x1_p1, y1_p1);
        Point nose_p1 = new Point(x2_p1, y2_p1);
        Point right_p1 = new Point(x3_p1, y3_p1);
        static bool rotation_p1 = false;
        static int points_per_round_p1 = 0;
        static int rounds_win_count_p1 = 0;

        //bullet of player 1
        static bool shot_p1 = false;

        //player 2 variables
        static int angle_p2 = 180;
        static double x0_p2 = max_width - 50;
        static double y0_p2 = min_height + 50;
        static double x1_p2 = x0_p2 - 15 * players_scale; //left
        static double y1_p2 = y0_p2 - 27 * players_scale;
        static double x2_p2 = x0_p2; //nose
        static double y2_p2 = y0_p2 + 8 * players_scale;
        static double x3_p2 = x0_p2 + 15 * players_scale; //right
        static double y3_p2 = y0_p2 - 27 * players_scale;
        static double velocityX_p2 = 0;
        static double velocityY_p2 = 0; 
        static int bullet_p2_count = 0;
        Polygon p2 = new Polygon();
        Point left_p2 = new Point(x1_p2, y1_p2);
        Point nose_p2 = new Point(x2_p2, y2_p2);
        Point right_p2 = new Point(x3_p2, y3_p2);
        static bool rotation_p2 = false;
        static int points_per_round_p2 = 0;
        static int rounds_win_count_p2 = 0;

        //bullet of player 2
        static bool shot_p2 = false;

        //general bullet
        static double[] bullet_velocityX = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static double[] bullet_velocityY = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<Point> bullet_coord1 = new List<Point>();
        List<Point> bullet_coord2 = new List<Point>();
        List<Polygon> bullet = new List<Polygon>();
        static bool[] IsBulletFlying = {false, false, false, false, false, false, false, false, false, false, false, false };
        int bulletc = -1;
        static bool shotable = true;

        public Game()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(MainGameTimerEvent);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
            match_end_text.IsEnabled = true;

            switch (MainWindow.difficulty)
            {
                case 1: players_scale = 1.8; break;
                case 2: players_scale = 1.4; break;
                case 3: players_scale = 1; break;
            }

            x1_p1 = x0_p1 - 15 * players_scale;
            y1_p1 = y0_p1 + 27 * players_scale;
            x2_p1 = x0_p1;
            y2_p1 = y0_p1 - 8 * players_scale;
            x3_p1 = x0_p1 + 15 * players_scale;
            y3_p1 = y0_p1 + 27 * players_scale;

            x1_p2 = x0_p2 - 15 * players_scale;
            y1_p2 = y0_p2 - 27 * players_scale;
            x2_p2 = x0_p2;
            y2_p2 = y0_p2 + 8 * players_scale;
            x3_p2 = x0_p2 + 15 * players_scale;
            y3_p2 = y0_p2 - 27 * players_scale;

            //отрисовка игроков
            p1.Points.Add(left_p1);
            p1.Points.Add(nose_p1);
            p1.Points.Add(right_p1);
            p1.Stroke = Brushes.White;
            p1.Fill = Brushes.Black;
            Field.Children.Add(p1);

            p2.Points.Add(left_p2);
            p2.Points.Add(nose_p2);
            p2.Points.Add(right_p2);
            p2.Stroke = Brushes.Black;
            p2.Fill = Brushes.White;
            Field.Children.Add(p2);

            rounds_text_p1.Text = points_per_round_p1.ToString();
            rounds_text_p2.Text = points_per_round_p2.ToString();
        }

        private void rotate_p1()
        {
            //вычисление положения точки поворота
            x1_ori = x0_p1 - 15 * players_scale;
            y1_ori = y0_p1 + 27 * players_scale;
            x2_ori = x0_p1;
            y2_ori = y0_p1 - 8 * players_scale;
            x3_ori = x0_p1 + 15 * players_scale;
            y3_ori = y0_p1 + 27 * players_scale;
            //поворот
            CosA = Math.Round(Math.Cos(angle_rad), 5);
            SinA = Math.Round(Math.Sin(angle_rad), 5);
            x2_p1 = (x2_ori - x0_p1) * CosA - (y2_ori - y0_p1) * SinA + x0_p1;
            y2_p1 = (x2_ori - x0_p1) * SinA + (y2_ori - y0_p1) * CosA + y0_p1;
            x1_p1 = (x1_ori - x0_p1) * CosA - (y1_ori - y0_p1) * SinA + x0_p1;
            y1_p1 = (x1_ori - x0_p1) * SinA + (y1_ori - y0_p1) * CosA + y0_p1;
            x3_p1 = (x3_ori - x0_p1) * CosA - (y3_ori - y0_p1) * SinA + x0_p1;
            y3_p1 = (x3_ori - x0_p1) * SinA + (y3_ori - y0_p1) * CosA + y0_p1;
            //движение игроков 
            velocityX_p1 = (x2_p1 - x0_p1) * speed * deltaTime;
            velocityY_p1 = (y2_p1 - y0_p1) * speed * deltaTime;
        }
        private void rotate_p2()
        {
            x1_ori = x0_p2 - 15 * players_scale;
            y1_ori = y0_p2 + 27 * players_scale;
            x2_ori = x0_p2;
            y2_ori = y0_p2 - 8 * players_scale;
            x3_ori = x0_p2 + 15 * players_scale;
            y3_ori = y0_p2 + 27 * players_scale;

            CosA = Math.Round(Math.Cos(angle_rad), 5);
            SinA = Math.Round(Math.Sin(angle_rad), 5);
            x2_p2 = (x2_ori - x0_p2) * CosA - (y2_ori - y0_p2) * SinA + x0_p2;
            y2_p2 = (x2_ori - x0_p2) * SinA + (y2_ori - y0_p2) * CosA + y0_p2;
            x1_p2 = (x1_ori - x0_p2) * CosA - (y1_ori - y0_p2) * SinA + x0_p2;
            y1_p2 = (x1_ori - x0_p2) * SinA + (y1_ori - y0_p2) * CosA + y0_p2;
            x3_p2 = (x3_ori - x0_p2) * CosA - (y3_ori - y0_p2) * SinA + x0_p2;
            y3_p2 = (x3_ori - x0_p2) * SinA + (y3_ori - y0_p2) * CosA + y0_p2;

            velocityX_p2 = (x2_p2 - x0_p2) * speed * deltaTime;
            velocityY_p2 = (y2_p2 - y0_p2) * speed * deltaTime;
        }

        //функции сброса позиций игроков
        private void reset_p1()
        {
            angle_p1 = 0;
            x0_p1 = min_width + 50;
            y0_p1 = max_height - 50;
            x1_p1 = x0_p1 - 15 * players_scale; //left
            y1_p1 = y0_p1 + 27 * players_scale;
            x2_p1 = x0_p1; //nose
            y2_p1 = y0_p1 - 8 * players_scale;
            x3_p1 = x0_p1 + 15 * players_scale; //right
            y3_p1 = y0_p1 + 27 * players_scale;
            velocityX_p1 = 0;
            velocityY_p1 = 0;
            rotation_p1 = false;
        }

        private void reset_p2()
        {
            angle_p2 = 180;
            x0_p2 = max_width - 50;
            y0_p2 = min_height + 50;
            x1_p2 = x0_p2 - 15 * players_scale; //left
            y1_p2 = y0_p2 - 27 * players_scale;
            x2_p2 = x0_p2; //nose
            y2_p2 = y0_p2 + 8 * players_scale;
            x3_p2 = x0_p2 + 15 * players_scale; //right
            y3_p2 = y0_p2 - 27 * players_scale;
            velocityX_p2 = 0;
            velocityY_p2 = 0;
            rotation_p2 = false;
        }
        //сброс настроек пули
        private void reset_bullet()
        {
            for (int i = 0; i <= 11; i++)
            {
                bullet_velocityX[i] = 0;
                bullet_velocityY[i] = 0;
                IsBulletFlying[i] = false;
                Field.Children.Remove(bullet[i]);
                bullet[i].Points.Remove(bullet_coord1[i]);
                bullet[i].Points.Remove(bullet_coord2[i]); 
            }           
            bullet_coord1.Clear();
            bullet_coord2.Clear();
            bullet.Clear();
            bullet_coord1.Add(new Point());
            bullet_coord2.Add(new Point());
            bullet.Add(new Polygon());
            bulletc = -1;
            bullet_p1_count = 0;
            bullet_p2_count = 0;
            last_bullet_collised = false;
        }
        
        private void general_reset()
        {            
            rounds_win_count_p1 = 0;
            rounds_win_count_p2 = 0;
            points_per_round_p1 = 0;
            points_per_round_p2 = 0;
            bullet_p1_count = 0;
            bullet_p2_count = 0;
            shotable = true;
            bullet.Clear();
            
            rounds_p1_1.Fill = default;
            rounds_p1_2.Fill = default;
            rounds_p1_3.Fill = default;
            rounds_p1_4.Fill = default;
            rounds_p1_5.Fill = default;
            rounds_p2_1.Fill = default;
            rounds_p2_2.Fill = default;
            rounds_p2_3.Fill = default;
            rounds_p2_4.Fill = default;
            rounds_p2_5.Fill = default;
        }
        
        //функция появления пули
        private void Bullet()
        {
            if (shot_p1 && velocityX_p1*velocityY_p1 != 0)
            {
                bullet_p1_count++;
                bulletc++;
                bullet_coord1.Add(new Point());
                bullet_coord2.Add(new Point());
                bullet.Add(new Polygon());
                IsBulletFlying[bulletc] = true;
                bullet_coord1[bulletc] = nose_p1;
                bullet_coord2[bulletc] = new Point(nose_p1.X + (nose_p1.X - x0_p1) * 0.2, nose_p1.Y + (nose_p1.Y - y0_p1) * 0.2);
                bullet[bulletc] = new Polygon();

                bullet_velocityX[bulletc] = (x2_p1 - x0_p1) * bullet_speed;
                bullet_velocityY[bulletc] = (y2_p1 - y0_p1) * bullet_speed;

                bullet[bulletc].Points.Add(bullet_coord1[bulletc]);
                bullet[bulletc].Points.Add(bullet_coord2[bulletc]);
                bullet[bulletc].Stroke = Brushes.Black;
                Field.Children.Add(bullet[bulletc]);
                shot_p1 = false;
            }

            if (shot_p2 && velocityX_p2 * velocityY_p2 != 0)
            {
                bullet_p2_count++;
                bulletc++;
                bullet_coord1.Add(new Point());
                bullet_coord2.Add(new Point());
                bullet.Add(new Polygon());
                IsBulletFlying[bulletc] = true;
                bullet_coord1[bulletc] = nose_p2;
                bullet_coord2[bulletc] = new Point(nose_p2.X + (nose_p2.X - x0_p2) * 0.2, nose_p2.Y + (nose_p2.Y - y0_p2) * 0.2);
                bullet.Add(new Polygon());

                bullet_velocityX[bulletc] = (x2_p2 - x0_p2) * bullet_speed;
                bullet_velocityY[bulletc] = (y2_p2 - y0_p2) * bullet_speed;

                bullet[bulletc].Points.Add(bullet_coord1[bulletc]);
                bullet[bulletc].Points.Add(bullet_coord2[bulletc]);
                bullet[bulletc].Stroke = Brushes.Black;
                Field.Children.Add(bullet[bulletc]);
                shot_p2 = false;
            }
        }

        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            if (rotation_p1)
                {
                    angle_p1 += 3;
                    if (angle_p1 >= 360) angle_p1 -= 360;
                    angle_rad = (angle_p1 * Math.PI) / 180;
                    rotate_p1();
                }

                if (rotation_p2)
                {
                    angle_p2 += 3;
                    if (angle_p2 >= 360) angle_p2 -= 360;
                    angle_rad = (angle_p2 * Math.PI) / 180;
                    rotate_p2();
                }
                //коллизии со стенками
                if (x0_p1 - 12 <= min_width || x0_p1 + 12 >= max_width)
                {
                    velocityX_p1 = 0;
                    if ((x0_p1 - 12 <= min_width && (x2_p1 - x0_p1) > 0) || (x0_p1 + 12 >= max_width && (x2_p1 - x0_p1) < 0))
                        velocityX_p1 = (x2_p1 - x0_p1) * speed * deltaTime;
                    if (x2_p1 - x0_p1 == 0)
                        velocityX_p1 = 0;
                }
                if (y0_p1 - 12 <= min_height || y0_p1 + 12 >= max_height)
                {
                    velocityY_p1 = 0;
                    if ((y0_p1 - 12 <= min_height && y2_p1 - y0_p1 > 0 || y0_p1 + 12 >= max_height && y2_p1 - y0_p1 < 0))
                        velocityY_p1 = (y2_p1 - y0_p1) * speed * deltaTime;
                    if (y2_p1 - y0_p1 == 0)
                        velocityY_p1 = 0;
                }

                if (x0_p2 - 12 <= min_width || x0_p2 + 12 >= max_width)
                {
                    velocityX_p2 = 0;
                    if ((x0_p2 - 12 <= min_width && (x2_p2 - x0_p2) > 0) || (x0_p2 + 12 >= max_width && (x2_p2 - x0_p2) < 0))
                        velocityX_p2 = (x2_p2 - x0_p2) * speed * deltaTime;
                    if (x2_p2 - x0_p2 == 0)
                        velocityX_p2 = 0;
                }
                if (y0_p2 - 12 <= min_height || y0_p2 + 12 >= max_height)
                {
                    velocityY_p2 = 0;
                    if ((y0_p2 - 12 <= min_height && y2_p2 - y0_p2 > 0 || y0_p2 + 12 >= max_height && y2_p2 - y0_p2 < 0))
                        velocityY_p2 = (y2_p2 - y0_p2) * speed * deltaTime;
                    if (y2_p2 - y0_p2 == 0)
                        velocityY_p2 = 0;
                }

                for (int i = 0; i < 12; i++)
                {
                    if (IsBulletFlying[i])
                    {
                        //пересчёт положения пули, её отрисовка
                        Field.Children.Remove(bullet[i]);
                        bullet[i].Points.Remove(bullet_coord1[i]);
                        bullet[i].Points.Remove(bullet_coord2[i]);

                        Point costc1 = new Point(bullet_coord1[i].X + bullet_velocityX[i] * deltaTime, bullet_coord1[i].Y + bullet_velocityY[i] * deltaTime);
                        Point costc2 = new Point(bullet_coord2[i].X + bullet_velocityX[i] * deltaTime, bullet_coord2[i].Y + bullet_velocityY[i] * deltaTime);

                        bullet_coord1[i] = costc1;
                        bullet_coord2[i] = costc2;

                        bullet[i].Points.Add(bullet_coord1[i]);
                        bullet[i].Points.Add(bullet_coord2[i]);
                        Field.Children.Add(bullet[i]);

                        if (costc2.X <= min_width || costc2.Y <= min_height || costc2.X >= max_width || costc2.Y >= max_height)
                        {
                            Field.Children.Remove(bullet[i]);
                            bullet[i].Points.Remove(bullet_coord1[i]);
                            bullet[i].Points.Remove(bullet_coord2[i]);
                            IsBulletFlying[i] = false;
                            if (bulletc == 11) last_bullet_collised = true;
                        }

                        double a_collision_p1 = (x1_p1 - bullet_coord1[i].X) * (y2_p1 - y1_p1) - (x2_p1 - x1_p1) * (y1_p1 - bullet_coord1[i].Y);
                        double b_collision_p1 = (x2_p1 - bullet_coord1[i].X) * (y3_p1 - y2_p1) - (x3_p1 - x2_p1) * (y2_p1 - bullet_coord1[i].Y);
                        double c_collision_p1 = (x3_p1 - bullet_coord1[i].X) * (y1_p1 - y3_p1) - (x1_p1 - x3_p1) * (y3_p1 - bullet_coord1[i].Y);

                        double a_collision_p2 = (x1_p2 - bullet_coord1[i].X) * (y2_p2 - y1_p2) - (x2_p2 - x1_p2) * (y1_p2 - bullet_coord1[i].Y);
                        double b_collision_p2 = (x2_p2 - bullet_coord1[i].X) * (y3_p2 - y2_p2) - (x3_p2 - x2_p2) * (y2_p2 - bullet_coord1[i].Y);
                        double c_collision_p2 = (x3_p2 - bullet_coord1[i].X) * (y1_p2 - y3_p2) - (x1_p2 - x3_p2) * (y3_p2 - bullet_coord1[i].Y);

                        //коллизия с пулей
                        if (a_collision_p1 >= 0 && b_collision_p1 >= 0 && c_collision_p1 >= 0 || a_collision_p1 <= 0 && b_collision_p1 <= 0 && c_collision_p1 <= 0)
                        {
                            reset_p1();
                            points_per_round_p2++;
                            
                            rounds_text_p2.Text = points_per_round_p2.ToString();
                            Field.Children.Remove(bullet[i]);
                            bullet[i].Points.Remove(bullet_coord1[i]);
                            bullet[i].Points.Remove(bullet_coord2[i]);
                            IsBulletFlying[i] = false;
                            if (bulletc == 11) last_bullet_collised = true;
                        }

                        if (a_collision_p2 >= 0 && b_collision_p2 >= 0 && c_collision_p2 >= 0 || a_collision_p2 <= 0 && b_collision_p2 <= 0 && c_collision_p2 <= 0)
                        {
                            reset_p2();
                            points_per_round_p1++;
                            
                            rounds_text_p1.Text = points_per_round_p1.ToString();
                            Field.Children.Remove(bullet[i]);
                            bullet[i].Points.Remove(bullet_coord1[i]);
                            bullet[i].Points.Remove(bullet_coord2[i]);
                            IsBulletFlying[i] = false;
                            if (bulletc == 11) last_bullet_collised = true;
                        }
                    }
                }

                

                Field.Children.Remove(p1);
                p1.Points.Remove(left_p1);
                p1.Points.Remove(nose_p1);
                p1.Points.Remove(right_p1);

                Field.Children.Remove(p2);
                p2.Points.Remove(left_p2);
                p2.Points.Remove(nose_p2);
                p2.Points.Remove(right_p2);

                x0_p1 += velocityX_p1;
                y0_p1 += velocityY_p1;
                x2_p1 += velocityX_p1;
                y2_p1 += velocityY_p1;
                x1_p1 += velocityX_p1;
                y1_p1 += velocityY_p1;
                x3_p1 += velocityX_p1;
                y3_p1 += velocityY_p1;

                x0_p2 += velocityX_p2;
                y0_p2 += velocityY_p2;
                x2_p2 += velocityX_p2;
                y2_p2 += velocityY_p2;
                x1_p2 += velocityX_p2;
                y1_p2 += velocityY_p2;
                x3_p2 += velocityX_p2;
                y3_p2 += velocityY_p2;

                left_p1.X = x1_p1;
                left_p1.Y = y1_p1;
                nose_p1.X = x2_p1;
                nose_p1.Y = y2_p1;
                right_p1.X = x3_p1;
                right_p1.Y = y3_p1;

                left_p2.X = x1_p2;
                left_p2.Y = y1_p2;
                nose_p2.X = x2_p2;
                nose_p2.Y = y2_p2;
                right_p2.X = x3_p2;
                right_p2.Y = y3_p2;

                p1.Points.Add(left_p1);
                p1.Points.Add(nose_p1);
                p1.Points.Add(right_p1);
                Field.Children.Add(p1);

                p2.Points.Add(left_p2);
                p2.Points.Add(nose_p2);
                p2.Points.Add(right_p2);
                Field.Children.Add(p2);


            if (bulletc == 11 && last_bullet_collised)
                {
                    reset_p1();
                    reset_p2();
                    reset_bullet();

                    if (points_per_round_p1 > points_per_round_p2)
                    {
                        rounds_win_count_p1++;
                        switch (rounds_win_count_p1)
                        {
                            case 1:
                                rounds_p1_1.Fill = Brushes.Black; break;
                            case 2:
                                rounds_p1_2.Fill = Brushes.Black; break;
                            case 3:
                                rounds_p1_3.Fill = Brushes.Black; break;
                            case 4:
                                rounds_p1_4.Fill = Brushes.Black; break;
                            case 5:
                                rounds_p1_5.Fill = Brushes.Black; break;
                        }
                    }

                    if (points_per_round_p2 > points_per_round_p1)
                    {
                        rounds_win_count_p2++;
                        switch (rounds_win_count_p2)
                        {
                            case 1:
                                rounds_p2_1.Fill = Brushes.White; break;
                            case 2:
                                rounds_p2_2.Fill = Brushes.White; break;
                            case 3:
                                rounds_p2_3.Fill = Brushes.White; break;
                            case 4:
                                rounds_p2_4.Fill = Brushes.White; break;
                            case 5:
                                rounds_p2_5.Fill = Brushes.White; break;
                        }
                    }
                    points_per_round_p1 = 0;
                    points_per_round_p2 = 0;
                    rounds_text_p1.Text = points_per_round_p1.ToString();
                    rounds_text_p2.Text = points_per_round_p2.ToString();
                    if (rounds_win_count_p1 == 5)
                    {                    
                        match_end_text.Content = "Player 1 wins";
                        res_butt.Visibility = Visibility.Visible;
                        exit_butt.Visibility = Visibility.Visible;
                        shotable = false;
                    }
                    if (rounds_win_count_p2 == 5)
                    {                      
                        match_end_text.Content = "Player 2 wins";
                        res_butt.Visibility = Visibility.Visible;
                        exit_butt.Visibility = Visibility.Visible;
                        shotable = false;
                    }
                }

        }
        
        public void Game1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "d" || e.Key.ToString() == "D") rotation_p1 = !rotation_p1;
            if (e.Key == Key.Right) rotation_p2 = !rotation_p2;

            if ((e.Key.ToString() == "w" || e.Key.ToString() == "W") && bullet_p1_count < 6 && shotable)
            {
                shot_p1 = true;
                Bullet();
            }
            if (e.Key == Key.Up && bullet_p2_count < 6 && shotable)
            {
                shot_p2 = true;
                Bullet();
            }
        }

        private void res_butt_Click(object sender, RoutedEventArgs e)
        {
            general_reset();         
            match_end_text.Content = "";
            res_butt.Visibility = Visibility.Hidden;
            exit_butt.Visibility = Visibility.Hidden;
        }

        private void exit_butt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
