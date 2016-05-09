using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using ProductNames.DataManagement.Implementation.EntityFramework;

namespace ProductNames.Migrations
{
    [DbContext(typeof(ProductNameContext))]
    partial class ProductNameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-final");

            modelBuilder.Entity("ProductNames.DataManagement.Implementation.DataModel.ProductName", b =>
                {
                    b.Property<Guid>("ProductNameID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ProductNameID");
                });
        }
    }
}
