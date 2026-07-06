using Domain.Models;

namespace WarehouseManagementApi;

public class FakeSupplierDirectory
{
    public static List<Supplier> Suppliers { get; set; }

    static FakeSupplierDirectory()
    {
        Suppliers =
        [
            new Supplier
            {
                Id = "a10a3176-53d8-42e0-b4a4-523e738e9b71",
                Name = "Supplier1",
                Country = "Lebanon",
                ContactEmail = "supplier1@email.com",
                Phone = "+96101234567",
            },
            new Supplier
            {
                Id = "7de0ab87-0046-499b-b8f4-2bd7acf0ea8f",
                Name = "Supplier2",
                Country = "Spain",
                ContactEmail = "supplier2@email.com",
                Phone = "+34911234089",
            },
            new Supplier
            {
                Id = "a62f515d-c26f-44e4-b6c3-1954aab22ae3",
                Name = "Supplier3",
                Country = "Qatar",
                ContactEmail = "supplier3@email.com",
                Phone = "+974800123456",
            }
        ];
    }
}