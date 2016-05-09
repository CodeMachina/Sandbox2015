using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using ProductNames.DataManagement.Implementation.EntityFramework;

namespace ProductNames.Migrations
{
    [DbContext(typeof(ProductNameContext))]
    [Migration("20151019184842_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
