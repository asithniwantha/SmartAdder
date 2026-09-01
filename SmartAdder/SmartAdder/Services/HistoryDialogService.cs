using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using SmartAdder.Models;

namespace SmartAdder.Services
{
    /// <summary>
    /// View-layer implementation of <see cref="IHistoryDialogService"/>.
    /// Owns all UI/XAML concerns for presenting calculation history so that
    /// ViewModels stay free of any Microsoft.UI.Xaml dependencies.
    /// </summary>
    public class HistoryDialogService : IHistoryDialogService
    {
        public async Task ShowHistoryAsync(IReadOnlyList<HistoryRecord> history)
        {
            var contentDialog = new ContentDialog
            {
                Title = "Calculation History",
                CloseButtonText = "Close",
            };

            if (history.Count == 0)
            {
                contentDialog.Content = new TextBlock { Text = "No history available." };
            }
            else
            {
                var listView = new ListView
                {
                    ItemsSource = history,
                    ItemTemplate = CreateHistoryTemplate(),
                    SelectionMode = ListViewSelectionMode.None
                };
                contentDialog.Content = listView;
            }

            if (App.Current is App app)
            {
                var window = app.GetMainWindow();
                if (window?.Content != null)
                {
                    contentDialog.XamlRoot = window.Content.XamlRoot;
                    await contentDialog.ShowAsync();
                }
            }
        }

        private static Microsoft.UI.Xaml.DataTemplate CreateHistoryTemplate()
        {
            string xaml = @"
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <Button Background=""Transparent"" BorderThickness=""0"" Padding=""0"" HorizontalAlignment=""Stretch"" HorizontalContentAlignment=""Stretch"">
                    <Button.Flyout>
                        <Flyout>
                            <ScrollViewer MaxHeight=""300"">
                                <ItemsControl ItemsSource=""{Binding Entries}"">
                                    <ItemsControl.ItemTemplate>
                                        <DataTemplate>
                                            <TextBlock Text=""{Binding}"" Margin=""0,0,0,4"" />
                                        </DataTemplate>
                                    </ItemsControl.ItemTemplate>
                                </ItemsControl>
                            </ScrollViewer>
                        </Flyout>
                    </Button.Flyout>
                    <StackPanel Margin=""0,0,0,12"">
                        <TextBlock Text=""{Binding Timestamp}"" FontWeight=""Bold"" />
                        <StackPanel Orientation=""Horizontal"">
                            <TextBlock Text=""Total: "" />
                            <TextBlock Text=""{Binding TotalSum}"" />
                        </StackPanel>
                    </StackPanel>
                </Button>
            </DataTemplate>";
            return (Microsoft.UI.Xaml.DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(xaml);
        }
    }
}
