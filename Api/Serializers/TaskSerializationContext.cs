using System.Text.Json.Serialization;
using Task = Database.Models.Task;

namespace Api.Serializers;

[JsonSerializable(typeof(Task))]
[JsonSerializable(typeof(Task[]))]
internal partial class TaskSerializationContext : JsonSerializerContext;
