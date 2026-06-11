using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JobRunner.Core.DTO.Pages
{
    /// <summary>
    /// Безопасный запрос для пагинации и фильтрации
    /// </summary>
    /// <remarks>
    /// Используется вместо Expression<Func<T, bool>> для защиты от инъекций.
    /// Реализация на уровне Infrastructure сама преобразует эти параметры в безопасные LINQ-выражения.
    /// </remarks>
    public class PagedRequest
    {
        [DisplayName("Номер страницы (начиная с 1)")]
        public int Page { get; set; } = 1;

        [DisplayName("Размер страницы (1-1000)")]
        public int PageSize { get; set; } = 20;

        [DisplayName("Поисковый запрос (применяется к Name и Description)")]
        public string? SearchTerm { get; set; }

        [DisplayName("Поле для сортировки (только разрешенные: \"Name\", \"StartRun\", \"EndRun\", \"CreatedAt\")")]
        public string? SortBy { get; set; }

        [DisplayName("Направление сортировки")]
        public bool SortDescending { get; set; } = false;

        [DisplayName("Фильтры по конкретным полям (безопасные, проверяемые)")]
        public Dictionary<string, object> Filters { get; set; } = new();

        [DisplayName("Фильтр по статусу задачи")]
        public bool? IsEnabled { get; set; }

        [DisplayName("Фильтр по тегам (список ID)")]
        public List<Guid>? TagIds { get; set; }
    }
}
