using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JobRunner.Core;
using JobRunner.Core.Interfaces;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace JobRunner.Avalonia.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        private readonly IJobScheduler _scheduler;

        private JobTask selectedJobTask;
        public JobTask SelectedJobTask 
        { 
            get => selectedJobTask; 
            set => selectedJobTask = value; 
        }

        private readonly object _tasksLock = new();  // Для потокобезопасности
        private List<JobTask> _allTasks = new();
        // private ConcurrentBag<JobTask> _allTasks = new();

        /// <summary>
        /// для UI
        /// </summary>
        public ObservableCollection<JobTask> Tasks { get; } = new();

        private string _newTaskName = string.Empty;
        public string NewTaskName
        {
            get => _newTaskName;
            set => this.RaiseAndSetIfChanged(ref _newTaskName, value);
        }

        private string _executionPath = string.Empty;
        public string ExecutionPath
        {
            get => _executionPath;
            set => this.RaiseAndSetIfChanged(ref _executionPath, value);
        }

        private string _arguments = string.Empty;
        public string Arguments
        {
            get => _arguments;
            set => this.RaiseAndSetIfChanged(ref _arguments, value);
        }

        private bool _isEncrypt;

        public bool IsEncrypt
        {
            get => _isEncrypt;
            set => this.RaiseAndSetIfChanged(ref _isEncrypt, value);
        }

        /// <summary>
        /// Unit — это "пустое значение" в ReactiveUI
        /// Аналог void, но для типизированных систем.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> EditTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> CopyTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCategoryTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> RunTaskCommand { get; }

        public MainWindowVM(IJobScheduler scheduler)
        {
            _scheduler = scheduler;

            AddTaskCommand = ReactiveCommand.CreateFromTask(OpenAddTaskAsync);
            EditTaskCommand = ReactiveCommand.CreateFromTask(EditTaskAsync);
            DeleteTaskCommand = ReactiveCommand.CreateFromTask(DeleteTaskAsync);
            CopyTaskCommand = ReactiveCommand.CreateFromTask(CopyTaskAsync);
            AddCategoryTaskCommand = ReactiveCommand.CreateFromTask(AddCategoryTaskAsync);
            PauseTaskCommand = ReactiveCommand.CreateFromTask(PauseTaskAsync);
            RunTaskCommand = ReactiveCommand.CreateFromTask(RunTaskAsync);
            
            LoadTasks();
        }


        private Window GetMainWindow()
        {
            return (Window)(Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow!;
        }

        /// <summary>
        /// Загрузка тасок
        /// </summary>
        private async void LoadTasks()
        {
            var tasks = await _scheduler.GetAllTasksList();
            
            lock (_tasksLock)
            {
                _allTasks = tasks.ToList();
            }
            
            ApplyTaskNameFilter();
        }

        #region CRUD команды (реализация всех команд)
        /// <summary>
        /// Добавление таски (через новое окно)
        /// </summary>
        /// <returns></returns>
        private async Task OpenAddTaskAsync()
        {
            var window = new AddTaskWindow();
            var viewModel = new AddTaskWindowVM();
            viewModel.SetCurrentWindow(window);

            window.DataContext = viewModel;
            await window.ShowDialog<object?>(GetMainWindow());

            if (viewModel.ResultTask != null)
            {
                await _scheduler.CreateTaskAsync(viewModel.ResultTask);

                lock (_tasksLock)
                {
                    _allTasks.Add(viewModel.ResultTask);
                }
                ApplyTaskNameFilter();
            }
        }

        /// <summary>
        /// Изменение выбранной задачи
        /// </summary>
        /// <returns></returns>
        private async Task EditTaskAsync()
        {
            if (SelectedJobTask == null) return;

            var window = new AddTaskWindow();
            var viewModel = new AddTaskWindowVM();
            viewModel.SetCurrentWindow(window);

            viewModel.TaskName = SelectedJobTask.Name;
            viewModel.ExecutionPath = SelectedJobTask.ExecutionPath;
            viewModel.Arguments = SelectedJobTask.Arguments;
            viewModel.IsEncrypt = SelectedJobTask.IsEncrypt;

            if (SelectedJobTask.ScheduleSettings != null)
            {
                viewModel.SelectedPeriodType = SelectedJobTask.ScheduleSettings.PeriodType;
                viewModel.Hour = SelectedJobTask.ScheduleSettings.Hour.ToString();
                viewModel.Minute = SelectedJobTask.ScheduleSettings.Minute.ToString();
                viewModel.IntervalMinutes = SelectedJobTask.ScheduleSettings.IntervalMinutes.ToString();
            }

            window.DataContext = viewModel;
            await window.ShowDialog<object?>(GetMainWindow());

            if (viewModel.ResultTask != null)
            {
                viewModel.ResultTask.Id = SelectedJobTask.Id;
                await _scheduler.UpdateTaskAsync(viewModel.ResultTask);

                lock (_tasksLock)
                {
                    var index = _allTasks.FindIndex(t => t.Id == SelectedJobTask.Id);
                    if (index != -1)
                        _allTasks[index] = viewModel.ResultTask;
                }
                ApplyTaskNameFilter();
            }
        }

        /// <summary>
        /// Удаление задачи из списка (навсегда)
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task DeleteTaskAsync()
        {
            if (SelectedJobTask == null) return;  

            var messageBox = MessageBoxManager.GetMessageBoxStandardWindow(
                "Подтверждение",
                $"Удалить задачу \"{SelectedJobTask.Name}\"?",  
                ButtonEnum.YesNo);

            var result = await messageBox.ShowDialog(GetMainWindow());

            if (result == ButtonResult.Yes)
            {
                await _scheduler.DeleteTaskAsync(SelectedJobTask.Id);
                lock (_tasksLock)
                {
                    var task = _allTasks.FirstOrDefault(t => t.Id == SelectedJobTask.Id);
                    if (task != null) _allTasks.Remove(task);
                }
                ApplyTaskNameFilter();
            }
        }

        /// <summary>
        /// Запуск выбранной задани сейчас
        /// </summary>
        /// <returns></returns>
        private async Task RunTaskAsync()
        {
            if (SelectedJobTask == null) return;
            await _scheduler.TriggerNowAsync(SelectedJobTask.Id);
        }

        /// <summary>
        /// Остановить выбранную задачу
        /// </summary>
        /// <returns></returns>
        private async Task PauseTaskAsync()
        {
            if (SelectedJobTask == null) return;
            await _scheduler.StopTaskAsync(SelectedJobTask.Id);
        }

        /// <summary>
        /// Добавить метку (категорию) к выбранной задаче
        /// </summary>
        /// <returns></returns>
        private async Task AddCategoryTaskAsync()
        {
            if (SelectedJobTask == null) return;
            //todo
        }

        private async Task CopyTaskAsync()
        {
            
        }
        #endregion

        #region Фильтрация задач

        private string _searchTaskByNameText = string.Empty;
        public string SearchTaskByNameText 
        {
            get => _searchTaskByNameText;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchTaskByNameText, value);
                ApplyTaskNameFilter();
            }
        }

        /// <summary>
        /// Фильтрация таски по имени
        /// </summary>
        public void ApplyTaskNameFilter()
        {
            Tasks.Clear();

            var allTasksList = _allTasks.ToList(); // ConcurrentBag нужно преобразовать в List для фильтрации

            var filtered = string.IsNullOrWhiteSpace(SearchTaskByNameText)
                ? allTasksList
                : allTasksList.Where(t => t.Name.Contains(SearchTaskByNameText, StringComparison.OrdinalIgnoreCase))
                              .ToList();

            foreach (var task in filtered)
                Tasks.Add(task);
        }
        #endregion
    }
}
