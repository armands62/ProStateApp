
namespace ProStateApp2
{
    internal class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; internal set; }
        public string Name { get; internal set; }
        public double AvailableMoney { get; internal set; }
        public double ReservedMoney { get; internal set; }
        public override string ToString()
        {
            return $"{AccountNumber} - {Name}";
        }
    }
}