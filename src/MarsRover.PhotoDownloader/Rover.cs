using Ardalis.SmartEnum;

namespace MarsRover.PhotoDownloader
{
    public sealed class Rover : SmartEnum<Rover>
    {
        public static readonly Rover Curiosity = new Rover("Curiosity", 5);
        public static readonly Rover Opportunity = new Rover("Opportunity", 6);
        public static readonly Rover Spirit = new Rover("Spirit", 7);

        private Rover(string name, int id) : base(name, id) { }
    }
}