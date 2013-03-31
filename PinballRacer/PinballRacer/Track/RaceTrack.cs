using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PinballRacer.Track.Walls;
using PinballRacer.Track.Obstacles;

namespace PinballRacer.Track
{
    public class RaceTrack
    {
        public const int TRACK_WIDTH = 50;
        public const int TRACK_HEIGHT = 100;
        //Bottom left and top right corners of the inner walls
        public const int TRACK_WIDTH_IN = 15;
        public const int TRACK_HEIGHT_IN = 30;
        public const int TRACK_WIDTH_OUT = 35;
        public const int TRACK_HEIGHT_OUT = 70;

        ContentManager content;

        List<Obstacle> obstacles;
        List<Wall> walls;
        List<Floor> floors;

        TrackGraph TileGraph;
        TrackGraph NodeGraph;

        public float[,] TerrainMap;
        public enum squareStates { EMPTY = 0, WALL, PLAYER1, PLAYER2, OBSTACLE, GOAL1, LAST};
        public squareStates[,] board;

        public RaceTrack(ContentManager c)
        {
            content = c;
            InitializeFloor();
            InitializeOutterWalls();
            InitializeInnerWalls();
            InitializeObstacles();
        }

        private void InitializeFloor()
        {
            floors = new List<Floor>();
            Model m = content.Load<Model>("floor");
            //Setting up the floor
            for (int i = 0; i < TRACK_WIDTH; ++i)
            {
                for (int j = 0; j < TRACK_HEIGHT; ++j)
                {
                    floors.Add(new Floor(i, j, m));
                }
            }
        }

        private void InitializeOutterWalls()
        {
            TerrainMap = new float[TRACK_WIDTH, TRACK_HEIGHT];
            walls = new List<Wall>();

            for (int i = 0; i < TRACK_WIDTH; ++i)
            {
                for (int j = 0; j < TRACK_HEIGHT; ++j)
                {
                    if (j == 0 || i == 0 || j == TRACK_HEIGHT - 1 || i == TRACK_WIDTH - 1)
                    {
                        AddWall(i, j);
                    }
                }
            }
        }

        private void InitializeInnerWalls()
        {
            for (int i = TRACK_WIDTH_IN; i < TRACK_WIDTH_OUT; ++i)
            {
                for (int j = TRACK_HEIGHT_IN; j < TRACK_HEIGHT_OUT; ++j)
                {
                    if (j == TRACK_HEIGHT_IN || i == TRACK_WIDTH_IN || j == TRACK_HEIGHT_OUT - 1 || i == TRACK_WIDTH_OUT - 1)
                    {
                        AddWall(i, j);
                    }
                }
            }
            for (int k = 1; k < TRACK_HEIGHT_OUT-5; ++k)
            {
                AddWall(TRACK_WIDTH - 4, k);                
            }
            
        }

        private void InitializeObstacles()
        {
            obstacles = new List<Obstacle>();
            obstacles.Add(new Bumper(30, 50, content.Load<Model>("bumper_1")));
            obstacles.Add(new Bumper(10, 70, content.Load<Model>("bumper_1")));
            var bump = new Bumper(20, 30, content.Load<Model>("bumper_1"));
            bump.isHit = true;
            obstacles.Add(bump);
        }

        private void AddWall(int x, int y)
        {
            board = new squareStates[TRACK_WIDTH, TRACK_HEIGHT];
            Model m = content.Load<Model>("cube");
            if (board[x, y] != squareStates.WALL)
            {
                board[x, y] = squareStates.WALL;
                walls.Add(new WallRegular(x, y, m));
                //Add to track graph
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            foreach (Floor f in floors)
            {
                f.draw(view, projection);
            }

            foreach (Wall w in walls)
            {
                w.draw(view, projection);
            }

            foreach (Obstacle o in obstacles)
            {
                o.draw(view, projection);
            }
        }

    }
}
