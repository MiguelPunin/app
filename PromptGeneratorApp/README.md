# Generador Inteligente de Prompts (.NET MAUI)

Aplicación de ejemplo creada con **.NET MAUI**, **XAML**, **C#** y **SQLite** para gestionar prompts enfocados en esos lenguajes y tecnologías.

## Características

- Interfaz creada con XAML siguiendo la estética mostrada en la captura (badges para .NET MAUI, XAML, C# y SQLite).
- Lógica en C# con patrón MVVM usando `CommunityToolkit.Mvvm`.
- Persistencia local con `sqlite-net-pcl` sobre una base de datos SQLite almacenada en el directorio de datos de la app.
- Vista principal que permite crear y listar prompts guardados.

## Estructura principal

- `MauiProgram.cs`: Configuración de servicios y dependencias.
- `App.xaml` / `App.xaml.cs`: Recursos compartidos y página principal.
- `Views/MainPage.xaml`: Interfaz de usuario con badges y formulario.
- `ViewModels/MainViewModel.cs`: Estado y comandos para la UI.
- `Data/PromptDatabase.cs`: Acceso a SQLite.

## Ejecución

1. Instala el .NET SDK 8 y las workloads de MAUI.
2. Restaura los paquetes: `dotnet restore`.
3. Ejecuta en la plataforma deseada, por ejemplo Android: `dotnet build -t:Run -f net8.0-android`.

> Este repositorio no incluye los recursos de fuentes ni imágenes del template original de MAUI; añade los que necesites al directorio `Resources/`.
