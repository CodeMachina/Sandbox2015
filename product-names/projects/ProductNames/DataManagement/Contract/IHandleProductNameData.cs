using ProductNames.DataManagement.Contract.Commands;
using ProductNames.DataManagement.Contract.Queries;

namespace ProductNames.DataManagement.Contract {
    public interface IHandleProductNameData {
        Models.DomainModel.ProductName GetByIdHandler(GetByIdQuery query);
        void CreateOrUpdateHandler(CreateOrUpdateCommand command);
    }
}
