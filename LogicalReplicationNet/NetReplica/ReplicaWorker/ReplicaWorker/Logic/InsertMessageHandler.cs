using Npgsql.Replication.PgOutput.Messages;
using ReplicaWorker.Models;

namespace ReplicaWorker.Logic;

public static class InsertMessageHandler
{
    public static async Task<Blog> Handle(InsertMessage message, CancellationToken ct)
    {
        var columnNumber = 0;
        var eventTypeName = string.Empty;

        var blog = new Blog();
        await foreach (var value in message.NewRow)
        {
            // iteracja po kolumnach
            switch (columnNumber)
            {
                case 0:
                    var stringValue = await value.Get();
                    if (Int32.TryParse(stringValue.ToString(), out int intValue))
                    {
                        blog.test_id = intValue;
                    }
                    break;
            }

            columnNumber++;
        }
        return blog;
    }
}