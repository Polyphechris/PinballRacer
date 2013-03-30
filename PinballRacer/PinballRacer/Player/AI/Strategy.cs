using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PinballRacer.Player.AI
{
    public abstract class Strategy
    {
        public const float MAX_SPEED = 4;
        public const float MAX_ACCELERATION = 1f;
        public const float RADIUS_ARRIVE = 3;
        public const float SAFE_ZONE = 12;
        protected const float MAX_ALPHA = 0.1f;
        protected const float MAX_W = 1f;
        public float detection = 3;
        protected const float FRICTION = 1f;
        protected const float TOLERANCE = 0.05f;
        protected Player player;
        protected Player target;
        public Player tag;

        public Vector3 steering;

        //Two sets of behaviours for each NPC
        public abstract Vector3 Chase(Player p, List<Player> players);
        public abstract Vector3 Evade(Player p, List<Player> players);
    }
}
