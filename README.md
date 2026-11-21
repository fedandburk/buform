# Buform
![GitHub](https://img.shields.io/github/license/fedandburk/buform.svg)
![Nuget](https://img.shields.io/nuget/v/Fedandburk.Buform.svg)
[![CI](https://github.com/fedandburk/buform/actions/workflows/ci.yml/badge.svg)](https://github.com/fedandburk/buform/actions/workflows/ci.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/fedandburk/buform/badge)](https://www.codefactor.io/repository/github/fedandburk/buform)

Buform is a framework for list-based screens in .NET apps for iOS and Android. It builds menus, settings, details, data entry and collections of records from one model in shared code, and shows them with native views: UIKit cells on iOS and Material 3 list items on Android.

## Highlights

Buform does the wiring of list screens, so the app code can focus on its own UI.

- **list mechanics:** data sources, adapters, cell reuse, animated updates, selection, swipe to delete and reorder
- **bindings:** each row reads and writes its property of the view model, in both directions
- **validation:** errors show on the rows, also with FluentValidation
- **your design kit:** cells and view holders from your own design kit for any item, header or footer, each registered with one call
- **system look by default:** the built-in rows use Liquid Glass on iOS and Material 3 on Android

## Packages

| Package | Contents |
|---|---|
| [Fedandburk.Buform](https://www.nuget.org/packages/Fedandburk.Buform) | the form model and the native iOS and Android views |
| [Fedandburk.Buform.Maui](https://www.nuget.org/packages/Fedandburk.Buform.Maui) | the views for .NET MAUI |
| [Fedandburk.Buform.FluentValidation](https://www.nuget.org/packages/Fedandburk.Buform.FluentValidation) | validation with [FluentValidation](https://github.com/FluentValidation/FluentValidation) |
| [Fedandburk.Buform.ColorPicker](https://www.nuget.org/packages/Fedandburk.Buform.ColorPicker) | a colour picker item |

Buform works with any view model that implements `INotifyPropertyChanged`, for example with [MvvmCross](https://github.com/MvvmCross/MvvmCross) or [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet). No other package is necessary.

## Installation

```bash
dotnet add package Fedandburk.Buform
```

Add the other packages in the same way.

## Usage

The [wiki](https://github.com/fedandburk/buform/wiki) describes the features, the requirements and the usage.
