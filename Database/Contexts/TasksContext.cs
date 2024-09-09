using System.Data.Common;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Task = Database.Models.Task;

namespace Database.Contexts;

public class TasksContext(DbConnection connection) : BaseContext(connection)
{
	private DbSet<Task> Tasks { get; }

	public async ValueTask<bool> CreateTask(Task task, CancellationToken token)
	{
		task.Id = Guid.NewGuid();
		await Tasks.AddAsync(task, token);
		return await SaveChangesAsync(token) == 1;
	}

	public async ValueTask<(sbyte, Task? task)> UpdateTask(Task task, CancellationToken token)
	{
		var taskToUpdate = await Tasks.FirstOrDefaultAsync(x => x.UserId == task.UserId && x.Id == task.Id, token);

		if (taskToUpdate == null) return (-1, taskToUpdate);

		if (taskToUpdate.Title != task.Title) taskToUpdate.Title                                            = task.Title;
		if (taskToUpdate.Description != task.Description) taskToUpdate.Description                          = task.Description;
		if (task.Status != Status.None && taskToUpdate.Status != task.Status) taskToUpdate.Status           = task.Status;
		if (task.Priority != Priority.None && taskToUpdate.Priority != task.Priority) taskToUpdate.Priority = task.Priority;
		if (taskToUpdate.DueDate != task.DueDate) taskToUpdate.DueDate                                      = task.DueDate;

		return ((sbyte)(await SaveChangesAsync(token) == 1 ? 1 : 0), taskToUpdate);
	}

	public async ValueTask<sbyte> DeleteTask(Guid userId, Guid id, CancellationToken token)
	{
		var taskToRemove = await Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id, token);
		if (taskToRemove == null) return -1;

		Tasks.Remove(taskToRemove);
		return (sbyte)(await SaveChangesAsync(token) == 1 ? 1 : 0);
	}

	public Task<Task?> FindTask(Guid userId, Guid id, CancellationToken token)
		=> Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id, token);

	public ValueTask<Task[]> GetTasks(
			Guid              userId,
			ushort            page,
			Status?           status,
			Priority?         priority,
			DateTime?         dueDate,
			CancellationToken token
	)
	{
		return Tasks.AsAsyncEnumerable()
					.Where(
						   x => x.UserId == userId
								&& (!status.HasValue || status.Value == x.Status)
								&& (!priority.HasValue || priority.Value == x.Priority)
								&& (!dueDate.HasValue || dueDate.Value == x.DueDate)
						  )
					.Skip((page == 0 ? page : page - 1) * 10)
					.Take(10)
					.ToArrayAsync(cancellationToken: token);
	}
}
