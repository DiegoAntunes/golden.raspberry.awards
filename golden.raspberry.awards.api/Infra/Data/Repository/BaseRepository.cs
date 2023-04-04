using golden.raspberry.awards.api.Domain.Entities;
using golden.raspberry.awards.api.Domain.Interfaces;
using golden.raspberry.awards.api.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace golden.raspberry.awards.api.Infra.Data.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly AppDbContext _appDbContext;

        public BaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IList<TEntity> Select()
        {
            return _appDbContext.Set<TEntity>().ToList();
        }

        public TEntity Select(int id)
        {
            return _appDbContext.Set<TEntity>().Find(id);
        }

        public void Insert(TEntity obj)
        {
            _appDbContext.Set<TEntity>().Add(obj);
            _appDbContext.SaveChanges();
        }

        public void Update(TEntity obj)
        {
            _appDbContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _appDbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _appDbContext.Set<TEntity>().Remove(Select(id));
            _appDbContext.SaveChanges();
        }
    }
}