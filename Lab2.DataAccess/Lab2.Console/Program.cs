using Lab2.DataAccess;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1 - Filter baskets by year");
        Console.WriteLine("2 - Find delivery by city");
        Console.WriteLine("3 - Add new baskets and delivery");
        Console.WriteLine("4 - Delete baskets and delivery");
        Console.WriteLine("5 - Update baskets and delivery");
        Console.WriteLine("6 - Add Bread");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                FilterBasketByYearDiscount();
                break;
            case "2":
                FindDeliveryByCity();
                break;
            case "3":
                AddNewBasketAndAddress();
                break;
            case "4":
                DeleteBasketAndAddress();
                break;
            case "5":
                UpdateBasketAndAddress();
                break;
            case "6":
                AddBread();
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    static void FilterBasketByYearDiscount()
    {
        using (var context = new BasketDbContext())
        {
            var filteredHouses = context.Basket
                                        .Where(h => h.BasketDiscount > 2000)
                                        .ToList();

            foreach (var house in filteredHouses)
            {
                Console.WriteLine($"Delivery ID: {house.Id}, Client: {house.Client}, Discount: {house.BasketDiscount}");
            }
        }
    }

    static void FindDeliveryByCity()
    {
        using (var context = new BasketDbContext())
        {
            var address = context.Delivery.FirstOrDefault(a => a.City == "Chicago");

            if (address != null)
            {
                Console.WriteLine($"Address ID: {address.Id}, Street: {address.Street}, City: {address.City}, Postal Code: {address.PostalCode}, Country: {address.Country}");
            }
            else
            {
                Console.WriteLine("No delivery found in Chicago.");
            }
        }
    }

    static void AddNewBasketAndAddress()
    {
        using (var context = new BasketDbContext())
        {
            var newBasket = new Basket
            {
                BasketDiscount = 2024,
                Client = "Dima Test",
                Store = 440.0,
                Priority = 4
            };

            context.Basket.Add(newBasket);
            context.SaveChanges();

            var newDelivery = new Delivery
            {
                Street = "Jelgavas st",
                City = "Riga",
                PostalCode = "Lv-****",
                Country = "LV",
                Notes = "New house",
                BasketId = newBasket.Id
            };

            context.Delivery.Add(newDelivery);
            context.SaveChanges();

            Console.WriteLine("New Basket and delivery added successfully.");
        }
    }

    static void DeleteBasketAndAddress()
    {
        using (var context = new BasketDbContext())
        {
            var deliveryToDelete = context.Delivery.FirstOrDefault(a => a.BasketId == 22);

            if (deliveryToDelete != null)
            {
                context.Delivery.Remove(deliveryToDelete);
                Console.WriteLine("Delivery deleted successfully.");
            }

            var basketToDelete = context.Basket.FirstOrDefault(h => h.Id == 22);

            if (basketToDelete != null)
            {
                context.Basket.Remove(basketToDelete);
                Console.WriteLine("Basket deleted successfully.");
            }

            context.SaveChanges();
        }
    }

    static void UpdateBasketAndAddress()
    {
        using (var context = new BasketDbContext())
        {
            var basketToUpdate = context.Basket.FirstOrDefault(h => h.Id == 3);

            if (basketToUpdate != null)
            {
                basketToUpdate.Client = "Denis Petrov";
                context.SaveChanges();
                Console.WriteLine("Basket updated successfully.");
            }

            var addressToUpdate = context.Delivery.FirstOrDefault(a => a.BasketId == 3);

            if (addressToUpdate != null)
            {
                addressToUpdate.Notes = "Family home";
                context.SaveChanges();
                Console.WriteLine("Address updated successfully.");
            }
        }
    }

    static void AddBread()
    {
        using (var context = new BasketDbContext())
        {
            var bread = context.Basket.FirstOrDefault(h => h.Client == "Antov Petrov");

            if (bread != null)
            {
                var newBread = new Bread
                {
                    Type = "White",
                    Size = 50.0,
                    BasketId = bread.Id
                };

                context.Bread.Add(newBread);
                context.SaveChanges();

                Console.WriteLine("Bread added successfullyv.");
            }
            else
            {
                Console.WriteLine("Bread not found.");
            }
        }
    }
}
