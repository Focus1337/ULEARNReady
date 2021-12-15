using System;
using System.Collections.Generic;

namespace Inheritance.Geometry.Visitor
{
    public interface IVisitor
    {
        Body Visit(Ball ball);
        Body Visit(RectangularCuboid cube);
        Body Visit(Cylinder cylinder);
        Body Visit(CompoundBody compoundBody);

    }

    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor visitor);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;

        }

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
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

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
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

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor visitor) =>
            visitor.Visit(this);
    }

    public class BoundingBoxVisitor : IVisitor
    {
        public Body Visit(Ball ball) =>
            new RectangularCuboid(ball.Position, 2 * ball.Radius, 2 * ball.Radius, 2 * ball.Radius);

        public Body Visit(RectangularCuboid cube) =>
            new RectangularCuboid(cube.Position, cube.SizeX, cube.SizeY, cube.SizeZ);

        public Body Visit(Cylinder cylinder) =>
            new RectangularCuboid(cylinder.Position, 2 * cylinder.Radius, 2 * cylinder.Radius, cylinder.SizeZ);

        public Body Visit(CompoundBody compoundBody)
        {
            var radius = 0.0;
            var sizeX = 0.0;
            var sizeY = 0.0;
            var sizeZ = 0.0;
            var isBallContains = false;
            const int figuresCount = 6;
            const double indent = 2.2;

            foreach (var loc in compoundBody.Parts)
            {
                if (loc is not Ball) continue;
                radius = ((Ball) loc).Radius;
                isBallContains = true;
                sizeX = sizeY = 2 * radius;
                sizeZ = radius * 20 + indent * (figuresCount - 1);
            }

            if (isBallContains || compoundBody.Parts.Count != 2)
                return new RectangularCuboid(
                    new Vector3(compoundBody.Position.X, compoundBody.Position.Y,
                        sizeZ / 2 - radius + compoundBody.Position.Z), sizeX, sizeY, sizeZ);
            var subX = Math.Abs(((RectangularCuboid) compoundBody.Parts[0]).Position.X -
                                ((RectangularCuboid) compoundBody.Parts[1]).Position.X);
            var subY = Math.Abs(((RectangularCuboid) compoundBody.Parts[0]).Position.Y -
                                ((RectangularCuboid) compoundBody.Parts[1]).Position.Y);
            var subZ = Math.Abs(((RectangularCuboid) compoundBody.Parts[0]).Position.Z -
                                ((RectangularCuboid) compoundBody.Parts[1]).Position.Z);
            sizeX = ((RectangularCuboid) compoundBody.Parts[0]).SizeX + subX;
            sizeY = ((RectangularCuboid) compoundBody.Parts[0]).SizeY + subY;
            sizeZ = ((RectangularCuboid) compoundBody.Parts[0]).SizeZ + subZ;

            return new RectangularCuboid(
                new Vector3(compoundBody.Position.X, compoundBody.Position.Y,
                    sizeZ / 2 - radius + compoundBody.Position.Z), sizeX, sizeY, sizeZ);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        public Body Visit(Ball ball) =>
            new RectangularCuboid(ball.Position, 2 * ball.Radius, 2 * ball.Radius, 2 * ball.Radius);

        public Body Visit(RectangularCuboid cube) =>
            new RectangularCuboid(cube.Position, cube.SizeX, cube.SizeY, cube.SizeZ);

        public Body Visit(Cylinder cylinder) =>
            new RectangularCuboid(cylinder.Position, 2 * cylinder.Radius, 2 * cylinder.Radius, cylinder.SizeZ);

        public Body Visit(CompoundBody compoundBody)
        {
            var list = new List<Body> { };
            foreach (var loc in compoundBody.Parts)
            {
                if (loc is CompoundBody)
                {
                    var internalList = new List<RectangularCuboid> { };
                    foreach (var t in ((CompoundBody) loc).Parts)
                    {
                        internalList.Add(t.TryAcceptVisitor<RectangularCuboid>(new BoundingBoxVisitor()));
                    }

                    var internalCompound = new CompoundBody(internalList);
                    list.Add(internalCompound);
                }
                else
                {
                    list.Add(loc.TryAcceptVisitor<RectangularCuboid>(new BoundingBoxVisitor()));
                }
            }

            return new CompoundBody(list);
        }
    }
}