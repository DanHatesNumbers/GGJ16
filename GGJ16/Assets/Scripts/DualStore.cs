using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DualStore<T, L>
{
    public List<KeyValuePair<T, L>> KeyValuePairs; 


    public DualStore()
    {
        KeyValuePairs = new List<KeyValuePair<T, L>>(); 
    }

    public void Add(T key, L value)
    {
        KeyValuePairs.Add(new KeyValuePair<T, L>(key, value)); 
    }
}

