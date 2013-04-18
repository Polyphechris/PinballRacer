using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Track;

namespace PinballRacer.Players.Strategies
{
    public abstract class Strategy
    {
        //public const float MAX_SPEED = 4;
        //public const float MAX_ACCELERATION = 1f;
        //public const float RADIUS_ARRIVE = 3;
        //public const float SAFE_ZONE = 12;
        //protected const float MAX_ALPHA = 0.1f;
        //protected const float MAX_W = 1f;
        //public float detection = 3;
        //protected const float FRICTION = 1f;
        //protected const float TOLERANCE = 0.05f;
        //protected Player player;
        //protected Player target;
        //public Player tag;

        //public Vector3 steering;

        protected float points_value;
        protected float distance_value;
        float rankModifier;
        float pointModifier;

        Random random;

        public float GenerateHeuristic(Vector2 nodePosition, Vector2 goalPosition, Obstacle obstacle)
        {
            float heuristic = 0f;
            float distanceH = 0;//heuristc for distance
            float scoreH = 0;//heuristc for score

            //Calculate heuritic based on score
            int score = 0;
            if(obstacle != null)
            {
                score = obstacle.score;
                if (score == 5000)
                {
                    heuristic = 1;
                }
            }
            scoreH = (((float)score) / 10000f) * 100f;

            //Calculate heuristic based on distance to goal
            distanceH = Vector2.Distance(nodePosition, goalPosition);

            //Max value to 100 then multiply them by probabilities (weighted average)
            distanceH = Math.Min(distanceH, 100);
            scoreH = Math.Min(scoreH, 100);

            float randomMultiple = random.Next(90, 110);
            heuristic = (scoreH * points_value) + (distanceH * distance_value);
            heuristic = heuristic * randomMultiple / 100;
            return heuristic;
        }

        protected virtual void Initialize()
        {
            random = new Random();
        }
        ////Two sets of behaviours for each NPC
        //public abstract Vector3 Chase(Player p, List<Player> players);
        //public abstract Vector3 Evade(Player p, List<Player> players);
    }
}
