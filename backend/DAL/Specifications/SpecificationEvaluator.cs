using DAL.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(this IQueryable<TEntity> inputQuery
            , ISpecification<TEntity> specs) where TEntity : BaseEntity
        {
            var query = inputQuery;
            if (specs?.Criteria != null)
            {
                query = query.Where(specs.Criteria);
            }
            if(specs?.Includes != null)
            {
                foreach (var include in specs.Includes)
                {
                    query = query.Include(include);
                }
            }
            if (specs?.OrderBy != null)
                query = query.OrderBy(specs.OrderBy);
            else if (specs?.OrderByDescending != null)
                query = query.OrderByDescending(specs.OrderByDescending);

            return query;
        }
    }
}
