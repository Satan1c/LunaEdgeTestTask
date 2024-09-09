namespace Api.Requests;

public interface ILoginRequest
{
	public string Login    { get; }
	public string Password { get; }
}
