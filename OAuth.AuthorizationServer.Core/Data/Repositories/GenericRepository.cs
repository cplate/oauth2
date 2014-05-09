using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using OAuth.AuthorizationServer.Core.Data.Model;

namespace OAuth.AuthorizationServer.Core.Data.Repositories
{
    // Generic interface allowing simple reads of a particular entity type
    public interface IGenericReadRepository<TEntity> where TEntity : class
    {
        ICollection<TEntity> Find(params Expression<Func<TEntity, object>>[] includeExpressions);
        TEntity GetById(object id);
        int GetCount();
    }

    // Generic interface allowing modification of a particular entity type
    public interface IGenericWriteRepository<TEntity> where TEntity : class
    {
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void DeleteChild<TChild>(TEntity parent, ICollection<TChild> children, TChild child) where TChild : class;
        void Edit(TEntity entityToEdit);
        void Insert(TEntity entity);
        void Save();
    }

    // Base class to avoid boilerplate stuff to invade our model repositories
    public class ReadOnlyRepository<TEntity> : IGenericReadRepository<TEntity> where TEntity : class
    {
        public ReadOnlyRepository(IObjectContextAdapter context)
        {
            Context = context as OAuthDataContext;
            if (Context == null)
            {
                throw new ArgumentException("Argument is not of the correct type.", "context");
            }

            DatabaseSet = Context.Set<TEntity>();
        }

        protected OAuthDataContext Context { get; set; }

        protected DbSet<TEntity> DatabaseSet { get; set; }

        public virtual ICollection<TEntity> Find(params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var qry = AddIncludes(DatabaseSet, includeExpressions);
            return qry.ToList();
        }

        public virtual TEntity GetById(object id)
        {
            return DatabaseSet.Find(id);
        }

        public virtual int GetCount()
        {
            return DatabaseSet.Count();
        }

        protected IQueryable<TEntity> AddIncludes(IQueryable<TEntity> qry, params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    qry = qry.Include(includeExpression);
                }
            }
            return qry;
        }
    }

    // Base class to avoid boilerplate stuff to invade our model repositories
    public class GenericRepository<TEntity> : ReadOnlyRepository<TEntity> where TEntity : class
    {
        public GenericRepository(IObjectContextAdapter context) : base(context)
        {
        }
        
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DatabaseSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DatabaseSet.Attach(entityToDelete);
            }

            DatabaseSet.Remove(entityToDelete);
        }

        public virtual void DeleteChild<TChild>(TEntity parent, ICollection<TChild> children, TChild entityToDelete) where TChild:class
        {
            Edit(parent);
            children.Remove(entityToDelete);
            Context.Entry(entityToDelete).State = EntityState.Deleted;
        }

        public virtual void Edit(TEntity entityToEdit)
        {
            if (Context.Entry(entityToEdit).State == EntityState.Detached)
            {
                DatabaseSet.Attach(entityToEdit);
            }
            Context.Entry(entityToEdit).State = EntityState.Modified;
        }

        public virtual void Insert(TEntity entity)
        {
            DatabaseSet.Add(entity);
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }
    }
}
