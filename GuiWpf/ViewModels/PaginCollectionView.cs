using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace GuiWpf.ViewModels
{
    public class PaginCollectionView<T> : BindableBase
    {   
        List<T> _items;
        public List<T> ItemsSource { get => _items; }

        ObservableCollection<T> ShowItems = new(); 
        
        public ICollectionView Items { get; set; }

        public PaginCollectionView(IEnumerable<T> items, int itemsPerPage)
        {
            Init(items, itemsPerPage);
        }

        public PaginCollectionView() { }

        public void Init(IEnumerable<T> items, int itemsPerPage)
        {
            _items = items.ToList();
            _itemsPerPage = itemsPerPage;
           
            Refresh(true);
        }

        public void Refresh(bool init = false)
        {
            ShowItems.Clear();
            ShowItems.AddRange(_items.Skip(StartIndex).Take(_itemsPerPage));
            if (init)
            {
                Items = CollectionViewSource.GetDefaultView(ShowItems);
            }
            else
            {
                Items.Refresh();
            }
        }        

        public Predicate<object> Filter { get => Items.Filter; set => Items.Filter = value; }
        
        public SortDescriptionCollection SortDescriptions { get => Items.SortDescriptions; }

        public ObservableCollection<GroupDescription> GroupDescriptions { get => Items.GroupDescriptions; }


        int _itemsPerPage;        
        public int ItemsPerPage { get => _itemsPerPage; set => SetProperty(ref _itemsPerPage, value); }

        private int _currentPage = 0;
        public int CurrentPage
        {
            get => _currentPage;
            set 
            { 
                if(SetProperty(ref _currentPage, value))
                {
                    
                } 
            }
        }

        public int PageCount => (int)Math.Ceiling((double) _items.Count / _itemsPerPage);

        private int EndIndex
        {
            get
            {
                var end = CurrentPage * _itemsPerPage;
                return (end > _items.Count) ? _items.Count : end;
            }
        }

        private int StartIndex => CurrentPage * _itemsPerPage;

        public void MoveToNextPage()
        {
            if (CurrentPage < PageCount)
            {
                CurrentPage += 1;
            }
            Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (CurrentPage > 0)
            {
                CurrentPage -= 1;
            }
            Refresh();
        }
    }
}
