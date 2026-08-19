# SmartAdder

SmartAdder is a WinUI 3 desktop application that functions as a continuous, dynamic calculation and adding tool. Designed with an overlay/floating control aesthetic, it automatically manages a list of number entries and calculates a running total, streamlining fast data entry workflows.

This documentation is intended for developers who wish to contribute to the codebase.

## Features

*   **Dynamic Entry List:** Automatically generates new rows for number entry as you type, keeping the interface uncluttered.
*   **Continuous Summation:** Calculates the total sum of all entered values in real-time.
*   **Intelligent UI Visibility:** The input list operates as a floating control that hides when not hovered over or focused, providing an unobtrusive overlay experience.
*   **Specialized Keyboard Navigation:** Navigate seamlessly through input fields using the `Enter`, `Up`/`Down`, and `Plus` keys.
*   **Calculation History Logging:** Local SQLite database integration logs history records including timestamps, individual entries, and total sums when the list is cleared.
*   **Clear Functionality:** Reset the current list and automatically log the session to history.

## Architecture

SmartAdder strictly follows the **Model-View-ViewModel (MVVM)** architectural pattern to ensure separation of concerns and maintainability.

*   **MVVM Framework:** The project leverages the `CommunityToolkit.Mvvm` library (using attributes like `[ObservableProperty]` and `[RelayCommand]`) to minimize boilerplate code in ViewModels and Models.
*   **UI Interactions:** To maintain strict MVVM separation and avoid code-behind, the app utilizes `Microsoft.Xaml.Behaviors.WinUI.Managed`. All custom UI interactions (such as hover detection, focus management, and specialized keyboard navigation) are implemented as reusable Behaviors (found in the `Behaviors/` directory).
*   **Storage:** Local data storage for calculation history is handled using `Microsoft.Data.Sqlite` within a dedicated `DatabaseService`.

## Build and Installation Instructions

### Prerequisites
*   Windows App SDK / WinUI 3 environment.
*   .NET 8 SDK.

### Building the Project
You can build and run the project using Visual Studio (with WinUI workloads installed) or the .NET CLI.

To build via the command line:
```bash
dotnet build ./SmartAdder/SmartAdder/SmartAdder.csproj
```

**Note for Non-Windows Environments:** If you are trying to restore or build the WinUI project on a Linux/non-Windows environment (e.g., for CI/CD or codebase inspection), ensure that the `<EnableWindowsTargeting>true</EnableWindowsTargeting>` property is present in the `.csproj` file to prevent platform compatibility errors during restore.

## Developer Usage Guide

### Code Structure
When contributing, familiarize yourself with the following key directories in `SmartAdder/SmartAdder/`:
*   **`Models/`**: Contains data entities (e.g., `NumberCell`, `HistoryRecord`).
*   **`ViewModels/`**: Contains the application logic (e.g., `SmartAdderViewModel`).
*   **`Views/`**: Contains the XAML UI definitions (e.g., `SmartAdderControl.xaml`).
*   **`Behaviors/`**: Contains XAML Behaviors managing direct UI interactions (e.g., `HoverBehavior`, `NumericTextBoxBehavior`, `FocusWithinBehavior`).
*   **`Services/`**: Contains background and data logic (e.g., `DatabaseService`).

### Running Tests
To execute tests, use the .NET CLI pointing directly to the `.csproj` file, as the repository does not contain a solution (`.sln`) file at the root:

```bash
dotnet test ./SmartAdder/SmartAdder/SmartAdder.csproj
```
