# SmartAdder Technical Features Guide

This document details the features implemented in SmartAdder and provides a technical overview of how they function within the codebase. It is intended for developers to understand the inner workings of the application.

## 1. Dynamic Entry List & Continuous Summation
The core functionality of SmartAdder revolves around a dynamically expanding list of number inputs and a real-time calculation of their total sum.
- **ViewModel Logic (`SmartAdderViewModel`):** The state is managed via an `ObservableCollection<NumberCell> Cells`. A continuous sum is maintained in the `TotalSum` property.
- **Automatic Row Management (`EnsureEmptyCellAtBottom`):** As the user types into an input field, the `NumberCell.InputValue` fires a `PropertyChanged` event. The ViewModel catches this and immediately recalculates the `TotalSum`. Furthermore, if the last cell in the collection becomes populated, a new empty `NumberCell` is automatically instantiated and appended to the bottom, ensuring there is always a free row for the next entry without requiring manual "add row" button clicks.

## 2. Specialized Keyboard Navigation & Input Constraints
SmartAdder utilizes custom behaviors attached to the UI elements to provide an "Excel-like" rapid data entry experience and constrain user input without violating MVVM principles.
- **Numeric Filtering:** The `NumericTextBoxBehavior` intercepts the `TextChanging` event of the `TextBox`. It uses regular expressions (`Regex.Replace(text, "[^0-9.]", "")`) to immediately strip out any non-numeric or non-decimal characters. It also ensures only a single decimal point is allowed per cell.
- **Keyboard Navigation Intercepts:** The same behavior captures `PreviewKeyDown` events.
  - Hitting `Enter`, `Down Arrow`, or `Plus (+)` programmatically moves the keyboard focus to the next cell below using `FocusManager.FindNextElement(FocusNavigationDirection.Down)`. If the target container is reached, `VisualTreeHelper` is used to find the inner `TextBox` to place the cursor directly inside it for immediate typing.
  - Hitting `Up Arrow` performs the exact reverse logic (`FocusNavigationDirection.Up`).
  - Hitting `Delete` invokes a bounded `DeleteCommand` on the ViewModel to remove the currently focused cell, and focuses the previous cell automatically.
- **Hover & Focus Visibility:** The `HoverBehavior` and `FocusWithinBehavior` track pointer presence and keyboard focus over the list respectively. They bind to `IsHovering` and `IsListFocused` booleans in the ViewModel. The list's visibility is entirely dependent on these states, giving it an unobtrusive, auto-hiding overlay aesthetic.

## 3. Calculation History Logging
The app automatically archives sessions to a local database when the user clears the list.
- **SQLite Database (`DatabaseService`):** Implements `IDatabaseService` using `Microsoft.Data.Sqlite`. On initialization, it creates a `history.db` file (if one doesn't exist) with a `History` table schema containing `Id`, `Timestamp`, `Entries` (TEXT), and `TotalSum` (REAL).
- **Session Serialization:** When the `ClearAllCommand` is triggered, the ViewModel extracts all non-empty `NumberCell` values, serializes the list of doubles into a JSON string using `System.Text.Json`, and passes it alongside the total sum to the `DatabaseService` for insertion.

## 4. History Presentation & Dialog UI
Viewing past calculations is handled elegantly through a dialog overlay.
- **View-Layer Service (`HistoryDialogService`):** To keep the ViewModel decoupled from `Microsoft.UI.Xaml` types, the presentation logic resides in a dedicated service implementing `IHistoryDialogService`.
- **Dynamic XAML Generation:** The service constructs a `ContentDialog` and dynamically loads a `DataTemplate` for the `ListView` items from a C# raw string literal using `Microsoft.UI.Xaml.Markup.XamlReader.Load()`. This prevents the need for extensive static XAML files or ResourceDictionaries for a localized feature.
- **Flyout Details:** The dynamic template wraps each history record in a transparent button. When clicked, it opens a `Flyout` containing a `ScrollViewer` that deserializes and lists the individual numerical entries that made up that specific past session.

## 5. Strict MVVM Architectural Conformance
The entire application enforces strict separation of concerns, ensuring high testability and maintainability.
- **CommunityToolkit.Mvvm:** ViewModels heavily utilize attributes like `[ObservableProperty]` and `[RelayCommand]`. The toolkit's source generators automatically produce the boilerplate `INotifyPropertyChanged` backing fields and `ICommand` implementations during compilation.
- **Behavior-Driven UI Events:** Standard code-behind files (`.xaml.cs`) are kept virtually empty. All UI-specific interactions (like pointer hovering, custom focus traversal, text filtering) are abstracted into reusable classes inheriting from `Behavior<T>` via the `Microsoft.Xaml.Behaviors.WinUI.Managed` package.
