using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public abstract class RobotAI<TCommand>: IMoveCommand
    {
        public Point Destination => throw new NotImplementedException();

        public abstract object GetCommand();
    }

    public class ShooterAI : RobotAI<IShooterMoveCommand>
    {
        int counter = 1;

        public override object GetCommand()
        {
            return ShooterCommand.ForCounter(counter++);
        }
    }

    public class BuilderAI : RobotAI<IMoveCommand>
    {
        int counter = 1;

        public override object GetCommand()
        {
            return BuilderCommand.ForCounter(counter++);
        }
    }

    public interface  IDevice<TCommand>: IMoveCommand, IShooterMoveCommand
    {
          string ExecuteCommand(object command);
    }

    public class Mover : IDevice<IMoveCommand>, IDevice<IShooterMoveCommand>
    {
        public Point Destination => throw new NotImplementedException();

        public bool ShouldHide => throw new NotImplementedException();

        public  string ExecuteCommand(object commands)
        {
            var command = commands as IMoveCommand;
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }
   
    public class ShooterMover : IDevice<IMoveCommand>, IDevice<IShooterMoveCommand>
    {
        public bool ShouldHide => throw new NotImplementedException();

        public Point Destination => throw new NotImplementedException();

        public  string ExecuteCommand(object commands)
        {
            var command = commands as IShooterMoveCommand;
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public class Robot<TCommand>
    {
        RobotAI<TCommand> ai;
        IDevice<TCommand> device;

        public Robot(RobotAI<TCommand> ai, IDevice<TCommand> executor)
        {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
    }

    public static class Robot
    {
        public static Robot<T> Create<T>(RobotAI<T> ai, IDevice<T> executor)
        {
            return new Robot<T>(ai, executor);
        }
    }
}