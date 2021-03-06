﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace PokerBot
{
    public class HumanMouseSpinWait : Mouse
    {
        private DeviceControl control;
        private Random random = new Random();

        public HumanMouseSpinWait(DeviceControl control)
        {
            this.control = control;
        }

        public void Move(int newx, int newy)
        {
            // common
            Point startpos = Position;
            int startx = Position.X;
            int starty = Position.Y;
            double stepx = newx - startx;
            double stepy = newy - starty;
            int direction = 0;
            if (Position.X > newx) direction = 90;
            if (Position.X < newx) direction = 270;
            double turnangle = direction + (Math.Atan((-stepy) / (stepx)) / Math.PI * 180.0);
            int totaldist = (int)(Math.Sqrt((stepx * stepx) + (stepy * stepy)));

            if (totaldist > 30 && random.NextDouble() > .5)
            {
                int DISTANCE_BETWEEN_POINTS = 200;
                int MAX_Y_DISTANCE = 50;
                int SIN_WAVE_Y = 10;

                // # points

                // random points
                int pointsCount = (totaldist / DISTANCE_BETWEEN_POINTS);
                // start
                List<Point> points = new List<Point>();
                points.Add(new Point(0, 0));
                // middle center
                for (int i = 1; i <= pointsCount + 1; i++)
                {
                    // # last and next random points
                    int prevX = points[points.Count-1].X;
                    int prevY = points[points.Count-1].Y;
                    int nextX = (int)(i * DISTANCE_BETWEEN_POINTS);
                    int nextY = (int)((random.NextDouble() - .5) * MAX_Y_DISTANCE);

                    // # last point
                    if (nextX >= totaldist)
                    {
                        nextX = totaldist;
                        nextY = 0;
                    }                    

                    // # smooth fill
                    // distance
                    int distanceX = nextX - prevX;
                    int distanceY = nextY - prevY;
                    int distance = (int)(Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY)));

                    // position
                    double posX = prevX;
                    double posY = prevY;
                    double stepX = distanceX / (double) distance;
                    double stepY = distanceY / (double) distance;

                    // down or up
                    bool goDown = nextY < prevY;

                    // adjust amplitude for last point
                    int sinAmplitude = (int)(((double)distance / DISTANCE_BETWEEN_POINTS) * SIN_WAVE_Y);

                    // iterate
                    for (double pos = 0; pos < distance; pos++)
                    {
                        double done = pos / distance;
                        int sinY = (int)(Math.Sin(done * Math.PI) * sinAmplitude);
                        if (goDown) sinY *= -1;
                        posY += stepY;
                        posX += stepX;
                        points.Add(new Point((int)posX, (int)posY + sinY));
                    }
                }

                // # rotation
                List<Point> rotatedPoints = new List<Point>();
                // movements
                foreach (Point point in points)
                {
                    double rotY = starty - (point.X * Math.Cos(turnangle * Math.PI / 180.0) + point.Y * Math.Sin(turnangle * Math.PI / 180.0));
                    double rotX = startx + (-1 * point.X * Math.Sin(turnangle * Math.PI / 180.0) + point.Y * Math.Cos(turnangle * Math.PI / 180.0));
                    rotatedPoints.Add(new Point((int)rotX, (int)rotY));
                }

                // # move
                foreach (Point point in rotatedPoints)
                {
                    control.MouseMoveTo(point.X, point.Y);
                    Thread.SpinWait(RandomInt(100000, 14000));
                }
            }
            else
            {
                // sinus curve
                int amplitude = totaldist / (10 + RandomInt(1, 10));
                int speed = RandomInt(40000, 60000);
                for (int stepsX = 0; stepsX <= totaldist; stepsX++)
                {
                    double pi_step = Math.PI / totaldist;
                    double posy = amplitude * Math.Sin(pi_step * stepsX);
                    double posy1 = starty - (stepsX * Math.Cos(turnangle * Math.PI / 180) + posy * Math.Sin(turnangle * Math.PI / 180));
                    double posx1 = startx + (-1 * stepsX * Math.Sin(turnangle * Math.PI / 180) + posy * Math.Cos(turnangle * Math.PI / 180));
                    control.MouseMoveTo((int) posx1, (int) posy1);
                    double percentage = stepsX / (double)(totaldist == 0 ? 1 : totaldist);
                    RandomSleep(speed, percentage);
                    speed += RandomInt(-10000, +10000);
                }
            }
        }

        private int RandomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        private void RandomSleep(int offset, double percentage)
        {
            double factor = Math.Abs(percentage - 0.5) * 2 + 0.1;            
            int add = (int)((random.NextDouble() * 250000) * factor);
            Thread.SpinWait(offset + add);
        }

        public void MoveAndLeftClick(int x, int y, int rx, int ry)
        {
            int rx2 = (int) (randomNormalScaled(2*rx, 0, 1) - (rx));
            int ry2 = (int) (randomNormalScaled(2*ry, 0, 1) - (ry));
            Move(x + rx2, y + ry2);
            Thread.Sleep(RandomInt(250, 300));
            LeftClick();
        }

        public void LeftClick()
        {
            control.MouseLeftClick();
        }

        public void RightClick()
        {
            control.MouseRightClick();
        }

        public void DoubleLeftClick()
        {
            control.MouseDoubleLeftClick();
        }

        public Point Position
        {
            get { return control.MousePosition; }
        }

        // ## random normal distribution ##

        private bool uselast = false;
        private double y2 = 0;

        private double randomNormal(double m, double s)
        {
            /* normal random variate generator */
            /* mean m, standard deviation s */
            double x1, x2, w, y1;

            /* use value from previous call */
            if (uselast) { y1 = y2; uselast = false; }
            else
            {
                do
                {
                    x1 = 2.0 * random.NextDouble() - 1.0;
                    x2 = 2.0 * random.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                uselast = true;
            }
            return (m + y1 * s);
        }

        // random number - 0 -> scale, with normal distribution
        // ignore results outside 3 stds from the mean
        private double randomNormalScaled(double scale, double m, double s)
        {
            double res = -99;
            while (res < -3.5 || res > 3.5) res = randomNormal(m, s);
            return (res / 3.5 * s + 1) * (scale / 2.0);
        }
    }
}
