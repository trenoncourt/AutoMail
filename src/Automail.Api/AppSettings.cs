namespace Automail.Api
{
    public class AppSettings
    {
        public Cors Cors { get; set; }
    }

    public class Cors
    {
        public bool Enabled { get; set; }

        public string Methods { get; set; }

        public string Origins { get; set; }

        public string Headers { get; set; }
    }
}