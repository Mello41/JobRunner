using System;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Attributes
{
    /// <summary>
    /// Указание на то, что метод требует определенного разрешения (permission-based).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | 
        AttributeTargets.Class, 
        Inherited = true, 
        AllowMultiple = true)]
    public class RequiresPermissionAttribute : Attribute
    {
        [Display(Name = "Название разрешения (например, \"tasks:read\", \"tasks:delete\").")]
        public string Permission { get; }

        /// <summary>
        /// Создает атрибут с указанным разрешением.
        /// </summary>
        /// <param name="permission">Название разрешения</param>
        public RequiresPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
