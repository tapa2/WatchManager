public class WatchList
{
    private List<Watch> watches;

    public WatchList()
    {
        watches = new List<Watch>();
    }

    public WatchList(List<Watch> watches)
    {
        this.watches = watches;
    }

    public void AddWatch(Watch watch)
    {
        watches.Add(watch);
    }

    public void EditWatch(int id, Watch updatedWatch)
    {
        // Знаходимо годинник за ідентифікатором
        var watch = watches.FirstOrDefault(w => w.Id == id);
        if (watch != null)
        {
            // Оновлюємо властивості годинника
            watch.Brand = updatedWatch.Brand;
            watch.Model = updatedWatch.Model;
            watch.Price = updatedWatch.Price;
            watch.InStock = updatedWatch.InStock;
        }
    }

    public void RemoveWatch(int id)
    {
        var watch = watches.FirstOrDefault(w => w.Id == id);
        if (watch != null)
        {
            watches.Remove(watch);
        }
    }

    public List<Watch> GetAllWatches()
    {
        return watches;
    }

    public List<Watch> SearchByBrand(string brand)
    {
        return watches.Where(w => w.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Watch> SearchByPriceRange(decimal minPrice, decimal maxPrice)
    {
        return watches.Where(w => w.Price >= minPrice && w.Price <= maxPrice).ToList();
    }

    public List<Watch> SearchByStockStatus(bool inStock)
    {
        return watches.Where(w => w.InStock == inStock).ToList();
    }
}
