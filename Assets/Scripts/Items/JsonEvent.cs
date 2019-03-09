using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class JsonEvent
{
    public double _time;
    public int _type;
    public int _value;

    public JsonEvent(double _time, int _type, int _value)
    {
        this._time = _time;
        this._type = _type;
        this._value = _value;
    }
}
