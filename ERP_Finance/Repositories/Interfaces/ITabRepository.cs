using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface ITabRepository
{
    bool AddToRepository(Tab tab);
    Tab? GetTabById(Guid id);
    Tab? GetTabByNumber(int tabNumber);
    IReadOnlyList<Tab> GetAllTabs();
    int GetLastTabNumberOfDay(DateOnly date);
}
