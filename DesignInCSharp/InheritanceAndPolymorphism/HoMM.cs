namespace Inheritance.MapObjects
{
    public interface IMapObject
    {
        void Interact(Player player);
    }

    interface IArmy
    {
        Army Army { get; set; }
    }

    public interface IOwner
    {
        int Owner { get; set; }
    }
    
    interface ITreasure
    {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : IMapObject
    {
        public int Owner { get; set; }

        public void Interact(Player p) => Owner = p.Id;
    }

    public class Mine : IMapObject, IArmy, IOwner
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }

        public void Interact(Player player)
        {
            if (player.CanBeat(Army))
            {
                Owner = player.Id;
                player.Consume(Treasure);
            }
            else player.Die();
        }
    }

    public class Creeps : IMapObject, IArmy
    {
        public Treasure Treasure { get; set; }
        public Army Army { get; set; }

        public void Interact(Player player)
        {
            if (player.CanBeat(Army))
                player.Consume(Treasure);
            else
                player.Die();
        }
    }

    public class Wolves : IMapObject
    {
        public Army Army { get; set; }

        public void Interact(Player player)
        {
            if (!player.CanBeat(Army))
                player.Die();
        }
    }

    public class ResourcePile : IMapObject
    {
        public Treasure Treasure { get; set; }

        public void Interact(Player player) => player.Consume(Treasure);
    }

    public static class Interaction
    {
        public static void Make(Player player, IMapObject mapObject) => mapObject.Interact(player);
    }
}