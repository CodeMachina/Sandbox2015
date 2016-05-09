using ProductNames.DataManagement.Implementation.DataModel;
using ProductNames.DataManagement.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Microsoft.Data.Entity;
using ProductNames.DataManagement.Contract.Queries;

namespace ProductNames.Tests.DataManagement.EntityFramework {
    public class EFProductNameDataAccessorTests {
        [Fact]
        //TODO: Figure out why this test throws a null reference
        public void GetByIdTest() {
            var guid = new Guid("1dcefb30-1a8d-44c2-b7d8-1b1a82226e74");
            var productName = "Laptop";
            var productNameSet = new List<ProductName> {
                new ProductName {
                    Name = productName,
                    ProductNameID = guid
                },
                new ProductName {
                    Name = "Computer",
                    ProductNameID = Guid.NewGuid()
                }
            };

            var mockProductNameSet = new Mock<DbSet<ProductName>>();
            mockProductNameSet.Object.AddRange(productNameSet);           

            var mockProductNameContext = new Mock<ProductNameContext>();
            mockProductNameContext.Setup(x => x.ProductNames).Returns(mockProductNameSet.Object);
            mockProductNameContext.Object.SaveChanges();

            var efDataAccessor = new EFProductNameDataAccessor(mockProductNameContext.Object);
            var result = efDataAccessor.GetByIdHandler(new GetByIdQuery { ProductId = guid });

            Assert.Equal(result.ProductId, guid);
            Assert.Equal(result.Name, productName);
        }
    }
}
