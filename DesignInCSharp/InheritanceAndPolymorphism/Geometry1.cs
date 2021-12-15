using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Inheritance.Geometry.Virtual
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point);

        public abstract RectangularCuboid GetBoundingBox();
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, 2 * Radius, 2 * Radius, 2 * Radius);
        }
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var minPoint = new Vector3(
                Position.X - SizeX / 2,
                Position.Y - SizeY / 2,
                Position.Z - SizeZ / 2);
            var maxPoint = new Vector3(
                Position.X + SizeX / 2,
                Position.Y + SizeY / 2,
                Position.Z + SizeZ / 2);

            return point >= minPoint && point <= maxPoint;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, SizeX, SizeY, SizeZ);
        }
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;

            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, 2 * Radius, 2 * Radius, SizeZ);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override bool ContainsPoint(Vector3 point) =>
            Parts.Any(body => body.ContainsPoint(point));

        public override RectangularCuboid GetBoundingBox()
        {
            var radius = 0.0;
            var sizeX = 0.0;
            var sizeY = 0.0;
            var sizeZ = 0.0;
            var isBallContains = false;
            const int figuresCount = 6;
            const double indent = 2.2;

            foreach (var loc in Parts)
            {
                if (loc is not Ball ball)
                    continue;
                radius = ball.Radius;
                isBallContains = true;
                sizeX = sizeY = 2 * radius;
                sizeZ = radius * 20 + indent * (figuresCount - 1);
            }

            if (isBallContains || Parts.Count != 2)
                return new RectangularCuboid(
                    new Vector3(Position.X, Position.Y, (float) (sizeZ / 2 - radius + Position.Z)),
                    sizeX, sizeY, sizeZ);

            var subX = Math.Abs(((RectangularCuboid) Parts[0]).Position.X -
                                ((RectangularCuboid) Parts[1]).Position.X);
            var subY = Math.Abs(((RectangularCuboid) Parts[0]).Position.Y -
                                ((RectangularCuboid) Parts[1]).Position.Y);
            var subZ = Math.Abs(((RectangularCuboid) Parts[0]).Position.Z -
                                ((RectangularCuboid) Parts[1]).Position.Z);
            sizeX = ((RectangularCuboid) Parts[0]).SizeX + subX;
            sizeY = ((RectangularCuboid) Parts[0]).SizeY + subY;
            sizeZ = ((RectangularCuboid) Parts[0]).SizeZ + subZ;

            return new RectangularCuboid(new Vector3(Position.X, Position.Y, (float) (sizeZ / 2 - radius + Position.Z)),
                sizeX, sizeY, sizeZ);
        }
    }
}