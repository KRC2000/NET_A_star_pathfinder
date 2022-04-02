using System;

namespace Framework.Pathfinder
{
    public enum NodeState
    {
        Idle, Opened, Expired
    }
    class Node
    {
        public Node parent = null;
        public uint posX, posY;
        public NodeState state = NodeState.Idle; 
        public float g; // Distance to the start node through all parents
        public float h; // Distance to the finish node
        public float f; // g + h  

        public void RecalculateValues(uint finPosX, uint finPosY)
        {
            Recalculate_g();
            Recalculate_h(finPosX, finPosY);
            Recalculate_f();
        }

        public void RecalculateValuesBut_h()
        {
            Recalculate_g();
            Recalculate_f();
        }

        public float GetValueAccordingToParent_g(Node parentNode)
        {
            if (parentNode.posX == posX || parentNode.posY == posY) return parentNode.g + 1;
            return parentNode.g + 1.4142f;
        }

        private void Recalculate_g()
        {
            if (parent.posX == posX || parent.posY == posY) g = parent.g + 1;
            else g = parent.g + 1.4142f;
        }

        private void Recalculate_h(uint finPosX, uint finPosY)
        {
            uint toFinishVecX = finPosX - posX;
            uint toFinishVecY = finPosY - posY;
            h = (float)Math.Sqrt(toFinishVecX * toFinishVecX + toFinishVecY * toFinishVecY);
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