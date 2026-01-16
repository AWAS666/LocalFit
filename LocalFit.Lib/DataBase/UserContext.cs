using Microsoft.EntityFrameworkCore;

namespace LocalFit.Lib.DataBase;

public class UserContext : DbContext
{
    public DbSet<FoodPresets> FoodPresets { get; set; }
    public DbSet<FoodEaten> FoodEaten { get; set; }
    public DbSet<WeightTracker> Weight { get; set; }
    public DbSet<DayTracker> Day { get; set; }
    public DbSet<FitnessGoal> Goal { get; set; }
    public UserContext(string db = "database.db")
    {
        var local = Environment.SpecialFolder.LocalApplicationData;
        string basedir = AppContext.BaseDirectory; // might have to use this on android?
        var folder = Path.Combine(Environment.GetFolderPath(local), "LocalFit");
        Directory.CreateDirectory(folder);
        DbPath = Path.Combine(folder, db);
    }

    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class FoodPresets
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Calories { get; set; }
    public decimal Fat { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Sugar { get; set; }
    public FoodUnit Unit { get; set; } = FoodUnit.Gram100;
    public int UseCount { get; set; } = 0;
    public decimal? LastAmount { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public string UnitDisplay => Unit == FoodUnit.Gram100 ? "/100g" : "/piece";
}

public class FoodEaten
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public FoodPresets Preset { get; set; }
    public decimal Calories { get => Preset.Calories * DayTracker.GetMultiplier(Preset.Unit, Amount); }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public string AmountDisplay => Preset.Unit == FoodUnit.Gram100 
        ? $"{Amount:F0}g" 
        : $"{Amount:F0} piece{(Amount == 1 ? "" : "s")}";
}
public enum FoodUnit
{
    Gram100,
    Piece,
}

public class WeightTracker
{
    public int Id { get; set; }

    /// <summary>
    /// should be static, just in case person grows I guess
    /// </summary>
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Note { get; set; }
    public byte[]? Image { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public int Age
    {
        get
        {
            var difference = DateTime.UtcNow - BirthDate.ToDateTime(TimeOnly.MinValue);
            return (int)(difference.TotalDays / 365.25);
        }
    }
}

[PrimaryKey(nameof(Date))]
public class DayTracker
{
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public List<FoodEaten> FoodsEaten { get; set; } = new();

    public FitnessGoal? Goal { get; set; }
    public WeightTracker? Weight { get; set; }

    public decimal TotalCalories()
    {
        return FoodsEaten.Sum(x => x.Preset.Calories * GetMultiplier(x.Preset.Unit, x.Amount));
    }
    public decimal TotalFat()
    {
        return FoodsEaten.Sum(x => x.Preset.Fat * GetMultiplier(x.Preset.Unit, x.Amount));
    }
    public decimal TotalProtein()
    {
        return FoodsEaten.Sum(x => x.Preset.Protein * GetMultiplier(x.Preset.Unit, x.Amount));
    }
    public decimal TotalCarbs()
    {
        return FoodsEaten.Sum(x => x.Preset.Carbs * GetMultiplier(x.Preset.Unit, x.Amount));
    }
    public decimal TotalSugar()
    {
        return FoodsEaten.Sum(x => x.Preset.Sugar * GetMultiplier(x.Preset.Unit, x.Amount));
    }

    public static decimal GetMultiplier(FoodUnit unit, decimal amount)
    {
        return unit switch
        {
            FoodUnit.Gram100 => amount / 100,
            FoodUnit.Piece => amount,
            _ => amount
        };
    }
}


public class FitnessGoal
{
    public int Id { get; set; }
    public CalorieGoal Goal { get; set; }
    public FitnessLevel FitLevel { get; set; }
    public Sex Sex { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;
}