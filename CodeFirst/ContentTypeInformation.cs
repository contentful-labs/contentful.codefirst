using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Encapsulates a ContentType and its EditorInterFaceControls.
    /// </summary>
    public class ContentTypeInformation
    {
        /// <summary>
        /// The ContentType.
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// The interface controls of the content type.
        /// </summary>
        public List<EditorInterfaceControl> InterfaceControls { get; set; }
    }
}
