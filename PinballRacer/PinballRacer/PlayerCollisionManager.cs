using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Content;
using PinballRacer.Track;

namespace PinballRacer
{
    class PlayerCollisionManager
    {
        const float DETECTION_LENGTH = 3f;

        public List<Player> NPC;
        RaceTrack Track;
        ContentManager content;
        Model wall;
        Model ball;

        public PlayerCollisionManager(ContentManager c, RaceTrack t)
        {
            Track = t;
            content = c;
            NPC = new List<Player>();
            InitializePlayers();
        }

        void InitializePlayers()
        {
            Model m = content.Load<Model>("mky");
            Model c = content.Load<Model>("cube");

            NPC.Add(new NpcPlayer());
            NPC.Add(new NpcPlayer());
            NPC.Add(new NpcPlayer());
            //NPC.Add(new Player(m, 1, 3));
            //NPC.Add(new Player(m, 1, 2));
            //NPC.Add(new Player(m, 1, 4));
            //NPC.Add(new Player(m, 1, 5));
        }

        public void update(GameTime time)
        {
            foreach (Player p in NPC)
            {
                //Check P2P collision
                foreach (Player p2 in NPC)
                {
                    if (p2 != p)
                    {
                        if (Collision(p, p2))
                        {
                            HandleCollision(p, p2);
                        }
                    }
                }

                //Only use walls that are in range
                int x = (int)p.position.X;
                int y = (int)p.position.Y;
                if (x == 0) ++x;
                if (y == 0) ++y;
                List<Obstacle> obstaclesInRange = new List<Obstacle>();
                for (int i = x - 1; i <= x + 2; ++i)
                {
                    for (int j = y - 1; j <= y + 2; ++j)
                    {
                        if (Track.tiles[i, j] != 0)
                        {
                            obstaclesInRange.Add(Track.obstacles[Track.tiles[i, j]]);
                        }
                    }
                }

                List<Vector4> Impulses = new List<Vector4>();
                foreach (Obstacle o in obstaclesInRange)
                {
                    Vector3 i = o.getResultingForce(p.position);
                    if (!i.Equals(Vector3.Zero))
                    {
                        Impulses.Add(new Vector4(i.X, i.Y, i.Z, 0));
                    }
                }
                //Check Player to Wall Collision
                //Vector3 WallCollision = Collision(p, walls);
                //if (WallCollision != Vector3.Zero)
                //{
                //    HandleCollision(p, WallCollision);
                //}

                ////Check for collision with ray
                //obstacles.AddRange(walls);

                p.Update(time);
            }    

        }

        public void draw(Matrix view, Matrix projection)
        {
            foreach (Player p in NPC)
            {
                p.Draw(view, projection);
            }
        }

        public Vector3 Collision(Player p, List<Vector3> walls)
        {
            //Create the world transforms for each shape
            Matrix world1 = Matrix.CreateScale(Player.RADIUS) *
                 Matrix.CreateTranslation(p.position);

            foreach (Vector3 wallitem in walls)
            {
                Matrix world2 = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(wallitem);
                //Check bounding spheres
                for (int meshIndex1 = 0; meshIndex1 < p.model.Meshes.Count; meshIndex1++)
                {
                    BoundingSphere sphere1 = p.model.Meshes[meshIndex1].BoundingSphere;
                    sphere1 = sphere1.Transform(world1);
                    //Check bounding Boxes
                    if (UpdateBoundingBox(wall, world2).Intersects(sphere1))
                    {
                        return wallitem;
                    }                    
                }
            }
            return Vector3.Zero;
        }

        /// <summary>
        ///USED code from previous lab work in COMP 376
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public bool Collision(Player p1, Player p2)
        {
            float distance = (p1.position - p2.position).Length();
            if(distance < Player.RADIUS * 2)
            {
                return true;
            }
            return false;
        }

        public void HandleCollision(Player p1, Player p2)
        {
            //Recompute a path, apply bounce forces, steer
            Vector3 ImpulseDirection = p1.position - p2.position;
            ImpulseDirection.Normalize();

            // p1.impulses.Add(ImpulseDirection);
           // p2.impulses.Add(-ImpulseDirection);
        }

        public Vector3 HandleCollision(Player p, Vector3 wall)
        {
            //Force can be one of 4 directions
            //Figure out which face of the cube we hit
            float playerRight = p.position.X + Player.RADIUS;
            float playerLeft = p.position.X - Player.RADIUS;
            float playerTop = p.position.Y + Player.RADIUS;
            float playerBottom = p.position.Y - Player.RADIUS;

            float wallRight = p.position.X + 0.5f;
            float wallLeft = p.position.X - 0.5f;
            float wallTop = p.position.Y + 0.5f;
            float wallBottom = p.position.Y - 0.5f;

            if (playerRight >= wallRight && playerLeft <= wallRight)
            {
                return new Vector3(1, 0, 0);
            }
            if (playerRight >= wallLeft && playerLeft <= wallLeft)
            {
                return new Vector3(-1, 0, 0);
            }
            if (playerTop >= wallTop && playerBottom <= wallTop)
            {
                return new Vector3(0, 1, 0);
            }
            if (playerTop >= wallBottom && playerBottom <= wallBottom)
            {
                return new Vector3(0, -1, 0);
            }
            return Vector3.Zero;
        }               


//Method obtained from online discussion boards at Stack Overflow
//http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        public BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }
    }
}
