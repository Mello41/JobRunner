using System;

namespace JobRunner.Core.Attributes
{
    /// <summary>
    /// Указание на требование прав администратора.
    /// При генерации контроллера этот атрибут будет транслироваться в [Authorize(Roles = "Admin")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method |
        AttributeTargets.Class, Inherited = true)]
    public class RequiresAdminAttribute : Attribute
    {
        public string Role { get; set; } = "Admin";
        public string? Policy { get; set; }
    }
}
