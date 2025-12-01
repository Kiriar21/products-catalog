namespace Application.Contracts;

public interface ICommand { }

public interface ICommand<out TResponse> { }