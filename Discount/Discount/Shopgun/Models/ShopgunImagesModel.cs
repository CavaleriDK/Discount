using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Shopgun
{
    /// <summary>
    /// The JSON model for a single Offer Images object from the Shopgun offers endpoint
    /// </summary>
    [DataContract] class ShopgunImagesModel
    {
        /// <summary>
        /// Image View URL attribute from JSON object
        /// </summary>
        public string View { get => view; }

        /// <summary>
        /// Image Zoom URL attribute from JSON object
        /// </summary>
        public string Zoom { get => zoom; }

        /// <summary>
        /// Image Thumbnail URL attribute from JSON object
        /// </summary>
        public string Thumb { get => thumb; }

        [DataMember] private string view;
        [DataMember] private string zoom;
        [DataMember] private string thumb;

    }
}
