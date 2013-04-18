using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PinballRacer.Track.Walls;
using PinballRacer.Track.Obstacles;
using PinballRacer.Track.Pathfinding;

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
        public const int WAYPOINT_RADIUS = 12;

        ContentManager content;
        Model spring;
        Model floor;
        public float springLevel;
        public float stableSpringLevel;
        public float minSpringLevel;
        public float maxSpringLevel;
        public bool springShot = false;

        public Dictionary<int, Obstacle> obstacles;
        public int[,] tiles;
        List<Floor> floors;
        public List<Vector2> Waypoints;

        public PathManager PathController;

        public enum squareStates { EMPTY = 0, WALL, PLAYER1, PLAYER2, OBSTACLE, GOAL1, LAST };
        public enum trackStates { PLAYING = 0, START, GAMEOVER };
        public squareStates[,] board;

        //Wal performance patch
        Wall tokenWall = new WallRegular(0,0,null);
        List<Matrix> WallWorlds;

        public RaceTrack(ContentManager c)
        {
            obstacles = new Dictionary<int, Obstacle>();
            tiles = new int[TRACK_WIDTH+1, TRACK_HEIGHT+1];
            board = new squareStates[TRACK_WIDTH+1, TRACK_HEIGHT+1];

            content = c;
            InitializeObstacles();
            InitializeFinishingLine();
            InitializeOutterWalls();
            InitializeInnerWalls();
            spring = content.Load<Model>("spring");
            floor = content.Load<Model>("plane");
            springLevel = 0.5f;
            minSpringLevel = 0.3f;
            maxSpringLevel = 0.9f;
            stableSpringLevel = 0.5f;
            springLevel = minSpringLevel;

            Waypoints = new List<Vector2>();
            Waypoints.Add(new Vector2(47, 97));
            Waypoints.Add(new Vector2(TRACK_WIDTH/2 - 2, TRACK_HEIGHT/2 - 10));
            Waypoints.Add(new Vector2(3, 97));
            Waypoints.Add(new Vector2(TRACK_WIDTH_IN - 5, TRACK_HEIGHT_IN - 5));
            Waypoints.Add(new Vector2(TRACK_WIDTH_OUT + 5, TRACK_HEIGHT_IN - 5));

            InitializeWallWorlds();
            PathController = new PathManager(obstacles, tiles, Waypoints);
        }

        private void InitializeWallWorlds()
        {
            WallWorlds = new List<Matrix>();

            WallWorlds.Add(Matrix.CreateScale(new Vector3(TRACK_WIDTH / 2, 1, 1)) *
                Matrix.CreateTranslation(new Vector3(TRACK_WIDTH / 2,0, -0.5f)));

            WallWorlds.Add(Matrix.CreateScale(new Vector3(TRACK_WIDTH / 2, 1, 1)) *
                Matrix.CreateTranslation(new Vector3(TRACK_WIDTH / 2, 100, -0.5f)));

            WallWorlds.Add(Matrix.CreateScale(new Vector3(1, TRACK_HEIGHT / 2, 1)) *
                Matrix.CreateTranslation(new Vector3(0, TRACK_HEIGHT / 2, -0.5f)));

            WallWorlds.Add(Matrix.CreateScale(new Vector3(1, TRACK_HEIGHT / 2, 1)) *
                Matrix.CreateTranslation(new Vector3(TRACK_WIDTH, TRACK_HEIGHT / 2, -0.5f)));
                
        }
        private void InitializeFinishingLine()
        {
            floors = new List<Floor>();
            Model m = content.Load<Model>("cube");
            
            //Setting up the finishing line
            int row = 89;
            Vector3 colour;
            for (int i = 42; i < 49; ++i)
            {
                if (i % 2 == 1)
                {
                    colour = Color.Black.ToVector3();
                }
                else
                {
                    colour = Color.White.ToVector3();
                }
                
                floors.Add(new Floor(i, row, m, colour));
            }
        }

        private void InitializeOutterWalls()
        {
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
            Vector2 bottomLeft = new Vector2(TRACK_WIDTH / 2 - 4, TRACK_HEIGHT - 15);
            for (int i = 0; i <= 3; ++i)
            {
                for (int j = 0; j <= 4; ++j)
                {
                    AddWall((int)bottomLeft.X + (i * 3), (int)bottomLeft.Y + j);
                }
            }

            for (int i = TRACK_WIDTH_IN; i < TRACK_WIDTH_OUT; ++i)
            {
                for (int j = TRACK_HEIGHT_IN; j < TRACK_HEIGHT_OUT; ++j)
                {
                    if (j == TRACK_HEIGHT_IN || i == TRACK_WIDTH_IN || j == TRACK_HEIGHT_OUT - 1 || i == TRACK_WIDTH_OUT - 1)
                    {
                        if (!(j == TRACK_HEIGHT_OUT - 1 && i >= TRACK_WIDTH_IN + 1 && i <= TRACK_WIDTH_IN + 5)
                            && !(i == TRACK_WIDTH_IN && j <= TRACK_HEIGHT_OUT - 8 && j >= TRACK_HEIGHT_OUT - 14))
                        {
                            AddWallBumper(i, j);
                        }
                    }
                }
            }

            for (int k = 1; k < TRACK_HEIGHT_OUT-5; ++k)
            {
                AddWall(TRACK_WIDTH - 4, k);                
            }
            for (int l = 10; l < 26; ++l)
            {
                AddWall(5, l);
            } 
            for (int r = 10; r < 26; ++r)
            {
                AddWall(TRACK_WIDTH - 9, r);
            }

            int limit = 10;
            int limit2 = 15;
            int limit3 = 6;
            int rowCounter = TRACK_HEIGHT_OUT;

            // Diagonal line on the right of the inner track
            for (int i = TRACK_WIDTH_OUT; i < TRACK_WIDTH_OUT + limit; ++i)
            {
                if (rowCounter >= 0 || i >= 0 || rowCounter <= TRACK_HEIGHT - 1 || i <= TRACK_WIDTH - 1)
                {
                    AddWallBumper(i, rowCounter);
                    AddWallBumper(i, rowCounter - 1);
                    rowCounter += 1;
                }
            }

            // Vertical line extending the left side of the inner track
            for (int j = TRACK_HEIGHT_OUT; j < TRACK_HEIGHT - 1; ++j)
            {
                if ((TRACK_WIDTH_IN >= 0 || j >= 0 || TRACK_WIDTH_IN <= TRACK_WIDTH - 1 || j <= TRACK_HEIGHT - 1))
                {
                   AddWallBumper(TRACK_WIDTH_IN, j);
                }
            }

            limit = 8;

            // Vertical line on the left of the inner track
            for (int j = TRACK_HEIGHT_OUT - limit2; j < TRACK_HEIGHT - limit; ++j)
            {
                if ((TRACK_WIDTH_IN >= 0 || j >= 0 || TRACK_WIDTH_IN <= TRACK_WIDTH - 1 || j <= TRACK_HEIGHT - 1))
                {
                    AddWallBumper(TRACK_WIDTH_IN - limit3, j);
                }
            }

            // Horizontal line on the inside of the inner track 
            for (int i = TRACK_WIDTH_IN; i <= TRACK_WIDTH_OUT - limit - 1; ++i)
            {
                if (TRACK_HEIGHT_OUT - limit >= 0 || i >= 0 || TRACK_HEIGHT_OUT - limit <= TRACK_HEIGHT - 1 || i <= TRACK_WIDTH_OUT - 1)
                {
                    AddWallBumper(i, TRACK_HEIGHT_OUT - limit);
                }
            }           

            limit = limit += 1;
            // Horizontal line on the left of the inner track
            for (int i = TRACK_WIDTH_IN - limit3; i <= TRACK_WIDTH_IN; ++i)
            {
                if (TRACK_HEIGHT_OUT - limit >= 0 || i >= 0 || TRACK_HEIGHT_OUT - limit <= TRACK_HEIGHT - 1 || i <= TRACK_WIDTH_OUT - 1)
                {
                    AddWallBumper(i, TRACK_HEIGHT_OUT - limit2);
                }
            }

            // Vertical line in the middle of inner track
            int newColumn = TRACK_WIDTH_IN + ((TRACK_WIDTH_OUT - TRACK_WIDTH_IN) / 2);
            for (int j = TRACK_HEIGHT_OUT - limit; j >= TRACK_HEIGHT_IN + limit2; --j)
            {
                if (newColumn >= 0 || j >= 0 || newColumn <= TRACK_WIDTH - 1 || j <= TRACK_WIDTH - 1)
                {
                    AddWallBumper(newColumn, j);
                }
            }
        }

        private void InitializeObstacles()
        {
            AddObstacle(new Bumper(TRACK_WIDTH - 14, 50, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 11, 60, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 8, 40, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 8, 70, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(TRACK_WIDTH - 12, 75, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(
                TRACK_WIDTH_IN + ((TRACK_WIDTH_OUT - TRACK_WIDTH_IN) / 2), 
                TRACK_HEIGHT_IN + 8, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(
                TRACK_WIDTH_IN + ((TRACK_WIDTH_OUT - TRACK_WIDTH_IN) / 2),
                TRACK_HEIGHT_IN - 8, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(5, 80, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(10, 50, content.Load<Model>("bumper_1")));
            AddObstacle(new Bumper(7, 60, content.Load<Model>("bumper_1")));
            var bump = new Bumper(5, 40, content.Load<Model>("bumper_1"));
           // bump.isHit = true;
            AddObstacle(bump);

            AddObstacle(new Slingshot(new Vector3((TRACK_WIDTH - 4) / 2 - 12.5f, 20.6f, -0.2f), content.Load<Model>("slingshotright"), Matrix.CreateRotationZ(MathHelper.ToRadians(-127)), false));
            AddObstacle(new Slingshot(new Vector3((TRACK_WIDTH - 4) / 2 + 12.25f, 20.0f, 2.65f), content.Load<Model>("slingshotleft"), Matrix.CreateRotationZ(MathHelper.ToRadians(180)), true));

            AddObstacle(new Flipper((TRACK_WIDTH - 4) / 2 - 10, 10, content.Load<Model>("flipper"), 0f - 0.3f, false));
            AddObstacle(new Flipper((TRACK_WIDTH - 4) / 2 + 10, 10, content.Load<Model>("flipper"), (float)Math.PI + 0.3f, true));
            Flipper.flipperSphere = content.Load<Model>("ball");

            Vector2 bottomLeft = new Vector2(TRACK_WIDTH / 2 - 4, TRACK_HEIGHT - 15);
            AddObstacle(new Switch(bottomLeft, content.Load<Model>("ball"), content.Load<Model>("cube")));

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
                tiles[x, y] = w.ID;
                //Add to track graph
            }
        }

        public void SetSpringLevel(float newSpringLevel)
        {
            if (newSpringLevel >= minSpringLevel && newSpringLevel <= maxSpringLevel)
            {
                springLevel = newSpringLevel;
            }
            else
            {
                if (newSpringLevel < minSpringLevel)
                {
                    newSpringLevel = minSpringLevel;
                }
                else
                {
                    newSpringLevel = maxSpringLevel;
                }
            }
        }

        public void Update(float time)
        {
            if (Game1.closeLoader)
            {
                AddWall(TRACK_WIDTH - 2, TRACK_HEIGHT - 26);
                AddWall(TRACK_WIDTH - 3, TRACK_HEIGHT - 26);
            }

            foreach (Obstacle o in obstacles.Values)
            {
                o.update(time);
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            PathController.Draw(view, projection, content.Load<Model>("ball"), content.Load<Model>("cube"));
            DrawFloor();
           // DrawWalls(view, projection, content.Load<Model>("cube"));
            foreach (Floor f in floors)
            {
                f.draw(view, projection);
                
            }

            foreach (Obstacle o in obstacles.Values)
            {
            //    if(o.GetType() != tokenWall.GetType())
                o.draw(view, projection);
            }
            DrawSpring();
        }

        public void DrawWalls(Matrix view, Matrix projection, Model model)
        {
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    //OUTTER WALLs
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.LightingEnabled = true;
            //        effect.EnableDefaultLighting();         
            //        effect.AmbientLightColor = new Vector3(0.55f);
            //        effect.DirectionalLight0.DiffuseColor = new Vector3(1,0, 0);// Shinnyness/reflexive
            //        effect.World = Matrix.CreateScale(new Vector3(TRACK_WIDTH / 2, 1, 1)) *
            //            Matrix.CreateTranslation(new Vector3(TRACK_WIDTH / 2,0, -0.5f));
            //        effect.View = Game1.view;
            //        effect.Projection = Game1.projection;
            //        //effect.Alpha = 0.8f;
            //    }
            //    mesh.Draw();
            //}  
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();
            //        effect.AmbientLightColor = new Vector3(0.55f);
            //        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0, 0);// Shinnyness/reflexive
            //        effect.World = Matrix.CreateScale(new Vector3(TRACK_WIDTH / 2, 1, 1)) *
            //            Matrix.CreateTranslation(new Vector3(TRACK_WIDTH / 2, 100, -0.5f));
            //        effect.View = Game1.view;
            //        effect.Projection = Game1.projection;
            //        //effect.Alpha = 0.8f;
            //    }
            //    mesh.Draw();
            //}   
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();
            //        effect.AmbientLightColor = new Vector3(0.55f);
            //        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0, 0);// Shinnyness/reflexive
            //        effect.World = Matrix.CreateScale(new Vector3(1, TRACK_HEIGHT / 2, 1)) *
            //            Matrix.CreateTranslation(new Vector3(0, TRACK_HEIGHT / 2, -0.5f));
            //        effect.View = Game1.view;
            //        effect.Projection = Game1.projection;
            //        //effect.Alpha = 0.8f;
            //    }
            //    mesh.Draw();
            //}
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();
            //        effect.AmbientLightColor = new Vector3(0.55f);
            //        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0, 0);// Shinnyness/reflexive
            //        effect.World = Matrix.CreateScale(new Vector3(1, TRACK_HEIGHT / 2, 1)) *
            //            Matrix.CreateTranslation(new Vector3(TRACK_WIDTH, TRACK_HEIGHT / 2, -0.5f));
            //        effect.View = Game1.view;
            //        effect.Projection = Game1.projection;
            //        //effect.Alpha = 0.8f;
            //    }
            //    mesh.Draw();
            //}
                //INNER WALLS
            foreach (Matrix m in WallWorlds)
            {           
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(0.45f);
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                        effect.World = m;
                        effect.View = Game1.view;
                        effect.Projection = Game1.projection;
                        //effect.Alpha = 0.8f;
                    }
                    mesh.Draw();
                }
            }
            
        }

        public void DrawFloor()
        {
            foreach (ModelMesh mesh in floor.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);
                    effect.AmbientLightColor = new Vector3(0.55f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(new Vector3(TRACK_WIDTH/2,TRACK_HEIGHT/2,1)) *
                        Matrix.CreateTranslation(new Vector3(TRACK_WIDTH/2, TRACK_HEIGHT/2, -0.5f));
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
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
