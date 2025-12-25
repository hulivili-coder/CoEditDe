using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CoEditDeInstaller;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Perform installation
        await PerformInstallation();
        
        // Launch the installed app
        LaunchInstalledApp();
        
        // Close installer
        Application.Current.Shutdown();
    }

    private async Task PerformInstallation()
    {
        // Default install location
        string installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "CoEditDe");
        
        // Create directory
        Directory.CreateDirectory(installDir);
        
        // Copy main app files (assuming the installer is run from the same directory as the published files)
        string sourceDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        
        // Copy all files except the installer itself
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            if (!Path.GetFileName(file).Contains("Installer"))
            {
                File.Copy(file, Path.Combine(installDir, Path.GetFileName(file)), true);
            }
        }
        
        // Copy www folder
        string wwwSource = Path.Combine(Path.GetDirectoryName(sourceDir), "www");
        if (Directory.Exists(wwwSource))
        {
            CopyDirectory(wwwSource, Path.Combine(installDir, "www"));
        }
        
        // Create desktop shortcut
        CreateShortcut(installDir);
    }

    private void CopyDirectory(string source, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (string file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
        }
        foreach (string dir in Directory.GetDirectories(source))
        {
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }
    }

    private void CreateShortcut(string installDir)
    {
        string exePath = Path.Combine(installDir, "CoEditDe.exe");
        string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CoEditDe.lnk");
        
        // Create shortcut using PowerShell
        string script = $@"
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut('{desktopPath}')
$Shortcut.TargetPath = '{exePath}'
$Shortcut.WorkingDirectory = '{installDir}'
$Shortcut.Description = 'CoEditDe Code Editor'
$Shortcut.Save()
";
        
        Process.Start(new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{script}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        })?.WaitForExit();
    }

    private void LaunchInstalledApp()
    {
        string installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "CoEditDe");
        string exePath = Path.Combine(installDir, "CoEditDe.exe");
        
        if (File.Exists(exePath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = installDir
            });
        }
    }
}