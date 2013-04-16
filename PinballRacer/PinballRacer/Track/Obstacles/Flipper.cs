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
        public const float E = 1f;
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

        public Flipper(float x, float y, Model m, float a, bool i)
        {
            w = 0;
            idle_timer = 0f;
            state = states.IDLE;
            inverted = i;
            angle = a;
            model = m;
            position = new Vector3(x, y, 0f);
            scale = new Vector3(0.7f);
        }
        
        public override Vector3 getResultingForce(Player p)
        {
            Vector3 player = p.position;
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
