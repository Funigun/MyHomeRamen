using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed record CreateProductCommand(CreateProductRequest CreateProductRequest) : ICommand<CreateProductResponse>;
