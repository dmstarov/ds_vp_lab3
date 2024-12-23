using System.Collections.ObjectModel;
using Lab2.DataAccess;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;


namespace Lab3.WpfApplication.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public BasketDbContext _dbContext;

        public ObservableCollection<Basket> Basket { get; set; }
        public ObservableCollection<Delivery> Delivery { get; set; }
        public ObservableCollection<Bread> Bread { get; set; }
        public ObservableCollection<int> FloorsOptions { get; set; }

        private string _ownerFilter;
        public string OwnerFilter
        {
            get => _ownerFilter;
            set => SetProperty(ref _ownerFilter, value);
        }

        private string _yearBuiltFilter;
        public string YearBuiltFilter
        {
            get => _yearBuiltFilter;
            set => SetProperty(ref _yearBuiltFilter, value);
        }

        private int? _floorsFilter;
        public int? FloorsFilter
        {
            get => _floorsFilter;
            set => SetProperty(ref _floorsFilter, value);
        }


        private Basket _selectedBasket;
        public Basket SelectedBasket
        {
            get => _selectedBasket;
            set
            {
                _selectedBasket = value;
                OnPropertyChanged(nameof(SelectedBasket));
                LoadChildData();
            }
        }

        public ICommand SelectBasketCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateBasketCommand { get; }
        public ICommand UpdateDeliveryCommand { get; }
        public ICommand UpdateBreadCommand { get; }

        public MainWindowViewModel()
        {
            _dbContext = new BasketDbContext();
            Basket = new ObservableCollection<Basket>(_dbContext.Baskets.ToList());
            Delivery= new ObservableCollection<Delivery>();
            Bread = new ObservableCollection<Bread>();

            FloorsOptions = new ObservableCollection<int>(Enumerable.Range(1, 10));

            SelectBasketCommand = new RelayCommand(LoadChildData);
            SearchCommand = new RelayCommand(ApplyFilters);
            DeleteCommand = new RelayCommand(DeleteBasket);

            UpdateBasketCommand = new RelayCommand(UpdateBasket);
            UpdateDeliveryCommand = new RelayCommand<Delivery>(UpdateDelivery);
            UpdateBreadCommand = new RelayCommand<Bread>(UpdateBread);
        }

        public void LoadChildData()
        {
            if (_selectedBasket == null)
            {
                Delivery.Clear();
                Bread.Clear();
                return;
            }

            LoadDeliveryes();
            LoadBread();
        }

        public void LoadDeliveryes()
        {
            if (SelectedBasket != null)
            {
                var relatedDeliveryes = _dbContext.Deliveryes
                    .Where(a => a.BasketId == SelectedBasket.Id)
                    .ToList();

                Delivery.Clear();
                foreach (var Delivery in relatedDeliveryes)
                {
                    Delivery.Add(Delivery);
                }
            }
            else
            {
                Delivery.Clear();
            }
        }

        public void LoadBread()
        {
            if (SelectedBasket != null)
            {
                var relatedBreads = _dbContext.Breads
                    .Where(g => g.BasketId == SelectedBasket.Id)
                    .ToList();

                Bread.Clear();
                foreach (var Bread in relatedBreads)
                {
                    Bread.Add(Bread);
                }
            }
            else
            {
                Bread.Clear();
            }
        }

        public void ApplyFilters()
        {
            var query = _dbContext.Baskets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(OwnerFilter))
            {
                query = query.Where(h => h.Owner.Contains(OwnerFilter));
            }

            if (!string.IsNullOrWhiteSpace(YearBuiltFilter) && int.TryParse(YearBuiltFilter, out int yearBuilt))
            {
                query = query.Where(h => h.YearBuilt == yearBuilt);
            }

            if (FloorsFilter.HasValue)
            {
                query = query.Where(h => h.Floors == FloorsFilter.Value);
            }

            Basket.Clear();
            foreach (var Basket in query.ToList())
            {
                Basket.Add(Basket);
            }
        }

        public void DeleteBasket()
        {
            if (_selectedBasket != null)
            {
                var relatedDeliveryes = _dbContext.Deliveryes
                    .Where(a => a.BasketId == _selectedBasket.Id)
                    .ToList();

                if (relatedDeliveryes.Any())
                {
                    _dbContext.Deliveryes.RemoveRange(relatedDeliveryes);
                }

                var relatedBreads = _dbContext.Breads
                    .Where(g => g.BasketId == _selectedBasket.Id)
                    .ToList();

                if (relatedBreads.Any())
                {
                    _dbContext.Breads.RemoveRange(relatedBreads);
                }

                _dbContext.Baskets.Remove(_selectedBasket);

                _dbContext.SaveChanges();

                Basket.Remove(_selectedBasket);

                Delivery.Clear();
                Bread.Clear();

                SelectedBasket = null;
            }
        }

        public void UpdateBasket()
        {
            if (_selectedBasket != null)
            {
                var existingBasket = _dbContext.Baskets.FirstOrDefault(h => h.Id == _selectedBasket.Id);
                if (existingBasket != null)
                {
                    existingBasket.Owner = _selectedBasket.Owner;
                    existingBasket.YearBuilt = _selectedBasket.YearBuilt;
                    existingBasket.Area = _selectedBasket.Area;
                    existingBasket.Floors = _selectedBasket.Floors;

                    _dbContext.Entry(existingBasket).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                }
            }
        }

        public void UpdateDelivery(Delivery Delivery)
        {
            if (Delivery != null)
            {
                var existingDelivery = _dbContext.Deliveryes.FirstOrDefault(a => a.Id == Delivery.Id);
                if (existingDelivery != null)
                {
                    existingDelivery.Street = Delivery.Street;
                    existingDelivery.City = Delivery.City;
                    existingDelivery.PostalCode = Delivery.PostalCode;
                    existingDelivery.Country = Delivery.Country;
                    existingDelivery.Notes = Delivery.Notes;


                    _dbContext.SaveChanges();

                    var index = Delivery.IndexOf(Delivery);
                    Delivery[index] = existingDelivery;
                }
            }
        }

        public void UpdateBread(Bread Bread)
        {
            if (Bread != null)
            {
                var existingBread = _dbContext.Breads.FirstOrDefault(g => g.Id == Bread.Id);
                if (existingBread != null)
                {
                    existingBread.Type = Bread.Type;
                    existingBread.Size = Bread.Size;

                    _dbContext.SaveChanges();

                    var index = Bread.IndexOf(Bread);
                    Bread[index] = existingBread;
                }
            }
        }

    }
}


