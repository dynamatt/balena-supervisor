namespace Balena.Models
{
    public class Application
    {
        public int Id { get; set; }
        //public string User { get; set; }
        public bool OwnsDevice { get; set; }

        public string AppName { get; set; }

        public string DeviceType { get; set; }

        public override string ToString()
        {
            return $"{Id} - {AppName}";
        }
    }
}
