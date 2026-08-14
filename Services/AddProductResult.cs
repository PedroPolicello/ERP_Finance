using ERP_Finance.Models;

namespace ERP_Finance.Service;

public sealed record AddProductResult(Product Product, bool WasCreated);