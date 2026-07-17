using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
namespace FBSC.ODMS.Infrastructure.Services
{
    public class DynamicLoaderService(ApplicationContext context)
    {

        // Cache for entity types to avoid repeated lookups
        private static readonly ConcurrentDictionary<string, Type?> EntityTypeCache = new();

        // Cache for compiled query delegates
        private static readonly ConcurrentDictionary<string, Delegate> QueryCache = new();

        // Cache for DbSet accessors
        private static readonly ConcurrentDictionary<Type, MethodInfo> DbSetMethodCache = new();

        /// <summary>
        /// Load model dynamically with maximum performance optimizations
        /// Uses compiled queries and caching
        /// </summary>
        public async Task<object?> LoadModelAsync(string tableName, object dataId)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            try
            {
                // Get cached entity type
                var clrType = GetCachedEntityType(tableName);
                if (clrType == null)
                    return null;

                // Get or create compiled query
                var cacheKey = $"{tableName}_{clrType.FullName}";
                var compiledQuery = QueryCache.GetOrAdd(cacheKey, _ => CreateCompiledQuery(clrType));

                // Execute compiled query
                var queryMethod = compiledQuery.GetType().GetMethod("Invoke");
                Task? resultTask = (Task?)queryMethod?.Invoke(compiledQuery, [context, dataId]);
                await resultTask!.ConfigureAwait(false);

                var resultProperty = resultTask.GetType().GetProperty("Result");
                return resultProperty?.GetValue(resultTask);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dynamic model: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Load model with navigation properties using optimized compiled queries
        /// </summary>
        public async Task<object?> LoadModelWithNavigationsAsync(string tableName, object dataId, params string[] includeProperties)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            try
            {
                var clrType = GetCachedEntityType(tableName);
                if (clrType == null)
                    return null;

                // Create cache key including navigation properties
                var includesKey = string.Join(",", includeProperties.OrderBy(p => p));
                var cacheKey = $"{tableName}_{clrType.FullName}_includes_{includesKey}";

                var compiledQuery = QueryCache.GetOrAdd(cacheKey, _ =>
                    CreateCompiledQueryWithIncludes(clrType, includeProperties));

                // Execute compiled query
                var queryMethod = compiledQuery.GetType().GetMethod("Invoke");
                Task? resultTask = (Task?)queryMethod?.Invoke(compiledQuery, [context, dataId]);
                await resultTask!.ConfigureAwait(false);

                var resultProperty = resultTask.GetType().GetProperty("Result");
                return resultProperty?.GetValue(resultTask);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dynamic model with navigations: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get cached entity type to avoid repeated model lookups
        /// </summary>
        private Type? GetCachedEntityType(string tableName)
        {
            return EntityTypeCache.GetOrAdd(tableName, name =>
            {
                var entityType = context.Model.FindEntityType(name);
                entityType ??= context.Model.GetEntityTypes()
                        .FirstOrDefault(e => e.GetTableName()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

                return entityType?.ClrType;
            });
        }

        /// <summary>
        /// Create compiled query for maximum performance
        /// Compiled queries are cached and reused across calls
        /// </summary>
        private Delegate CreateCompiledQuery(Type entityType)
        {
            // Create expression: (ApplicationContext context, object id) => context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id)
            var contextParam = Expression.Parameter(typeof(ApplicationContext), "context");
            var idParam = Expression.Parameter(typeof(object), "id");

            // Get DbSet<TEntity>
            var setMethod = GetCachedDbSetMethod(entityType);
            var dbSetCall = Expression.Call(contextParam, setMethod);

            // Build: x => x.Id == id
            var parameter = Expression.Parameter(entityType, "x");
            var idProperty = Expression.Property(parameter, "Id");
            var convertedId = Expression.Convert(idParam, idProperty.Type);
            var equality = Expression.Equal(idProperty, convertedId);
            var predicate = Expression.Lambda(equality, parameter);

            // Build: FirstOrDefaultAsync(predicate)
            var firstOrDefaultMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" &&
                           m.GetParameters().Length == 3 &&
                           m.GetParameters()[1].ParameterType.IsGenericType)
                .MakeGenericMethod(entityType);

            var cancellationToken = Expression.Constant(default(System.Threading.CancellationToken));
            var queryCall = Expression.Call(firstOrDefaultMethod, dbSetCall, predicate, cancellationToken);

            // Compile the expression
            var lambdaType = typeof(Func<,,>).MakeGenericType(typeof(ApplicationContext), typeof(object), queryCall.Type);
            var lambda = Expression.Lambda(lambdaType, queryCall, contextParam, idParam);

            return lambda.Compile();
        }

        /// <summary>
        /// Create compiled query with Include statements
        /// </summary>
        private static Delegate CreateCompiledQueryWithIncludes(Type entityType, string[] includeProperties)
        {
            var contextParam = Expression.Parameter(typeof(ApplicationContext), "context");
            var idParam = Expression.Parameter(typeof(object), "id");

            // Get DbSet<TEntity>
            var setMethod = GetCachedDbSetMethod(entityType);
            Expression query = Expression.Call(contextParam, setMethod);

            // Add Include calls for each navigation property
            foreach (var includeProp in includeProperties)
            {
                var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Include" &&
                               m.GetParameters().Length == 2 &&
                               m.GetParameters()[1].ParameterType == typeof(string))
                    .MakeGenericMethod(entityType);

                query = Expression.Call(includeMethod, query, Expression.Constant(includeProp));
            }

            // Build: x => x.Id == id
            var parameter = Expression.Parameter(entityType, "x");
            var idProperty = Expression.Property(parameter, "Id");
            var convertedId = Expression.Convert(idParam, idProperty.Type);
            var equality = Expression.Equal(idProperty, convertedId);
            var predicate = Expression.Lambda(equality, parameter);

            // Build: Where(predicate)
            var whereMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            query = Expression.Call(whereMethod, query, predicate);

            // Build: FirstOrDefaultAsync()
            var firstOrDefaultMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" &&
                           m.GetParameters().Length == 2 &&
                           !m.GetParameters()[1].ParameterType.IsGenericType)
                .MakeGenericMethod(entityType);

            var cancellationToken = Expression.Constant(default(System.Threading.CancellationToken));
            var queryCall = Expression.Call(firstOrDefaultMethod, query, cancellationToken);

            // Compile the expression
            var lambdaType = typeof(Func<,,>).MakeGenericType(typeof(ApplicationContext), typeof(object), queryCall.Type);
            var lambda = Expression.Lambda(lambdaType, queryCall, contextParam, idParam);

            return lambda.Compile();
        }

        /// <summary>
        /// Get cached DbSet method to avoid repeated reflection
        /// </summary>
        private static MethodInfo GetCachedDbSetMethod(Type entityType)
        {
            return DbSetMethodCache.GetOrAdd(entityType, type =>
            {
                return typeof(ApplicationContext)
                    .GetMethod("Set", Type.EmptyTypes)!
                    .MakeGenericMethod(type);
            });
        }

        /// <summary>
        /// Clear all caches - call this if memory usage becomes a concern
        /// </summary>
        public static void ClearCache()
        {
            EntityTypeCache.Clear();
            QueryCache.Clear();
            DbSetMethodCache.Clear();
        }
    }
}
