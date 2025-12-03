namespace WebStore.Model.DataModels;

public class StationaryStore {
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public IList<StationaryStoreEmployee> Employees { get; set; }
}

