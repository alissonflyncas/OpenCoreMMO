﻿using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts;

/// <summary>
///     A contract to represent anything that can store things
/// </summary>
public interface IHasItem
{
    /// <summary>
    ///     Checks if thing can be added to destination
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <param name="slot"></param>
    /// <returns></returns>
    Result CanAddItem(IItem item, byte amount = 1, byte? slot = null);

    /// <summary>
    ///     Gives amount that can be added to store
    /// </summary>
    /// <param name="thing"></param>
    /// <param name="toPosition"></param>
    /// <returns></returns>
    uint PossibleAmountToAdd(IItem thing, byte? toPosition = null);

    /// <summary>
    ///     Checks if thing can be removed from store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool CanRemoveItem(IItem item);

    /// <summary>
    ///     Stores thing
    /// </summary>
    /// <param name="thing">thing to be stored</param>
    /// <param name="position">position where thing will be stored</param>
    /// <returns></returns>
    Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null);

    /// <summary>
    ///     Removes thing from store
    /// </summary>
    /// <param name="thing">thing to be removed</param>
    /// <param name="amount">amount of the thing to be removed</param>
    /// <param name="fromPosition">position where thing will be removed</param>
    /// <param name="removedThing">removed thing instance from store</param>
    /// <returns></returns>
    Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition, out IItem removedThing);
    
    /// <summary>
    ///     Checks if thing can be added to destination
    /// </summary>
    Result<uint> CanAddItem(IItemType itemType);
}