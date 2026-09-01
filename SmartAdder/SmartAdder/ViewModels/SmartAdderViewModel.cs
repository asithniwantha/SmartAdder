using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAdder.Models;
using SmartAdder.Services;
using System.Text.Json;
using Microsoft.UI.Xaml;

namespace SmartAdder.ViewModels
{
    public partial class SmartAdderViewModel : ObservableObject
    {
        public ObservableCollection<NumberCell> Cells { get; } = new();

        [ObservableProperty]
        private double _totalSum;

        private bool _isHovering;
        public bool IsHovering
        {
            get => _isHovering;
            set
            {
                if (SetProperty(ref _isHovering, value))
                {
                    OnPropertyChanged(nameof(ListVisibility));
                }
            }
        }

        private bool _isListFocused;
        public bool IsListFocused
        {
            get => _isListFocused;
            set
            {
                if (SetProperty(ref _isListFocused, value))
                {
                    OnPropertyChanged(nameof(ListVisibility));
                }
            }
        }

        public Visibility ListVisibility => (IsHovering || IsListFocused) ? Visibility.Visible : Visibility.Collapsed;

        private readonly IDatabaseService _databaseService;
        private readonly IHistoryDialogService _historyDialogService;

        public SmartAdderViewModel(IDatabaseService databaseService, IHistoryDialogService historyDialogService)
        {
            _databaseService = databaseService;
            _historyDialogService = historyDialogService;
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

        [RelayCommand]
        private void DeleteCell(NumberCell? cell)
        {
            if (cell == null) return;

            cell.PropertyChanged -= OnCellPropertyChanged;
            Cells.Remove(cell);

            RecalculateSum();

            if (Cells.Count == 0)
            {
                AddNewCell();
            }
            else
            {
                EnsureEmptyCellAtBottom();
            }
        }

        [RelayCommand]
        private void ClearAll()
        {
            var entries = Cells
                .Where(c => !string.IsNullOrWhiteSpace(c.InputValue))
                .Select(c => c.Value)
                .ToList();

            if (entries.Any())
            {
                string entriesJson = JsonSerializer.Serialize(entries);
                _databaseService.SaveHistory(entriesJson, TotalSum);
            }

            foreach (var cell in Cells)
            {
                cell.PropertyChanged -= OnCellPropertyChanged;
            }
            Cells.Clear();
            TotalSum = 0;
            AddNewCell();
        }

        [RelayCommand]
        private async Task ViewHistory()
        {
            var history = _databaseService.GetHistory();
            await _historyDialogService.ShowHistoryAsync(history);
        }
    }
}
