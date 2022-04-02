using System;

using Microsoft.Xna.Framework;

namespace Framework.Pathfinder
{
    public enum NodeState
    {
        Idle, Opened, Expired
    }
    class Node
    {
        public Node parent = null;
        public Point pos;
        public NodeState state = NodeState.Idle; 
        public float g; // Distance to the start node through all parents
        public float h; // Distance to the finish node
        public float f; // g + h  

        public void RecalculateValues(Point finPos)
        {
            Recalculate_g();
            Recalculate_h(finPos);
            Recalculate_f();
        }

        public void RecalculateValuesBut_h()
        {
            Recalculate_g();
            Recalculate_f();
        }

        public float GetValueAccordingToParent_g(Node parentNode)
        {
            if (parentNode.pos.X == pos.X || parentNode.pos.Y == pos.Y) return parentNode.g + 1;
            return parentNode.g + 1.4142f;
        }

        private void Recalculate_g()
        {
            if (parent.pos.X == pos.X || parent.pos.Y == pos.Y) g = parent.g + 1;
            else g = parent.g + 1.4142f;
        }

        private void Recalculate_h(Point finPos)
        {
            Point toFinishVec = finPos - pos; 
            h = (float)Math.Sqrt(toFinishVec.X * toFinishVec.X + toFinishVec.Y * toFinishVec.Y);
        }

        private void Recalculate_f()
        {
            f = g + h;
        }
        public void Reset()
        {
            parent = null;
            state = NodeState.Idle;
            f = 0; g = 0; h = 0;
        }
    }
}