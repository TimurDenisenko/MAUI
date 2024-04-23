namespace MAUI
{
    public class LocalPage
    {
        public string Word { get; set; }
        public string Translated { get; set; }
        public static int Created { get; set; }
        public int Num { get; set; }
        public string Do { get; set; }
        
        public LocalPage() { }
        public LocalPage(string Word, string TranslatedWord) 
        { 
            this.Word = Word;
            this.Translated = TranslatedWord;
            Num = ++Created;
        }
    }
}
