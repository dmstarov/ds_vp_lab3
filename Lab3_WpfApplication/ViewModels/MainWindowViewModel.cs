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
        public HouseDbContext _dbContext;

        public ObservableCollection<House> Houses { get; set; }
        public ObservableCollection<Address> Addresses { get; set; }
        public ObservableCollection<Garage> Garages { get; set; }
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


        private House _selectedHouse;
        public House SelectedHouse
        {
            get => _selectedHouse;
            set
            {
                _selectedHouse = value;
                OnPropertyChanged(nameof(SelectedHouse));
                LoadChildData();
            }
        }

        public ICommand SelectHouseCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateHouseCommand { get; }
        public ICommand UpdateAddressCommand { get; }
        public ICommand UpdateGarageCommand { get; }

        public MainWindowViewModel()
        {
            _dbContext = new HouseDbContext();
            Houses = new ObservableCollection<House>(_dbContext.Houses.ToList());
            Addresses = new ObservableCollection<Address>();
            Garages = new ObservableCollection<Garage>();

            FloorsOptions = new ObservableCollection<int>(Enumerable.Range(1, 10));

            SelectHouseCommand = new RelayCommand(LoadChildData);
            SearchCommand = new RelayCommand(ApplyFilters);
            DeleteCommand = new RelayCommand(DeleteHouse);

            UpdateHouseCommand = new RelayCommand(UpdateHouse);
            UpdateAddressCommand = new RelayCommand<Address>(UpdateAddress);
            UpdateGarageCommand = new RelayCommand<Garage>(UpdateGarage);
        }

        public void LoadChildData()
        {
            if (_selectedHouse == null)
            {
                Addresses.Clear();
                Garages.Clear();
                return;
            }

            LoadAddresses();
            LoadGarage();
        }

        public void LoadAddresses()
        {
            if (SelectedHouse != null)
            {
                var relatedAddresses = _dbContext.Addresses
                    .Where(a => a.HouseId == SelectedHouse.Id)
                    .ToList();

                Addresses.Clear();
                foreach (var address in relatedAddresses)
                {
                    Addresses.Add(address);
                }
            }
            else
            {
                Addresses.Clear();
            }
        }

        public void LoadGarage()
        {
            if (SelectedHouse != null)
            {
                var relatedGarages = _dbContext.Garages
                    .Where(g => g.HouseId == SelectedHouse.Id)
                    .ToList();

                Garages.Clear();
                foreach (var garage in relatedGarages)
                {
                    Garages.Add(garage);
                }
            }
            else
            {
                Garages.Clear();
            }
        }

        public void ApplyFilters()
        {
            var query = _dbContext.Houses.AsQueryable();

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

            Houses.Clear();
            foreach (var house in query.ToList())
            {
                Houses.Add(house);
            }
        }

        public void DeleteHouse()
        {
            if (_selectedHouse != null)
            {
                var relatedAddresses = _dbContext.Addresses
                    .Where(a => a.HouseId == _selectedHouse.Id)
                    .ToList();

                if (relatedAddresses.Any())
                {
                    _dbContext.Addresses.RemoveRange(relatedAddresses);
                }

                var relatedGarages = _dbContext.Garages
                    .Where(g => g.HouseId == _selectedHouse.Id)
                    .ToList();

                if (relatedGarages.Any())
                {
                    _dbContext.Garages.RemoveRange(relatedGarages);
                }

                _dbContext.Houses.Remove(_selectedHouse);

                _dbContext.SaveChanges();

                Houses.Remove(_selectedHouse);

                Addresses.Clear();
                Garages.Clear();

                SelectedHouse = null;
            }
        }

        public void UpdateHouse()
        {
            if (_selectedHouse != null)
            {
                var existingHouse = _dbContext.Houses.FirstOrDefault(h => h.Id == _selectedHouse.Id);
                if (existingHouse != null)
                {
                    existingHouse.Owner = _selectedHouse.Owner;
                    existingHouse.YearBuilt = _selectedHouse.YearBuilt;
                    existingHouse.Area = _selectedHouse.Area;
                    existingHouse.Floors = _selectedHouse.Floors;

                    _dbContext.Entry(existingHouse).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                }
            }
        }

        public void UpdateAddress(Address address)
        {
            if (address != null)
            {
                var existingAddress = _dbContext.Addresses.FirstOrDefault(a => a.Id == address.Id);
                if (existingAddress != null)
                {
                    existingAddress.Street = address.Street;
                    existingAddress.City = address.City;
                    existingAddress.PostalCode = address.PostalCode;
                    existingAddress.Country = address.Country;
                    existingAddress.Notes = address.Notes;


                    _dbContext.SaveChanges();

                    var index = Addresses.IndexOf(address);
                    Addresses[index] = existingAddress;
                }
            }
        }

        public void UpdateGarage(Garage garage)
        {
            if (garage != null)
            {
                var existingGarage = _dbContext.Garages.FirstOrDefault(g => g.Id == garage.Id);
                if (existingGarage != null)
                {
                    existingGarage.Type = garage.Type;
                    existingGarage.Size = garage.Size;

                    _dbContext.SaveChanges();

                    var index = Garages.IndexOf(garage);
                    Garages[index] = existingGarage;
                }
            }
        }

    }
}


