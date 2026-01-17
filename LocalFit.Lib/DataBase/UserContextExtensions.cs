using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.Lib.DataBase;

/// <summary>
/// Extension methods for UserContext to provide common database query helpers.
/// </summary>
public static class UserContextExtensions
{
    /// <summary>
    /// Gets the most recent weight entry.
    /// </summary>
    public static WeightTracker? GetLatestWeight(this UserContext db)
    {
        return db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
    }

    /// <summary>
    /// Gets the most recent fitness goal entry.
    /// </summary>
    public static FitnessGoal? GetLatestGoal(this UserContext db)
    {
        return db.Goal.OrderByDescending(x => x.Created).FirstOrDefault();
    }

    /// <summary>
    /// Gets active (non-deleted) food presets, optionally ordered by use count.
    /// </summary>
    public static IQueryable<FoodPresets> GetActiveFoodPresets(this UserContext db, bool orderByUseCount = false)
    {
        var query = db.FoodPresets.Where(f => !f.IsDeleted);
        
        if (orderByUseCount)
        {
            return query.OrderByDescending(x => x.UseCount).ThenBy(x => x.Name);
        }
        
        return query.OrderBy(x => x.Name);
    }

    /// <summary>
    /// Gets active food presets as a list asynchronously.
    /// </summary>
    public static async Task<List<FoodPresets>> GetActiveFoodPresetsAsync(this UserContext db, bool orderByUseCount = false)
    {
        return await db.GetActiveFoodPresets(orderByUseCount).ToListAsync();
    }
}
