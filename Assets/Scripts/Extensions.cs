using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Extensions
{
    public static double GetNearestRoundedDown(this double value, double nearest)
    {
        return value - (value % nearest);
    }
}