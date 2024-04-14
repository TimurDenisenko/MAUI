using System.Collections.ObjectModel;

namespace MAUI;

public partial class TestPage : ContentPage
{
    CarouselView cv;
    ObservableCollection<LocalPage> pages;
    int i = 0;
    public TestPage()
	{
        Title = "TestPage";
        cv = new CarouselView();
        pages = new ObservableCollection<LocalPage>();
        CreatePage("Noorem tarkvaraarendaja", "https://www.tthk.ee/opetatavad_erialad/noorem-tarkvaraarendaja/");
        CreatePage("Logistika IT-süsteemide nooremspetsialist", "https://www.tthk.ee/opetatavad_erialad/logistika-it-susteemide-nooremspetsialist/");
        CreatePage("Mehhatroonik", "https://www.tthk.ee/opetatavad_erialad/mehhatroonik/");
        CreatePage("Tööstusinformaatik", "https://www.tthk.ee/opetatavad_erialad/toostusinformaatik/");
        CreatePage("Juuksur", "https://www.tthk.ee/opetatavad_erialad/juuksur/");
        CreatePage("Robotitehnik", "https://www.tthk.ee/opetatavad_erialad/robotitehnik/");
        GeneratePages();
    }
    private void GeneratePages()
    {
        cv.ItemsSource = pages;
        cv.ItemTemplate = new DataTemplate(() =>
        {
            Label header = new Label
            {
                FontSize = 32,
                HorizontalOptions = LayoutOptions.Center,
            };
            header.SetBinding(Label.TextProperty, "Header");

            Label desc = new Label
            {
                FontSize = 25,
                HorizontalOptions = LayoutOptions.Center,
            };
            desc.SetBinding(Label.TextProperty, "Description");
            Button btn = new Button { WidthRequest = 300 };
            btn.Clicked += async (s, e) => await Browser.OpenAsync(desc.Text);
            Label num = new Label
            {
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
            };
            num.SetBinding(Label.TextProperty, "Num");
            Button create = new Button { Text = "Loo uus küsimus" };
            create.Clicked += (s, e) => CreateLocalPage();
            Button delete = new Button { Text = "Kustuta küsimus" };
            HorizontalStackLayout hsl = new HorizontalStackLayout
            {
                Children = { create, delete }
            };
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 100 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                },
                Children = { header, btn, desc, num, hsl }
            };
            delete.Clicked += (s, e) =>
            {
                int i = 0;
                pages = new ObservableCollection<LocalPage>(pages.Where(x => x.Header != header.Text).Select(x => { x.Num = ++i; return x; }).Cast<LocalPage>());
                GeneratePages();
            };
            grid.SetRow(hsl, 0);
            grid.SetRow(header, 1);
            grid.SetRow(btn, 3);
            grid.SetRow(desc, 5);
            grid.SetRow(num, 7);
            return grid;
        });
        Content = cv;
    }
    private void CreatePage(string header, string desc)
    {
        pages.Add(new LocalPage(header, desc));
    }
    private async void CreateLocalPage()
    {
        string header = await DisplayPromptAsync("Päis", "Kirjuta päis", cancel: "Tühista");
        if (header == "Tühista" || header == string.Empty) { return; }
        string desc = await DisplayPromptAsync("Kirjeldus", "Kirjuta kirjeldus", cancel: "Tühista");
        if (desc == "Tühista" || desc == string.Empty) { return; }
        CreatePage(header, desc);
    }
}