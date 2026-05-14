using System;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Attributes
{
    /// <summary>
    /// Указание на требование прав администратора.
    /// При генерации контроллера этот атрибут будет транслироваться в [Authorize(Roles = "Admin")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method |
        AttributeTargets.Class, 
        Inherited = true)]
    public class RequiresAdminAttribute : Attribute
    {
        [Display(Name = "Роль администратора (по умолчанию \"Admin\")")]
        public string Role { get; set; } = "Admin";

        [Display(Name = "Имя политики авторизации (если используется policy-based).")]
        public string? Policy { get; set; }

        /// <summary>
        /// Создает атрибут с ролью по умолчанию "Admin".
        /// </summary>
        public RequiresAdminAttribute() 
        {

        }

        /// <summary>
        /// Создает атрибут с указанной ролью.
        /// </summary>
        /// <param name="role">Название роли (например, "SuperAdmin")</param>
        public RequiresAdminAttribute(string role)
        {
            Role = role;
        }
    }
}
