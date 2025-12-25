using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoEditDe;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private MainWindow mainWindow;

    public SettingsWindow(MainWindow owner)
    {
        InitializeComponent();
        this.Owner = owner;
        mainWindow = owner;
        
        // Load current settings
        var settings = AppSettings.Load();
        ThemeComboBox.SelectedIndex = settings.Theme == "vs-dark" ? 0 : 1;
        FontSizeComboBox.Text = settings.FontSize.ToString();
    }

    private async void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem item)
        {
            string theme = item.Content.ToString() == "Dark" ? "vs-dark" : "vs";
            var script = $"if (window.editor) window.editor.updateOptions({{theme: '{theme}'}});";
            await mainWindow.webView.ExecuteScriptAsync(script);
            
            // Save setting
            var settings = AppSettings.Load();
            settings.Theme = theme;
            settings.Save();
        }
    }

    private async void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FontSizeComboBox.SelectedItem is ComboBoxItem item && int.TryParse(item.Content.ToString(), out int fontSize))
        {
            var script = $"if (window.editor) window.editor.updateOptions({{fontSize: {fontSize}}});";
            await mainWindow.webView.ExecuteScriptAsync(script);
            
            // Save setting
            var settings = AppSettings.Load();
            settings.FontSize = fontSize;
            settings.Save();
        }
    }

    private void ExecuteButton_Click(object sender, RoutedEventArgs e)
    {
        ExecuteCommand();
    }

    private void ConsoleInputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ExecuteCommand();
        }
    }

    private async void ExecuteCommand()
    {
        string command = ConsoleInputTextBox.Text.Trim();
        if (string.IsNullOrEmpty(command)) return;

        ConsoleOutputTextBox.AppendText($"> {command}\n");

        if (command.ToLower() == "version")
        {
            ConsoleOutputTextBox.AppendText("CoEditDe Version 1.0.0\n");
            ConsoleOutputTextBox.AppendText("Updates:\n");
            ConsoleOutputTextBox.AppendText("- 0.1: Initial release with Monaco Editor\n");
            ConsoleOutputTextBox.AppendText("- 0.2: Added file opening and settings\n");
            ConsoleOutputTextBox.AppendText("- 1.0: Added developer console\n");
        }
        else
        {
            // Execute Windows command
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    ConsoleOutputTextBox.AppendText(output);
                if (!string.IsNullOrEmpty(error))
                    ConsoleOutputTextBox.AppendText("Error: " + error);
            }
            catch (Exception ex)
            {
                ConsoleOutputTextBox.AppendText($"Error executing command: {ex.Message}\n");
            }
        }

        ConsoleOutputTextBox.AppendText("\n");
        ConsoleInputTextBox.Clear();
        ConsoleOutputTextBox.ScrollToEnd();
    }
}