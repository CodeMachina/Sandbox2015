using ProductNames.DataManagement.Implementation.DataModel;
using Microsoft.Data.Entity;

namespace ProductNames.DataManagement.Implementation.EntityFramework {
    public class ProductNameContext : DbContext {
        public virtual DbSet<ProductName> ProductNames {get; set;}
    }
}
