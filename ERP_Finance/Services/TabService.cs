using ERP_Finance.DTOs.Tab;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Services;

public class TabService
{
    private readonly ITabRepository _tabRepository;

    public TabService(ITabRepository tabRepository)
    {
        _tabRepository = tabRepository;
    }

    public Tab CreateTabService(CreateTabDTO tabDTO)
    {
        if (tabDTO == null)
            throw new ArgumentNullException(nameof(tabDTO));

        var tab = new Tab(
            tabNumber: tabDTO.TabNumber,
            serviceType: tabDTO.ServiceType,
            tableNumber: tabDTO.TableNumber,
            note: tabDTO.Note
        );

        var created = _tabRepository.AddToRepository(tab);

        if(!created)
            throw new InvalidOperationException("Tab could not be created");

        return tab;
    }

    public Tab GetTabByIdService(Guid id)
    {
        var tab = _tabRepository.GetTabById(id);

        if (tab == null)
            throw new KeyNotFoundException("Tab not found");

        return tab;
    }

    public Tab GetTabByNumberService(int tabNumber)
    {
        var tab = _tabRepository.GetTabByNumber(tabNumber);

        if (tab == null)
            throw new KeyNotFoundException("Tab not found");

        return tab;
    }

    public IReadOnlyList<Tab> GetAllTabsService()
    {
        return _tabRepository.GetAllTabs();
    }
}
