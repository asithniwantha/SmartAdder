# WinUI 3 Calculator App / Control

This is a WinUI 3 (Windows App SDK) application that functions like a dynamic Excel-cell calculator. It allows users to enter numbers into textboxes. A sum of all numbers is displayed at the top, and entering a number into the last blank textbox automatically generates a new blank textbox below it.

## Architecture

This project strictly follows the Model-View-ViewModel (MVVM) architecture to ensure modularity and ease of integration into existing applications.

*   **ViewModels (`ViewModels/`)**: Contains `ViewModelBase`, `CellItemViewModel`, and `CalculatorViewModel`. The logic for tracking cell values, summing them, and dynamically adding new cells resides entirely within `CalculatorViewModel`.
*   **Views (`Views/`)**: Contains `CalculatorControl.xaml` and `CalculatorControl.xaml.cs`. This is a self-contained `UserControl` that binds to the `CalculatorViewModel`.

## Running the Standalone App

To run this as a standalone application on a Windows machine with Visual Studio installed:

1.  Open Visual Studio.
2.  Open the `WinUICalculatorApp.csproj` project file (or add it to a Solution).
3.  Ensure you have the "Windows App SDK C# Templates" installed in Visual Studio.
4.  Build and Run the project (typically targeting `net8.0-windows10.0.19041.0` or higher).

## Integrating into an Existing App

Because the core functionality is built as a reusable `UserControl` (`CalculatorControl`), integrating this feature into an existing WinUI 3 application is straightforward:

1.  **Copy the Files**: Copy the `ViewModels` folder and the `Views` folder into your existing WinUI 3 project.
2.  **Adjust Namespaces**: Ensure the namespaces in the copied `.cs` and `.xaml` files match your existing project's structure (if you placed them in a different folder path).
3.  **Embed the Control**: In any of your existing XAML Pages or Windows where you want the calculator to appear, add a reference to the `Views` namespace and use the control:

```xml
<!-- In your Page or Window XAML header -->
xmlns:views="using:YourAppNamespace.Views"

<!-- Where you want the calculator to appear -->
<views:CalculatorControl />
```

The control handles its own ViewModel instantiation and data binding internally.
