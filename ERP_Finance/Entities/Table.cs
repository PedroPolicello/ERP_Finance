using ERP_Finance.Types;

namespace ERP_Finance.Entities;

public class Table
{
    public Guid Id { get; private set; }
    public int TableNumber { get; private set; }
    public TableStatus Status { get; private set; }

    // Lista de comandas associadas à mesa?
    public ICollection<Tab> Tabs { get; private set; } = new List<Tab>();

    public Table(int tableNumber)
    {
        ValidateTableNumber(tableNumber);

        Id = Guid.NewGuid();
        TableNumber = tableNumber;
        Status = TableStatus.Available;
    }

    public void AddTab(Tab tab)
    {
        if(tab == null)
            throw new ArgumentNullException(nameof(tab));

        if(Status == TableStatus.Reserved)
            throw new InvalidOperationException("Cannot add tab to an reserved table.");

        if(Tabs.Contains(tab))
            throw new ArgumentException("Tab already exists in the table.", nameof(tab));

        Tabs.Add(tab);
    }

    public void RemoveTab(Tab tab)
    {
        if (tab == null)
            throw new ArgumentNullException(nameof(tab));

        if (Status == TableStatus.Reserved)
            throw new InvalidOperationException("Cannot remove tab from an reserved table.");

        if (!Tabs.Contains(tab))
            throw new ArgumentException("Tab does not exist in the table.", nameof(tab));

        Tabs.Remove(tab);
    }

    public Tab? GetTabById(Guid id)
    {
        return Tabs.FirstOrDefault(tab => tab.Id == id);
    }

    public Tab? GetTabByNumber(int tabNumber)
    {
        return Tabs.FirstOrDefault(tab => tab.TabNumber == tabNumber);
    }

    public IReadOnlyList<Tab> GetAllTabs()
    {
        return Tabs.ToList();
    }

    public decimal GetTotal()
    {
        return Tabs.Where(tab => tab.IsOpen)
                   .Sum(tabs => tabs.GetTotal());
    }

    // ========== Table Status Management Methods ==========

    public void MarkAsAvailable() => Status = TableStatus.Available;
    public void MarkAsOccupied() => Status = TableStatus.Occupied;
    public void MarkAsReserved() => Status = TableStatus.Reserved;

    // ======================================================

    private void ValidateTableNumber(int tableNumber)
    {
        if (tableNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(tableNumber), "Table number must be greater than zero.");
    }

}
