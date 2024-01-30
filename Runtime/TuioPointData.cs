using System;
using UnityEngine;

namespace HRYooba.TUIO
{
    public class TuioPointData : IEquatable<TuioPointData>
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

        public bool Equals(TuioPointData other)
        {
            return Id == other.Id && Type == other.Type;
        }

        internal void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public override bool Equals(object obj) => Equals(obj as TuioPointData);
        public override int GetHashCode() => (Type, Id).GetHashCode();
        public override string ToString() => $"Type: {Type}, Id: {Id}, Position: {Position}";
    }
}