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

JobRunner.Core v.last (nuget по переключению, при скачивании в автоматическом режиме скачивает последнюю версию JobRunner.Core, которая по мнению разработчика балы наиболее совместима)
Quartz.NET v3.18.0

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

# Переопределение поведения
Если стандартная реализация не подходит:

Наследовать QuartzScheduler и переопределить методы
Реализовать свой IScheduleConverter
Создать свою реализацию IJobScheduler

