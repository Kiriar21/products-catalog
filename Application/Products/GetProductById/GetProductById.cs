using Application.ClassDto;
using Application.Contracts;
using Application.Products.ClassDto;
using FluentResults;

namespace Application.Products.GetProductById;

public record GetProductById(string Id) : IQuery<ProductDto> { }