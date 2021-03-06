﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Walls
{
    class WallRegular : Wall
    {
        const int SCORE = 500;
        public const float E = 3f;
        public WallRegular(float x, float y, Model m)
        {
            score = SCORE;
            model = m;
            scale = new Vector3(0.5f,  0.5f, 2f);
            position = new Vector3(x,  y, 0);
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        }

        public bool Collides(Player player)
        {
            Matrix world1 = Matrix.CreateScale(player.scale) * Matrix.CreateTranslation(player.position);
            Matrix world2 = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            //Check bounding spheres
            for (int meshIndex1 = 0; meshIndex1 < player.model.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = player.model.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(world1);
                //Check bounding Boxes
                if (UpdateBoundingBox(model, world2).Intersects(sphere1))
                {
                    AudioManager.playEffect(AudioEffect.WALL_BOUNCE);
                    return true;
                }
            }
            return false;
        }

        public override Vector3 getResultingForce(Player p)
        {
            Vector3 player = p.position;
            if (Collides(p))
            {
                //Force can be one of 4 directions
                //Figure out which face of the cube we hit
                float playerRight = player.X + Player.RADIUS;
                float playerLeft = player.X - Player.RADIUS;
                float playerTop = player.Y + Player.RADIUS;
                float playerBottom = player.Y - Player.RADIUS;

                float wallRight = position.X + scale.X;
                float wallLeft = position.X - scale.X;
                float wallTop = position.Y + scale.Y;
                float wallBottom = position.Y - scale.Y;

                //Assume a hit until the end
                isHit = true;
                Vector3 impulse = Vector3.Zero;
                if (playerRight >= wallRight && playerLeft <= wallRight)
                {
                    impulse += new Vector3(1, 0, 0);
                }
                if (playerRight >= wallLeft && playerLeft <= wallLeft)
                {
                    impulse += new Vector3(-1, 0, 0);
                }
                if (playerTop >= wallTop && playerBottom <= wallTop)
                {
                    impulse += new Vector3(0, 1, 0);
                }
                if (playerTop >= wallBottom && playerBottom <= wallBottom)
                {
                    impulse += new Vector3(0, -1, 0);
                }
                if (!impulse.Equals(Vector3.Zero))
                {
                    p.score += score;
                    impulse.Normalize();
                }
                return impulse * E;
            }
            //If we got to the end, hit is false
            isHit = false; ;
            return Vector3.Zero;
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
