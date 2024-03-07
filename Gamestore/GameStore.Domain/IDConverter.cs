using MongoDB.Bson;

namespace GameStore.Domain;

public static class IDConverter
{
    public static Guid AsGuid(this ObjectId oid)
    {
        var bytes = oid.ToByteArray().Concat(new byte[] { 5, 5, 5, 5 }).ToArray();
        return new Guid(bytes);
    }

    public static ObjectId AsObjectId(this Guid gid)
    {
        var bytes = gid.ToByteArray().Take(12).ToArray();
        return new ObjectId(bytes);
    }
}