namespace MAUI;

public partial class MainPage : ContentPage
{
    VerticalStackLayout st;
    Button btn;
    List<ContentPage> pages =
        new List<ContentPage>() { new TestPage(), new FilePage()
        };

    public MainPage()
	{
        st = new VerticalStackLayout();
        foreach (ContentPage item in pages)
        {
            btn = new Button
            {
                Text = "Ava "+item.Title,
                Margin = 30
            };
            btn.Clicked+=async (s, e) => await Navigation.PushAsync(item);
            st.Children.Add(btn);
        }
        Content = new ScrollView { Content = st };
    }

	//private async void OnCounterClicked(object sender, EventArgs e)
	//{
	//	count++;

	//	if (count == 1)
 //           await Navigation.PushAsync(new Teine());
	//	else
	//		CounterBtn.Text = $"Clicked {count} times";

	//	SemanticScreenReader.Announce(CounterBtn.Text);
	//}
}

