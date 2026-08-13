using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAdder.Models;

namespace SmartAdder.ViewModels
{
    public partial class SmartAdderViewModel : ObservableObject
    {
        public ObservableCollection<NumberCell> Cells { get; } = new();

        [ObservableProperty]
        private double _totalSum;

        public SmartAdderViewModel()
        {
            // Start the app with exactly one empty cell
            AddNewCell();
        }

        private void AddNewCell()
        {
            var newCell = new NumberCell();

            // Listen to this specific cell's changes
            newCell.PropertyChanged += OnCellPropertyChanged;

            Cells.Add(newCell);
        }

        private void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Whenever a user types into a textbox, this fires
            if (e.PropertyName == nameof(NumberCell.InputValue))
            {
                RecalculateSum();
                EnsureEmptyCellAtBottom();
            }
        }

        private void RecalculateSum()
        {
            TotalSum = Cells.Sum(c => c.Value);
        }

        private void EnsureEmptyCellAtBottom()
        {
            var lastCell = Cells.LastOrDefault();

            // If the very last cell in the list has text in it, generate a new blank one below it
            if (lastCell != null && !string.IsNullOrWhiteSpace(lastCell.InputValue))
            {
                AddNewCell();
            }
        }
    }
}