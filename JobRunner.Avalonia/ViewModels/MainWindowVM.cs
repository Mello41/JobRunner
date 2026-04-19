using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JobRunner.Core;
using JobRunner.Core.Interfaces;
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

        public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> EditTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> CopyTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCategoryTaskCommand { get; }

        public MainWindowVM(IJobScheduler scheduler)
        {
            _scheduler = scheduler;

            AddTaskCommand = ReactiveCommand.CreateFromTask(OpenAddTaskAsync);

            LoadTasks();
        }

        /// <summary>
        /// Добавление таски
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
