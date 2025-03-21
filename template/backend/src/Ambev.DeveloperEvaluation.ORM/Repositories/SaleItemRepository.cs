using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleItemRepository using Entity Framework Core.
/// </summary>
public class SaleItemRepository : ISaleItemRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="SaleItemRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SaleItemRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale item in the database.
    /// </summary>
    /// <param name="saleItem">The sale item to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale item.</returns>
    public async Task<SaleItem> CreateAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        await _context.SaleItems.AddAsync(saleItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return saleItem;
    }

    /// <summary>
    /// Updates an existing sale item in the database.
    /// </summary>
    /// <param name="saleItem">The sale item to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale item.</returns>
    public async Task<SaleItem> UpdateAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _context.SaleItems.Update(saleItem);
        await _context.SaveChangesAsync(cancellationToken);
        return saleItem;
    }

    /// <summary>
    /// Retrieves a sale item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale item if found, null otherwise.</returns>
    public async Task<SaleItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves all sale items associated with a given sale.
    /// </summary>
    /// <param name="saleId">The identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of sale items for the specified sale.</returns>
    public async Task<List<SaleItem>> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Where(si => si.SaleId == saleId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a sale item from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the sale item to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale item was deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var saleItem = await GetByIdAsync(id, cancellationToken);
        if (saleItem == null)
            return false;

        _context.SaleItems.Remove(saleItem);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
