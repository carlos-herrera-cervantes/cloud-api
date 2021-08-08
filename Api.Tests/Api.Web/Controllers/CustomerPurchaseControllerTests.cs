using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Tests.Api.Web.Controllers
{
    public class CustomerPurchaseControllerTests
    {
        private readonly List<CustomerPurchase> _sales = new List<CustomerPurchase>
        {
            new CustomerPurchase
            {
                Id = "60f79b1944ed74adb7fb603c",
                Folio = "RE-26GDH363H16",
                Vat = 3,
                Subtotal = 17,
                Total = 20,
                TotalLetters = "TWENTY DOLLARS",
                UserId = "5fd5ddc549c67b3c6527462f",
                Products = new []
                {
                    new ProductSold
                    {
                        Name = "Test Product 1",
                        Description = "Test description 1",
                        Quantity = 1,
                        PriceUnit = 15,
                        Price = 20,
                        MeasurementUnit = "LTR",
                        MeasurementUnitSat = "L",
                        Taxes = new []
                        {
                            new Tax
                            {
                                Percentage = 0.16M,
                                Name = "IVA"
                            }
                        }
                    }
                },
                Payments = new []
                {
                    new Payment
                    {
                        Quantity = 20,
                        Key = "01",
                        Description = "Cash payment"
                    }
                },
                Client = new Client
                {
                    Email = "ruth.villa@gmail.com"
                },
                Station = "5f947448c83ad110d92cde25"
            }
        };

        private readonly string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImNhcmxvc0ByZW1hc3RlcmVkLmNvbSIsInJvbGUiOiJTdGF0aW9uQWRtaW4iLCJuYW1laWQiOiI1ZmQ1ZGRjNTQ5YzY3YjNjNjUyNzQ2MmYiLCJzdGF0aW9uIjoiNWY5NDc0NDhjODNhZDExMGQ5MmNkZTI1IiwibmJmIjoxNjI0MDMzNjgzLCJleHAiOjE2MjQ0NjU2ODMsImlhdCI6MTYyNDAzMzY4M30.FVa35rRAJHN0oYUbk0eab-B3CJRmdy7AkZiCnB0e-Q4";

        [Fact]
        public async Task GetAllAsync_ShouldReturn_SalesList()
        {
            var mockRepository = new Mock<ICustomerPurchaseRepository>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_sales);

            var customerPurchaseController = new CustomerPurchaseController(mockRepository.Object);

            var result = await customerPurchaseController.GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListCustomerPurchaseResponse;

            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListCustomerPurchaseResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetMeAsync_ShouldReturn_SalesBySpecificUser()
        {
            var mockRepository = new Mock<ICustomerPurchaseRepository>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync((ListResourceRequest request) =>
                {
                    var stationId = request.Filters.Split('=').LastOrDefault();
                    return _sales.Where(s => s.Station == stationId);
                });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var customerPurchaseController = new CustomerPurchaseController(mockRepository.Object);
            customerPurchaseController.ControllerContext.HttpContext = httpContext;

            var result = await customerPurchaseController.GetMeAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListCustomerPurchaseResponse;

            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListCustomerPurchaseResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_SpecificSale()
        {
            var mockRepository = new Mock<ICustomerPurchaseRepository>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _sales.SingleOrDefault(s => s.Id == id));

            var customerPurchaseController = new CustomerPurchaseController(mockRepository.Object);

            var result = await customerPurchaseController.GetByIdAsync("60f79b1944ed74adb7fb603c", new SingleResourceRequest());
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleCustomerPurchaseResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleCustomerPurchaseResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }
    }
}