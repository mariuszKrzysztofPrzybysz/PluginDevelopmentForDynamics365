using Microsoft.Xrm.Sdk;

namespace Training.Core.Extensions
{
    public static class EntityExtensions
    {
        public static T GetAttribute<T>(this Entity entity, Entity preImage, string attributeName)
        {
            if (entity != null && entity.Contains(attributeName))
            {
                return (T)entity[attributeName];
            }
            else if (preImage != null && preImage.Contains(attributeName))
            {
                return (T)preImage[attributeName];
            }
            return default(T);
        }
    }
}
