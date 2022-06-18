using UnityEngine;

namespace HRYooba.Library.Network
{
    public class TuioPointData
    {
        public TuioPointType Type { get; private set; }
        public int Id { get; private set; }
        public Vector2 Position { get; private set; }

        public TuioPointData(TuioPointType type, int id, Vector2 position)
        {
            Type = type;
            Id = id;
            Position = position;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
        }
    }
}