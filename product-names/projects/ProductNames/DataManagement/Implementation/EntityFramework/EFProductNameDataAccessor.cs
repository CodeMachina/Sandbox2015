using System.Linq;
using ProductNames.DataManagement.Implementation.DataModel;
using ProductNames.DataManagement.Contract.Commands;
using ProductNames.DataManagement.Contract.Queries;
using ProductNames.DataManagement.Contract;

namespace ProductNames.DataManagement.Implementation.EntityFramework {
    public class EFProductNameDataAccessor : IHandleProductNameData {
        private readonly ProductNameContext dbContext;

        public EFProductNameDataAccessor(ProductNameContext dbContext) {
            this.dbContext = dbContext;
        }

        public void CreateOrUpdateHandler(CreateOrUpdateCommand command) {
            dbContext.ProductNames.Add(new ProductName {
                ProductNameID = command.ProductId,
                Name = command.ProductName
            });
            dbContext.SaveChanges();
        }

        public Models.DomainModel.ProductName GetByIdHandler(GetByIdQuery query) {
            var dbResult = dbContext.ProductNames.FirstOrDefault(x => x.ProductNameID == query.ProductId);

            if(dbResult == null) {
                return new Models.DomainModel.ProductName();
            }

            return new Models.DomainModel.ProductName {
                ProductId = dbResult.ProductNameID,
                Name = dbResult.Name
            };
        }       
    }
}
