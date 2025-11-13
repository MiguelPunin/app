 (cd "$(git rev-parse --show-toplevel)" && git apply --3way <<'EOF' 
diff --git a/PromptGeneratorApp.sln b/PromptGeneratorApp.sln
new file mode 100644
index 0000000000000000000000000000000000000000..07ed0a4987470d8a0344795f4c27e342232cc388
--- /dev/null
+++ b/PromptGeneratorApp.sln
@@ -0,0 +1,21 @@
+Microsoft Visual Studio Solution File, Format Version 12.00
+# Visual Studio Version 17
+VisualStudioVersion = 17.0.31612.314
+MinimumVisualStudioVersion = 10.0.40219.1
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "PromptGeneratorApp", "PromptGeneratorApp/PromptGeneratorApp.csproj", "{2A0DFAE5-91AA-4ED7-9A56-9804175B3B2F}"
+EndProject
+Global
+GlobalSection(SolutionConfigurationPlatforms) = preSolution
+Debug|Any CPU = Debug|Any CPU
+Release|Any CPU = Release|Any CPU
+EndGlobalSection
+GlobalSection(ProjectConfigurationPlatforms) = postSolution
+{2A0DFAE5-91AA-4ED7-9A56-9804175B3B2F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
+{2A0DFAE5-91AA-4ED7-9A56-9804175B3B2F}.Debug|Any CPU.Build.0 = Debug|Any CPU
+{2A0DFAE5-91AA-4ED7-9A56-9804175B3B2F}.Release|Any CPU.ActiveCfg = Release|Any CPU
+{2A0DFAE5-91AA-4ED7-9A56-9804175B3B2F}.Release|Any CPU.Build.0 = Release|Any CPU
+EndGlobalSection
+GlobalSection(SolutionProperties) = preSolution
+HideSolutionNode = FALSE
+EndGlobalSection
+EndGlobal
diff --git a/PromptGeneratorApp/App.xaml b/PromptGeneratorApp/App.xaml
new file mode 100644
index 0000000000000000000000000000000000000000..9a28b4be4f2f7ce670470d6aefaae0513a8f16c6
--- /dev/null
+++ b/PromptGeneratorApp/App.xaml
@@ -0,0 +1,33 @@
+<?xml version="1.0" encoding="utf-8" ?>
+<Application
+    x:Class="PromptGeneratorApp.App"
+    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
+    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
+    RequestedTheme="Light">
+
+    <Application.Resources>
+        <ResourceDictionary>
+            <ResourceDictionary.MergedDictionaries>
+                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
+            </ResourceDictionary.MergedDictionaries>
+
+            <Color x:Key="PrimaryColor">#715AFF</Color>
+            <Color x:Key="SecondaryColor">#FCD34D</Color>
+            <Color x:Key="AccentColor">#F472B6</Color>
+
+            <Style TargetType="Label">
+                <Setter Property="TextColor" Value="White" />
+                <Setter Property="FontFamily" Value="OpenSansRegular" />
+            </Style>
+
+            <Style x:Key="BadgeStyle" TargetType="Label">
+                <Setter Property="Margin" Value="4,0" />
+                <Setter Property="Padding" Value="16,6" />
+                <Setter Property="BackgroundColor" Value="{DynamicResource PrimaryColor}" />
+                <Setter Property="CornerRadius" Value="18" />
+                <Setter Property="FontSize" Value="12" />
+                <Setter Property="HorizontalOptions" Value="Start" />
+            </Style>
+        </ResourceDictionary>
+    </Application.Resources>
+</Application>
diff --git a/PromptGeneratorApp/App.xaml.cs b/PromptGeneratorApp/App.xaml.cs
new file mode 100644
index 0000000000000000000000000000000000000000..77d278ffa3ef4dd06165399684693d4c7c3423b6
--- /dev/null
+++ b/PromptGeneratorApp/App.xaml.cs
@@ -0,0 +1,25 @@
+using Microsoft.Maui.Controls;
+using Microsoft.Maui.Graphics;
+using PromptGeneratorApp.Views;
+
+namespace PromptGeneratorApp;
+
+public partial class App : Application
+{
+    public App(MainPage mainPage)
+    {
+        InitializeComponent();
+
+        var navigationPage = new NavigationPage(mainPage)
+        {
+            BarTextColor = Colors.White
+        };
+
+        if (Resources.TryGetValue("PrimaryColor", out var color) && color is Color primary)
+        {
+            navigationPage.BarBackgroundColor = primary;
+        }
+
+        MainPage = navigationPage;
+    }
+}
diff --git a/PromptGeneratorApp/Data/PromptDatabase.cs b/PromptGeneratorApp/Data/PromptDatabase.cs
new file mode 100644
index 0000000000000000000000000000000000000000..0b84d0010e14f851013aadc65bc2beab44f0a276
--- /dev/null
+++ b/PromptGeneratorApp/Data/PromptDatabase.cs
@@ -0,0 +1,41 @@
+using System.IO;
+using Microsoft.Maui.Storage;
+using SQLite;
+
+namespace PromptGeneratorApp.Data;
+
+public class PromptDatabase
+{
+    private const string DatabaseFileName = "prompts.db3";
+    private static readonly string DatabasePath =
+        Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
+
+    private readonly SQLiteAsyncConnection _database;
+
+    public PromptDatabase()
+    {
+        _database = new SQLiteAsyncConnection(DatabasePath);
+    }
+
+    public async Task InitializeAsync()
+    {
+        await _database.CreateTableAsync<PromptRecord>();
+    }
+
+    public Task<List<PromptRecord>> GetPromptsAsync() => _database.Table<PromptRecord>().ToListAsync();
+
+    public Task<int> SavePromptAsync(PromptRecord record) => _database.InsertAsync(record);
+}
+
+public class PromptRecord
+{
+    [PrimaryKey, AutoIncrement]
+    public int Id { get; set; }
+
+    [MaxLength(120)]
+    public string Title { get; set; } = string.Empty;
+
+    public string Context { get; set; } = string.Empty;
+
+    public string Language { get; set; } = string.Empty;
+}
diff --git a/PromptGeneratorApp/Data/PromptRepository.cs b/PromptGeneratorApp/Data/PromptRepository.cs
new file mode 100644
index 0000000000000000000000000000000000000000..fcaced12942a7c4bbe92bd389ea2117edacc1f54
--- /dev/null
+++ b/PromptGeneratorApp/Data/PromptRepository.cs
@@ -0,0 +1,29 @@
+using System.Collections.Generic;
+using System.Linq;
+using PromptGeneratorApp.Models;
+
+namespace PromptGeneratorApp.Data;
+
+public class PromptRepository
+{
+    private readonly PromptDatabase _database;
+
+    public PromptRepository(PromptDatabase database)
+    {
+        _database = database;
+    }
+
+    public async Task<IReadOnlyList<Prompt>> GetPromptsAsync()
+    {
+        await _database.InitializeAsync();
+        var records = await _database.GetPromptsAsync();
+        return records.Select(Prompt.FromRecord).ToList();
+    }
+
+    public async Task AddPromptAsync(Prompt prompt)
+    {
+        await _database.InitializeAsync();
+        var record = prompt.ToRecord();
+        await _database.SavePromptAsync(record);
+    }
+}
diff --git a/PromptGeneratorApp/MauiProgram.cs b/PromptGeneratorApp/MauiProgram.cs
new file mode 100644
index 0000000000000000000000000000000000000000..39b59317ce17fba316b2bac8c45b883318138e4c
--- /dev/null
+++ b/PromptGeneratorApp/MauiProgram.cs
@@ -0,0 +1,32 @@
+using CommunityToolkit.Maui;
+using Microsoft.Extensions.DependencyInjection;
+using Microsoft.Maui.Controls.Hosting;
+using Microsoft.Maui.Hosting;
+using PromptGeneratorApp.Data;
+using PromptGeneratorApp.ViewModels;
+using PromptGeneratorApp.Views;
+
+namespace PromptGeneratorApp;
+
+public static class MauiProgram
+{
+    public static MauiApp CreateMauiApp()
+    {
+        var builder = MauiApp.CreateBuilder();
+        builder
+            .UseMauiApp<App>()
+            .UseMauiCommunityToolkit()
+            .ConfigureFonts(fonts =>
+            {
+                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
+                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
+            });
+
+        builder.Services.AddSingleton<PromptDatabase>();
+        builder.Services.AddSingleton<PromptRepository>();
+        builder.Services.AddSingleton<MainViewModel>();
+        builder.Services.AddSingleton<MainPage>();
+
+        return builder.Build();
+    }
+}
diff --git a/PromptGeneratorApp/Models/Prompt.cs b/PromptGeneratorApp/Models/Prompt.cs
new file mode 100644
index 0000000000000000000000000000000000000000..818d637133da258b7a2881229371e52893745314
--- /dev/null
+++ b/PromptGeneratorApp/Models/Prompt.cs
@@ -0,0 +1,15 @@
+using PromptGeneratorApp.Data;
+
+namespace PromptGeneratorApp.Models;
+
+public record Prompt(string Title, string Context, string Language)
+{
+    public static Prompt FromRecord(PromptRecord record) => new(record.Title, record.Context, record.Language);
+
+    public PromptRecord ToRecord() => new()
+    {
+        Title = Title,
+        Context = Context,
+        Language = Language
+    };
+}
diff --git a/PromptGeneratorApp/Platforms/Android/AndroidManifest.xml b/PromptGeneratorApp/Platforms/Android/AndroidManifest.xml
new file mode 100644
index 0000000000000000000000000000000000000000..e1163099a0a2076ac6b144ce710b8a9e838b2aec
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Android/AndroidManifest.xml
@@ -0,0 +1,5 @@
+<?xml version="1.0" encoding="utf-8"?>
+<manifest xmlns:android="http://schemas.android.com/apk/res/android">
+    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:label="Generador Inteligente de Prompts"></application>
+    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
+</manifest>
diff --git a/PromptGeneratorApp/Platforms/Android/MainActivity.cs b/PromptGeneratorApp/Platforms/Android/MainActivity.cs
new file mode 100644
index 0000000000000000000000000000000000000000..c50fc321d5f8128334c8cb21ec8028bae6890ec9
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Android/MainActivity.cs
@@ -0,0 +1,10 @@
+using Android.App;
+using Android.Content.PM;
+using Android.OS;
+
+namespace PromptGeneratorApp;
+
+[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
+public class MainActivity : MauiAppCompatActivity
+{
+}
diff --git a/PromptGeneratorApp/Platforms/Android/MainApplication.cs b/PromptGeneratorApp/Platforms/Android/MainApplication.cs
new file mode 100644
index 0000000000000000000000000000000000000000..986f9f133bf1b5725b59a5a562070f9ff8f28fe1
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Android/MainApplication.cs
@@ -0,0 +1,15 @@
+using Android.App;
+using Android.Runtime;
+
+namespace PromptGeneratorApp;
+
+[Application]
+public class MainApplication : MauiApplication
+{
+    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
+        : base(handle, ownership)
+    {
+    }
+
+    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
+}
diff --git a/PromptGeneratorApp/Platforms/MacCatalyst/AppDelegate.cs b/PromptGeneratorApp/Platforms/MacCatalyst/AppDelegate.cs
new file mode 100644
index 0000000000000000000000000000000000000000..d06882336ff37615a698b268a95c54a2cb8cea85
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/MacCatalyst/AppDelegate.cs
@@ -0,0 +1,9 @@
+using Foundation;
+
+namespace PromptGeneratorApp;
+
+[Register("AppDelegate")]
+public class AppDelegate : MauiUIApplicationDelegate
+{
+    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
+}
diff --git a/PromptGeneratorApp/Platforms/MacCatalyst/Info.plist b/PromptGeneratorApp/Platforms/MacCatalyst/Info.plist
new file mode 100644
index 0000000000000000000000000000000000000000..36a67284060bedbd79ebe6ed261bef1fc0e9b68f
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/MacCatalyst/Info.plist
@@ -0,0 +1,8 @@
+<?xml version="1.0" encoding="UTF-8"?>
+<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
+<plist version="1.0">
+<dict>
+    <key>CFBundleDisplayName</key>
+    <string>Generador Prompts</string>
+</dict>
+</plist>
diff --git a/PromptGeneratorApp/Platforms/MacCatalyst/Program.cs b/PromptGeneratorApp/Platforms/MacCatalyst/Program.cs
new file mode 100644
index 0000000000000000000000000000000000000000..944e405796b61e7a8123f93de881401c4e59c6f2
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/MacCatalyst/Program.cs
@@ -0,0 +1,12 @@
+using ObjCRuntime;
+using UIKit;
+
+namespace PromptGeneratorApp;
+
+public class Program
+{
+    static void Main(string[] args)
+    {
+        UIApplication.Main(args, null, typeof(AppDelegate));
+    }
+}
diff --git a/PromptGeneratorApp/Platforms/Tizen/Main.cs b/PromptGeneratorApp/Platforms/Tizen/Main.cs
new file mode 100644
index 0000000000000000000000000000000000000000..fc781d74ddb794537618505fb628f2f231306ab8
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Tizen/Main.cs
@@ -0,0 +1,16 @@
+using System;
+using Microsoft.Maui;
+using Microsoft.Maui.Hosting;
+
+namespace PromptGeneratorApp;
+
+class Program : MauiApplication
+{
+    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
+
+    static void Main(string[] args)
+    {
+        var app = new Program();
+        app.Run(args);
+    }
+}
diff --git a/PromptGeneratorApp/Platforms/Windows/App.xaml b/PromptGeneratorApp/Platforms/Windows/App.xaml
new file mode 100644
index 0000000000000000000000000000000000000000..05570e1a156ea42082ff770c47fd679e8c89226e
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Windows/App.xaml
@@ -0,0 +1,5 @@
+<Application
+    x:Class="PromptGeneratorApp.WinUI.App"
+    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
+    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
+</Application>
diff --git a/PromptGeneratorApp/Platforms/Windows/App.xaml.cs b/PromptGeneratorApp/Platforms/Windows/App.xaml.cs
new file mode 100644
index 0000000000000000000000000000000000000000..bd3988afce954e2e438405b6b544e63cb2aef2fb
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Windows/App.xaml.cs
@@ -0,0 +1,11 @@
+namespace PromptGeneratorApp.WinUI;
+
+public partial class App : MauiWinUIApplication
+{
+    public App()
+    {
+        InitializeComponent();
+    }
+
+    protected override MauiApp CreateMauiApp() => PromptGeneratorApp.MauiProgram.CreateMauiApp();
+}
diff --git a/PromptGeneratorApp/Platforms/Windows/Package.appxmanifest b/PromptGeneratorApp/Platforms/Windows/Package.appxmanifest
new file mode 100644
index 0000000000000000000000000000000000000000..38df8ff194cb3c4a1ca80b8cacf5d9b4349fa224
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/Windows/Package.appxmanifest
@@ -0,0 +1,23 @@
+<?xml version="1.0" encoding="utf-8"?>
+<Package
+  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
+  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
+  IgnorableNamespaces="uap">
+  <Identity Name="com.example.promptgenerator" Publisher="CN=Contoso" Version="1.0.0.0" />
+  <Properties>
+    <DisplayName>Generador Prompts</DisplayName>
+    <PublisherDisplayName>Contoso</PublisherDisplayName>
+    <Logo>Assets\StoreLogo.png</Logo>
+  </Properties>
+  <Dependencies>
+    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
+  </Dependencies>
+  <Resources>
+    <Resource Language="x-generate" />
+  </Resources>
+  <Applications>
+    <Application Id="App" Executable="PromptGeneratorApp.exe" EntryPoint="PromptGeneratorApp.WinUI.App">
+      <uap:VisualElements DisplayName="Generador Prompts" Description="Generador Inteligente de Prompts" Square150x150Logo="Assets\Square150x150Logo.png" Square44x44Logo="Assets\Square44x44Logo.png" BackgroundColor="transparent" />
+    </Application>
+  </Applications>
+</Package>
diff --git a/PromptGeneratorApp/Platforms/iOS/AppDelegate.cs b/PromptGeneratorApp/Platforms/iOS/AppDelegate.cs
new file mode 100644
index 0000000000000000000000000000000000000000..d06882336ff37615a698b268a95c54a2cb8cea85
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/iOS/AppDelegate.cs
@@ -0,0 +1,9 @@
+using Foundation;
+
+namespace PromptGeneratorApp;
+
+[Register("AppDelegate")]
+public class AppDelegate : MauiUIApplicationDelegate
+{
+    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
+}
diff --git a/PromptGeneratorApp/Platforms/iOS/Info.plist b/PromptGeneratorApp/Platforms/iOS/Info.plist
new file mode 100644
index 0000000000000000000000000000000000000000..50983fe5bcb6306dae8101d6e550a862c04460f4
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/iOS/Info.plist
@@ -0,0 +1,10 @@
+<?xml version="1.0" encoding="UTF-8"?>
+<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
+<plist version="1.0">
+<dict>
+    <key>CFBundleDisplayName</key>
+    <string>Generador Prompts</string>
+    <key>UILaunchStoryboardName</key>
+    <string>LaunchScreen</string>
+</dict>
+</plist>
diff --git a/PromptGeneratorApp/Platforms/iOS/Program.cs b/PromptGeneratorApp/Platforms/iOS/Program.cs
new file mode 100644
index 0000000000000000000000000000000000000000..944e405796b61e7a8123f93de881401c4e59c6f2
--- /dev/null
+++ b/PromptGeneratorApp/Platforms/iOS/Program.cs
@@ -0,0 +1,12 @@
+using ObjCRuntime;
+using UIKit;
+
+namespace PromptGeneratorApp;
+
+public class Program
+{
+    static void Main(string[] args)
+    {
+        UIApplication.Main(args, null, typeof(AppDelegate));
+    }
+}
diff --git a/PromptGeneratorApp/PromptGeneratorApp.csproj b/PromptGeneratorApp/PromptGeneratorApp.csproj
new file mode 100644
index 0000000000000000000000000000000000000000..54d61c266b7089ede90b8194724b9f2cd782ea77
--- /dev/null
+++ b/PromptGeneratorApp/PromptGeneratorApp.csproj
@@ -0,0 +1,24 @@
+<Project Sdk="Microsoft.NET.Sdk">
+  <PropertyGroup>
+    <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0</TargetFrameworks>
+    <OutputType>Exe</OutputType>
+    <RootNamespace>PromptGeneratorApp</RootNamespace>
+    <UseMaui>true</UseMaui>
+    <SingleProject>true</SingleProject>
+    <ImplicitUsings>enable</ImplicitUsings>
+    <Nullable>enable</Nullable>
+    <ApplicationTitle>Generador Inteligente de Prompts</ApplicationTitle>
+    <ApplicationId>com.example.promptgenerator</ApplicationId>
+    <SupportedOSPlatformVersion>10.0.19041.0</SupportedOSPlatformVersion>
+  </PropertyGroup>
+
+  <ItemGroup>
+    <PackageReference Include="CommunityToolkit.Maui" Version="8.0.10" />
+    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
+    <PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
+  </ItemGroup>
+
+  <ItemGroup>
+    <MauiAsset Include="Resources\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
+  </ItemGroup>
+</Project>
diff --git a/PromptGeneratorApp/README.md b/PromptGeneratorApp/README.md
new file mode 100644
index 0000000000000000000000000000000000000000..89646bf7efbaebbcb701cd12c2aba1f97f076feb
--- /dev/null
+++ b/PromptGeneratorApp/README.md
@@ -0,0 +1,26 @@
+# Generador Inteligente de Prompts (.NET MAUI)
+
+Aplicación de ejemplo creada con **.NET MAUI**, **XAML**, **C#** y **SQLite** para gestionar prompts enfocados en esos lenguajes y tecnologías.
+
+## Características
+
+- Interfaz creada con XAML siguiendo la estética mostrada en la captura (badges para .NET MAUI, XAML, C# y SQLite).
+- Lógica en C# con patrón MVVM usando `CommunityToolkit.Mvvm`.
+- Persistencia local con `sqlite-net-pcl` sobre una base de datos SQLite almacenada en el directorio de datos de la app.
+- Vista principal que permite crear y listar prompts guardados.
+
+## Estructura principal
+
+- `MauiProgram.cs`: Configuración de servicios y dependencias.
+- `App.xaml` / `App.xaml.cs`: Recursos compartidos y página principal.
+- `Views/MainPage.xaml`: Interfaz de usuario con badges y formulario.
+- `ViewModels/MainViewModel.cs`: Estado y comandos para la UI.
+- `Data/PromptDatabase.cs`: Acceso a SQLite.
+
+## Ejecución
+
+1. Instala el .NET SDK 8 y las workloads de MAUI.
+2. Restaura los paquetes: `dotnet restore`.
+3. Ejecuta en la plataforma deseada, por ejemplo Android: `dotnet build -t:Run -f net8.0-android`.
+
+> Este repositorio no incluye los recursos de fuentes ni imágenes del template original de MAUI; añade los que necesites al directorio `Resources/`.
diff --git a/PromptGeneratorApp/Resources/Images/.gitkeep b/PromptGeneratorApp/Resources/Images/.gitkeep
new file mode 100644
index 0000000000000000000000000000000000000000..e69de29bb2d1d6434b8b29ae775ad8c2e48c5391
diff --git a/PromptGeneratorApp/Resources/Styles/Styles.xaml b/PromptGeneratorApp/Resources/Styles/Styles.xaml
new file mode 100644
index 0000000000000000000000000000000000000000..46d5f187ca3fdc2c01236b13ae86172bb5d51211
--- /dev/null
+++ b/PromptGeneratorApp/Resources/Styles/Styles.xaml
@@ -0,0 +1,9 @@
+<?xml version="1.0" encoding="utf-8" ?>
+<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
+                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
+    <Style TargetType="Button">
+        <Setter Property="CornerRadius" Value="12" />
+        <Setter Property="HeightRequest" Value="48" />
+        <Setter Property="FontAttributes" Value="Bold" />
+    </Style>
+</ResourceDictionary>
diff --git a/PromptGeneratorApp/ViewModels/MainViewModel.cs b/PromptGeneratorApp/ViewModels/MainViewModel.cs
new file mode 100644
index 0000000000000000000000000000000000000000..a4a59eff38e15d8655ff8c0ed20e692079c00d80
--- /dev/null
+++ b/PromptGeneratorApp/ViewModels/MainViewModel.cs
@@ -0,0 +1,72 @@
+using System.Collections.ObjectModel;
+using CommunityToolkit.Mvvm.ComponentModel;
+using CommunityToolkit.Mvvm.Input;
+using Microsoft.Maui.Controls;
+using PromptGeneratorApp.Data;
+using PromptGeneratorApp.Models;
+
+namespace PromptGeneratorApp.ViewModels;
+
+public partial class MainViewModel : ObservableObject
+{
+    private readonly PromptRepository _repository;
+
+    [ObservableProperty]
+    private string title = string.Empty;
+
+    [ObservableProperty]
+    private string context = string.Empty;
+
+    [ObservableProperty]
+    private string selectedLanguage = "C#";
+
+    public ObservableCollection<Prompt> Prompts { get; } = new();
+
+    public IReadOnlyList<string> Languages { get; } = new[]
+    {
+        ".NET MAUI",
+        "XAML",
+        "C#",
+        "SQLite"
+    };
+
+    public MainViewModel(PromptRepository repository)
+    {
+        _repository = repository;
+    }
+
+    [RelayCommand]
+    private async Task LoadAsync()
+    {
+        Prompts.Clear();
+        var prompts = await _repository.GetPromptsAsync();
+        foreach (var prompt in prompts)
+        {
+            Prompts.Add(prompt);
+        }
+    }
+
+    [RelayCommand]
+    private async Task SaveAsync()
+    {
+        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Context))
+        {
+            if (Application.Current?.MainPage is not null)
+            {
+                await Application.Current.MainPage.DisplayAlert(
+                    "Datos incompletos",
+                    "Completa título y contexto para generar un prompt.",
+                    "Entendido");
+            }
+
+            return;
+        }
+
+        var prompt = new Prompt(Title, Context, SelectedLanguage);
+        await _repository.AddPromptAsync(prompt);
+        Prompts.Add(prompt);
+
+        Title = string.Empty;
+        Context = string.Empty;
+    }
+}
diff --git a/PromptGeneratorApp/Views/MainPage.xaml b/PromptGeneratorApp/Views/MainPage.xaml
new file mode 100644
index 0000000000000000000000000000000000000000..716d92494a83c887c1d7aa9eab43fef387ad4579
--- /dev/null
+++ b/PromptGeneratorApp/Views/MainPage.xaml
@@ -0,0 +1,67 @@
+<?xml version="1.0" encoding="utf-8" ?>
+<ContentPage
+    x:Class="PromptGeneratorApp.Views.MainPage"
+    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
+    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
+    Title="Generador Inteligente de Prompts">
+
+    <Grid
+        RowDefinitions="Auto,Auto,Auto,*"
+        Padding="24"
+        RowSpacing="16">
+
+        <StackLayout Grid.Row="0" Orientation="Horizontal" Spacing="8">
+            <Label Text=".NET MAUI" Style="{StaticResource BadgeStyle}" />
+            <Label Text="XAML" Style="{StaticResource BadgeStyle}" BackgroundColor="{DynamicResource SecondaryColor}" />
+            <Label Text="C#" Style="{StaticResource BadgeStyle}" BackgroundColor="{DynamicResource AccentColor}" />
+            <Label Text="SQLite" Style="{StaticResource BadgeStyle}" BackgroundColor="#34D399" />
+        </StackLayout>
+
+        <VerticalStackLayout Grid.Row="1" Spacing="12">
+            <Entry
+                Placeholder="Título del prompt"
+                Text="{Binding Title, Mode=TwoWay}" />
+            <Editor
+                Placeholder="Describe el contexto para el prompt"
+                AutoSize="TextChanges"
+                Text="{Binding Context, Mode=TwoWay}" />
+        </VerticalStackLayout>
+
+        <Picker
+            Grid.Row="2"
+            Title="Selecciona el lenguaje principal"
+            ItemsSource="{Binding Languages}"
+            SelectedItem="{Binding SelectedLanguage, Mode=TwoWay}" />
+
+        <VerticalStackLayout Grid.Row="3" Spacing="12">
+            <Button
+                Text="Guardar prompt"
+                BackgroundColor="{DynamicResource PrimaryColor}"
+                Command="{Binding SaveCommand}"
+                TextColor="White" />
+
+            <CollectionView ItemsSource="{Binding Prompts}">
+                <CollectionView.EmptyView>
+                    <Label
+                        HorizontalTextAlignment="Center"
+                        Text="Aún no hay prompts guardados."
+                        TextColor="#D1D5DB" />
+                </CollectionView.EmptyView>
+                <CollectionView.ItemTemplate>
+                    <DataTemplate>
+                        <Frame
+                            Margin="0,0,0,12"
+                            BorderColor="#3730A3"
+                            CornerRadius="12">
+                            <VerticalStackLayout>
+                                <Label FontAttributes="Bold" FontSize="18" Text="{Binding Title}" />
+                                <Label FontSize="14" Text="{Binding Context}" />
+                                <Label FontSize="12" TextColor="#A855F7" Text="{Binding Language}" />
+                            </VerticalStackLayout>
+                        </Frame>
+                    </DataTemplate>
+                </CollectionView.ItemTemplate>
+            </CollectionView>
+        </VerticalStackLayout>
+    </Grid>
+</ContentPage>
diff --git a/PromptGeneratorApp/Views/MainPage.xaml.cs b/PromptGeneratorApp/Views/MainPage.xaml.cs
new file mode 100644
index 0000000000000000000000000000000000000000..aed706953bd6a587bf7d447219dd1cd6154dbbc9
--- /dev/null
+++ b/PromptGeneratorApp/Views/MainPage.xaml.cs
@@ -0,0 +1,24 @@
+using CommunityToolkit.Mvvm.Input;
+using PromptGeneratorApp.ViewModels;
+
+namespace PromptGeneratorApp.Views;
+
+public partial class MainPage : ContentPage
+{
+    private readonly MainViewModel _viewModel;
+
+    public MainPage(MainViewModel viewModel)
+    {
+        InitializeComponent();
+        BindingContext = _viewModel = viewModel;
+    }
+
+    protected override async void OnAppearing()
+    {
+        base.OnAppearing();
+        if (_viewModel.LoadCommand is IAsyncRelayCommand loadCommand)
+        {
+            await loadCommand.ExecuteAsync(null);
+        }
+    }
+}
diff --git a/README.md b/README.md
new file mode 100644
index 0000000000000000000000000000000000000000..5c5e0f7458db63f18d757f020afbc63984c4f3c2
--- /dev/null
+++ b/README.md
@@ -0,0 +1,29 @@
+# Prompt Generator App
+
+Este repositorio contiene un ejemplo de aplicación **.NET MAUI** que emplea **XAML**, **C#** y **SQLite** para administrar una colección de prompts.
+
+## Estructura
+- `PromptGeneratorApp.sln`: Solución principal de .NET MAUI.
+- `PromptGeneratorApp/`: Código fuente de la aplicación, organizado por capas (Views, ViewModels, Data, Models).
+
+## Requisitos
+- .NET SDK 8
+- Workloads de .NET MAUI instaladas
+
+## Ejecución rápida
+```bash
+dotnet restore PromptGeneratorApp.sln
+dotnet build PromptGeneratorApp/PromptGeneratorApp.csproj
+```
+
+Para ejecutar en una plataforma específica (por ejemplo Android):
+```bash
+dotnet build -t:Run -f net8.0-android PromptGeneratorApp/PromptGeneratorApp.csproj
+```
+
+## Publicación en GitHub
+1. Crea un repositorio en GitHub y copia su URL.
+2. Añade el remoto: `git remote add origin <url>`.
+3. Sube la rama actual: `git push -u origin work`.
+
+Esto dejará disponible la app en tu repositorio de GitHub.
 
EOF
)
