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
        CreatePage(new List<LocalPage>
        {
            new LocalPage("Tarkvara","Программное обеспечение"),
            new LocalPage("Sõnastik","Словарь"),
        });
        GeneratePages();
    }
    private void GeneratePages()
    {
        cv.ItemsSource = pages;
        cv.ItemTemplate = new DataTemplate(() =>
        {
            Button btn = new Button { WidthRequest = 300, HeightRequest = 400 };
            btn.SetBinding(Button.TextProperty, "Word");
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
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                },
                Children = { btn,  num, hsl }
            };
            btn.Clicked += async (s, e) => 
            {
                while (btn.RotationY!=90)
                {
                    ++btn.RotationY;
                    btn.Opacity -= 0.011;
                    await Task.Delay(1);
                }
                btn.RotationY = 270;
                btn.SetBinding(Button.TextProperty, "Translated");
                while (btn.RotationY != 360)
                {
                    ++btn.RotationY;
                    btn.Opacity += 0.011;
                    await Task.Delay(1);
                }
            };
            delete.Clicked += (s, e) =>
            {
                int i = 0;
                --LocalPage.Created;
                pages = new ObservableCollection<LocalPage>(pages.Where(x => x.Word != btn.Text).Select(x => { x.Num = ++i; return x; }).Cast<LocalPage>());
                GeneratePages();
            };
            grid.SetRow(hsl, 0);
            grid.SetRow(btn, 3);
            grid.SetRow(num, 6);
            return grid;
        });
        Content = cv;
    }
    private void CreatePage(List<LocalPage> pages)
    {
        foreach (LocalPage item in pages)
        {
            CreatePage(item);
        }
    }
    private void CreatePage(string word, string translate) => pages.Add(new LocalPage(word, translate));
    private void CreatePage(LocalPage localPage) => pages.Add(localPage); 
    private async void CreateLocalPage()
    {
        string word, translate;
        while (true)
        {
            word = await DisplayPromptAsync("Sõna", "Kirjuta sõna", cancel: "Tühista");
            if (pages.Select(x => x.Word.ToLower()).Contains(word.ToLower()))
            {
                await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                continue;
            }
            break;
        }
        if (word == null) { return; }
        while (true)
        {
            translate = await DisplayPromptAsync("Tõlge", "Kirjuta sõna tõlge", cancel: "Tühista");
            if (pages.Select(x => x.Translated).Contains(translate))
            {
                await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                continue;
            }
            break;
        }
        if (translate == null) { return; }
        CreatePage(word, translate);
    }
}