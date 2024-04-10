namespace MAUI;

public partial class FilePage : ContentPage
{
    string folderPath=Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
	public FilePage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateFiles();
    }
    private async void ButtonFile_Clicked(object sender, EventArgs e)
    {
        string fileName = EntryFile.Text;
        if (string.IsNullOrEmpty(fileName)) return;
        if (File.Exists(Path.Combine(folderPath,fileName)))
        {
            bool isSave = await DisplayAlert("Kinnitus","Fail un juba olemas. Kas tahas ümber kirjutada?","Jah","Ei");
            if (!isSave) return;
        }
        await File.WriteAllTextAsync(Path.Combine(folderPath, fileName), EditorText.Text);
        UpdateFiles();
        ListFile.SelectedItem = null;
    }
    private void UpdateFiles() => ListFile.ItemsSource=Directory.GetFiles(folderPath).Select(Path.GetFileName);
    private void ListFile_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem==null) return;
        string fileName = e.SelectedItem.ToString();
        EditorText.Text = File.ReadAllText(Path.Combine(folderPath, fileName));
        EntryFile.Text = fileName;
        ListFile.SelectedItem = null;
    }

    private void Delete_Clicked(object sender, EventArgs e)
    {

    }

    private void ToList_Clicked(object sender, EventArgs e)
    {

    }
}