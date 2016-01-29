using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TileNotFoundException : Exception
{
    public TileNotFoundException(string message) : base(message)
    {

    }
}

