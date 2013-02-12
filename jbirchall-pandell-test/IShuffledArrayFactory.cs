/*
 * Copyright (c) James Birchall 2013
*/

using System;
using System.Collections.Generic;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Interface for generating shuffled arrays.
    /// </summary>
    interface IShuffledArrayFactory
    {
        Array Generate(int size);
    }
}
