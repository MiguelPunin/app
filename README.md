# Prompt Generator App

Este repositorio contiene un ejemplo de aplicación **.NET MAUI** que emplea **XAML**, **C#** y **SQLite** para administrar una colección de prompts.

## Estructura
- `PromptGeneratorApp.sln`: Solución principal de .NET MAUI.
- `PromptGeneratorApp/`: Código fuente de la aplicación, organizado por capas (Views, ViewModels, Data, Models).

## Requisitos
- .NET SDK 8
- Workloads de .NET MAUI instaladas

## Ejecución rápida
```bash
dotnet restore PromptGeneratorApp.sln
dotnet build PromptGeneratorApp/PromptGeneratorApp.csproj
```

Para ejecutar en una plataforma específica (por ejemplo Android):
```bash
dotnet build -t:Run -f net8.0-android PromptGeneratorApp/PromptGeneratorApp.csproj
```

## Publicación en GitHub
1. Crea un repositorio en GitHub y copia su URL.
2. Añade el remoto: `git remote add origin <url>`.
3. Sube la rama actual: `git push -u origin work`.

Esto dejará disponible la app en tu repositorio de GitHub.
