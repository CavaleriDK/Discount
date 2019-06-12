using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Shopgun
{
    /// <summary>
    /// The JSON model for a single Pricing object from the Shopgun offers endpoint
    /// </summary>
    [DataContract] class ShopgunPriceModel
    {
        /// <summary>
        /// The price attribute from the JSON object
        /// </summary>
        public float Price { get => price; }

        /// <summary>
        /// The Previous price attribute from the JSON object
        /// </summary>
        public object Pre { get => pre_price; }

        /// <summary>
        /// The currency attribute from the JSON object
        /// </summary>
        public string Currency { get => currency; }

        [DataMember] private float price;
        [DataMember] private object pre_price;
        [DataMember] private string currency;
    }
}
