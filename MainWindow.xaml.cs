using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CoEditDe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private AppSettings settings;

    public MainWindow()
    {
        InitializeComponent();
        
        settings = AppSettings.Load();
        
        if (settings.FirstRun)
        {
            var wizard = new SetupWizard();
            if (wizard.ShowDialog() == true)
            {
                settings = AppSettings.Load(); // Reload after wizard
            }
        }
        
        this.Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await webView.EnsureCoreWebView2Async();
        webView.Source = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www", "index.html"));
        
        // Apply saved settings
        var themeScript = $"if (window.editor) window.editor.updateOptions({{theme: '{settings.Theme}'}});";
        await webView.ExecuteScriptAsync(themeScript);
        
        var fontScript = $"if (window.editor) window.editor.updateOptions({{fontSize: {settings.FontSize}}});";
        await webView.ExecuteScriptAsync(fontScript);
    }

    private async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Code files|*.cs;*.py;*.js;*.ts;*.html;*.css;*.cpp;*.c;*.java|All files|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            var content = File.ReadAllText(dialog.FileName);
            var ext = System.IO.Path.GetExtension(dialog.FileName).ToLower();
            var language = GetLanguageFromExtension(ext);
            var encodedContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            var script = $"setEditorContent(atob('{encodedContent}'), '{language}')";
            await webView.ExecuteScriptAsync(script);
        }
    }

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(this);
        settingsWindow.ShowDialog();
    }

    private string GetLanguageFromExtension(string ext)
    {
        return ext switch
        {
            ".cs" => "csharp",
            ".py" => "python",
            ".js" => "javascript",
            ".ts" => "typescript",
            ".html" => "html",
            ".css" => "css",
            ".cpp" => "cpp",
            ".c" => "c",
            ".java" => "java",
            _ => "plaintext"
        };
    }
}