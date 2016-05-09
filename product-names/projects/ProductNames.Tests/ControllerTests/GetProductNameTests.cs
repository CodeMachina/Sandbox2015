using ProductNames.Controllers;
using ProductNames.DataManagement.Contract;
using ProductNames.DataManagement.Contract.Queries;
using ProductNames.Models.ViewModel.ProductName;
using Microsoft.AspNet.Mvc;
using System;
using Xunit;

namespace ProductNames.Tests.ControllerTests {
    public class GetByIdTests {
        [Fact]
        public void ZeroGuid_Test()
        {
            Guid guid = new Guid();

            var mockIHandleProductNameData = new Moq.Mock<IHandleProductNameData>();
            mockIHandleProductNameData.Setup(x => x.GetByIdHandler(Moq.It.IsAny<GetByIdQuery>())).Returns(new Models.DomainModel.ProductName {
                ProductId = guid,
                Name = "Computer"
            });

            var controller = new ProductNameController(mockIHandleProductNameData.Object);
            var result = controller.GetById(new GetByIdInput { ProductId = guid }) as JsonResult;
            Assert.NotNull(result);

            var outputResult = result.Value as GetByIdOutput;
            Assert.NotNull(outputResult);
            Assert.Equal(outputResult.ProductId, guid);
        }

        [Fact]
        public void NullProductNameReturned_Test() {
            var guid = new Guid();

            var mockIHandleProductNameData = new Moq.Mock<IHandleProductNameData>();
            mockIHandleProductNameData.Setup(x => x.GetByIdHandler(Moq.It.IsAny<GetByIdQuery>())).Returns((Models.DomainModel.ProductName)null);

            var controller = new ProductNameController(mockIHandleProductNameData.Object);
            var result = controller.GetById(new GetByIdInput { ProductId = guid }) as JsonResult;
            Assert.NotNull(result);

            var outputResult = result.Value as GetByIdOutput;
            Assert.NotNull(outputResult);
            Assert.Equal(outputResult.ProductId, guid);
        }

        [Fact]
        public void KnownGuid_Test() {
            var guid = new Guid("1dcefb30-1a8d-44c2-b7d8-1b1a82226e74");

            var mockIHandleProductNameData = new Moq.Mock<IHandleProductNameData>();
            mockIHandleProductNameData.Setup(x => x.GetByIdHandler(Moq.It.IsAny<GetByIdQuery>())).Returns(new Models.DomainModel.ProductName {
                ProductId  = guid,
                Name = "Ipod"
            });

            var controller = new ProductNameController(mockIHandleProductNameData.Object);
            var result = controller.GetById(new GetByIdInput { ProductId = guid }) as JsonResult;
            Assert.NotNull(result);

            var outputResult = result.Value as GetByIdOutput;
            Assert.NotNull(outputResult);
            Assert.Equal(outputResult.ProductId, guid);
            Assert.Equal(outputResult.Name, "Ipod");
        }
    }
}
