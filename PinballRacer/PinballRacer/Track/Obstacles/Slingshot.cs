using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Obstacles
{
    class Slingshot : Obstacle
    {
        public const float E = 1f;
        Matrix rotation;
        Vector3[] vertices;
        Vector3[] lines; // (a,b,c)
        Vector3[] forces;

        public Slingshot(Vector3 pos, Model m, Matrix r)
        {
            rotation = r;
            model = m;
            position = pos;
            scale = new Vector3(7f, 7f, 7f);
            InitializeSegments();
            CollisionBox = new Rectangle((int)(vertices[0].X - 1), (int)(vertices[2].Y - 1), 
                (int)(vertices[2].X - vertices[0].X + 2), (int)(vertices[0].Y - vertices[2].Y + 2));
        }

        public void InitializeSegments()
        {
            vertices = new Vector3[3];
            vertices[0] = Vector3.Zero; //Top
            vertices[1] = Vector3.Zero; //Bottom
            vertices[2] = Vector3.Zero; //Right
            ////TEST DATA
            vertices[0] = Vector3.UnitY + 3*Vector3.One; //Top
            vertices[1] = Vector3.Zero + 3 * Vector3.One; //Bottom
            vertices[2] = Vector3.UnitX + 3 * Vector3.One; //Right

            //Line Equations for each side of the shape
            lines = new Vector3[3];
            float a = (vertices[1].Y - vertices[0].Y) / (vertices[1].X - vertices[0].X);
            float b = -1f;
            float c = vertices[0].Y - a * vertices[0].X;
            //lines[0] = new Vector3(a,b,c); //0 - 1
            lines[0] = new Vector3(1, 0, vertices[0].X); //0 - 1

            a = (vertices[2].Y - vertices[1].Y) / (vertices[2].X - vertices[1].X);
            b = -1f;
            c = vertices[1].Y - a * vertices[1].X;
            lines[1] = new Vector3(a, b, c); //1 - 2            float a = (vertices[1].Y - vertices[0].Y) / vertices[1].X - vertices[0].X;

            a = (vertices[0].Y - vertices[2].Y) / (vertices[0].X - vertices[2].X);
            b = -1f;
            c = vertices[2].Y - a * vertices[2].X;
            lines[2] = new Vector3(a, b, c); //2 - 0

            //Resulting force corresponding to a side
            forces = new Vector3[3];
            forces[0] = Vector3.Cross(Vector3.UnitZ, vertices[0] - vertices[1]);
            forces[1] = Vector3.Cross(Vector3.UnitZ, vertices[1] - vertices[2]);
            forces[2] = Vector3.Cross(Vector3.UnitZ, vertices[2] - vertices[0]);
            forces[0].Normalize(); forces[1].Normalize(); forces[2].Normalize();
        }

        //Check is a point is in a triangle
        public bool InTriangle(Vector3 player)
        {
            if (SameSide(player, vertices[0], vertices[1], vertices[2]) &&
                SameSide(player, vertices[1], vertices[0], vertices[2]) &&
                SameSide(player, vertices[2], vertices[0], vertices[1]))
            {
                return true;
            }
            return false;
        }
        
        public override Vector3 getResultingForce(Player p)
        {
            Vector3 player = p.position;
            for (int i = 0; i < 3; ++i)
            {    
                int end = (i+1) % 3;
                if (isBetween(vertices[i].X, vertices[end].X, player.X) &&
                    isBetween(vertices[i].Y, vertices[end].Y, player.Y))
                {
                    float d = (lines[i].X * player.X + lines[i].Y * player.Y + lines[i].Z) / (float)(Math.Sqrt(Math.Pow(lines[i].X, 2) + Math.Pow(lines[i].Y, 2)));
                    if (d <= Player.RADIUS)
                    {
                        isHit = true;
                        return forces[i] * E;
                    }
                }
            }
            return Vector3.Zero;
        }

        //http://stackoverflow.com/questions/1992970/check-if-int-is-between-two-numbers
        // Checks if c is between a and b
        public static bool isBetween(float a, float b, float c)
        {
            return b > a ? c > a && c < b : c > b && c < a;
        }

        //http://www.blackpawn.com/texts/pointinpoly/default.html
        public bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b-a, p1-a);
            Vector3 cp2 = Vector3.Cross(b-a, p2-a);
            if (Vector3.Dot(cp1, cp2) >= 0) return true;
            else return false;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(0.2f);
                    effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection; effect.EmissiveColor = Vector3.Zero;                    
                }
                mesh.Draw();
            }
        }
    }
}
