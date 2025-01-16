using PairMatching.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GuiWpf.ViewModels
{
    public class PaginCollectionViewModel<T> : BindableBase where T : class
    {
        #region Ptoperties

        private T _selectedItem;
        public T SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        #region Collections

        ObservableCollection<T> _itemsSource = new();
        public ObservableCollection<T> ItemsSource
        {
            // Show only the items that pass the filter
            get
            {
                return _itemsSource;
            }
            set => SetProperty(ref _itemsSource, value);
        }

        ObservableCollection<T> _showItems = new();
        public ObservableCollection<T> FilterdItems
        {
            get => _showItems;
            private set
            {
                if (SetProperty(ref _showItems, value))
                {
                    CurrentPage = 0;
                }
            }
        }

        public int Count { get => FilterdItems.Count; }

        ObservableCollection<T> _items = new();
        public ObservableCollection<T> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        const int initItemsPerPage = 5;

        private List<int> _maxRecordsInPage;
        public List<int> MaxRecordsInPage
        {
            get => _maxRecordsInPage ??= Enumerable
                .Range(initItemsPerPage, 80)
                .Where(x => x % initItemsPerPage == 0)
                .ToList();
            //   set => SetProperty(ref _maxRecordsInPage, value);
        }
        #endregion

        int _itemsPerPage = initItemsPerPage;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                if (SetProperty(ref _itemsPerPage, value))
                {
                    Refresh();
                }
            }
        }

        private int _currentPage = 0;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        int _pageCount = 0;
        public int PageCount
        {
            get
            {
                PageCount = GetPageCount();
                return _pageCount;
            }
            set
            {
                SetProperty(ref _pageCount, value);
            }
        }

        private int EndIndex
        {
            get
            {
                var end = StartIndex + ItemsPerPage;
                return (end > FilterdItems.Count) ? FilterdItems.Count : end;
            }
        }

        private int _startIndex = 0;
        private int StartIndex
        {
            get
            {
                if (FilterdItems.Any())
                {
                    int temp = CurrentPage * ItemsPerPage;
                    _startIndex = temp > FilterdItems.Count ? _startIndex : temp;
                }
                return _startIndex;
            }
        }
        
        private bool _canGoNext;
        public bool CanGoNext
        {
            get => _canGoNext;
            set
            {
                SetProperty(ref _canGoNext, value);
                NextPageCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _canGoPrev;
        public bool CanGoPrev
        {
            get => _canGoPrev;
            set
            {
                SetProperty(ref _canGoPrev, value);
                PrevPageCommand.RaiseCanExecuteChanged();
            }         
        }

        Predicate<T> _filter = new(_ => true);
        public Predicate<T> Filter { get => _filter; set => SetProperty(ref _filter, value); }
        #endregion

        #region Commands

        DelegateCommand _nextPageCommand;
        public DelegateCommand NextPageCommand => _nextPageCommand ??= new(
        () =>
        {
            CurrentPage += 1;
            Refresh();
        },
        () =>
        {
            return CanGoNext;
        });

        DelegateCommand _prevPageCommand;
        public DelegateCommand PrevPageCommand => _prevPageCommand ??= new(
        () =>
        {
            CurrentPage -= 1;
            Refresh();
        }, 
        () => 
        {
            return CanGoPrev;
        });

        DelegateCommand<object> _SortByCommand;
        public DelegateCommand<object> SortByCommand => _SortByCommand ??= new(
        (e) =>
        {
            if (e is System.Windows.Controls.DataGridSortingEventArgs ev)
            {
                var sortPath = ev.Column?.SortMemberPath;
                var sortDir = ev.Column?.SortDirection;
                if (!string.IsNullOrWhiteSpace(sortPath) && sortDir != null)
                {
                    var prop = typeof(T).GetProperty(sortPath);
                    if(prop != null)
                    {
                        if(sortDir == ListSortDirection.Ascending)
                        {
                            ItemsSource = new(ItemsSource.OrderBy(item => prop.GetValue(item)));
                        }
                        else
                        {
                            ItemsSource = new (ItemsSource.OrderByDescending(item => prop.GetValue(item)));
                        }
                        Refresh();
                    }

                }
            }
        });
        #endregion

        #region Methods
        public void Init(IEnumerable<T> items, int itemsPerPage, Predicate<T> filter = null)
        {
            ItemsSource = new(items);

            ItemsPerPage = itemsPerPage;
            
            Filter = filter ?? Filter;
            
            Refresh();
        }
        
        public void Refresh(bool add = false)
        {
            var temp = FilterdItems.ToList();
            FilterdItems.Clear();
            var filterItems = ItemsSource.Where(item => Filter.Invoke(item));
            FilterdItems.AddRange(filterItems);
            
            if (!add && !temp.SequenceEqual(FilterdItems))
            {   // return to the first page if there is a change by the filter
                CurrentPage = 0;
            }

            CanGoNext = CurrentPage + 1 < PageCount;
            CanGoPrev = CurrentPage > 0;

            var showItems = FilterdItems.Where(item => PagingFilter(item));
            var reverseITems = showItems.Reverse();
            Items.Clear();
            Items.AddRange(showItems);
            
            PageCount = GetPageCount();
        }

        public void Update(T item, bool inPlace = false) 
        {
            if (item is null)
            {
                return;
            }
            var itemModel = ItemsSource.FirstOrDefault(x => (x as BaseModel)!.Id == (item as BaseModel)!.Id);
            if(itemModel == null)
            {
                return;
            }
            var index = ItemsSource.IndexOf(itemModel);
            ItemsSource.Remove(itemModel);
            ItemsSource.Insert(inPlace ? index : 0, item);
            Refresh();
        }

        public void Add(T item)
        {
            if(item is null)
            {
                return;
            }
            var itemModel = ItemsSource.FirstOrDefault(x => (x as BaseModel)!.Id == (item as BaseModel)!.Id);
            if (itemModel != null)
            {
                return;
            }
            ItemsSource.Insert(0, item);
            Refresh(true);
        }

        public void Remove(T item)
        {
            if (item is null)
            {
                return;
            }
            var itemModel = ItemsSource.FirstOrDefault(x => (x as BaseModel)!.Id == (item as BaseModel)!.Id);
            if (itemModel == null)
            {
                return;
            }
            ItemsSource.Remove(itemModel);
            Refresh();
        }

        bool PagingFilter(T item)
        {
            var index = FilterdItems.IndexOf(item);
            return index < EndIndex && index >= StartIndex;
        }

        private int GetPageCount()
        {
            return FilterdItems is null ?
                                    0 : (int)Math.Ceiling((double)FilterdItems.Count / ItemsPerPage);
        } 
        #endregion
    }
}
