using Application.Contracts;
using Application.Products.ClassDto;

namespace Application.Products.AddNewProduct;

public record AddNewProduct(string Type) : ICommand;