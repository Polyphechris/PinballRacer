﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Obstacles
{
    public class Bumper : Obstacle
    {
        public const float RADIUS = 1f;
        public const float E = 1f;
        public bool isHit;

        public Bumper(float x, float y, Model m)
        {
            model = m;
            position = new Vector3(x, y, 0.5f);
            scale = new Vector3(0.7f, 0.7f, 0.025f);
            isHit = false;
        }

        public override Vector3 getResultingForce(Microsoft.Xna.Framework.Vector3 player)
        {
            if (Vector3.Distance(player, position) <= RADIUS + Player.RADIUS)
            {
                Vector3 force = player - position;
                force.Normalize();
                return force * E;
            }
            return Vector3.Zero;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, 0, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                    if (isHit)
                    {
                        effect.DirectionalLight1.SpecularColor = new Vector3(1f, 1f, 1f);
                        effect.EmissiveColor = new Vector3(0.2f, 0.2f, 0.2f);
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    }
                    else
                    {
                        effect.DirectionalLight1.SpecularColor = Vector3.Zero;
                        effect.EmissiveColor = Vector3.Zero;
                    }
                }
                mesh.Draw();
            }
        }
    }
}
