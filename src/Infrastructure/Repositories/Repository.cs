using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    // Infrastructure/Repositories/Repository.cs
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<TEntity> _set;

        public Repository(AppDbContext db)
        {
            _db = db;
            _set = db.Set<TEntity>();
        }

        public Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
            => _set.FindAsync(new object?[] { id }, ct).AsTask();

        public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = _set.AsNoTracking();
            if (predicate is not null) query = query.Where(predicate);
            return await query.ToListAsync(ct);
        }

        public Task AddAsync(TEntity entity, CancellationToken ct = default) => _set.AddAsync(entity, ct).AsTask();
        public void Update(TEntity entity) => _set.Update(entity);
        public void Remove(TEntity entity) => _set.Remove(entity);
    }
}
