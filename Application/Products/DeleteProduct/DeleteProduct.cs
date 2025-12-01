using Application.Contracts;
using FluentResults;

namespace Application.Products.DeleteProduct;

public record DeleteProduct(string Id) : ICommand;