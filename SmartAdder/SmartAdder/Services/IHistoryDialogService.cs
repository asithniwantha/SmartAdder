using System.Collections.Generic;
using System.Threading.Tasks;
using SmartAdder.Models;

namespace SmartAdder.Services
{
    public interface IHistoryDialogService
    {
        Task ShowHistoryAsync(IReadOnlyList<HistoryRecord> history);
    }
}
