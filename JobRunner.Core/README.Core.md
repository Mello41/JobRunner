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

IJobTask<TId> - основная модель задачи
	IScheduleSettings - настройки расписания (DailySchedule, WeeklySchedule и др.)
	INotifySettings - настройки уведомлений
	IJobTaskMetadata - метаданные выполнения (статистика, PID, IsRunning)
	IScheduleArguments - аргументы командной строки (с поддержкой шифрования)
	IRetrySettings - настройки повторных попыток при ошибке
	IGroupingSettings - настройки группового выполнения задач
	ExecutionTarget - где выполнять задачу (сервер / удалённый узел)
	
Метки и группировка
	IJobTag<TId> - метка для группировки задач
	AllowGroupRun - разрешить запуск всех задач метки одной командой
	AllowGroupPause - разрешить приостановку всех задач метки
	
Распределённое выполнение (Target System)
	ExecutionTarget - цель выполнения (сервер / конкретный узел / любой узел)
	ExecutionMode - режим: Server, SpecificNode, AnyNode, RoundRobin
	IJobNodeRegistry<TId> - реестр узлов-исполнителей
	NodeInfo / NodeRegistration - информация об узле
	NodeStatus - статус узла (Online, Offline, Busy, Maintenance)
	
## Операции с сущностями

ICrudService<T, TKey> - CRUD интерфейс для любых сущностей
	IJobTaskService<T, TId> - специфичные операции с задачами (GetByPIDAsync и др.)
	IJobTagService<TTag, TId> - специфичные операции с метками (SetColor, CountJobsToTag)
	
Групповые операции (IJobTagService)
	PauseAllByTagAsync - приостановить все задачи метки
	ResumeAllByTagAsync - возобновить все задачи метки
	RunAllByTagAsync - запустить все задачи метки сейчас
	GetGroupSummaryAsync - получить сводку по группе (статистика)
	ApplySettingsToGroupAsync - применить настройки ко всем задачам группы
	
Режимы группового выполнения (GroupExecutionMode)
	Parallel - все задачи параллельно
	Sequential - одна задача за другой
	Pipeline - конвейер (результат передаётся следующей задаче)
	RoundRobin - распределение по разным узлам

## Интерфейсы сервисов

Сервисы (интерфейсы)
IJobExecutor<TId> - исполнитель задачи (запуск внешнего процесса)
IJobScheduler<TId> - планировщик (регистрация, запуск, пауза, остановка)
IEncryptionService - шифрование чувствительных аргументов
INotificationService<TId> - сервис уведомлений (Email, Telegram, Webhook - по каждому статусу)
IUiNotificationService - сервис уведомлений UI (SignalR, EventAggregator)
IDomainEventDispatcher - диспетчер доменных событий
IPlatformDetector - определение ОС (Windows / Linux / macOS)
IPlatformFactory - фабрика платформозависимых сервисов
IProcessRunner - кросс-платформенный запуск процессов
ICommandLineEscaper - экранирование аргументов командной строки
IUnitOfWork - атомарное сохранение изменений (всё или ничего)

## Управление жизненным циклом задачи

IExecutionScope<TTask, TId> - инкапсулирует состояние выполнения задачи
	(расшифровка → выполнение → шифрование)

## Результаты операций

JobExecutionResult - результат выполнения задачи (Success, ExitCode, stdout/stderr)
DomainValidationResult - результат валидации доменной модели
PagedRequest / PagedResult - пагинация и фильтрация (безопасная, без Expression)

## Типы расписаний (PeriodType.cs и реализации IScheduleSettings)

OnceSchedule - однократное выполнение
DailySchedule - ежедневное выполнение
WeeklySchedule - еженедельное выполнение
MonthlySchedule - ежемесячное выполнение
YearlySchedule - ежегодное выполнение
EveryMinutesSchedule - с интервалом в минутах
HourlySchedule - с интервалом в часах
QuarterlySchedule - ежеквартальное выполнение
IntervalSchedule - периодическое (кастомизируемое, в разработке)

## Перечисления

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

## Уровень важности (NotificationSeverity)

Info, 
Success, 
Warning, 
Error

## Стратегии повторов (RetryStrategy)

None, 
FixedDelay, 
ExponentialBackoff, 
Incremental

## Состояние сущности в UoW (EntityState)
Detached, 
Added, 
Modified, 
Deleted, 
Unchanged

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
- Настройку ExecutionScope для управления жизненным циклом (расшифровка → выполнение → шифрование)
- Шифрование/дешифрование аргументов через IEncryptionService
- Регистрацию задач в вашем планировщике через IJobScheduler
- Хранение задач через ICrudService (EF Core / Dapper / ADO.NET)
- Отправку уведомлений через INotificationService
- Детекцию платформы через IPlatformDetector (если нужен кросс-платформенный запуск)
- Реестр узлов через IJobNodeRegistry (если нужно распределённое выполнение)
- Unit of Work через IUnitOfWork (если нужна атомарность)

## Зависимости

Библиотека не имеет внешних зависимостей (кроме .NET стандартных сборок)

Microsoft.Extensions.DependencyInjection - используется только в DomainEventDispatcher
(можно заменить на любую другую реализацию DI или убрать, реализовав свой диспетчер)

## последние изменения (v.2.0.1)

1. Изменение логики работы с кварталами (1,2,3,4)
2. Добавление расписания "список однократных выполнений"
3. новые сервисы и DTO для программных реализаций задачи
4. мелкие фиксы (изменение пространств имён и тд)