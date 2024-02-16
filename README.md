# Buform
![GitHub](https://img.shields.io/github/license/fedandburk/buform.svg)
![Nuget](https://img.shields.io/nuget/v/Fedandburk.Buform.svg)
[![CI](https://github.com/fedandburk/buform/actions/workflows/ci.yml/badge.svg)](https://github.com/fedandburk/buform/actions/workflows/ci.yml)
[![CD](https://github.com/fedandburk/buform/actions/workflows/cd.yml/badge.svg)](https://github.com/fedandburk/buform/actions/workflows/cd.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/fedandburk/buform/badge)](https://www.codefactor.io/repository/github/fedandburk/buform)

Buform is a library specifically designed for Xamarin developers, offering a solution for creating and managing forms in mobile apps.

With Buform, Xamarin developers can easily design and implement various types of forms, such as settings forms, registration forms, and more, with flexible and customizable form components.

The library provides a user-friendly API that simplifies form creation and input handling, along with robust validations support.

## Features

- Compatible with [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms), [Maui](https://github.com/dotnet/maui), and [MvvmCross](https://github.com/MvvmCross/MvvmCross)
- Can handle live targets, e.g. targets that implement `INotifyPropertyChanged` interface
- Can handle dynamic form definition updates
- Supports user input validation and compatible with [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- Can display a read-only form or item
- Provides a set of default items, like text, switch, button, date and time pickers, etc.
- Supports custom item types and UI implementation overrides for existing items

## Installation

Use [NuGet](https://www.nuget.org) package manager to install this library.

```bash
Install-Package Fedandburk.Buform
```

## Usage

See the [Wiki](https://github.com/fedandburk/buform/wiki) section.
