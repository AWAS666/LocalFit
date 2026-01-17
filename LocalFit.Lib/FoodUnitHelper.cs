using LocalFit.Lib.DataBase;

namespace LocalFit.Lib;

public static class FoodUnitHelper
{
    public static decimal GetIncrementForUnit(FoodUnit? unit)
    {
        return unit switch
        {
            FoodUnit.Gram100 => 50,
            FoodUnit.Piece => 1,
            _ => 1
        };
    }
}
