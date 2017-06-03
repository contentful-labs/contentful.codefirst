using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    public class ContentTypeInformation
    {
        public ContentType ContentType { get; set; }
        public List<EditorInterfaceControl> InterfaceControls { get; set; }
    }
}
