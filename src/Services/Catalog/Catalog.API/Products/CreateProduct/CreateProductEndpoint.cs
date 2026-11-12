namespace Catalog.API.Products.CreateProduct;

public record CreateProductRequest(string Name, string Description, decimal ImageFile, decimal Price, List<string> Category);

public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (CreateProductRequest request, ISender sender) => 
        { 
            var command = request.Adapt<CreateProductCommand>();

            var result = sender.Send(command);

            var response = result.Adapt<CreateProductResponse>();

            return Results.Created($"/products/{response.Id}", response);

        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Create Product")
        .WithSummary("Create Product");

    }
}
