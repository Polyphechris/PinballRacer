using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*Class skeleton adapted from assignemnt 2*/

namespace PinballRacer.Track.Pathfinding
{
    public class Path
    {
        const float UNIT_LEN = 20;
        const float NEXT_THRESHOLD = 0.25f;
        float runtime;
        float started;
        float end;
        int id;
        public float lastStretch;
        public bool done;

        int currentPoint;
        public List<Vector3> points = new List<Vector3>();

        public Path()
        {
            // TODO: Complete member initialization
        }

        public Path(float time, int i)
        {
            id = i;
            currentPoint = 0;
            started = time;
            runtime = time;
            end = started + time;
            done = false;

            lastStretch = Vector3.Distance(points[points.Count - 1], points[points.Count - 2]);
        }

        public void InitializePath(List<Vector3> list)
        {
            done = false;
            currentPoint = 0;
            points = list;
        }

        public Vector3 getStartPoint()
        {
            return points[0];
        }

        public bool checkSlowDownPoint()
        {
            if (currentPoint == points.Count - 2)
                return true;

            return false;
        }

        public bool checkEnd()
        {
            return done;
        }

        public bool checkIfNext(Vector3 ballPos)
        {
            float xDist = Math.Abs(ballPos.X - points[currentPoint + 1].X);
            float yDist = Math.Abs(ballPos.Y - points[currentPoint + 1].Y);
            float zDist = Math.Abs(ballPos.Z - points[currentPoint + 1].Z);

            if (xDist <= NEXT_THRESHOLD && yDist <= NEXT_THRESHOLD)
            {
                if (currentPoint < points.Count - 2)
                {
                    ++currentPoint;
                }
                else
                {
                    done = true;
                }
                return true;
            }

            return false;
        }

        public Vector3 getDirection(float time, Vector3 position)
        {
            checkIfNext(position);

            float xDir = 0;
            float yDir = 0;
            float zDir = 0;
            Vector3 dir = Vector3.Zero;

            xDir = points[currentPoint + 1].X - position.X;
            yDir = points[currentPoint + 1].Y - position.Y;
            zDir = points[currentPoint + 1].Z - position.Z;
            dir = new Vector3(xDir, yDir, zDir);
            dir.Normalize();

            return dir;
        }

        public Vector3 getNextPoint0(float time)
        {
            return points[0];
        }
        public Vector3 getNextPoint1(float time)
        {
            return points[0];
        }
        public Vector3 getNextPoint2(float time)
        {
            return points[0];
        }

        public void Draw(Matrix view, Matrix projection, Model model)
        {
            int count = 0;
            foreach (Vector3 p in points)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(0.8f, 0.1f, 0.1f);
                        if (count == currentPoint + 1)
                            effect.DiffuseColor = new Vector3(1f, 0.2f, 0.2f);
                        else
                            effect.DiffuseColor = new Vector3(1f, 1.0f, 0.0f);
                        effect.World = Matrix.CreateScale(0.15f) * Matrix.CreateTranslation(p);
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
                count++;
            }

        }
    }
}
