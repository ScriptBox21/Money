﻿using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Dialogs
{
    public sealed partial class CategoryPicker : ContentDialog
    {
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private List<CategoryModel> models;

        public IKey SelectedKey
        {
            get { return (IKey)GetValue(SelectedKeyProperty); }
            set { SetValue(SelectedKeyProperty, value); }
        }

        public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
            "SelectedKey",
            typeof(IKey),
            typeof(CategoryPicker),
            new PropertyMetadata(KeyFactory.Empty(typeof(Category)), OnSelectedKeyChanged)
        );

        private static void OnSelectedKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoryPicker control = (CategoryPicker)d;
            control.OnSelectedKeyChanged();
        }

        public CategoryPicker()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnSelectedKeyChanged()
        {
            isCategoriesViewChangedAttached = false;

            if (SelectedKey.IsEmpty)
                gvwCategories.SelectedItem = null;
            else
                gvwCategories.SelectedItem = gvwCategories.Items.OfType<CategoryModel>().FirstOrDefault(c => c.Key.Equals(SelectedKey));

            isCategoriesViewChangedAttached = true;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            models = (await queryDispatcher.QueryAsync(new ListAllCategory())).ToList();
            gvwCategories.ItemsSource = models;

            if (!SelectedKey.IsEmpty)
                OnSelectedKeyChanged();
        }
        
        private bool isCategoriesViewChangedAttached = true;

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isCategoriesViewChangedAttached)
                return;

            if (e.AddedItems.Count > 0)
                SelectedKey = ((CategoryModel)e.AddedItems[0]).Key;
            else
                SelectedKey = KeyFactory.Empty(typeof(Category));
        }
    }
}
