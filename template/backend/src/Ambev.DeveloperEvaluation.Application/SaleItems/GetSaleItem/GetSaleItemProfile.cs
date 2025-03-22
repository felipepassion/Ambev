using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;

/// <summary>
/// Profile for mapping between SaleItem entity and GetSaleItemResponse
/// </summary>
public class GetSaleItemProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSaleItem operation
    /// </summary>
    public GetSaleItemProfile()
    {
        CreateMap<SaleItem, GetSaleItemResult>();
    }
}
