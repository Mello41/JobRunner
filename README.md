# JobRunner

C# проект для реализации функционала:
1. Запуск задач windows (как в планировщике)
2. Управление свойствами задач
3. Автоматизация управлением задач (продление и тд.)
4. 

В решении несколько проектов:

	JobRunner.Core - библиотека классов (.NET Standart 2.0)
			модели данных, шифрование, DTO, интерфейсы
		зависимости: 
			System.Text.Json v6.0.10

	JobRunner.Jobs - библиотека классов (.NET 8.0)
			реализация интерфейса IJobScheduler с библиотекой Quartz + регистрация в DI
		зависимости: 
			JobRunner.Core
			
			Microsoft.Extensions.DependencyInjection.Abstractions
			Quartz v3.18.0
		
	JobRunner.Avalonia - Avalonia .NET MVVM App
			UI составляющая проекта JobRunner, DI настройка + MVVM
			MVVM подход: RelativeUI v20.0.1
		Зависимости:
			JobRunner.Core 
			JobRunner.Jobs 
			
         	Microsoft.Extensions.DependencyInjection v8.0.1
			
	Схема зависимостей по проекту:
		JobRunner.Avalonia
		 --> JobRunner.Core
		 --> JobRunner.Jobs
			 --> JobRunner.Core