using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Obstacles
{
    class Flipper : Obstacle
    {
        public static Model flipperSphere;
        public const float E = 1f;
        public const int SCORE = 2500;

        public const float INTERVAL = 1000f;
        public float W1 = 3f;
        public float W2= 0.75f;
        public float START_D = -0.3f;
        public float MAX_D = (float)Math.PI/4;

        public enum Align { LEFT = 0, RIGHT};
        Matrix rotation;
        private bool inverted;

        float idle_timer;
        float angle;
        float w; //angular speed
        float delta_angle;
        public enum states { FIRING, IDLE, RELOAD }; //Shooting the ball, doing nothing, going back down
        states state;

        float[] sphereRadii;
        float[] spherePositions;
        Vector3[] velocities;

        public Flipper(float x, float y, Model m, float a, bool i)
        {
            score = SCORE;
            w = 0;
            idle_timer = 0f;
            state = states.IDLE;
            inverted = i;
            angle = a;
            model = m;
            position = new Vector3(x, y, 0f);
            scale = new Vector3(0.7f);
            if (!inverted)
            {
                CollisionBox = new Rectangle(13, 5, 20 - 13, (int)(13 - 5.5));
            }
            else
            {
                CollisionBox = new Rectangle(26, 5, 34 - 26, (int)(13 - 5.5));
            }

            InitializeSphereDetectors();
        }

        private void InitializeSphereDetectors()
        {
            sphereRadii = new float[5];
            spherePositions = new float[5];
            velocities = new Vector3[5];
            sphereRadii[0] = 1.15f;
            sphereRadii[1] = 1.05f;
            sphereRadii[2] = 0.85f;
            sphereRadii[3] = 0.75f;
            sphereRadii[4] = 0.50f;

            for (int i = 0; i < spherePositions.Length; ++i)
            {
                int positions = 0;
                spherePositions[i] = 0.0f;
                while (positions < i)
                {
                    spherePositions[i] += sphereRadii[positions] * 1.65f;
                    positions += 1;
                }
            }

            velocities[0] = new Vector3(0.0f, 0.25f, 0.0f);
            velocities[1] = new Vector3(0.0f, 0.50f, 0.0f); 
            velocities[2] = new Vector3(0.0f, 0.75f, 0.0f);
            velocities[3] = new Vector3(0.0f, 1.0f, 0.0f);
            velocities[4] = new Vector3(0.0f, 1.25f, 0.0f);
        }

        public bool Collides(Player player, Matrix sphereWorld)
        {
            Matrix playerWorld = Matrix.CreateScale(player.scale) * Matrix.CreateTranslation(player.position);
            
            //Check bounding spheres
            for (int meshIndex1 = 0; meshIndex1 < player.model.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = player.model.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(playerWorld);

                for (int meshIndex2 = 0; meshIndex1 < flipperSphere.Meshes.Count; meshIndex1++)
                {
                    BoundingSphere sphere2 = flipperSphere.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(sphereWorld);
                    if (sphere2.Intersects(sphere1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override Vector3 getResultingForce(Player p)
        {
            for (int i = 0; i < sphereRadii.Length; ++i)
            {
                Matrix sphereWorld = Matrix.CreateScale(sphereRadii[i]) *
                    Matrix.CreateTranslation(new Vector3(spherePositions[i], 0.0f, 0.0f)) *
                    Matrix.CreateRotationZ(angle) *
                    Matrix.CreateTranslation(position);

                if (Collides(p, sphereWorld))
                {
                    Vector3 sphereWorldPosition = sphereWorld.Translation;
                    Vector3 direction = p.position - sphereWorldPosition;
                    direction.Z = 0.0f;                    

                    Vector3 flipperVelocity = w * (-direction) / 100;//angular velocity * distance from anchor
                    direction.Normalize();

                    if (state == states.FIRING)
                    {
                        if (direction.Y > 0.0f)
                        {
                            p.velocity += velocities[i] * direction;
                        }
                        else
                        {
                            p.velocity = -p.velocity + Vector3.Multiply(flipperVelocity, direction);
                        }
                    }
                    else if (state == states.RELOAD)
                    {
                        p.velocity = direction * 0.1f;                        
                    }
                    else if (state == states.IDLE)
                    {
                        p.velocity = direction * 0.1f;
                    }

                    break;
                }
            }
            
            return Vector3.Zero;
        }

        public override void update(float time)
        {
            base.update(time);
            if (state == states.IDLE)
            {
                w = 0;
                idle_timer += time;
                if (idle_timer >= INTERVAL)
                {
                    idle_timer = 0;
                    state = states.FIRING;
                }
            }
            else if (state == states.FIRING)
            {
                w = W1;
                int minus = 1;
                if (inverted) { minus = -1; }
                angle += w * minus * time / 1000;
                delta_angle += w * time / 1000;
                //if we reach the max start going back down
                if (delta_angle >= MAX_D) 
                    state = states.RELOAD;                
            }
            else if(state == states.RELOAD)
            {
                w = W2;
                int minus = -1;
                if (inverted) { minus = 1; }
                angle += w * minus * time / 1000;
                delta_angle -= w * time / 1000;
                //if we reach the max start going back down
                if (delta_angle <= START_D) 
                    state = states.IDLE;
            }
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {                    
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }

            //for (int i = 0; i < sphereRadii.Length; i++)
            //{
            //    DrawSphereCollisions(view, projection, sphereRadii[i], spherePositions[i]);
            //}
        }

        public void DrawSphereCollisions(Matrix view, Matrix projection, float sphereRadius, float spherePosition)
        {            
            foreach (ModelMesh mesh in flipperSphere.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(sphereRadius) * Matrix.CreateTranslation(new Vector3(spherePosition, 0.0f, 0.0f)) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }

        }


        //Method obtained from online discussion boards at Stack Overflow
        //http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
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
