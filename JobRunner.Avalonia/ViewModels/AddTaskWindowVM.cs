using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JobRunner.Core.Settings;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using JobRunner.WindowsService.Extensions;

namespace JobRunner.Avalonia.ViewModels
{
    public class AddTaskWindowVM : ViewModelBase
    {
        private Window _currentWindow;

        private string _taskName = string.Empty;
        public string TaskName
        {
            get => _taskName;
            set => this.RaiseAndSetIfChanged(ref _taskName, value);
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

        #region Расписание

        /// <summary>
        /// Словарь с периодами выполнения JobTask
        /// </summary>
        public Dictionary<PeriodType, string> PeriodTypeNames { get; } = new()
        {
            { PeriodType.Once, "Однократно" },
            { PeriodType.EveryMinutes, "Каждые N минут" },
            { PeriodType.EveryHourly, "Каждый час" },
            { PeriodType.EveryDaily, "Каждый день" },
            { PeriodType.EveryWeekly, "Каждую неделю" },
            { PeriodType.EveryMonthly, "Каждый месяц" },
            { PeriodType.EveryQuarterly, "Каждый квартал" },
            { PeriodType.EveryYearly, "Каждый год" }
        };

        public List<KeyValuePair<PeriodType, string>> PeriodTypeList => PeriodTypeNames.ToList();

        private PeriodType _selectedPeriodType = PeriodType.Once;
        public PeriodType SelectedPeriodType
        {
            get => _selectedPeriodType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPeriodType, value);
                this.RaisePropertyChanged(nameof(ShowTimeSettings));
                this.RaisePropertyChanged(nameof(ShowInterval));
            }
        }

        private KeyValuePair<PeriodType, string> _selectedPeriodTypeItem;
        public KeyValuePair<PeriodType, string> SelectedPeriodTypeItem
        {
            get => _selectedPeriodTypeItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPeriodTypeItem, value);
                SelectedPeriodType = value.Key;  // Обновляем PeriodType
            }
        }

        private string _hour = "0";
        public string Hour
        {
            get => _hour;
            set => this.RaiseAndSetIfChanged(ref _hour, value);
        }

        private string _minute = "0";
        public string Minute
        {
            get => _minute;
            set => this.RaiseAndSetIfChanged(ref _minute, value);
        }

        private string _intervalMinutes = "60";
        public string IntervalMinutes
        {
            get => _intervalMinutes;
            set => this.RaiseAndSetIfChanged(ref _intervalMinutes, value);
        }

        public bool ShowTimeSettings => SelectedPeriodType != PeriodType.Once;
        public bool ShowInterval => SelectedPeriodType == PeriodType.EveryMinutes;
        #endregion

        public ReactiveCommand<Unit, Unit> BrowseCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public Core.JobTask? ResultTask { get; private set; }

        public AddTaskWindowVM()
        {
            SelectedPeriodTypeItem = PeriodTypeList.First();

            BrowseCommand = ReactiveCommand.CreateFromTask(BrowseAsync);
            SaveCommand = ReactiveCommand.Create(Save);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private async Task BrowseAsync()
        {
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow == null) return;

            var dialog = new OpenFileDialog();
            dialog.Title = "Выберите программу или скрипт";
            dialog.AllowMultiple = false;

            var result = await dialog.ShowAsync(mainWindow);
            if (result != null && result.Any())
            {
                ExecutionPath = result[0];
            }
        }

        /// <summary>
        /// Сохранение задачи и добавление ее в список
        /// </summary>
        private void Save()
        {
            ResultTask = new Core.JobTask
            {
                Name = TaskName,
                ExecutionPath = ExecutionPath,
                Arguments = Arguments,
                IsEncrypt = IsEncrypt,
                ScheduleSettings = new ScheduleSettings
                {
                    PeriodType = SelectedPeriodType,
                    Hour = int.TryParse(Hour, out var h) ? h : 0,
                    Minute = int.TryParse(Minute, out var m) ? m : 0,
                    IntervalMinutes = int.TryParse(IntervalMinutes, out var i) ? i : 60
                }
            };
            _currentWindow.Close();
        }

        /// <summary>
        /// Отмена создания таски (избежание Null)
        /// </summary>
        private void Cancel()
        {
            ResultTask = null;
            _currentWindow.Close();
        }

        /// <summary>
        /// Метод для установки окна, которое будет закрываться (ShowDialog)
        /// </summary>
        /// <param name="window">Текущее окно</param>
        public void SetCurrentWindow(Window window)
        {
            _currentWindow = window;
        }
    }
}
