// <summary> The ISeaGrid defines the read only interface of a Grid. This allows each player to see and attack their opponents grid.
using System;

public interface ISeaGrid
{
    int Width { get; }

    int Height { get; }

    // <summary> Indicates that the grid has changed.
    event EventHandler Changed;

    // <summary> Provides access to the given row/column
    // <param name=row&column>the row&column to access</param>
    /// <value&returns>what the player can see at that location</value>
    TileView Item(int x, int y);

    // <summary> Mark the indicated tile as shot.
    // <param name="row"&"col">the row&column of the tile</param>
    // <returns>the result of the attack</returns>
    AttackResult HitTile(int row, int col);
}

