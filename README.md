# JobRunner

C# + Quartz + WindowsTaskScheduler приложение для реализации функционала:
1. Запуск задач windows (как в планировщике)
2. Управление свойствами задач
3. Автоматизация управлением задач (продление и тд.)
4. Другой (пока определение)

В решении несколько проектов (4):

	JobRunner.Core - библиотека классов (.NET Standart 2.0 с версией языка C# 8.0)
			модели данных, шифрование, DTO, интерфейсы
		Зависимости: 
			System.Text.Json v6.0.10
			System.ComponentModel.DataAnnotations v5.0.0

	JobRunner.Jobs - библиотека классов (.NET 8.0)
			реализация интерфейса IJobScheduler с библиотекой Quartz + регистрация в DI
		Зависимости: 
			JobRunner.Core
			
			System.Security.Cryptography.ProtectedData v8.0.0
			Microsoft.Extensions.DependencyInjection.Abstractions
			Quartz v3.18.0
		
	JobRunner.Avalonia - Avalonia .NET MVVM App
			UI составляющая проекта JobRunner, DI настройка + MVVM
			MVVM подход: RelativeUI v20.0.1
		Зависимости:
			JobRunner.Core 
			JobRunner.Jobs 
			
         	Microsoft.Extensions.DependencyInjection v8.0.1
			MessageBox.Avalonia v2.1.0
			Microsoft.Toolkit.Uwp.Notifications v7.1.3
			
	JobRunner.WindowsService - библиотека классов
			Windows Task Scheduler реализация (планировщик задач)
		Зависимости:
			JobRunner.Core
		
			Microsoft.Win32.TaskScheduler v2.12.0
			Microsoft.Extensions.DependencyInjection v8.0.1
			Microsoft.Extensions.DependencyInjection.Abstractions v8.0.2
			
# Планируемый функционал:
	1. Метки (категории) задач
	2. Сохранение конфигурации задач (опред. категории +-) в файл
	3. Фильтрация одновременно по нескольким параметрам (название, метки, группа, автор)
	4. Группировка задач (одновременно запуск и остановка)
	5. Параллельное выполнение
	6. Выборочное шифрование
	7. Вывод последней ошибки прямо в программе
	8. 