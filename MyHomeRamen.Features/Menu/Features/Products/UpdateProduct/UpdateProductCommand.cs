using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(ProductId Id, UpdateProductRequest UpdateProductRequest) : ICommand<UpdateProductResponse>;
