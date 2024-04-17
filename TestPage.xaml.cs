using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MAUI;

public partial class TestPage : ContentPage
{
    CarouselView cv;
    ObservableCollection<LocalPage> pages;
    int i = 0;
    char[] alp = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public TestPage()
	{
        Title = "TestPage";
        cv = new CarouselView { BackgroundColor = Color.FromRgb(50, 44, 43) };
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
        else
        {
            LocalPage.Created = pages.Count;
        }
        GeneratePages();
    }
    private void GeneratePages()
    {
        cv.ItemsSource = pages;
        cv.ItemTemplate = new DataTemplate(() =>
        {
            Button btn = new Button { WidthRequest = 300, HeightRequest = 400, FontSize = 25, BackgroundColor = Color.FromRgb(128, 61, 59), TextColor = Color.FromRgb(228, 197, 158), CornerRadius = 15, BorderWidth = 1, BorderColor = Color.FromRgb(175, 130, 96),  };
            btn.SetBinding(Button.TextProperty, "Word");
            Label num = new Label { FontSize = 15, HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromRgb(228, 197, 158) };
            num.SetBinding(Label.TextProperty, "Num");
            Button create = new Button { Text = "Loo uus küsimus",BackgroundColor = Color.FromRgb(128, 61, 59), BorderWidth = 1, BorderColor = Color.FromRgb(175, 130, 96), TextColor = Color.FromRgb(228, 197, 158) };
            create.Clicked += (s, e) => { CreateLocalPage();  } ;
            Button delete = new Button { Text = "Kustuta küsimus", BackgroundColor = Color.FromRgb(128, 61, 59), BorderWidth = 1, BorderColor = Color.FromRgb(175, 130, 96), TextColor = Color.FromRgb(228, 197, 158) };
            Button edit = new Button { Text = "Muuda küsimus", BackgroundColor = Color.FromRgb(128, 61, 59), BorderWidth = 1, BorderColor = Color.FromRgb(175, 130, 96), TextColor = Color.FromRgb(228, 197, 158) };
            Button doButton = new Button { };
            doButton.SetBinding(Label.TextProperty, "Do");
            doButton.Text = doButton.Text == "true" ? "Õpitud" : "Õppimata";
            Button sort = new Button { Text = "Sorteerimine" };
            HorizontalStackLayout hsl = new HorizontalStackLayout
            {
                Children = { create,new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent }, delete, new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent },edit },
                HorizontalOptions = LayoutOptions.Center,
            };
            HorizontalStackLayout hsl1 = new HorizontalStackLayout
            {
                Children = { doButton, new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent }, sort },
                HorizontalOptions = LayoutOptions.Center,
            };
            VerticalStackLayout vsl = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children = {hsl, hsl1}
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
                    new RowDefinition { Height = GridLength.Star },
                },
                Children = { btn,  num, vsl}
            };
            btn.Clicked += async (s, e) =>
            {
                if (btn.RotationY != 0) return;
                string text = btn.RotationX==1 ? "Word" : "Translated";
                while (btn.RotationY!=90)
                {
                    btn.RotationY += 3;
                    btn.Opacity -= 0.033;
                    await Task.Delay(1);
                }
                btn.RotationY = 270;
                btn.SetBinding(Button.TextProperty, text);
                while (btn.RotationY != 360)
                {
                    btn.RotationY+=3;
                    btn.Opacity += 0.033;
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
                    uus = await DisplayPromptAsync(valik, "Kirjuta " + valik.ToLower(), cancel: "Tühista");
                    if (uus==null) return;
                    if (uus == string.Empty)
                    {
                        await DisplayAlert("Viga", "See sõna on tühi", "Tühista");
                        continue;
                    }
                    else if (pages.Select(x => x.Word.ToLower()).Contains(uus.ToLower()) || pages.Select(x => x.Translated.ToLower()).Contains(uus.ToLower()))
                    {
                        await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                        continue;
                    }
                    else if (valik == "Sõna" && !(uus.ToUpper().Where(x => alp.Contains(x)).Count() == uus.Length) || valik != "Sõna" && !(uus.ToUpper().Where(x => !alp.Contains(x)).Count() == uus.Length))
                    {
                        await DisplayAlert("Viga", "Vale keel", "Tühista");
                        continue;
                    }
                    break;
                }
                uus = char.ToUpper(uus[0]) + uus.Substring(1);
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
            grid.SetRow(vsl, 0);
            grid.SetRow(btn, 3);
            grid.SetRow(num, 7);
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
            if (word == null) return;
            if (word == string.Empty)
            {
                await DisplayAlert("Viga", "See sõna on tühi", "Tühista");
                continue;
            }
            else if (pages.Select(x => x.Word.ToLower()).Contains(word.ToLower()))
            {
                await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                continue;
            }
            else if (!(word.ToUpper().Where(x => alp.Contains(x)).Count() == word.Length))
            {
                await DisplayAlert("Viga", "Vale keel", "Tühista");
                continue;
            }
            break;
        }
        while (true)
        {
            translate = await DisplayPromptAsync("Tõlge", "Kirjuta sõna tõlge", cancel: "Tühista");
            if (translate == null) return;
            if (translate == string.Empty)
            {
                await DisplayAlert("Viga", "See sõna on tühi", "Tühista");
                continue;
            }
            else if (pages.Select(x => x.Translated).Contains(translate))
            {
                await DisplayAlert("Viga", "See sõna on olemas", "Tühista");
                continue;
            }
            else if (!(translate.Where(x => !alp.Contains(x)).Count() == translate.Length))
            {
                await DisplayAlert("Viga", "Vale keel", "Tühista");
                continue;
            }
            break;
        }
        CreatePage(char.ToUpper(word[0]) + word.Substring(1), char.ToUpper(translate[0]) + translate.Substring(1));
        cv.ScrollTo(pages.Count());
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