using POSH.Socrata.Entity.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace POSH.Socrata.ViewModel.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        #region declarations

        private ObservableCollection<CityData> _searchList = null;

        /// <summary>
        /// Gets or sets searched dataset items
        /// </summary>
        public ObservableCollection<CityData> SearchList
        {
            get { return _searchList; }
            set
            {
                _searchList = value;
                this.NotifyPropertyChanged("SearchList");
            }
        }

        #endregion declarations

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchViewModel()
        {
            this.SearchList = new ObservableCollection<CityData>();
        }

        /// <summary>
        /// Searches DataSetItems from the fetched items
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchList"></param>
        /// <returns></returns>
        public async Task GetSearchResult(string searchText, List<CityData> searchList)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    var searchItems = searchList.Where(item => item.Name.ToLower().Contains(searchText) || item.Category.ToLower().Contains(searchText) || item.Address.ToLower().Contains(searchText) || item.SubTitle.ToLower().Contains(searchText)).ToList();
                    this.SearchList.Clear();
                    foreach (var searchedItem in searchItems)
                    {
                        this.SearchList.Add(searchedItem);
                    }
                }
                else
                {
                    this.SearchList.Clear();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}