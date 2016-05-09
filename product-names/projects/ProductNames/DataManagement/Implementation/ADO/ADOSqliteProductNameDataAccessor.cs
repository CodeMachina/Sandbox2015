using System;
using ProductNames.Models.DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.OptionsModel;
using ProductNames.DataManagement.Contract.Commands;
using ProductNames.DataManagement.Contract.Queries;
using ProductNames.DataManagement.Implementation.Utilities;
using ProductNames.DataManagement.Contract;

namespace ProductNames.DataManagement.Implementation.ADO {
    public class ADOSqliteProductNameDataAccessor : IHandleProductNameData {
        private readonly string connectionString;

        public ADOSqliteProductNameDataAccessor(IOptions<ADOProductNameDataAccessorOptions> optionsAccessor) {
            connectionString = optionsAccessor.Value.ConnectionString;
        }

        public void CreateOrUpdateHandler(CreateOrUpdateCommand command) {
            using(var connection = new SqliteConnection(connectionString)) {
                connection.Open();
                using(var sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = "insert or replace into ProductName (ProductNameID, Name) values (COALESCE((select ProductNameID from ProductName where ProductNameID = @productNameId), @productNameId), @productName);";
                    sqlCommand.Parameters.Add(new SqliteParameter("@productNameId", command.ProductId.ToByteArray()));
                    sqlCommand.Parameters.Add(new SqliteParameter("@productName", command.ProductName));

                    try {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch(Exception e) {
                        throw e;
                    }
                }
            }
        }

        public ProductName GetByIdHandler(GetByIdQuery query) {           
            using(var connection = new SqliteConnection(connectionString)) {
                connection.Open();
                using(var sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = "select hex(ProductNameID) as ProductNameID, Name from ProductName where ProductNameID = @productNameId;";
                    sqlCommand.Parameters.Add(new SqliteParameter("@productNameId", query.ProductId.ToByteArray()));

                    using(var dataReader = sqlCommand.ExecuteReader()) {
                        try {
                            dataReader.Read();

                            if(!dataReader.HasRows) {
                                return new ProductName();
                            }

                            return new ProductName {
                                ProductId = SqliteHelpers.GetGuidFromHexString(dataReader["ProductNameID"].ToString()),
                                Name = dataReader["Name"].ToString()
                            };
                        }
                        catch(Exception e) {
                            throw e;
                        }
                    }
                }              
            }
        }
    }
}
