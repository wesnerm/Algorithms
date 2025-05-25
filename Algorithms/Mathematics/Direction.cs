#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms;

/// <summary>
///     Direction of indexing
/// </summary>
public enum Direction : sbyte
{
    /// <summary>
    ///     Indicates preference to look or more backward
    /// </summary>
    Negative = -1,

    /// <summary>
    ///     Indicates preference to look or move forward
    /// </summary>
    Positive = 1,

    /// <summary>
    ///     Indicates a neutral position
    /// </summary>
    Neutral,
}