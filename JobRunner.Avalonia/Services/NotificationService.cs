using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace JobRunner.Avalonia.Services
{
    /// <summary>
    /// Сервис реализации уведомлений (пока закрыт, было удаление пакета)
    /// </summary>
    public static class NotificationService
    {
        /*
        /// <summary>
        /// Собственный метод для отображения уведомлений
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="success"></param>
        public static void ShowToast(string title, string message, bool success = true)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            try
            {
                var builder = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message);

                var field = typeof(ToastContentBuilder).GetField("_content",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field?.GetValue(builder) is ToastContent content)
                {
                    var xmlString = content.ToString();
                    var doc = new Windows.Data.Xml.Dom.XmlDocument();
                    doc.LoadXml(xmlString);

                    var toast = new ToastNotification(doc);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
            }
        }

        public static async Task ShowPopupAsync(string title, string message, Window owner)
        {
            var popup = new Window
            {
                Title = title,
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = TextWrapping.Wrap
                        },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Width = 75
                        }
                    }
                }
            };

            var button = ((StackPanel)popup.Content).Children[1] as Button;
            button.Click += (s, e) => popup.Close();

            await popup.ShowDialog(owner);
        }
        */
    }
}
