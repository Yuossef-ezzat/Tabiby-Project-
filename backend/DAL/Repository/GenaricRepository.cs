using DAL.Data;
using DAL.Shared;
using DAL.Specifications;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class GenaricRepository<TEntity> : IGenaricRepository<TEntity> where TEntity : BaseEntity, new()
{
    private readonly TabibyDbContext _context;

    public GenaricRepository(TabibyDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(a=>a.Id==id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity> Specs)
    {
        return await _context.Set<TEntity>().GetQuery(Specs).ToListAsync();
    }
}