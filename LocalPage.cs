namespace MAUI
{
    public class LocalPage
    {
        public string Header { get; set; }
        public string Description { get; set; }
        private static int created { get; set; }
        public int Num { get; set; }
        
        public LocalPage() { }
        public LocalPage(string header, string desc) 
        { 
            Header = header;
            Description = desc;
            Num = ++created;
        }
    }
}
