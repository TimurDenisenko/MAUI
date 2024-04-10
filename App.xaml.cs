namespace MAUI;

public partial class App : Application
{
	public App()
	{
		MainPage = new Shell
		{
			CurrentItem = new MainPage()
		};
	}
}
