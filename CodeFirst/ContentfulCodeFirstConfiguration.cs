using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Class encapsulating options for configuring your code first strategy.
    /// </summary>
    public class ContentfulCodeFirstConfiguration
    {
        /// <summary>
        /// The Contentful management api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The space to apply changes to.
        /// </summary>
        public string SpaceId { get; set; }

        /// <summary>
        /// Whether or not to overwrite existing content types if possible.
        /// Be careful as this can result in loss of data.
        /// </summary>
        public bool ForceUpdateContentTypes { get; set; }

        /// <summary>
        /// Whether the content types should be published automatically once created or updated.
        /// </summary>
        public bool PublishAutomatically { get; set; }
    }
}
