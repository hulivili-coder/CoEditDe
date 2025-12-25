using System.Windows;
using System.Windows.Controls;

namespace CoEditDe;

/// <summary>
/// Interaction logic for SetupWizard.xaml
/// </summary>
public partial class SetupWizard : Window
{
    private int currentPage = 0;
    private AppSettings settings;
    private StackPanel[] pages;

    public SetupWizard()
    {
        InitializeComponent();
        settings = new AppSettings();
        CreatePages();
        ShowPage(0);
    }

    private void CreatePages()
    {
        // Page 0: Welcome
        var welcomePage = new StackPanel();
        welcomePage.Children.Add(new TextBlock { Text = "Welcome to CoEditDe!", FontSize = 24, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
        welcomePage.Children.Add(new TextBlock { Text = "This setup wizard will help you configure CoEditDe for the first time.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 20) });
        welcomePage.Children.Add(new TextBlock { Text = "Click Next to continue.", FontStyle = FontStyles.Italic });

        // Page 1: Theme
        var themePage = new StackPanel();
        themePage.Children.Add(new TextBlock { Text = "Choose Your Theme", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
        var themeGroup = new GroupBox { Header = "Editor Theme" };
        var themePanel = new StackPanel { Margin = new Thickness(10) };
        var darkRadio = new RadioButton { Content = "Dark Theme", IsChecked = true, Margin = new Thickness(0, 5, 0, 5) };
        darkRadio.Checked += (s, e) => settings.Theme = "vs-dark";
        var lightRadio = new RadioButton { Content = "Light Theme", Margin = new Thickness(0, 5, 0, 5) };
        lightRadio.Checked += (s, e) => settings.Theme = "vs";
        themePanel.Children.Add(darkRadio);
        themePanel.Children.Add(lightRadio);
        themeGroup.Content = themePanel;
        themePage.Children.Add(themeGroup);

        // Page 2: Font Size
        var fontPage = new StackPanel();
        fontPage.Children.Add(new TextBlock { Text = "Choose Font Size", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
        var fontGroup = new GroupBox { Header = "Font Size (pixels)" };
        var fontPanel = new StackPanel { Margin = new Thickness(10) };
        var fontCombo = new ComboBox { SelectedIndex = 1 };
        fontCombo.Items.Add("12");
        fontCombo.Items.Add("14");
        fontCombo.Items.Add("16");
        fontCombo.Items.Add("18");
        fontCombo.Items.Add("20");
        fontCombo.SelectionChanged += (s, e) => 
        {
            if (int.TryParse(fontCombo.SelectedItem?.ToString(), out int size))
                settings.FontSize = size;
        };
        fontPanel.Children.Add(fontCombo);
        fontGroup.Content = fontPanel;
        fontPage.Children.Add(fontGroup);

        // Page 3: Language
        var langPage = new StackPanel();
        langPage.Children.Add(new TextBlock { Text = "Preferred Language", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
        var langGroup = new GroupBox { Header = "Default Language for New Files" };
        var langPanel = new StackPanel { Margin = new Thickness(10) };
        var langCombo = new ComboBox { SelectedIndex = 0 };
        langCombo.Items.Add("C#");
        langCombo.Items.Add("Python");
        langCombo.Items.Add("JavaScript");
        langCombo.Items.Add("Plain Text");
        langCombo.SelectionChanged += (s, e) => 
        {
            settings.PreferredLanguage = langCombo.SelectedItem?.ToString() switch
            {
                "C#" => "csharp",
                "Python" => "python", 
                "JavaScript" => "javascript",
                _ => "plaintext"
            };
        };
        langPanel.Children.Add(langCombo);
        langGroup.Content = langPanel;
        langPage.Children.Add(langGroup);

        // Page 4: Finish
        var finishPage = new StackPanel();
        finishPage.Children.Add(new TextBlock { Text = "Setup Complete!", FontSize = 24, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
        finishPage.Children.Add(new TextBlock { Text = "CoEditDe is now configured and ready to use.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 20) });
        finishPage.Children.Add(new TextBlock { Text = "You can change these settings anytime from the Settings menu.", FontStyle = FontStyles.Italic });

        pages = new[] { welcomePage, themePage, fontPage, langPage, finishPage };
    }

    private void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;
        PageContent.Content = pages[pageIndex];

        BackButton.Visibility = pageIndex > 0 ? Visibility.Visible : Visibility.Collapsed;
        NextButton.Visibility = pageIndex < pages.Length - 1 ? Visibility.Visible : Visibility.Collapsed;
        FinishButton.Visibility = pageIndex == pages.Length - 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentPage > 0) ShowPage(currentPage - 1);
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentPage < pages.Length - 1) ShowPage(currentPage + 1);
    }

    private void FinishButton_Click(object sender, RoutedEventArgs e)
    {
        settings.FirstRun = false;
        settings.Save();
        DialogResult = true;
        Close();
    }
}