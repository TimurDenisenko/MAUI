using Newtonsoft.Json;
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
        cv = new CarouselView { };
        pages = DeserializeFromFile<ObservableCollection<LocalPage>>();
        if (pages==null)
        {
            pages = new ObservableCollection<LocalPage>();
            CreatePage(new List<LocalPage>
            {
                new LocalPage("Tarkvara","Программное обеспечение"),
                new LocalPage("Sõnastik","Словарь"),
            });
        }
        GeneratePages();
    }
    private void GeneratePages()
    {
        cv.ItemsSource = pages;
        cv.ItemTemplate = new DataTemplate(() =>
        {
            Button btn = new Button { WidthRequest = 300, HeightRequest = 400};
            btn.SetBinding(Button.TextProperty, "Word");
            Label num = new Label
            {
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
            };
            num.SetBinding(Label.TextProperty, "Num");
            Button create = new Button { Text = "Loo uus küsimus" };
            create.Clicked += (s, e) => { CreateLocalPage();  } ;
            Button delete = new Button { Text = "Kustuta küsimus" };
            Button edit = new Button { Text = "Muuda küsimus" };
            HorizontalStackLayout hsl = new HorizontalStackLayout
            {
                Children = { create, delete, edit },
                HorizontalOptions = LayoutOptions.Center,
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
                if (btn.RotationY != 0) return;
                string text = btn.RotationX==1 ? "Word" : "Translated";
                while (btn.RotationY!=90)
                {
                    ++btn.RotationY;
                    btn.Opacity -= 0.011;
                    await Task.Delay(1);
                }
                btn.RotationY = 270;
                btn.SetBinding(Button.TextProperty, text);
                while (btn.RotationY != 360)
                {
                    ++btn.RotationY;
                    btn.Opacity += 0.011;
                    await Task.Delay(1);
                }
                btn.Opacity = 1;
                btn.IsEnabled = true;
                btn.RotationY = 0;
                btn.RotationX= btn.RotationX==1 ? 0 : 1;
            };
            delete.Clicked += (s, e) =>
            {
                int i = 0;
                --LocalPage.Created;
                pages = new ObservableCollection<LocalPage>(pages.Where(x => x.Word != btn.Text).Select(x => { x.Num = ++i; return x; }).Cast<LocalPage>());
                SerializeToFile(pages);
                GeneratePages();
            };
            edit.Clicked+=async (s, e) =>
            {
                string uus;
                string valik = await DisplayActionSheet("Mis sa tahad muutuda?", "Tühista", null, "Sõna", "Sõna tõlge");
                if (valik==null) return;
                btn.SetBinding(Button.TextProperty, valik == "Sõna" ? "Word" : "Translated");
                while (true)
                {
                    uus = await DisplayPromptAsync(valik, "Kirjuta" + valik.ToLower(), cancel: "Tühista");
                    if (pages.Select(x => valik == "Sõna" ? x.Word.ToLower() : x.Translated.ToLower()).Contains(uus.ToLower()))
                    {
                        await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                        continue;
                    }
                    break;
                }
                pages = new ObservableCollection<LocalPage>(pages.Select(x =>
                {
                    if (x.Word == btn.Text)
                    {
                        x.Word = uus;
                        btn.RotationX = 0;
                    }
                    else if (x.Translated == btn.Text)
                    {
                        x.Translated = uus;
                        btn.RotationX = 1;
                    }
                    return x;
                }).Cast<LocalPage>());
                btn.Text = uus;
                SerializeToFile(pages);
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
        SerializeToFile(pages);
    }
    public static void SerializeToFile<T>(T obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"/file.json", json);
    }
    public static T DeserializeFromFile<T>()
    {
        string json;
        try
        {
            json = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"/file.json");
        }
        catch (Exception)
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(json);
    }
}