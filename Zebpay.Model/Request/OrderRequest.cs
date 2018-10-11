namespace Zebpay.Model.Request
{
    public class OrderRequest
    {
        /// <summary>
        /// Tradepair / Cryptopair
        /// </summary>
        public string trade_pair { get; set; }

        /// <summary>
        /// Supported side are Ask/Bid
        /// </summary>
        public string side { get; set; }

        /// <summary>
        /// Quantity In Crypto
        /// </summary>
        public decimal size { get; set; }

        /// <summary>
        /// Rate at which the Ask/Bid request is being placed
        /// </summary>
        public decimal price { get; set; }
    }
}
