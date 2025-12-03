using Microsoft.AspNetCore.Identity;

namespace WebStore.Model.DataModels;

public class StationaryStoreEmployee : User {
    public DateTime HireDate { get; set; }
    public StationaryStore StationaryStore { get; set; } = default!;
    public int StationaryStoreId { get; set; }
}

