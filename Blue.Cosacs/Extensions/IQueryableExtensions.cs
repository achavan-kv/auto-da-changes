using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Blue.Cosacs
{
    public static class IQueryableExtensions
    {
        public static TEntity AnsiFirstOrDefault<TEntity>(this IQueryable<TEntity> query, Context ctx)
        {
            return PutAnsiParams<TEntity>(query, ctx).FirstOrDefault();
        }

        public static List<TEntity> AnsiToList<TEntity>(this IQueryable<TEntity> query, Context ctx)
        {
            return PutAnsiParams<TEntity>(query, ctx).ToList();
        }

        public static IQueryable<TEntity> AnsiTake<TEntity>(this IQueryable<TEntity> query, Context ctx, int Count)
        {
            return PutAnsiParams<TEntity>(query, ctx).Take(Count).AsQueryable();

        }

        private static IEnumerable<TEntity> PutAnsiParams<TEntity>(IQueryable<TEntity> query, Context ctx)
        {
            var command = ctx.GetCommand(query);

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();
           
            for (var i = 0; i < command.Parameters.Count; i++)
            {
                var param = command.Parameters[i];
                if (param.DbType == System.Data.DbType.String)
                    param.DbType = System.Data.DbType.AnsiString;
            }

            return ctx.Translate<TEntity>(command.ExecuteReader());
        }

         

    }
}

