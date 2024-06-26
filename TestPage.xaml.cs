﻿using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MAUI;

public partial class TestPage : ContentPage
{
    CarouselView cv;
    ObservableCollection<LocalPage> pages;
    char[] alp = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z','Ö','Ä','Ü','Õ','Ž','Š'};
    public TestPage()
    {
        Title = "TestPage";
        Shell.SetBackgroundColor(this, Color.FromRgb(50, 44, 43));
        cv = new CarouselView { BackgroundColor = Color.FromRgb(50, 44, 43), Loop = false };
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
            Button btn = new() { WidthRequest = 300, HeightRequest = 400, FontSize = 15, BackgroundColor = Color.FromRgb(128, 61, 59), TextColor = Color.FromRgb(228, 197, 158), CornerRadius = 15, BorderWidth = 1, BorderColor = Color.FromRgb(175, 130, 96),  };
            btn.SetBinding(Button.TextProperty, "Word");
            Label num = new() { FontSize = 15, HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromRgb(228, 197, 158) };
            num.SetBinding(Label.TextProperty, "Num");
            Button create = new() { Text = "Loo uus" };
            create.Clicked += (s, e) => { CreateLocalPage(); GeneratePages();};
            Button delete = new() {Text = "Kustuta" };
            Button edit = new() { Text = "Muuda"  };
            Button doButton = new();
            doButton.SetBinding(Button.TextProperty,"Do");
            doButton.Loaded += (s, e) => doButton.Text = doButton.Text == "Õpitud" ? "Õpitud" : "Õppimata";
            doButton.Clicked += async (s, e) =>
            {
                doButton.TextColor = Colors.White;
                if (doButton.Text == "Õpitud")
                {
                    doButton.BackgroundColor = Colors.Red;
                    doButton.Text = "Õppimata";
                }
                else
                {
                    doButton.BackgroundColor = Colors.Green;
                    doButton.Text = "Õpitud";
                }
                await Task.Delay(500);
                doButton.BackgroundColor = Color.FromRgb(128, 61, 59);
                doButton.TextColor = Color.FromRgb(228, 197, 158);
                pages = new ObservableCollection<LocalPage>(pages.Select(x => { x.Do = x.Word == btn.Text ? doButton.Text : x.Do; return x; }));
                SerializeToFile(pages);
            };
            Button sort = new() { Text = "Sorteeri" };
            sort.Clicked +=async(s, e) =>
            {
                string sort = await DisplayActionSheet("Sorteerimine","Tühista",null, "А-Я", "A-Z", "0-9", "Õpitud");
                if (sort == null) return;
                else if (sort == "A-Z")
                {
                    if (LocalPage.Sort == "A-Z")
                    {
                        pages = new ObservableCollection<LocalPage>(pages.Reverse().Cast<LocalPage>());
                        LocalPage.Sort = "Z-A";
                    }
                    else
                    {
                        pages = new ObservableCollection<LocalPage>(pages.OrderBy(x => x.Word[0]).Cast<LocalPage>());
                        LocalPage.Sort = "A-Z";
                    }
                }
                else if (sort == "А-Я")
                {
                    if (LocalPage.Sort == "А-Я")
                    {
                        pages = new ObservableCollection<LocalPage>(pages.Reverse().Cast<LocalPage>());
                        LocalPage.Sort = "Я-А";
                    }
                    else
                    {
                        pages = new ObservableCollection<LocalPage>(pages.OrderBy(x => x.Translated[0]).Cast<LocalPage>());
                        LocalPage.Sort = "А-Я";
                    }
                }
                else if (sort == "0-9")
                {
                    if (LocalPage.Sort == "0-9")
                    {
                        pages = new ObservableCollection<LocalPage>(pages.Reverse().Cast<LocalPage>());
                        LocalPage.Sort = "9-0";
                    }
                    else
                    {
                        pages = new ObservableCollection<LocalPage>(pages.OrderBy(x => x.Num).Cast<LocalPage>());
                        LocalPage.Sort = "0-9";
                    }
                }
                else if (sort == "Õpitud")
                {
                    if (LocalPage.Sort == "Õpitud")
                    {
                        pages = new ObservableCollection<LocalPage>(pages.Reverse().Cast<LocalPage>());
                        LocalPage.Sort = "Õppimata";
                    }
                    else
                    {
                        pages = new ObservableCollection<LocalPage>(pages.OrderBy(x => x.Do).Cast<LocalPage>());
                        LocalPage.Sort = "Õpitud";
                    }
                }
                GeneratePages();
            };
            foreach (Button item in new Button[] {create,delete,edit,doButton,sort})
            {
                item.WidthRequest = 100;
                item.BackgroundColor = Color.FromRgb(128, 61, 59);
                item.BorderWidth = 1;
                item.BorderColor = Color.FromRgb(175, 130, 96);
                item.TextColor = Color.FromRgb(228, 197, 158);
            }
            HorizontalStackLayout hsl = new HorizontalStackLayout
            {
                Children = { create,new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent, IsEnabled = false }, delete, new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent, IsEnabled = false },edit },
                HorizontalOptions = LayoutOptions.Center,
            };
            HorizontalStackLayout hsl1 = new HorizontalStackLayout
            {
                Children = { doButton, new Button { WidthRequest = 19, BackgroundColor = Colors.Transparent, IsEnabled = false }, sort },
                HorizontalOptions = LayoutOptions.Center,
            };
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 10 },
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 200 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 300 },
                    new RowDefinition { Height = GridLength.Star },
                },
                Children = { btn,  num, hsl,hsl1 }
            };
            btn.Clicked += async (s, e) =>
            {
                if (btn.RotationY != 0) return;
                string text = btn.RotationX==1 ? "Word" : "Translated";
                while (btn.RotationY<=90)
                {
                    btn.RotationY += 4;
                    btn.Opacity -= 4 / 90;
                    await Task.Delay(1);
                }
                btn.RotationY = 270;
                btn.SetBinding(Button.TextProperty, text);
                while (btn.RotationY <= 360)
                {
                    btn.RotationY+=4;
                    btn.Opacity += 4 / 90;
                    await Task.Delay(1);
                }
                btn.Opacity = 1;
                btn.IsEnabled = true;
                btn.RotationY = 0;
                btn.RotationX= btn.RotationX==1 ? 0 : 1;
            };
            delete.Clicked += (s, e) =>
            {
                if (pages.Count !=1)
                {
                    int i = 0;
                    --LocalPage.Created;
                    pages = new ObservableCollection<LocalPage>(pages.Where(x => x.Word != btn.Text).Select(x => { x.Num = ++i; return x; }).Cast<LocalPage>());
                    SerializeToFile(pages);
                    GeneratePages();
                }
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
            grid.SetRow(hsl, 0);
            grid.SetRow(hsl1, 2);
            grid.SetRow(btn, 4);
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