namespace GameStore.Domain.UserEntities;

public enum ClaimType
{
    Permission,
}

public enum Permissions
{
    AddGame,
    DeleteGame,
    Game,
    Games,
    UpdateGame,
    ViewDeletedGame,
    UpdateDeletedGame,

    AddGenre,
    Genre,
    Genres,
    UpdateGenre,
    DeleteGenre,

    AddPublisher,
    Publisher,
    Publishers,
    UpdatePublisher,
    DeletePublisher,

    AddPlatform,
    Platform,
    Platforms,
    UpdatePlatform,
    DeletePlatform,

    Buy,
    Basket,
    MakeOrder,
    Order,
    Orders,
    UpdateOrder,
    ShipOrder,

    AddComment,
    QuoteComment,
    ReplyComment,
    Comments,
    DeleteComment,

    BanUser,
    BanComment,

    Users,
    User,
    AddUser,
    DeleteUser,
    UpdateUser,

    Roles,
    Role,
    AddRole,
    DeleteRole,
    UpdateRole,

    Permisssions,

    History,
}
