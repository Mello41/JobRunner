using System;

namespace JobRunner.Core.Attributes
{
    /// <summary>
    /// Указывает, что эндпоинт публичный (не требует авторизации).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method |
        AttributeTargets.Class, 
        Inherited = true, 
        AllowMultiple = false)]
    public class PublicEndpointAttribute : Attribute
    {
        /// <summary>
        /// Разрешить доступ без аутентификации (даже без токена).
        /// </summary>
        public bool AllowAnonymous { get; set; } = true;

    }
}
