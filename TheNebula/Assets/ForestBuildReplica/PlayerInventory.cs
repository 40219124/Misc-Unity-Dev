using System.Collections;
using System.Collections.Generic;

public static class PlayerInventory 
{
    private static int _logs = 0;
   
    public static int LogCount { get { return _logs; } }
    public static bool AddLog()
    {
        if (_logs == 0)
        {
            _logs++;
            return true;
        }
        return false;
    }

    public static bool UseLog()
    {
        if(_logs > 0)
        {
            _logs--;
            return true;
        }
        return false;
    }
}
