# JobRunner.Quartz

Реализация планировщика на основе Quartz.NET для JobRunner.
Пакет зависит от JobRunner.Core

Реализует интерфейсы из Core:                              
	- IJobScheduler                                            
	- IScheduleConverter (CronConverter)                      
	- IExecutionScope (ExecutionScope) 

# Требования
.NET 10.0 (или совместимая версия)

Наличие реализаций (от JobRunner.Core):
	ITaskService<T> — хранилище задач
	IJobExecutor — исполнитель задач
	IEncryptionService — шифрование (опционально)

## Установка

```bash
dotnet add package JobRunner.Quartz
```

# Зависимости

JobRunner.Core — последняя версия (ориентируйтесь на момент 1.2.1 --> 1.2.1)
Quartz.NET — 3.18.0
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Logging
Microsoft.Extensions.Options

# Быстрый старт

Большая необходимая часть документации описана в JobRunner.Core 

Действия, которые нужно сделать после скачивания этого пакета:
1. Зарегистрировать сервисы DI

	builder.Services.AddQuartzScheduler();
	builder.Services.AddScoped<ITaskService<MyTask>, MyTaskService>();
	builder.Services.AddScoped<IJobExecutor, MyJobExecutor>();
	builder.Services.AddScoped<IEncryptionService, MyEncryptionService>();

2. Инициализировать оркестратор при старте приложения

	using var scope = app.Services.CreateScope();
	var orchestrator = scope.ServiceProvider.GetRequiredService<ITaskOrchestrator<MyTask>>();
	await orchestrator.InitializeAsync();

3. Инициализировать оркестратор

using var scope = app.Services.CreateScope();
var orchestrator = scope.ServiceProvider.GetRequiredService<ITaskOrchestrator<MyTask>>();
await orchestrator.InitializeAsync();

Для шифрования чувствительных аргументов (пароли, ключи, токены) необходимо реализовать `IEncryptionService`

## JobLoggingListener (мониторинг выполнения)

При включении enableLoggingListener: true автоматически добавляется слушатель,
который логирует ВСЕ события выполнения задач:
	До выполнения задачи -> LogInformation("Job is about to be executed")
	После выполнения -> LogInformation/LogError("Job completed/failed")
	При отмене (veto) -> LogWarning("Job execution was vetoed")
	Время выполнения задачи -> "Job took X ms"

Отключить слушатель:
	builder.Services.AddQuartzScheduler<MyJobTask, Guid>(
	enableLoggingListener: false);

## Cron-валидация
Перед регистрацией задачи в Quartz, Cron-выражение автоматически валидируется.
При невалидном выражении выбрасывается ArgumentException.

Методы для ручной валидации (JobRunner.Quartz.Extensions):

cronExpression.IsValidCron() -> bool
cronExpression.ValidateCron() -> void (throws if invalid)

## Дополнительные методы QuartzScheduler

QuartzScheduler предоставляет полезные методы для мониторинга:
	JobExistsAsync(TId taskId) -> bool
	GetAllScheduledJobIdsAsync() -> IReadOnlyList<TId>
	GetJobDetailAsync(TId taskId) -> IJobDetail?
	GetTriggerAsync(TId taskId) -> ITrigger?

# Переопределение поведения
Если стандартная реализация не подходит:

Наследовать QuartzScheduler и переопределить методы
Реализовать свой IScheduleConverter
Создать свою реализацию IJobScheduler