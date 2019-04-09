using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Extensions
{
    public static float GetNearestRoundedDown(this float value, float nearest)
    {
        return value - (value % nearest);
    }
}