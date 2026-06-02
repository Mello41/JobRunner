# JobRunner.Core

Доменная модель библиотеки JobRunner с интерфейсами для планирования задач
Проект не зависит от JobRunner.Quartz. JobRunner.Quartz зависит от JobRunner.Core. 
	подробнее в блоке "Переезд на другую библиотеку"

# Установка

```bash
dotnet add package JobRunner.Core
```
# Основные интерфейсы

## Модели

IJobTask --> Модель задачи
	IScheduleSettings --> Типы расписаний
	INotifySettings --> Настройки уведомлений
	IJobTaskMetadata --> Метаданные выполнения задачи
	IScheduleArguments --> Аргументы командной строки
	
## Операции с сущностями

ICrudService<T, in TKey> --> CRUD интерфейс для сущностей
	ITaskService<T> --> специфичные операции с задачей
	ITagService<T> --> специфичные операции с метками

## Интерфейсы сервисов

IJobExecutor --> Исполнитель задач
IJobScheduler --> Планировщик
IEncryptionService --> Шифрование аргументов
INotificationService --> Сервис уведомлений
IDomainEventDispatcher --> Диспетчер доменных событий

IExecutionScope --> Управление жизненным циклом задачи (расшифровка → выполнение → шифрование)

## Результаты

JobExecutionResult --> Результат выполнения задачи
DomainValidationResult --> Результат валидации доменной модели

## Типы расписаний (PeriodType.cs)

OnceSchedule --> Однократное выполнение
DailySchedule --> Ежедневное выполнение
WeeklySchedule --> Еженедельное выполнение
IntervalSchedule --> Периодическое выполнение с интервалом
MonthlySchedule --> Ежемесячное выполнение
YearlySchedule --> Ежегодное выполнение 

## Статусы (EventType.cs)

TaskCreated --> Задача создана
TaskUpdated --> Задача обновлена
TaskDeleted --> Задача удалена
TaskStarted --> Задача запущена
TaskCompleted --> Задача завершена (успешно или с ошибкой)
TaskPaused --> Задача приостановлена
TaskResumed --> Задача возобновлена
TaskStopped --> Задача остановлена принудительно
TaskSkipped --> Задача пропущена (например, из-за блокировки)
TaskFailed --> Ошибка выполнения задачи
TaskTimedOut --> Задача истекла по таймауту

## Типы оповещений (NotificationType.cs)

Popup --> Всплывающее окно (Desktop)
Sound --> Звук (Desktop)
Email --> Email (SMTP)
LogFile --> Файловый лог (сервер)
EventLog --> Windows Event Log
Webhook --> HTTP-вызов на указанный URL
Monitoring --> Sentry, OpenTelemetry
Telegram --> Telegram уведомление (сообщение)

## Уровень важности уведомлений (NotificationSeverity.cs)

Info --> Информационное сообщение (низкая важность)
Success --> Успешное выполнение операции
Warning --> Предупреждение (требует внимания, не критично, но может упасть)
Error --> Критическая ошибка (требует немедленного вмешательства)

## Переезд на другую библиотеку

Если вы хотите избавиться от зависимости `JobRunner.Quartz` в своем проекте, 
можно переписать реализацию промежуточного слоя под вашу библиотеку.

Так как `JobRunner.Quartz` зависит от `JobRunner.Core`, доменную модель можно использовать 
для построения нового проекта. Минимальный набор того, что нужно будет реализовать:

- Конвертацию пользовательского формата настроек задачи под логику вашей библиотеки
- Настройку `ExecutionScope` для управления жизненным циклом
- Шифрование/дешифровка аргументов
