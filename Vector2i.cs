
namespace Framework
{
    struct Vector2i
    {
        public int X { get; set; } 
        public int Y { get; set; } 

        public Vector2i(int x, int y) { X = x; Y = y; }
        public override string ToString()
        {
            return $"Vector2i {{ X: {this.X}; Y: {this.Y} }}";
        }
        public static Vector2i operator +(Vector2i a, Vector2i b) => new Vector2i(a.X + b.X, a.Y + b.Y);
        public static Vector2i operator -(Vector2i a, Vector2i b) => new Vector2i(a.X - b.X, a.Y - b.Y);
        public static bool operator ==(Vector2i a, Vector2i b) => (a.X == b.X && a.Y == b.Y)? true : false;
        public static bool operator !=(Vector2i a, Vector2i b) => (a.X != b.X || a.Y != b.Y)? true : false;
    }
}