public enum TileView
{
    // <summary> The viewer can see sea
    // <remarks> May be masking a ship if viewed via a sea adapter
    Sea,

    // <summary> The viewer knows that site was attacked but nothing was hit
    Miss,

    // <summary> The viewer can see a ship at this site
    Ship,

    // <summary> The viewer knows that the site was attacked and something was hit
    Hit
}

