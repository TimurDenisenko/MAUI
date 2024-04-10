using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;

namespace MAUI
{
    public partial class Teine : ContentPage
    {
        CarouselView cv;
        ObservableCollection<LocalPage> pages;
        public Teine()
        {
            Title = "CarouselPage";
            cv = new CarouselView();
            pages = new ObservableCollection<LocalPage>();
            cv.ItemsSource = pages;
            cv.ItemTemplate = new DataTemplate(() =>
            {
                ImageButton btn = new ImageButton
                {
                    HeightRequest = 200,
                    WidthRequest = 200,
                };
                Label header = new Label
                {
                    FontSize = 32,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.FromRgb(50, 120, 170),
                };
                return new StackLayout {  };

            });
            CreatePage("Noorem tarkvaraarendaja", "https://www.tthk.ee/opetatavad_erialad/noorem-tarkvaraarendaja/");
            CreatePage("Logistika IT-süsteemide nooremspetsialist", "https://www.tthk.ee/opetatavad_erialad/logistika-it-susteemide-nooremspetsialist/");
            CreatePage("Mehhatroonik", "https://www.tthk.ee/opetatavad_erialad/mehhatroonik/");
            CreatePage("Tööstusinformaatik", "https://www.tthk.ee/opetatavad_erialad/toostusinformaatik/");
            CreatePage("Juuksur", "https://www.tthk.ee/opetatavad_erialad/juuksur/");
            CreatePage("Robotitehnik", "https://www.tthk.ee/opetatavad_erialad/robotitehnik/");
            Content = cv;
        }

        private void CreatePage(string header, string url)
        {
            ImageButton btn = new ImageButton();
            btn.Clicked += async (sender, e) =>
                await Browser.OpenAsync(url);
            Label label = new Label
            {
                Text = header,
                FontSize = 32,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.FromRgb(50, 120, 170),
            };
            pages.Add(new LocalPage { });
        }
    }
    
}