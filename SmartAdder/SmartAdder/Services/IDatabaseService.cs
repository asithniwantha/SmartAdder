using System.Collections.Generic;
using SmartAdder.Models;

namespace SmartAdder.Services
{
    public interface IDatabaseService
    {
        void SaveHistory(string entries, double totalSum);

        List<HistoryRecord> GetHistory();
    }
}
