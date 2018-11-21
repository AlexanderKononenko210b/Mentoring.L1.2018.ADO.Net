using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Northwind.DAL.Infrastructure.Enums;
using Northwind.DAL.Infrastructure.Interfaces;
using Northwind.DAL.Infrastructure.Models;

namespace Northwind.DAL.Repositories
{
    /// <summary>
    /// Represents a <see cref="OrderRepository"/> class.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        /// <summary>
        /// Initialize a new <see cref="OrderRepository"/> instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="provider">The provider.</param>
        public OrderRepository(string connectionString, string provider)
        {
            _providerFactory = DbProviderFactories.GetFactory(provider);
            _connectionString = connectionString;
        }

        #region Public methods

        /// <inheritdoc/>>
        public virtual IEnumerable<Order> GetOrders()
        {
            var resultOrders = new List<Order>();

            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, " +
                                            "ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry " +
                                            "from dbo.Orders";
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order();

                            this.GetOrderFromReader(order, reader);

                            resultOrders.Add(order);
                        }
                    }
                }
            }

            return resultOrders;
        }

        /// <inheritdoc/>
        public virtual Order GetOrderDetailById(int id)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select o.OrderID, o.CustomerID, o.EmployeeID, o.OrderDate, o.RequiredDate, o.ShippedDate, o.ShipVia, o.Freight, o.ShipName, " +
                                             "o.ShipAddress, o.ShipCity, o.ShipRegion, o.ShipPostalCode, o.ShipCountry " +
                                             "from dbo.Orders as o " +
                                             "where OrderID = @id; " +
                                          "Select od.ProductID, od.OrderID, od.UnitPrice, od.Quantity, od.Discount, " +
                                            "p.ProductName, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued " +
                                            "from dbo.[Order Details] as od " +
                                            "join dbo.Products as p on od.ProductID = p.ProductID " +
                                            "where OrderID = @id;";
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@id";
                    paramId.Value = id;

                    command.Parameters.Add(paramId);

                    using (var reader = command.ExecuteReader())
                    {
                        var order = new Order();

                        while (reader.Read())
                        {
                            this.GetOrderFromReader(order, reader);

                            reader.NextResult();

                            var orderDetails = new List<Order_Detail>();
                            var products = new List<Product>();

                            while (reader.Read())
                            {
                                var orderDetail = new Order_Detail();
                                orderDetail.ProductID = (int)reader["ProductID"];
                                orderDetail.OrderID = (int)reader["OrderID"];
                                orderDetail.UnitPrice = (decimal)reader["UnitPrice"];
                                orderDetail.Quantity = (short)reader["Quantity"];
                                orderDetail.Discount = (float)reader["Discount"];

                                orderDetails.Add(orderDetail);

                                var product = new Product();
                                product.ProductID = (int)reader["ProductID"];
                                product.ProductName = (string)reader["ProductName"];
                                product.QuantityPerUnit = (string)reader["QuantityPerUnit"];
                                product.UnitPrice = DBNull.Value.Equals(reader["UnitPrice"]) ? default(decimal?) : (decimal?)reader["UnitPrice"];
                                product.UnitsInStock = DBNull.Value.Equals(reader["UnitsInStock"]) ? default(short?) : (short?)reader["UnitsInStock"];
                                product.UnitsOnOrder = DBNull.Value.Equals(reader["UnitsOnOrder"]) ? default(short?) : (short?)reader["UnitsOnOrder"];
                                product.ReorderLevel = DBNull.Value.Equals(reader["ReorderLevel"]) ? default(short?) : (short?)reader["ReorderLevel"];
                                product.Discontinued = (bool)reader["Discontinued"];

                                products.Add(product);
                            }

                            order.OrderDetails = orderDetails;
                            order.Products = products;
                        }

                        return order;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual Order AddOrder(Order order)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert dbo.Orders" +
                        "(CustomerID, EmployeeID, ShipVia, Freight, " +
                            "ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)" +
                          "values(@customerID, @employeeID, @shipVia, @freight, " +
                          "@shipName, @shipAddress, @shipCity, @shipRegion, @shipPostalCode, @shipCountry);" +
                          "Select SCOPE_IDENTITY() as OrderID;";

                    command.CommandType = CommandType.Text;

                    command.Parameters.AddRange(
                        new[]
                        {
                            new SqlParameter
                            {
                                ParameterName = "@customerID",
                                Value = order.CustomerID,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@employeeID",
                                Value = order.EmployeeID,
                                DbType = DbType.Int32
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipVia",
                                Value = order.ShipVia,
                                DbType = DbType.Int32
                            },
                            new SqlParameter
                            {
                                ParameterName = "@freight",
                                Value = order.Freight,
                                DbType = DbType.Decimal
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipName",
                                Value = order.ShipName,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipAddress",
                                Value = order.ShipAddress,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipCity",
                                Value = order.ShipCity,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipRegion",
                                Value = order.ShipRegion,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipPostalCode",
                                Value = order.ShipPostalCode,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipCountry",
                                Value = order.ShipCountry,
                                DbType = DbType.String
                            }
                        });

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!int.TryParse(reader["OrderID"].ToString(), out var newOrderID))
                            {
                                return null;
                            }

                            order.OrderID = newOrderID;
                        }
                    }
                }
            }

            return order;
        }

        /// <inheritdoc/>
        public virtual Order UpdateOrder(Order order)
        {
            var orderForUpdate = this.GetOrderById(order.OrderID);

            if (orderForUpdate == null)
            {
                return null;
            }

            var status = this.GetOrderStatus(orderForUpdate.OrderDate, orderForUpdate.ShippedDate);

            if (status != Status.Created)
            {
                return null;
            }

            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Update dbo.Orders " +
                                          "set CustomerID = @customerID, EmployeeID = @employeeID, ShipVia = @shipVia, " +
                                          "Freight = @freight, ShipName = @shipName, ShipAddress = @shipAddress, " +
                                          "ShipCity = @shipCity, ShipRegion = @shipRegion, ShipPostalCode = @shipPostalCode, ShipCountry = @shipCountry " +
                                          "where OrderID = @orderId;";

                    command.CommandType = CommandType.Text;

                    command.Parameters.AddRange(
                        new[]
                        {
                            new SqlParameter
                            {
                                ParameterName = "@customerID",
                                Value = order.CustomerID,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@employeeID",
                                Value = order.EmployeeID,
                                DbType = DbType.Int32
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipVia",
                                Value = order.ShipVia,
                                DbType = DbType.Int32
                            },
                            new SqlParameter
                            {
                                ParameterName = "@freight",
                                Value = order.Freight,
                                DbType = DbType.Decimal
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipName",
                                Value = order.ShipName,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipAddress",
                                Value = order.ShipAddress,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipCity",
                                Value = order.ShipCity,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipRegion",
                                Value = order.ShipRegion,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipPostalCode",
                                Value = order.ShipPostalCode,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@shipCountry",
                                Value = order.ShipCountry,
                                DbType = DbType.String
                            },
                            new SqlParameter
                            {
                                ParameterName = "@orderID",
                                Value = order.OrderID,
                                DbType = DbType.Int32
                            }
                        });

                    var countUpdateRows = command.ExecuteNonQuery();
                    if (countUpdateRows != 1)
                    {
                        return null;
                    }

                    return this.GetOrderById(order.OrderID);
                }
            }
        }

        /// <inheritdoc/>
        public virtual Order SetOrderStatusInWork(int id, DateTime orderDate, DateTime requiredDate)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Update dbo.Orders set OrderDate = @orderDate, RequiredDate = @requiredDate where OrderID = @orderID;";

                    command.Parameters.AddRange(
                        new []
                        {
                            new SqlParameter
                            {
                                ParameterName = "@orderDate",
                                Value = orderDate,
                                DbType = DbType.DateTime
                            },
                            new SqlParameter
                            {
                                ParameterName = "@requiredDate",
                                Value = requiredDate,
                                DbType = DbType.DateTime
                            },
                            new SqlParameter
                            {
                                ParameterName = "@orderID",
                                Value = id,
                                DbType = DbType.Int32
                            }
                        });

                    var countUpdateRows = command.ExecuteNonQuery();
                    if (countUpdateRows != 1)
                    {
                        return null;
                    }

                    return this.GetOrderById(id);
                }
            }
        }

        /// <inheritdoc/>
        public virtual Order SetOrderStatusFinished(int id, DateTime shippedDate)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Update dbo.Orders set ShippedDate = @shippedDate where OrderID = @orderID;";

                    command.Parameters.AddRange(
                        new []
                        {
                            new SqlParameter
                            {
                                ParameterName = "@shippedDate",
                                Value = shippedDate,
                                DbType = DbType.DateTime
                            },
                            new SqlParameter
                            {
                                ParameterName = "@orderID",
                                Value = id,
                                DbType = DbType.Int32
                            }
                        });

                    var countUpdateRows = command.ExecuteNonQuery();
                    if (countUpdateRows != 1)
                    {
                        return null;
                    }

                    return this.GetOrderById(id);
                }
            }
        }

        /// <inheritdoc/>
        public virtual Order DeleteOrder(int id)
        {
            var orderForDelete = this.GetOrderById(id);

            if (orderForDelete == null)
            {
                return null;
            }

            var status = this.GetOrderStatus(orderForDelete.OrderDate, orderForDelete.ShippedDate);

            if (status == Status.Finished)
            {
                return null;
            }

            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Delete from dbo.Orders where OrderID = @orderId;";

                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "@orderId",
                            Value = id,
                            DbType = DbType.Int32
                        }
                    );

                    var countDeleteRows = command.ExecuteNonQuery();
                    if (countDeleteRows != 1)
                    {
                        return null;
                    }

                    return orderForDelete;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerable<CustOrderHist> GetCustOrderHist(string customerId)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CustOrderHist";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "@CustomerID",
                            Value = customerId,
                            DbType = DbType.String
                        }
                    );

                    using (var reader = command.ExecuteReader())
                    {
                        var custList = new List<CustOrderHist>();
                        while (reader.Read())
                        {
                            var custOrderHist = new CustOrderHist();

                            custOrderHist.ProductName = (string) reader["ProductName"];
                            custOrderHist.CoastQuantity = (int) reader["Total"];

                            custList.Add(custOrderHist);
                        }

                        return custList;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerable<CustOrdersDetail> GetCustOrdersDetail(int orderId)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CustOrdersDetail";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "@OrderID",
                            Value = orderId,
                            DbType = DbType.Int32
                        }
                    );

                    using (var reader = command.ExecuteReader())
                    {
                        var custList = new List<CustOrdersDetail>();

                        while (reader.Read())
                        {
                            var custOrdersDetail = new CustOrdersDetail();

                            custOrdersDetail.ProductName = (string) reader["ProductName"];
                            custOrdersDetail.UnitPrice = (decimal) reader["UnitPrice"];
                            custOrdersDetail.Quantity = (short)reader["Quantity"];
                            custOrdersDetail.Discount = (int)reader["Discount"];
                            custOrdersDetail.ExtendedPrice = (decimal)reader["ExtendedPrice"];

                            custList.Add(custOrdersDetail);
                        }

                        return custList;
                    }
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get order by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The  <see cref="Order"/></returns>
        private Order GetOrderById(int id)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select o.OrderID, o.CustomerID, o.EmployeeID, o.OrderDate, o.RequiredDate, o.ShippedDate, o.ShipVia, o.Freight, o.ShipName, " +
                                          "o.ShipAddress, o.ShipCity, o.ShipRegion, o.ShipPostalCode, o.ShipCountry " +
                                          "from dbo.Orders as o " +
                                          "where OrderID = @id; ";
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@id";
                    paramId.Value = id;

                    command.Parameters.Add(paramId);

                    using (var reader = command.ExecuteReader())
                    {
                        var order = new Order();

                        while (reader.Read())
                        {
                            this.GetOrderFromReader(order, reader);
                        }

                        return order;
                    }
                }
            }
        }

        /// <summary>
        /// Get order from reader.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="reader">The reader.</param>
        private void GetOrderFromReader(Order order, DbDataReader reader)
        {
            order.OrderID = (int)reader["OrderID"];
            order.CustomerID = (string)reader["CustomerID"];
            order.EmployeeID = (int)reader["EmployeeID"];
            order.OrderDate = DBNull.Value.Equals(reader["OrderDate"]) ? default(DateTime?) : (DateTime?)reader["OrderDate"];
            order.RequiredDate = DBNull.Value.Equals(reader["RequiredDate"]) ? default(DateTime?) : (DateTime?)reader["RequiredDate"];
            order.ShippedDate = DBNull.Value.Equals(reader["ShippedDate"]) ? default(DateTime?) : (DateTime?)reader["ShippedDate"];
            order.ShipVia = DBNull.Value.Equals(reader["ShipVia"]) ? default(int?) : (int?)reader["ShipVia"];
            order.Freight = DBNull.Value.Equals(reader["Freight"]) ? default(decimal?) : (decimal?)reader["Freight"];
            order.ShipName = DBNull.Value.Equals(reader["ShipName"]) ? string.Empty : (string)reader["ShipName"];
            order.ShipAddress = DBNull.Value.Equals(reader["ShipAddress"]) ? string.Empty : (string)reader["ShipAddress"];
            order.ShipCity = DBNull.Value.Equals(reader["ShipCity"]) ? string.Empty : (string)reader["ShipCity"];
            order.ShipRegion = DBNull.Value.Equals(reader["ShipRegion"]) ? string.Empty : (string)reader["ShipRegion"];
            order.ShipPostalCode = DBNull.Value.Equals(reader["ShipPostalCode"]) ? string.Empty : (string)reader["ShipPostalCode"];
            order.ShipCountry = DBNull.Value.Equals(reader["ShipCountry"]) ? string.Empty : (string)reader["ShipCountry"];
            order.Status = this.GetOrderStatus(order.OrderDate, order.ShippedDate);
        }

        /// <summary>
        /// Get order status.
        /// </summary>
        /// <param name="orderDate">The order date.</param>
        /// <param name="shippedDate">The shipped date.</param>
        /// <returns></returns>
        private Status GetOrderStatus(DateTime? orderDate, DateTime? shippedDate)
        {
            if (!orderDate.HasValue)
            {
                return Status.Created;
            }

            if (!shippedDate.HasValue)
            {
                return Status.InWork;
            }

            return Status.Finished;
        }

        #endregion
    }
}
