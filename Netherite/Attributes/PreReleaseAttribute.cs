using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PreReleaseAttribute : Attribute
    {
        public string ReleaseVersion { get; private set; }

        public PreReleaseAttribute(string version)
        {
            ReleaseVersion = version;
        }
    }
}
