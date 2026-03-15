


using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Provides information for an ID3 V2 ownership frame.
    /// </summary>
    public readonly struct OwnershipFrameData
    {
        /// <summary>
        /// The premises responsible for selling the track.
        /// </summary>
        public readonly string Seller;

        /// <summary>
        /// The amount of money paid for the track (or the CD).
        /// </summary>
        public readonly string PricePaid;

        /// <summary>
        /// The exact date whence the transaction done.
        /// </summary>
        public readonly string DateOfPurchase;

        /// <summary>
        /// The currency under which this track was paid.
        /// </summary>
        public readonly string CurrencyOfPrice;

        /// <summary>
        /// Intializes a new instance of the <see cref="OwnershipFrameData"/> structure.
        /// </summary>
        /// <param name="price">The encoded price of the track.</param>
        /// <param name="date">The date of purchase.</param>
        /// <param name="seller">The premises responsible for selling this track.</param>
        public OwnershipFrameData(string price, string date, string seller)
        {
            ArgumentNullException.ThrowIfNull(date);
            ArgumentNullException.ThrowIfNull(price);
            ArgumentNullException.ThrowIfNull(seller);

            Seller = seller;
            DateOfPurchase = date;
            PricePaid = price.Substring(4);
            CurrencyOfPrice = price.Remove(3);
        }
    }
}