using System;
namespace VietCommerce.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AuditableAttribute : Attribute
    {
        public bool TrackCreate { get; }
        public bool TrackUpdate { get; }
        public bool TrackDelete { get; }
        public AuditableAttribute(bool trackCreate = true, bool trackUpdate = true, bool trackDelete = true)
        {
            TrackCreate = trackCreate;
            TrackUpdate = trackUpdate;
            TrackDelete = trackDelete;
        }
    }
}
