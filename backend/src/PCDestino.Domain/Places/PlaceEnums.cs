namespace PCDestino.Domain.Places;

public enum PlaceKind
{
    PublicService = 1,
    PrivateService = 2,
    Health = 3,
    Leisure = 4,
    Sport = 5,
    Tourism = 6,
    Food = 7
}

public enum PublicationStatus
{
    Pending = 1,
    Published = 2,
    Rejected = 3,
    Archived = 4
}
