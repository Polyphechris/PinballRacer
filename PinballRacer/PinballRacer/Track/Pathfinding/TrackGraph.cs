using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Pathfinding
{
    class TrackGraph
    {
        //The node class represents a single point int the race track
        public class Node
        {
            //used to keep track of path in A*
            public Node previous;

            public Node(Vector2 p)
            {
                children = new List<Node>();
                visited = false;
                isGoal = false;
                position = p;
                open = false;
                cost = 0;
            }

            public void Link(Node n)
            {
                if (!children.Contains(n))
                {
                    children.Add(n);
                }
                if (!n.children.Contains(this))
                {
                    n.children.Add(this);
                }
            }
            public List<Node> children;
            public float heuristic;
            public Vector2 position;
            public bool visited;
            public bool isGoal;
            public bool open;
            public float cost;
        }

        const float NODE_RADIUS = 0.5f;
        public Node Start;
        public Node End;
        
        public int height;
        public int width;

        public List<Node> allNodes;
        Node[,] openList;
        Node[,] closedList;
        
        public bool searchDone;        

        public TrackGraph(Vector2 start, Vector2 end, int w, int h)
        {
            width = w;
            height = h;
            Start = new Node(start);
          //  End = new Node(end);
          //  End.isGoal = true;
            allNodes = new List<Node>();
            allNodes.Add(Start);
           // allNodes.Add(End);
            searchDone = false;

            ResetLists();
        }

        //Creates a new node with n as parent at x,y
        public Node AddNode(Node n, float x, float y)
        {
            Node newNode = new Node(new Vector2(x, y));
            n.children.Add(newNode);
            allNodes.Add(newNode);
            newNode.children.Add(n);
            newNode.heuristic = Vector2.Distance(new Vector2(x, y), End.position);
          //  if (x == End.position.X && y == End.position.Y) newNode.isGoal = true;
            return newNode;
        }
        
        public void ComputeHeuristics(Player p, Dictionary<int, Obstacle> obstacles, int[,] tiles)
        {
            foreach (Node n in allNodes)
            {
                n.visited = false; n.open = false;
                int objectID = tiles[(int)n.position.X, (int)n.position.Y];
                if (objectID != 0)
                    n.heuristic = ((NpcPlayer)p).pickStrategy.GenerateHeuristic(n.position, End.position, obstacles[objectID]);
                else
                    n.heuristic = ((NpcPlayer)p).pickStrategy.GenerateHeuristic(n.position, End.position, null);
            } 
        }

        private void ResetLists()
        {
            openList = new Node[width, height];
            closedList = new Node[width, height];
            foreach (Node n in allNodes)
            {
                n.visited = false;
            }
        }

        public Path AStarPath()
        {
            Path bestPath = null;
            Node previous = null;
            while (!searchDone)
            {
                Node current = GetTopOpen();
                if (current == null)
                {
                    searchDone = true;
                    ResetLists();
                    Path p = ConstructPath(End);
                    return p;
                }
                if (current.isGoal)
                {
                    //Node top = GetTopOpen();
                   // if (top.cost >= current.cost)
                    //{
                        searchDone = true;
                        ResetLists();
                        return ConstructPath(current);
                    //}
                }
                AddToOpen(current);
                current.visited = true;
                openList[(int)current.position.X, (int)current.position.Y] = null;
                closedList[(int)current.position.X, (int)current.position.Y] = current;
                previous = current;
            }
            return bestPath;
        }

        public Node GetTopOpen()
        {
            float value = float.MaxValue;            
            Node node = null;

            foreach (Node n in openList)
            {
                if (n != null)
                {
                    if (value > n.cost + n.heuristic)
                    {
                        value = n.cost + n.heuristic;
                        node = n;
                    }
                }
            }
            return node;
        }
        public void AddToOpen(Node n)
        {
            foreach (Node child in n.children)
            {
                if (closedList[(int)child.position.X, (int)child.position.Y] == null)
                {
                    if (openList[(int)child.position.X, (int)child.position.Y] == null)
                    {
                        openList[(int)child.position.X, (int)child.position.Y] = child; 
                        child.previous = n;
                        child.cost = n.cost + Vector2.Distance(n.position, child.position);
                    }
                    else
                    {
                        if (openList[(int)child.position.X, (int)child.position.Y].cost > n.cost + Vector2.Distance(n.position, child.position))
                        {
                            openList[(int)child.position.X, (int)child.position.Y].cost = n.cost + Vector2.Distance(n.position, child.position);
                            child.previous = n;
                        }
                    }
                }
            }
        }

        public Path ConstructPath(Node goal)
        {
            Path path = new Path();
            List<Vector3> points = new List<Vector3>();
            Node current = goal;
            while (current != Start)
            {
                points.Add(new Vector3(current.position.X, current.position.Y, 0.5f));
                current = current.previous;
            }
            points.Add(new Vector3(current.position.X, current.position.Y, 0.5f));
            points.Reverse();
            //Add waypoint to the end of the path
            path.InitializePath(points);
            return path;
        }
        public void InitializeGrid(int[,] board)
        {
            openList[(int)Start.position.X, (int)Start.position.Y] = Start;

            Node[,] nodes = new Node[width, height];
            nodes[(int)Start.position.X, (int)Start.position.Y] = Start;
           // nodes[(int)End.position.X, (int)End.position.Y] = End;

            List<Node> toAdd = new List<Node>();
            toAdd.Add(Start);

            while (toAdd.Count != 0)
            {
                Node head = toAdd.First();
                foreach (Node n in GridChildren(board, (int)head.position.X, (int)head.position.Y))
                {
                    if (nodes[(int)n.position.X, (int)n.position.Y] == null)
                    {
                        Node newNode = AddNode(head, (int)n.position.X, (int)n.position.Y);
                        nodes[(int)n.position.X, (int)n.position.Y] = newNode;
                        toAdd.Add(newNode);
                    }
                    else
                    {
                        head.Link(nodes[(int)n.position.X, (int)n.position.Y]);
                    }
                }
                toAdd.Remove(head);
            }
        }

        public List<Node> GridChildren(int[,] board, int x, int y)
        {
            List<Node> returnList = new List<Node>();
            for (int i = x - 1; i < x + 2; ++i)
            {
                for (int j = y - 1; j < y + 2; ++j)
                {

                    if (!(x == i && j == y))
                    {
                        if(i < width && i > 0 && j < height && j > 0)
                        {
                            if (board[i, j] <= 17)
                            {
                                Node newn = new Node(new Vector2(i, j));
                                returnList.Add(newn);
                            }
                        }
                    }
                }
            }
            return returnList;
        }

        public void InitializeNodes()
        {
            //ROOM 1
            Node n61 = AddNode(Start, 6, 1);
            Node n51 = AddNode(n61, 5, 1);
            Node n31 = AddNode(n51, 3, 1);
            Node n11 = AddNode(n31, 1, 1);
            Node n22 = AddNode(n11, 2.5f, 2);
            Node n43 = AddNode(n22, 4, 3);
            Node n24 = AddNode(n43, 2.5f, 4);
            Node n13 = AddNode(n24, 1, 3);
            Node n15 = AddNode(n13, 1, 5);
            n13.Link(n22);
            n13.Link(n11);
            n51.Link(n43);
            //Dorrway UP
            Node n35 = AddNode(n24, 3, 5);
        }

        public void SetStart(Vector2 position)
        {
            float lowestDistance = float.MaxValue;
            int count = 0;
            int target = 0;
            foreach (Node n in allNodes)
            {
                if (Vector2.Distance(position, n.position) < lowestDistance)
                {
                    lowestDistance = Vector2.Distance(position, n.position);
                    target = count;
                }
                count++;
            }

            Start = allNodes.ElementAt(target);

            openList[(int)Start.position.X, (int)Start.position.Y] = Start;
            Start.open = true;
        }

        public void ResetGoal(Vector2 position)
        {
            if(End != null) End.isGoal = false;
            float lowestDistance = float.MaxValue;
            int count = 0;
            int target = 0;
            foreach (Node n in allNodes)
            {
                if (Vector2.Distance(position, n.position) < lowestDistance)
                {
                    lowestDistance = Vector2.Distance(position, n.position);
                    target = count;
                }
                count++;
            }
            End = allNodes.ElementAt(target);
            End.isGoal = true;
        }

        public void Draw(Matrix view, Matrix projection, Model node, Model edge)
        {
            //Draw the lists over the actual nodes
            DrawOpen(view, projection, node);
            DrawClosed(view, projection, node);

            float scale = 0.1f;
            foreach (Node n in allNodes)
            {
                n.visited = false;
            }
            List<Node> toAdd = new List<Node>();
            toAdd.Add(Start);

            while (toAdd.Count != 0)
            {
                Node head = toAdd.First();

                if (!head.visited)
                {
                    scale = 0.1f;
                    if (head.isGoal)
                        scale = 0.2f;
                    toAdd.AddRange(head.children);
                    foreach (Node child in head.children)
                    {
                        //DrawLines(view, projection, head.position, child.position);
                    }
                    foreach (ModelMesh mesh in node.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.AmbientLightColor = new Vector3(0.8f, 0.3f, 0.1f);
                            effect.DiffuseColor = new Vector3(0.1f, 0.1f, 0.1f);
                            effect.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.5f, 0.22f);// Shinnyness/reflexive
                            effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(head.position.X, head.position.Y, 0);
                            effect.View = view;
                            effect.Alpha = 0.7f;
                            effect.Projection = projection;
                        }
                        mesh.Draw();
                    }
                }
                head.visited = true;
                toAdd.Remove(head);
            }
            foreach (Node n in allNodes)
            {
                n.visited = false;
            }
        }

        public void DrawClosed(Matrix view, Matrix projection, Model node)
        {
            foreach (Node n in closedList)
            {
                if (n != null)
                {
                    foreach (ModelMesh mesh in node.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.AmbientLightColor = new Vector3(0.8f, 0.3f, 0.1f);
                            effect.DiffuseColor = new Vector3(1f, 0.1f, 0.1f);
                            effect.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.5f, 0.22f);// Shinnyness/reflexive
                            effect.World = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(n.position.X, n.position.Y, 0);
                            effect.View = view;
                            effect.Projection = projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }
        public void DrawOpen(Matrix view, Matrix projection, Model node)
        {
            foreach (Node n in openList)
            {
                if (n != null)
                {
                    foreach (ModelMesh mesh in node.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.AmbientLightColor = new Vector3(0.8f, 0.3f, 0.1f);
                            effect.DiffuseColor = new Vector3(0.1f, 1f, 0.1f);
                            effect.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.5f, 0.22f);// Shinnyness/reflexive
                            effect.World = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(n.position.X, n.position.Y, 0);
                            effect.View = view;
                            effect.Projection = projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

    }
}
