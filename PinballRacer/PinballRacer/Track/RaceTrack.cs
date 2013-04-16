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
        static int ID = 1;
        public const int TRACK_WIDTH = 50;
        public const int TRACK_HEIGHT = 100;
        //Bottom left and top right corners of the inner walls
        public const int TRACK_WIDTH_IN = 15;
        public const int TRACK_HEIGHT_IN = 30;
        public const int TRACK_WIDTH_OUT = 32;
        public const int TRACK_HEIGHT_OUT = 80;

        ContentManager content;
        Model spring;
        public float springLevel;

        public Dictionary<int, Obstacle> obstacles;
        List<Wall> walls;
        List<Floor> floors;

        TrackGraph TileGraph;
        TrackGraph NodeGraph;

        public float[,] TerrainMap;
        public enum squareStates { EMPTY = 0, WALL, PLAYER1, PLAYER2, OBSTACLE, GOAL1, LAST };
        public enum trackStates { PLAYING = 0, START, GAMEOVER };
        public squareStates[,] board;
        public int[,] tiles;

        public RaceTrack(ContentManager c)
        {
            obstacles = new Dictionary<int, Obstacle>();
            tiles = new int[TRACK_WIDTH+1, TRACK_HEIGHT+1];
            board = new squareStates[TRACK_WIDTH+1, TRACK_HEIGHT+1];

            content = c;
            InitializeObstacles();
            InitializeFloor();
            InitializeOutterWalls();
            InitializeInnerWalls();
            spring = content.Load<Model>("spring");
            springLevel = 0.5f;
        }

        private void InitializeFloor()
        {
            floors = new List<Floor>();
            Model m = content.Load<Model>("cube");
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
                        AddWallBumper(i, j);
                    }
                }
            }
            for (int k = 1; k < TRACK_HEIGHT_OUT-5; ++k)
            {
                AddWall(TRACK_WIDTH - 4, k);                
            }
            for (int l = 10; l < 20; ++l)
            {
                AddWall(5, l);
            } 
            for (int r = 10; r < 20; ++r)
            {
                AddWall(TRACK_WIDTH - 9, r);
            }
        }

        private void InitializeObstacles()
        {
            AddObstacle(new Bumper(TRACK_WIDTH - 14, 50, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 11, 60, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 8, 40, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(10, 50, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(7, 60, content.Load<Model>("bumper_1")));
            var bump = new Bumper(4, 40, content.Load<Model>("bumper_1"));
           // bump.isHit = true;
            AddObstacle(bump);

            AddObstacle(new Slingshot(new Vector3((TRACK_WIDTH - 4) / 2 - 10.7f, 23.1f, -0.3f), content.Load<Model>("slingshotright"), Matrix.CreateRotationZ(MathHelper.ToRadians(-127))));
            AddObstacle(new Slingshot(new Vector3((TRACK_WIDTH - 4) / 2 + 11, 22.5f, 2.3f), content.Load<Model>("slingshotleft"), Matrix.CreateRotationZ(MathHelper.ToRadians(180))));

            AddObstacle(new Flipper((TRACK_WIDTH - 4) / 2 - 10, 10, content.Load<Model>("flipper"), 0f - 0.3f, false));
            AddObstacle(new Flipper((TRACK_WIDTH - 4) / 2 + 10, 10, content.Load<Model>("flipper"), (float)Math.PI + 0.3f, true));

            AddObstacle(new Switch(new Vector2(TRACK_WIDTH / 2 - 4, TRACK_HEIGHT - 15), content.Load<Model>("ball"), content.Load<Model>("cube")));
        }

        private void AddObstacle(Obstacle o)
        {
            o.ID = ID;
            for (int i = o.CollisionBox.X; i <= o.CollisionBox.X + o.CollisionBox.Width; ++i)
            {
                for (int j = o.CollisionBox.Y; j <= o.CollisionBox.Y + o.CollisionBox.Height; ++j)
                {
                    tiles[i, j] = o.ID;
                }
            }
            obstacles.Add(ID, o);
            ID++;
        }

        private void AddWallBumper(int x, int y)
        {
            board = new squareStates[TRACK_WIDTH, TRACK_HEIGHT];
            Model m = content.Load<Model>("tirestack");
            if (board[x, y] != squareStates.WALL)
            {
                board[x, y] = squareStates.WALL;
                Wall w = new WallBumper(x, y, m);
                AddObstacle(w);
                walls.Add(w);
                tiles[x, y] = w.ID;
            }
        }

        private void AddWall(int x, int y)
        {
            board = new squareStates[TRACK_WIDTH, TRACK_HEIGHT];
            Model m = content.Load<Model>("cube");
            if (board[x, y] != squareStates.WALL)
            {
                board[x, y] = squareStates.WALL;
                Wall w = new WallRegular(x, y, m);
                AddObstacle(w);
                walls.Add(w);
                tiles[x, y] = w.ID;
                //Add to track graph
            }
        }

        public void Update(float time)
        {
            foreach (Obstacle o in obstacles.Values)
            {
                o.update(time);
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            foreach (Floor f in floors)
            {
                f.draw(view, projection);
            }

            //foreach (Wall w in walls)
            //{
            //    w.draw(view, projection);
            //}

            foreach (Obstacle o in obstacles.Values)
            {
                o.draw(view, projection);
            }
            DrawSpring();
        }

        public void DrawSpring()
        {
            foreach (ModelMesh mesh in spring.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);
                    effect.AmbientLightColor = new Vector3(0.55f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(new Vector3(0.5f,0.5f,springLevel)) * 
                        Matrix.CreateTranslation(new Vector3(47.5f, 0f, 4 * springLevel)) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(-90));
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
        }

    }
}
