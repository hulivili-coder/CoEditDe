# CoEditDe

A simple code editor application built with C# WPF and Monaco Editor, similar to Visual Studio Code but focused only on editing code files.

## Features

- Monaco Editor integration for rich code editing
- Support for multiple programming languages (C#, Python, JavaScript, TypeScript, HTML, CSS, C++, C, Java, etc.)
- File opening via menu
- Syntax highlighting and IntelliSense-like features
- Customizable theme and font size
- Built-in developer console
- First-run setup wizard for easy configuration
- Persistent settings storage

## Requirements

- .NET 8.0
- WebView2 runtime (usually installed with Windows)

## Building and Running

1. Ensure .NET 8.0 is installed.
2. Clone or download the project.
3. Run `dotnet build` to build the project.
4. Run `dotnet run` to launch the application.

## Publishing as Installer

To create a Windows installer exe:

```bash
dotnet publish CoEditDeInstaller -c Release -r win-x64 --self-contained
```

The installer will be in `CoEditDeInstaller\bin\Release\net8.0-windows\win-x64\publish\CoEditDeInstaller.exe`

This installer includes the setup wizard and automatically installs CoEditDe to Program Files.

## Settings

- **Basic Settings**: Change theme (Dark/Light) and font size
- **Developer Console**: Type commands like "version" for app info, or any Windows command (e.g., "dir", "ipconfig")

## Setting up GitHub Website

1. Create a new repository on GitHub named `CoEditDe`
2. Initialize git in the project folder:
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   ```
3. Add the remote repository:
   ```bash
   git remote add origin https://github.com/YOUR_USERNAME/CoEditDe.git
   git push -u origin main
   ```
4. Go to repository Settings > Pages
5. Set Source to "Deploy from a branch"
6. Set Branch to "main" and folder to "/docs"
7. Save

The website will be available at `https://YOUR_USERNAME.github.io/CoEditDe/`

## Usage

- Use File > Open to load a code file.
- The editor will automatically detect the language based on file extension.
- Use Settings to customize the editor appearance and access developer tools.

## Troubleshooting

- If the editor doesn't load, ensure you have an internet connection for loading Monaco Editor from CDN.
- For offline use, consider hosting Monaco locally (advanced setup required).
- If WebView2 fails, install the WebView2 runtime from Microsoft.