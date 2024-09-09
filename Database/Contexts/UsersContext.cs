using System.Data.Common;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Contexts;

public class UsersContext(DbConnection connection) : BaseContext(connection)
{
	private DbSet<User>      Users  { get; }
	private DbSet<TokenInfo> Tokens { get; }

	public Task<bool> IsTokenExist(Guid userId, string jwt, CancellationToken token)
		=> Tokens.AnyAsync(x => x.UserId == userId && x.Token == jwt, token);

	public Task<TokenInfo?> FindToken(Guid userId) => Tokens.FirstOrDefaultAsync(x => x.UserId == userId);

	public async ValueTask<bool> CreateOrUpdate(TokenInfo info, CancellationToken token)
	{
		if (await FindToken(info.UserId) is { } existed)
		{
			info.Id         = existed.Id;
			var (status, _) = await UpdateToken(info, token);
			return status == 1;
		}

		info.Id = Guid.NewGuid();
		await Tokens.AddAsync(info, token);
		return await SaveChangesAsync(token) == 1;
	}

	public async ValueTask<(sbyte, TokenInfo? task)> UpdateToken(TokenInfo info, CancellationToken token)
	{
		var tokenToUpdate = await Tokens.FirstOrDefaultAsync(x => x.UserId == info.UserId && x.Id == info.Id, token);

		if (tokenToUpdate == null) return (-1, tokenToUpdate);

		if (!string.IsNullOrEmpty(info.Token) && tokenToUpdate.Token != info.Token) tokenToUpdate.Token = info.Token;

		return ((sbyte)(await SaveChangesAsync(token) == 1 ? 1 : 0), tokenToUpdate);
	}

	public Task<bool> IsUserExists(Guid userId, CancellationToken token) => Users.AnyAsync(x => x.Id == userId, token);

	public Task<bool> IsUserExists(string login, CancellationToken token)
		=> Users.AnyAsync(x => x.Email == login || x.Username == login, token);

	public Task<bool> IsUserExistsByEmail(string email, CancellationToken token) => Users.AnyAsync(x => x.Email == email, token);

	public Task<bool> IsUserExistsByUserName(string username, CancellationToken token)
		=> Users.AnyAsync(x => x.Username == username, token);

	public Task<User?> FindUserByLogin(string login, string passwordHash, CancellationToken token)
		=> Users.FirstOrDefaultAsync(x => (x.Email == login || x.Username == login) && x.PasswordHash == passwordHash, token);

	public ValueTask<User?> FindUser(Guid userId, CancellationToken token) => Users.FindAsync([userId], token);

	public async ValueTask<bool> CreateUser(User user, CancellationToken token)
	{
		user.Id = Guid.NewGuid();
		await Users.AddAsync(user, token);
		return await SaveChangesAsync(token) == 1;
	}
}
